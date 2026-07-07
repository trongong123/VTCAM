using EQX.Core.Device.BarCodeScanner;

namespace EQX.Device.CognexDataMan150X
{
    public class BarCodeScannerBase : IBarCodeScanner
    {
        public int Id { get; init; }
        public string Name { get; set; }

        public virtual bool IsConnected { get; protected set; }

        public event EventHandler<string>? BarcodeReceived;
        public event EventHandler<string>? RawDataReceived;

        public BarCodeScannerBase(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual bool SendCommand(string command)
        {
            return true;
        }

        public virtual bool Trigger()
        {
            return SendCommand("TRIGGER ON");
        }

        public virtual bool GetResult()
        {
            return SendCommand("GET RESULT");
        }

        public virtual bool GetFirmware()
        {
            return SendCommand("GET FIRMWARE");
        }

        protected virtual void OnBarcodeReceived(string barcode)
        {
            BarcodeReceived?.Invoke(this, barcode);
        }

        protected virtual void OnRawDataReceived(string data)
        {
            RawDataReceived?.Invoke(this, data);
        }
    }
}

