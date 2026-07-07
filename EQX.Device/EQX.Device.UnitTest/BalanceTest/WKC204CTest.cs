using EQX.Core.Communication;
using EQX.Device.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Device.UnitTest.BalanceTest
{
    public class WKC204CTest
    {
        [Fact]
        public void ConnectTest()
        {
            SerialCommunicator serialCommunicator = new SerialCommunicator(1, "LeftBalance", "COM7", 38400);
            MettlerToledoWKC204C wKC204C = new MettlerToledoWKC204C(serialCommunicator);
            wKC204C.Connect();

            Assert.True(wKC204C.IsConnected);

            wKC204C.Disconnect();
            Assert.False(wKC204C.IsConnected);
        }

        [Fact]
        public async Task GetDataTest()
        {
            SerialCommunicator serialCommunicator = new SerialCommunicator(1, "LeftBalance", "COM7", 38400);
            MettlerToledoWKC204C wKC204C = new MettlerToledoWKC204C(serialCommunicator);
            wKC204C.WeightReceived += WKC204C_WeightReceived;
            wKC204C.Connect();

            Assert.True(wKC204C.IsConnected);

            WeightEventArgs? weightData = await wKC204C.RequestStableWeightAsync();
            Assert.NotNull(weightData);

            if (weightData == null) return;

            Assert.Equal(4.1701, weightData.Weight, 2);
            Assert.Equal("g", weightData.Unit);

            wKC204C.Disconnect();
            Assert.False(wKC204C.IsConnected);
        }

        private void WKC204C_WeightReceived(object? sender, WeightEventArgs e)
        {
            
        }
    }
}
