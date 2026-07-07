using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface IDOutput : IIdentifier
    {
        bool Value { get; set; }

        Dictionary<string, Func<bool>>? OutputEnableInterlocks { get; set; }
        void RaiseValueUpdated();
    }
}