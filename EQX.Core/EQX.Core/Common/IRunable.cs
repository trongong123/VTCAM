namespace EQX.Core.Common
{
    public interface IRunnable
    {
        ERunState State { get; }
        void Run();
    }
}
