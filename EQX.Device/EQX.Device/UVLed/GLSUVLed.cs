using EQX.Core.Communication;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EQX.Device.UVLed
{
    public class GLSUVLed : UVLedBase
    {
        private readonly SerialCommunicator _serialCommunicator;

        public override bool IsConnected => _serialCommunicator.IsConnected;

        public GLSUVLed(string name, int id, SerialCommunicator serialCommunicator) : base(name, id)
        {
            Name = name;
            Id = id;
            _serialCommunicator = serialCommunicator;
        }

        public override bool Connect()
        {
            if (IsConnected)
                return true;
            return _serialCommunicator.Connect();
        }

        public override bool Disconnect()
        {
            if (!IsConnected)
                return true;
            return _serialCommunicator.Disconnect();
        }

        public override bool AlarmCheck()
        {
            return base.AlarmCheck();
        }

        public override void LedOnTime(double second)
        {
            _serialCommunicator.Write(BuildCommand($"ONTIME{(int)Math.Round(second * 10):D3}"));
        }

        public override void SetValue(double value)
        {
            _serialCommunicator.Write(BuildCommand($"SETALL{value.ToString("F1").Replace(",", ".")}"));

            string returnValue = _serialCommunicator.Read();
        }

        public override void SetAll(bool isOn)
        {
            if (isOn)
            {
                _serialCommunicator.Write(BuildCommand("ALLON"));
            }
            else
            {
                _serialCommunicator.Write(BuildCommand("ALLOFF"));
            }
        }

        private byte[] BuildCommand(string command)
        {
            return new byte[] { 0x02 }.Concat(Encoding.ASCII.GetBytes(command))
                                      .Concat(new byte[] { 0x03 })
                                      .ToArray();
        }
    }
}
