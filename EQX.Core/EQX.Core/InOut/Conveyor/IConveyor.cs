using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface IConveyor : INameable
    {
        bool IsError { get; }
        void Run();
        void Stop();
        void RunReverse();
    }
}
