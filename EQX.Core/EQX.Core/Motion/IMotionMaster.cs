namespace EQX.Core.Motion
{
    public interface IMotionMaster
    {
        int NumberOfDevices { get; }

        ulong ControllerId { get; }

        bool IsConnected { get; }

        bool Connect();
        bool Disconnect();
    }
}
