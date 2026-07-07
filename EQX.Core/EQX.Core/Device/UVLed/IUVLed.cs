using EQX.Core.Common;

namespace EQX.Core.Device.UVLed
{
    public interface IUVLed : IIdentifier, IHandleConnection
    {
        void SetValue(double value);
        void LedOnTime(double second);
        void SetAll(bool isOn);
        bool AlarmCheck();
    }
}
