namespace EQX.Motion.ByVendor.AdvantechMotion
{
    public class MotionAdvantechParameter : MotionParameter
    {
        public double MinVelocity { get; set; }
        public double JogMinVelocity { get; set; }
        public double JogMaxVelocity { get; set; }
        public double JogAcceleration { get; set; }
        public double JogDeceleration { get; set; }

        #region Home parameter
        public uint HomeDirect { get; set; }
        public uint HomeMode { get; set; }
        public double HomeOffset { get; set; }
        public double HomeOffserVelocity { get; set; }
        public double HomeVelocity { get; set; }
        public double HomeAcceleration { get; set; }
        public double HomeDeceleration { get; set; }
        #endregion
    }
}
