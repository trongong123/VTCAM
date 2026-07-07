using EQX.Core.Common;
using EQX.Core.Communication.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Device.Indicator
{
    public interface IIndicator : IHandleConnection, IIdentifier
    {
        int ErrorCode { get; }
        double Temperature { get; }
        double Humidity { get; }
    }
}
