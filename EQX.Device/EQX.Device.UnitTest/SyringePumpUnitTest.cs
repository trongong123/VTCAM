using EQX.Core.Communication;
using EQX.Core.Communication.Modbus;
using EQX.Core.Device.Indicator;
using EQX.Core.Device.SpeedController;
using EQX.Core.Device.SyringePump;
using EQX.Device.Indicator;
using EQX.Device.Regulator;
using EQX.Device.SpeedController;
using EQX.Device.SyringePump;
using EQX.Device.Torque;
using System.Diagnostics;

namespace EQX.Device.UnitTest
{
    public class SyringePumpUnitTest
    {
        private async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
        {
            while (!condition())
            {
                await Task.Delay(checkIntervalMs); // tránh CPU 100%
            }
        }

        [Fact]
        public async Task PSD4SyringePumpTest()
        {
            SerialCommunicator serialCommunicator = new SerialCommunicator(1, "SyringePumpSerialCommunicator", "COM17", 38400);

            serialCommunicator.Connect();
            ISyringePump PSD4SyringePump = new PSD4SyringePump("AFCleanLeftSyringePump", 2, serialCommunicator, 1.0);

            PSD4SyringePump.SetSpeed(20);

            PSD4SyringePump.Initialize();

            Task.WaitAny(WaitUntilAsync(() => PSD4SyringePump.IsReady()), Task.Delay(10000));

            bool isReady = PSD4SyringePump.IsReady();

            Assert.True(isReady);

            //PSD4SyringePump.Dispense(0.6, 5);

            serialCommunicator.Disconnect();
        }

        [Fact]
        public void NEOSHSDIndicatorTest()
        {
            IModbusCommunication modbusCommunication = new ModbusRTUCommunication("COM7", 115200);

            NEOSHSDIndicator nEOSHSDIndicator = new NEOSHSDIndicator(1, "Indicator Test", modbusCommunication);
            modbusCommunication.Connect();

            Assert.True(nEOSHSDIndicator.IsConnected);
            Assert.True(nEOSHSDIndicator.Temperature > 25.0);
            Assert.True(nEOSHSDIndicator.Humidity > 47.0);
        }

        [Fact]
        public async Task RollerTest()
        {
            IModbusCommunication modbusCommunication = new ModbusRTUCommunication("COM15", 38400);

            BD201SRollerController roller = new BD201SRollerController(15, "Indicator Test", modbusCommunication);
            roller.Connect();

            Assert.True(roller.IsConnected);

            roller.Run();
            await Task.Delay(2000);
            roller.Stop();
        }

        [Fact]
        public void ITVRegulatorTest()
        {
            ITVRegulatorRC regulatorRC = new ITVRegulatorRC(1, "Regulator Test", 0.9, "COM11", 9600);

            regulatorRC.Connect();

            Assert.True(regulatorRC.IsConnected);

            regulatorRC.SetPressure(0.0);
            Trace.WriteLine(regulatorRC.GetPressure());
        }

        [Fact]
        public async Task DX3000Test()
        {
            IModbusCommunication modbusCommunication = new ModbusRTUCommunication("COM16", 115200);

            DX3000TorqueController dx3000 = new DX3000TorqueController(9, "DX3000 Test", modbusCommunication);
            dx3000.Connect();

            Assert.True(dx3000.IsConnected);

            int speed = 35;

            dx3000.SetTorque(speed);
            Assert.Equal(speed, dx3000.GetValue());
            dx3000.Run();
            await Task.Delay(10000);
            dx3000.Stop();
        }
    }
}