using EQX.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Test
{
    [TestClass]
    public class TCPClientHelperTest
    {
        [TestMethod]
        public void TestClientDictionary()
        {
            TCPEventCommunicator communicator
                = new TCPEventCommunicator(1, "Test client", IPAddress.Parse("192.168.1.111"), 1532);

            TCPEventCommunicator communicator1
                = new TCPEventCommunicator(1, "Test client", IPAddress.Parse("192.168.1.111"), 1532);

            string endPoint = string.Format($"{communicator.IPAddress}:{communicator.Port}");

            Assert.IsNotNull(TCPClientHelpers.GetClient(communicator.IPAddress, (uint)communicator.Port));

            communicator.Disconnect();
            communicator1.Disconnect();

            Assert.IsNull(TCPClientHelpers.GetClient(communicator.IPAddress, (uint)communicator.Port));
        }
    }
}
