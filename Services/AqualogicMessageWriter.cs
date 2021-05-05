using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AqualogicJumper.Model;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace AqualogicJumper.Services
{
    public class AqualogicMessageWriter
    {
        private readonly ILogger<AqualogicMessageWriter> _log;
        private readonly IAqualogicStream _stream;
        private readonly ConcurrentQueue<Message> _sendQueue = new ConcurrentQueue<Message>();
        public const int STATE_CHECK_DELAY = 500;
        public static readonly TimeSpan RetryTimeout = new TimeSpan(0,0,30);
        public static readonly TimeSpan RateLimitDelay = new TimeSpan(0, 0, 0,0,100);

        public AqualogicMessageWriter(ILogger<AqualogicMessageWriter> log, IAqualogicStream stream)
        {
            _log = log;
            _stream = stream;
        }
// Sends a key.

        private byte[] GetKeyEventFrame(Key key)
        {
            var frame = new List<byte>(AqualogicProtocolConstants.KEY_FRAME_LENGTH * 2);
            frame.Add(AqualogicProtocolConstants.FRAME_DLE);
            frame.Add(AqualogicProtocolConstants.FRAME_STX);
            AppendPrimative(frame, ((short)FrameType.WirelessKeyEvent).ToBigEndian());
            frame.Add(0x01);
            //        if key.value > 0xffff:
            var keyBytes = ((int)key).ToLittleEndian();
            AppendPrimative(frame, keyBytes);
            AppendPrimative(frame, keyBytes);
            frame.Add(0x00);
            //       else:
            //           self._append_data(frame, self.FRAME_TYPE_LOCAL_WIRED_KEY_EVENT)
            //           self._append_data(frame, key.value.to_bytes(2, byteorder='little'))
            //          self._append_data(frame, key.value.to_bytes(2, byteorder='little'))
            short crc = 0;
            foreach (var b in frame)
                crc += b;
            AppendPrimative(frame, crc.ToBigEndian());
            frame.Add(AqualogicProtocolConstants.FRAME_DLE);
            frame.Add(AqualogicProtocolConstants.FRAME_ETX);
            return frame.ToArray();
        }
        private void AppendPrimative(ICollection<byte> frame, byte[] data)
        {
            foreach (var b in data)
            {
                frame.Add(b);
                if (b == AqualogicProtocolConstants.FRAME_DLE)
                {
                    frame.Add(0);
                }
            }
        }
        public virtual void SendKey(Key key, Func<bool> until, Action<Message> complete=null)
        {
            _log.LogDebug($"Queueing key {key}");
            // Queue it to send immediately following the reception
            // of a keep-alive packet in an attempt to avoid bus collisions.
            this._sendQueue.Enqueue(
                new Message()
                {
                    Key = key,
                    RequestedTime = DateTime.UtcNow,
                    Attempts = 0,
                    Frame = GetKeyEventFrame((key)),
                    Validate = until,
                    Complete = complete
                });

        }
        public virtual async Task<MessageState> SendKeyAsync(Key key, Func<bool> until, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<Message>();
            SendKey(key, until, (msg) =>
             {
                //signal
                tcs.SetResult(msg);
             });
            return (await tcs.Task.WaitAsync(token)).State;
        }

        public virtual async Task<bool> SendFrame(CancellationToken token)
        {
            if (_stream.HasPendingReads) return false;
            Message toSend = null;
            while (toSend == null)
            {
                if (!_sendQueue.TryPeek(out toSend)) 
                    return false;
                if (toSend == null || 
                    (toSend.Attempts > 0 && (toSend.Validate == null || toSend.Validate())) ||
                    DateTime.UtcNow.Subtract(RetryTimeout) > toSend.FirstAttemptTime)
                {
                    if (toSend == null)
                    {
                        _log.LogDebug($"Removing null message from queue");
                    }
                    else if ((toSend.Attempts > 0 && (toSend.Validate == null || toSend.Validate())))
                    {
                        _log.LogDebug($"Removing {toSend.Key} from queue, result validated");
                        toSend.State = MessageState.Success;
                        toSend.Complete?.Invoke(toSend);
                    }
                    else if (DateTime.UtcNow.Subtract(RetryTimeout) > toSend.FirstAttemptTime)
                    {
                        _log.LogDebug($"Removing {toSend.Key} from queue , timeout");
                        toSend.State = MessageState.Timeout;
                    }
                    _sendQueue.TryDequeue(out _);
                    toSend?.Complete?.Invoke(toSend);
                    toSend = null;
                }
            }
            return await SendMessage(toSend,token);
        }

        public Task<bool> SendKey(Key key, CancellationToken token) =>
            SendMessage(new Message()
            {
                Key = key,
                RequestedTime = DateTime.UtcNow,
                Attempts = 0,
                Frame = GetKeyEventFrame((key)),
            }, token);

        public async Task<bool> SendMessage(Message toSend, CancellationToken token)
        {
            if (_stream.HasPendingReads) return false;
            await _stream.Write(toSend.Frame, token).ConfigureAwait(false);
            await _stream.Flush();
            LastMessageTime = DateTime.UtcNow;
            if (toSend.Attempts == 0)
                toSend.FirstAttemptTime = DateTime.UtcNow;
            toSend.LastAttemptTime = DateTime.UtcNow;
            toSend.Attempts++;
            return true;
        }

        public DateTime? LastMessageTime { get; set; }
        public async Task Process(CancellationToken token)
        {
            if (_sendQueue.IsEmpty) return;
            if (!LastMessageTime.HasValue || DateTime.UtcNow.Subtract(LastMessageTime.Value) > RateLimitDelay)
            {
                await SendFrame(token);
            }
        }
    }
}