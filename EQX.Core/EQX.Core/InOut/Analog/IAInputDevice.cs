using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface IAInputDevice : IIdentifier, IHandleConnection
    {
        List<IAInput> AnalogInputs { get; }
        bool Initialize();

        double GetVolt(int channel);
        double GetCurrent(int channel);
    }
}
