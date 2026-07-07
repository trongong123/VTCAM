using EQX.Core.Motion;

namespace EQX.Motion
{
    public class MotionMasterBase : IMotionMaster
    {
        protected ulong _DeviceId;

        public int NumberOfDevices { get; init; }

        public ulong ControllerId
        {
            get { return _DeviceId; }
            set { _DeviceId = value; }
        }

        public virtual bool IsConnected { get; protected set; }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }
    }
}
