using EQX.Core.Common;

namespace EQX.Core.InOut.Analog
{
    public interface IAOutput : IIdentifier
    {
        double Volt { get; set; }
    }
}
