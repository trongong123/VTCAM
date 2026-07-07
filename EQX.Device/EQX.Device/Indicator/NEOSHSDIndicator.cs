using EQX.Core.Communication.Modbus;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Device.Indicator
{
    public enum ENeosHsdProps
    {
        Temperature,
        Humidity,
    }

    public class NEOSHSDIndicator : ModbusDevice<ENeosHsdProps>
    {
        public NEOSHSDIndicator(int id, string name, IModbusCommunication modbusCommunication)
            : base(id, name, modbusCommunication)
        {
            KeyAddressPairs = new Dictionary<ENeosHsdProps, ushort>
            {
                { ENeosHsdProps.Temperature, 0xA004 },
                { ENeosHsdProps.Humidity, 0xA006 },
            };
            KeyNumberOfBits = new Dictionary<ENeosHsdProps, uint>
            {
                { ENeosHsdProps.Temperature, 16 },
                { ENeosHsdProps.Humidity, 16 },
            };
        }

        public double Temperature => Convert.ToDouble(this[ENeosHsdProps.Temperature]) / 100.0;

        public double Humidity => Convert.ToDouble(this[ENeosHsdProps.Humidity]) / 100.0;
    }
}
