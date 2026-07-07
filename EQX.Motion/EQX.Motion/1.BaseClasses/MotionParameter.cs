using EQX.Core.Motion;

namespace EQX.Motion
{
    public class MotionParameter : IMotionParameter
    {
        public string Name { get; set; } = string.Empty;

        public int Pulse { get; set; } = 1;
        public uint Unit { get; set; } = 1;

        public uint ReduceGearRatio { get; set; } = 1;

        public double Velocity { get; set; }
        public double MaxVelocity { get; set; }

        public double Acceleration { get; set; }
        public double Deceleration { get; set; }
    }
}
