using EQX.Motion.ByVendor.Inovance;

namespace EQX.Motion.Test
{
    public class MotionHelperTest
    {
        [Fact]
        public void PositionFactorTest()
        {
            double motorResolution = MotionHelpers.MotorResolution(8388608, 1, 20000);
            double shaftResolution = MotionHelpers.ShaftResolution(8388608, 1, 20000);

            Assert.Equal(4194304, motorResolution);
            Assert.Equal(10000, shaftResolution);
        }

        [Fact]
        public void PositionFactorTest2()
        {
            double motorResolution = MotionHelpers.MotorResolution(1048576, 5, 10);
            double shaftResolution = MotionHelpers.ShaftResolution(1048576, 5, 10);

            Assert.Equal(524288, motorResolution);
            Assert.Equal(1, shaftResolution);
        }
    }
}