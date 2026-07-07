namespace EQX.Core.Common
{
    public interface IHandleConnection
    {
        bool IsConnected { get; }
        bool Connect();
        bool Disconnect();
    }
}
