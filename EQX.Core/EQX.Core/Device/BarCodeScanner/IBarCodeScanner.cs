using EQX.Core.Common;

namespace EQX.Core.Device.BarCodeScanner
{
    public interface IBarCodeScanner : IHandleConnection, IIdentifier
    {
        event EventHandler<string>? BarcodeReceived;
        event EventHandler<string>? RawDataReceived;

        bool SendCommand(string command);
        bool Trigger();
        bool GetResult();
        bool GetFirmware();
    }
}

