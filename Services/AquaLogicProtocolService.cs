using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AqualogicJumper.Model;
using AqualogicJumper.Model.Status;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{

    // Hayward/Goldline AquaLogic/ProLogic pool controller.
    public class AquaLogicProtocolService : IDisposable
    {
        private readonly ILogger<AquaLogicProtocolService> _log;
        private readonly StatusUpdateService _updateProcessor;
        private readonly CommandService _commandService;

        //        private readonly SwitchService _switchService;
        //        private readonly SettingService _settingService;
        public const ControllerType ThisControllerType = ControllerType.Wireless;

        private readonly IAqualogicStream _stream;
        private static Encoding Encoding { get; } =Encoding.GetEncoding("ISO-8859-1");


        public AquaLogicProtocolService(IAqualogicStream stream, PoolStatusStore store, 
            ILogger<AquaLogicProtocolService> log, AqualogicMessageWriter writer, 
            StatusUpdateService updateProcessor, CommandService commandService,
            SwitchService switchService)
        {
            _log = log;
            _updateProcessor = updateProcessor;
            _commandService = commandService;
            _stream = stream;
        }
        public bool Connect(string serialPortName=null)
        {
            return IsRunning = _stream.Connect(serialPortName);
        }
        // Process data; returns when the reader signals EOF.
        //         Callback is notified when any data changes.
        public virtual async Task<bool> Process(CancellationToken token)
        {
            // Data framing (from the AQ-CO-SERIAL manual):
            //
            // Each frame begins with a DLE (10H) and STX (02H) character start
            // sequence, followed by a 2 to 61 byte long Command/Data field, a
            // 2-byte Checksum and a DLE (10H) and ETX (03H) character end
            // sequence.
            //
            // The DLE, STX and Command/Data fields are added together to
            // provide the 2-byte Checksum. If any of the bytes of the
            // Command/Data Field or Checksum are equal to the DLE character
            // (10H), a NULL character (00H) is inserted into the transmitted
            // data stream immediately after that byte. That NULL character
            // must then be removed by the receiver.
//            await _commandService.TryProcess(token);
            if (!await TryReadFrameStart(token)) return false;
            var frame = await ParseFrame(token);
            await ProcessFrame(frame, token);
            await _commandService.TryProcess(token);
            return true;
        }

        private async Task ProcessFrame(Frame frame, CancellationToken token)
        {
            switch (frame.Type)
            {
                case FrameType.KeepAlive:
                    break;
                case FrameType.LocalWiredKeyEvent:
                case FrameType.RemoteWiredKeyEvent:
                case FrameType.WirelessKeyEvent:
                case FrameType.RfRemoteKeyEvent:
                case FrameType.UnknownRemoteKey:
//                    _log.LogDebug($"{frame.Type}: {(Key)BitConverter.ToInt32(frame.Body[1..5])}");
                    break;
                case FrameType.Switches:
                    ProcessSwitchesFrame(frame, frame.Body);
                    break;
                case FrameType.DisplayUpdate:
                    ProcessDisplayUpdateFrame(frame, frame.Body);
                    break;
                case FrameType.LongDisplayUpdate:
//                    ProcessLongDisplayUpdateFrame(frame);
                    break;
                case FrameType.OnOffEvent:
                    WriteDebug(frame, $"OnOffEvent [{frame.Body.ToHexString()}]");
                    break;
                case FrameType.PumpSpeedRequest:
                    WriteDebug(frame, $"PumpSpeedRequest [{frame.Body.ToHexString()}]");
                    break;
                case FrameType.PumpStatus:
                    WriteDebug(frame, $"PumpStatus [{frame.Body.ToHexString()}]");
                    break;
                default:
                    WriteDebug(frame, $"Unknown frame: {((int)frame.Type).ToString("X")} [{frame.Body.ToHexString()}]");
                    break;
            }
        }

        private void ProcessLongDisplayUpdateFrame(Frame frame)
        {
            var remoteId = (ControllerType)BinaryPrimitives.ReadInt16LittleEndian(frame.Body[..2]);
            frame.Body = frame.Body[2..];
            WriteDebug(frame, $"LongDisplayUpdate - {remoteId}: [{frame.Body.ToHexString()}]");
            if (remoteId == ThisControllerType)
            {
                var frameDelimiter =
                    frame.Body.AsSpan().IndexOf(new byte[] { AqualogicProtocolConstants.FRAME_ETX });
                if (frameDelimiter < 0)
                    frameDelimiter = 0;
                //                            ParseLedsFrame(frame,frame.Body[..frameDelimiter]);
                //                            ParseDisplayText(frame, frame.Body[(frameDelimiter+1)..]);
            }

            //                        _store.SaveChanges();
        }

        private void WriteDebug(Frame frame, string msg) =>
            _log.LogDebug($"{frame.StartTime}: {msg}");

        private readonly ConcurrentQueue<Task<bool>> _pendingStateChecks = new ConcurrentQueue<Task<bool>>();

        private async Task<Frame> ParseFrame(CancellationToken token)
        {
            var frame = new Frame();

            frame.Data = (await ReadFrameBody(token)).ToArray();
            if (frame.Data.Length == 0) return frame;
            // Verify CRC
            frame.Type = (FrameType) BinaryPrimitives.ReadInt16BigEndian(frame.Data[..2]);
            frame.Body = frame.Data[2..^2];
            frame.FrameCrc = BinaryPrimitives.ReadInt16BigEndian(frame.Data[^2..]);
            if (ValidateCrc(frame)) return frame;
            return frame;
        }

        private bool ValidateCrc(Frame frame)
        {
            var calculatedCrc = AqualogicProtocolConstants.FRAME_DLE + AqualogicProtocolConstants.FRAME_STX;
            foreach (var fb in frame.Data[..^2])
                calculatedCrc += fb;
            if (frame.FrameCrc != calculatedCrc)
            {
                _log.LogDebug($"Bad CRC, expected {frame.FrameCrc} got {calculatedCrc}");
                return true;
            }

            return false;
        }

        private void ProcessSwitchesFrame(Frame frame,ReadOnlySpan<byte> data)
        {
            WriteDebug(frame, $"{frame.Type} [{frame.Body.ToHexString()}]");
            // First 4 bytes are the LEDs that are on;
            // second 4 bytes_ are the LEDs that are flashing
            if (data.Length < 8) return;
            var states = (SwitchName) BinaryPrimitives.ReadInt32LittleEndian(data[..4]);
            var flashingStates = (SwitchName) BinaryPrimitives.ReadInt32LittleEndian(data[4..8]);
            _updateProcessor.TryProcess(states, flashingStates);
        }
        private void ProcessDisplayUpdateFrame(Frame frame,ReadOnlySpan<byte> textBuffer)
        {
            _updateProcessor.TryProcess((Encoding.GetNullTerminatedSTring(textBuffer) ?? "").Trim());
        }


        private async Task<IEnumerable<byte>> ReadFrameBody(CancellationToken token)
        {
            byte[] buffer = new byte[1];
            var l = new List<byte>();
            while (await _stream.TryRead(buffer,token))
            {
                var b = buffer[0];
                if (b == AqualogicProtocolConstants.FRAME_DLE)
                {
                    // Should be FRAME_ETX or 0 according to
                    // the AQ-CO-SERIAL manual
                    if (!await _stream.TryRead(buffer,token))
                        break;
                    if (buffer[0] == AqualogicProtocolConstants.FRAME_ETX)
                        break;
                    if (buffer[0] != 0)
                        _log.LogDebug("Error?");
                }

                l.Add(b);
            }

            return l;
        }
        public bool IsRunning { get; private set; }
        public void Stop() => IsRunning = false;

        private async Task<bool> TryReadFrameStart(CancellationToken token)
        {
            byte[] buffer = new byte[1];
            while (IsRunning && !token.IsCancellationRequested)
            {
                if (!await _stream.TryRead(buffer,token)) return false;
                var b = buffer[0];
                if (b == AqualogicProtocolConstants.FRAME_DLE)
                {
                    if (!await _stream.TryRead(buffer, token))
                        return false;
                    if (buffer[0]== AqualogicProtocolConstants.FRAME_STX) return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
