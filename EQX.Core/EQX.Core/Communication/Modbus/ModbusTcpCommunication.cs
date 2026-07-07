using NModbus;
using System.Net.Sockets;

namespace EQX.Core.Communication.Modbus
{
    public class ModbusTcpCommunication : IModbusCommunication
    {
        public bool IsConnected => ModbusMaster != null && _tcpClient.Connected;
        public IModbusMaster ModbusMaster { get; private set; }
        private readonly string _ipAddress;
        private readonly int _port;
        private TcpClient _tcpClient;
        public ModbusTcpCommunication(string ipAddress = "127.0.0.1", int port = 502)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        public bool Connect()
        {
            try
            {
                _tcpClient = new TcpClient(_ipAddress, _port);

                if (_tcpClient.Connected == false)
                {
                    _tcpClient.Connect(_ipAddress, _port);
                }

                var factory = new ModbusFactory();
                ModbusMaster = factory.CreateMaster(_tcpClient);

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
                _tcpClient.Close();
                ModbusMaster = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
