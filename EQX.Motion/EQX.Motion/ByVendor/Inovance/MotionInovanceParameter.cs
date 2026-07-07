namespace EQX.Motion.ByVendor.Inovance
{
    public class MotionInovanceParameter : MotionParameter
    {
        /// <summary>
        /// Encoder Resolution, ex 8,388,608 for 23 bit encoder
        /// </summary>
        public int EncoderResolution { get; set; }
        public int HomeMethod { get; set; }
        public double HomeOffset { get; set; }
        public double HomeAcceleration { get; set; }
        public double HomeLowVelocity { get; set; }
        public double HomeHighVelocity { get; set; }

        public uint AccelUnit { get; set; }
    }
}
