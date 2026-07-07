using EQX.Core.Communication;
using System.IO.Ports;

namespace EQX.Device.Regulator
{
    public class ITVRegulatorRC : RegulatorBase
    {
        public ITVRegulatorRC(int id, string name ,double maxPressure, string comPort , int baudRate) : base(id, name)
        {
            Id = id;
            Name = name;
            _maxPressure = maxPressure;

            serialCommunicator = new SerialCommunicator(id, name, comPort, baudRate, Parity.None, 8, StopBits.One);
        }
        public override bool Connect()
        {
            return serialCommunicator.Connect();
        }

        public override bool Disconnect()
        {
            return serialCommunicator.Disconnect();
        }

        public override bool IsConnected => serialCommunicator.IsConnected;

        public override bool SetPressure(double value)
        {
            if(IsConnected == false) return false;

            if (value < 0 ) value = 0;
            if (value > _maxPressure) value = _maxPressure;

            int decValue = (int)((value / _maxPressure) * 1023);
            serialCommunicator.Write($"SET {decValue}\r\n");

            return serialCommunicator.Read().Replace("\r", "").Replace("\n", "") == decValue.ToString() ;
        }

        public override bool DecreasePressure()
        {
            if (IsConnected == false) return false;

            serialCommunicator.Write("DEC\r\n");
            return true;
        }

        public override bool IncreasePressure()
        {
            if (IsConnected == false) return false;

            serialCommunicator.Write("INC\r\n");
            return true;
        }

        public override double GetPressure()
        {
            if (IsConnected == false) return 0.0;

            serialCommunicator.Write("MON\r\n");

            string strResponsePressure = serialCommunicator.Read().Replace("\r", "").Replace("\n", "");
            if (int.TryParse(strResponsePressure, out int decValue))
            {
                return (decValue / 1023.0) * _maxPressure;
            }
            else
            {
                return 0.0;
            }
        }
        #region Privates
        private SerialCommunicator serialCommunicator;
        private double _maxPressure;
        #endregion
    }
}
