namespace EQX.Device.CognexDataMan150X
{
    public class SimulationDataManBarCodeScanner : BarCodeScannerBase
    {
        public SimulationDataManBarCodeScanner(int id, string name) : base(id, name)
        {
        }

        public override bool IsConnected => true;

        public override bool Connect()
        {
            return true;
        }

        public override bool Disconnect()
        {
            return true;
        }

        public override bool SendCommand(string command)
        {
            return true;
        }
    }
}

