using EQX.Core.Common;

namespace EQX.Core.Device.Regulator
{
    public interface IRegulator : IIdentifier, IHandleConnection
    {
        bool SetPressure(double value);
        bool IncreasePressure();
        bool DecreasePressure();
        double GetPressure();
    }
}
