using EQX.Core.Common;

namespace EQX.Core.InOut
{
    /// <summary>
    /// Digital output device (multiple output contact)
    /// </summary>
    public interface IDOutputDevice : IIdentifier, IHandleConnection
    {
        bool this[int index] { get; set; }
        List<IDOutput> Outputs { get; }

        bool Initialize();
        void ClearOutputs();
    }

    /// <summary>
    /// Digital output device (multiple output contact)
    /// </summary>
    public interface IDOutputDevice<TOutputs> : IDOutputDevice where TOutputs : Enum
    {
        IDOutput this[TOutputs index] { get; }
    }
}
