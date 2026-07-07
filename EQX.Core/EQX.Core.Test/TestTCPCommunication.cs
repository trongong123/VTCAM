using EQX.Core.Common;
using EQX.Core.Communication;
using System.Diagnostics;
using System.Net;

namespace EQX.Core.Test
{
    [TestClass]
    public class TestTCPCommunication
    {
        [TestMethod]
        public void TestConnectRepeat()
        {
            var communicator = new TCPBasicCommunicator(1, "Test", IPAddress.Parse("192.168.1.191"), 54600);

            communicator.Connect();
            communicator.Connect();
            Trace.WriteLine(communicator.IsConnected);
            communicator.Disconnect();
            Trace.WriteLine(communicator.IsConnected);
            communicator.Connect();
            //communicator.Connect();
        }
    }
}