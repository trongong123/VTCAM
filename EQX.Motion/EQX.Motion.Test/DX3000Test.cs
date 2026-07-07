using EQX.Core.Communication.Modbus;
using EQX.Core.TorqueController;
using System.IO.Ports;
using System.Threading.Tasks;

namespace EQX.Motion.Test
{
    public class DX3000Test
    {
        [Fact]
        public void ConnectTest()
        {
            IModbusCommunication communication = new ModbusRTUCommunication("COM2", 9600, Parity.None, 8, StopBits.One);

            communication.Connect();

            Assert.True(communication.IsConnected);
        }

        [Fact]
        public async Task DataTransferTest()
        {
            IModbusCommunication communication = new ModbusRTUCommunication("COM2", 9600, Parity.None, 8, StopBits.One);

            communication.Connect();
        }
    }
}