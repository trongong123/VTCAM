using EQX.Core.Communication;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace EQX.Device.CognexDataMan150X
{
    public class DataMan150XBarCodeScanner : BarCodeScannerBase, IDisposable
    {
        private readonly SerialCommunicator _serialCommunicator;
        private readonly object _lockObject = new object();
        private bool _disposed = false;
        private Task? _readTask;
        private CancellationTokenSource? _cancellationTokenSource;

        public override bool IsConnected => _serialCommunicator.IsConnected;

        public DataMan150XBarCodeScanner(int id, string name, SerialCommunicator serialCommunicator) : base(id, name)
        {
            _serialCommunicator = serialCommunicator ?? throw new ArgumentNullException(nameof(serialCommunicator));
        }

        public override bool Connect()
        {
            if (IsConnected)
            {
                return false;
            }

            lock (_lockObject)
            {
                try
                {
                    if (!_serialCommunicator.Connect())
                    {
                        return false;
                    }

                    _cancellationTokenSource = new CancellationTokenSource();
                    _readTask = Task.Run(() => ReadDataContinuously(_cancellationTokenSource.Token));

                    return true;
                }
                catch
                {
                    _serialCommunicator.Disconnect();
                    return false;
                }
            }
        }

        public override bool Disconnect()
        {
            lock (_lockObject)
            {
                _cancellationTokenSource?.Cancel();

                var result = _serialCommunicator.Disconnect();

                _readTask?.Wait(1000);
                _readTask = null;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                return result;
            }
        }

        public override bool SendCommand(string command)
        {
            if (!IsConnected)
            {
                return false;
            }

            lock (_lockObject)
            {
                try
                {
                    _serialCommunicator.WriteLine(command);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void ReadDataContinuously(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                try
                {
                    if (_serialCommunicator.IsConnected)
                    {
                        string data = _serialCommunicator.Read();
                        if (!string.IsNullOrEmpty(data))
                        {
                            OnRawDataReceived(data);
                            ProcessBarcodeData(data);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                    }
                }
            }
        }

        private void ProcessBarcodeData(string data)
        {
            string[] lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                if (string.IsNullOrEmpty(trimmedLine) || 
                    trimmedLine.StartsWith("OK", StringComparison.OrdinalIgnoreCase) ||
                    trimmedLine.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase) ||
                    trimmedLine.StartsWith("FIRMWARE", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (trimmedLine.Length > 0 && trimmedLine.Length < 200)
                {
                    OnBarcodeReceived(trimmedLine);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }
                _disposed = true;
            }
        }
    }
}

