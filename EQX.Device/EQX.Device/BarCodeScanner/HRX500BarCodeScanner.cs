using EQX.Core.Communication;
using EQX.Device.CognexDataMan150X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EQX.Device.BarCodeScanner
{
    public class HRX500BarCodeScanner : BarCodeScannerBase
    {
        private readonly SerialCommunicator _serialCommunicator;
        private readonly object _lockObject = new object();
        private Task? _readTask;
        private CancellationTokenSource? _cancellationTokenSource;

        public override bool IsConnected => _serialCommunicator.IsConnected;

        public HRX500BarCodeScanner(int id, string name, SerialCommunicator serialCommunicator) : base(id, name)
        {
            _serialCommunicator = serialCommunicator;
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
                    _readTask = Task.Run(async () => ReadDataContinuously(_cancellationTokenSource.Token));

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

        private async Task ReadDataContinuously(CancellationToken cancellationToken)
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
                            OnBarcodeReceived(data);
                        }
                    }
                    await Task.Delay(10);
                }
                catch
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                    }
                }
            }
        }

    }
}
