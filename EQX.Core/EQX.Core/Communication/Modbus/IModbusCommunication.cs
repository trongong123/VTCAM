using EQX.Core.Common;
using NModbus;

namespace EQX.Core.Communication.Modbus
{
    public interface IModbusCommunication : IHandleConnection
    {
        IModbusMaster ModbusMaster { get; }
    }
}
