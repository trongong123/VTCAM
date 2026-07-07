using EQX.Core.Common;
using EQX.Core.Communication.Modbus;

namespace EQX.Core.Device.Indicator
{
    public interface IModbusDevice<EKeys> : IHandleConnection, IIdentifier where EKeys : Enum
    {
        object this[EKeys key] { get; set; }
    }
}
