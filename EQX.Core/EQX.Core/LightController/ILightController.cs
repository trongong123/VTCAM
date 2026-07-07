using EQX.Core.Common;

namespace EQX.Core.LightController
{
    public interface ILightController : IIdentifier, IHandleConnection
    {
        bool SetLightLevel(int channel, int value);
        bool SetLightStatus(int channel, bool bOnOff);

        int GetLightLevel(int channel);
        bool GetLightStatus(int channel);
    }
}
