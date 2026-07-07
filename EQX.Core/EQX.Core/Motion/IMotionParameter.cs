using EQX.Core.Common;

namespace EQX.Core.Motion
{
    public interface IMotionParameter : INameable
    {
        /// <summary>
        /// PulsePerUnit = <see cref="Pulse"/> / <see cref="Unit"/>
        /// </summary>
        int Pulse { get; set; }
        /// <summary>
        /// PulsePerUnit = <see cref="Pulse"/> / <see cref="Unit"/>
        /// </summary>
        uint Unit { get; set; }

        /// <summary>
        /// Motion (default) work velocity
        /// </summary>
        double Velocity { get; set; }

        /// <summary>
        /// Reduce gear ratio
        /// </summary>
        uint ReduceGearRatio { get; set; }

        /// <summary>
        /// Motion max velocity
        /// </summary>
        double MaxVelocity { get; set; }

        /// <summary>
        /// Acceleration, it can be mm/s or msec, depend on third party support
        /// </summary>
        double Acceleration { get; set; }
        /// <summary>
        /// Deceleration, it can be mm/s or msec, depend on third party support
        /// </summary>
        double Deceleration { get; set; }
    }
}