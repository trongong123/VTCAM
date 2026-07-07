namespace EQX.Motion
{
    public class SimulationMotionMaster : MotionMasterBase
    {
        public override bool Connect()
        {
            IsConnected = true;
            return true;
        }

        public override bool Disconnect()
        {
            IsConnected = false;
            return true;
        }
    }
}
