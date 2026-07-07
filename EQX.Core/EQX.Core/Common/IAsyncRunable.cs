namespace EQX.Core.Common
{
    public interface IAsyncRunnable
    {
        ERunState State { get; }
        Task RunAsync(int timeoutMs);
        long ExecuteTime { get; }
    }
}
