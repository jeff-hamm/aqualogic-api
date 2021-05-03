using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;

//using RJCP.IO.Ports;
//using SerialPort = RJCP.IO.Ports.SerialPortStream;
//using Parity = RJCP.IO.Ports.Parity;
//using StopBits = RJCP.IO.Ports.StopBits;
namespace AqualogicJumper.Services
{
    public interface IAqualogicStream : IDisposable
    {
        bool HasPendingReads { get; }
        bool Connect(string name=null);
        Task<int> Read(CancellationToken? cancellationToken = null);
        Task<bool> TryRead(byte[] b, CancellationToken? cancellationToken = null);
        Task Write(byte[] data, CancellationToken? cancellationToken = null);
        Task Flush(CancellationToken? token = null);
    }

    public class SerialAqualogicStream : IAqualogicStream
    {
        private readonly ILogger _log;
        private SerialPort _port;
        public const int ReadTimeout = 500;
        public SerialAqualogicStream(ILogger<SerialAqualogicStream> log)
        {
            _log = log;
        }
        public bool Connect(string name=null)
        {
            if (String.IsNullOrEmpty(name))
            {
                _log.LogDebug($"No serial port specified, checking for first available port");
                name = SerialPort.GetPortNames()?.FirstOrDefault();
            }

            if (String.IsNullOrEmpty(name))
            {
                _log.LogDebug("No serial port found, cannot connect");
                return false;
            }

            if (_port?.IsOpen == true)
            {
                _log.LogDebug($"Port already open, closing and retrying");
                _port.Close();
            }
            _port?.Dispose();
            _log.LogDebug($"debug connecting to {name}");

            _port = new SerialPort(name, 19200);
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.Two;
            _port.ReadBufferSize = 256;
            _port.ReadTimeout = ReadTimeout;
            _port.WriteBufferSize = AqualogicProtocolConstants.KEY_FRAME_LENGTH;
            _port.DataReceived += _port_DataReceived;
            _port.Open();
            return true;
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Eof)
            {
                LastReadTime = DateTime.Now;
            }
        }

        public async Task<int> Read(CancellationToken? token = null)
        {
            if (token?.IsCancellationRequested == true) return -1;
            var buffer = new byte[1];
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token ?? CancellationToken.None);
            cts.CancelAfter(ReadTimeout);
            var r= await _port.BaseStream.ReadAsync(buffer, 0, 1, cts.Token);
            if (r < 1) return 0;
            return buffer[0];
        }

        public bool HasPendingReads => _port.BytesToRead > 0;
        public DateTime? LastReadTime { get; private set; }
        public async Task<bool> TryRead(byte[] buffer, CancellationToken? token = null)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token ?? CancellationToken.None);
            cts.CancelAfter(ReadTimeout);
            var r = await _port.BaseStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
            return r > 0;
        }

        public Task Write(byte[] data, CancellationToken? token=null) => Write(data, 0, data.Length,token);

        public async Task Write(byte[] data, int offset, int count, CancellationToken? token = null)
        {
            DateTime writeStart;
            int toRead;
            do
            {
                writeStart = DateTime.Now;
                toRead = _port.BytesToRead;
                await _port.BaseStream.WriteAsync(data, offset, count, token ?? CancellationToken.None)
                    .ConfigureAwait(false);
//                await _port.BaseStream.FlushAsync(token ?? CancellationToken.None)
//                    .ConfigureAwait(false); ;
                if ((LastReadTime > writeStart || _port.BytesToRead != toRead))
                {
                    _log.LogDebug($"Write Collision, Rewriting ({_port.BytesToRead} bytes current != {toRead} previous ||LastWrite: {LastReadTime.Value.Subtract(writeStart).TotalSeconds} seconds)");
                }
            } while (LastReadTime > writeStart || _port.BytesToRead != toRead);

            //_port.BaseStream.WriteAsync(data, offset, count);
//            return Task.CompletedTask;
        }

        public async Task Flush(CancellationToken? token) => 
            await _port.BaseStream.FlushAsync(token ?? CancellationToken.None).ConfigureAwait(false);

        public void Close()
        {
            _port?.Dispose();
            _port = null;
        }

        public void Dispose() => Close();
    }
}