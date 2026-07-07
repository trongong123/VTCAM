using NModbus;
using System.IO.Ports;
using NModbus.Serial;

namespace EQX.Core.Communication.Modbus
{
    public class ModbusRTUCommunication : IModbusCommunication
    {
        public bool IsConnected
        {
            get
            {
                if (serialPort == null) return false;
                return serialPort.IsOpen;
            }
        }
        public IModbusMaster ModbusMaster { get; private set; }

        public ModbusRTUCommunication(string comPort, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _comPort = comPort;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }

        public bool Connect()
        {
            try
            {
                if (serialPort == null)
                {
                    serialPort = new SerialPort(_comPort, _baudRate, _parity, _dataBits, _stopBits);
                    serialPort.ReadTimeout = 1000;
                    serialPort.WriteTimeout = 1000;
                }

                // Kiểm tra và đóng port nếu đã mở
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    System.Threading.Thread.Sleep(100);
                }

                serialPort.Open();

                ModbusMaster = new ModbusFactory().CreateRtuMaster(serialPort);

                ModbusMaster.Transport.WriteTimeout = 1000;
                ModbusMaster.Transport.ReadTimeout = 1000;
                ModbusMaster.Transport.Retries = 3;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                if (serialPort == null) return true;

                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    System.Threading.Thread.Sleep(100);
                }

                serialPort.Dispose();
                serialPort = null;
                ModbusMaster = null;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Privates
        private readonly string _comPort;
        private readonly int _baudRate;
        private readonly Parity _parity;
        private readonly int _dataBits;
        private readonly StopBits _stopBits;

        private SerialPort serialPort;
        #endregion
    }
}
