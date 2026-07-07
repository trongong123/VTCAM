using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface IAInputParameter
    {
        /// <summary>
        /// Voltage range min value
        /// </summary>
        double MinValue { get; }
        /// <summary>
        /// Voltage range max value
        /// </summary>
        double MaxValue { get; }
    }

    public interface IAInput : IIdentifier
    {
        IAInputParameter Parameter { get; }

        double Volt { get; }
        double Current { get; }
    }
}
