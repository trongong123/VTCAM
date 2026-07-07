using EQX.Core.Motion;

namespace EQX.Motion.ByVendor.Inovance
{
    public class MotionInovanceFactory : IMotionFactoryWithMaster<IMotion>
    {
        public IMotionMaster MotionMaster { get; set; }

        public MotionInovanceFactory(IMotionMaster motionMaster)
        {
            MotionMaster = motionMaster;
        }

        public IMotion Create(int id, string name, IMotionParameter parameter)
        {
            if (parameter.GetType() != typeof(MotionInovanceParameter))
            {
                throw new ArrayTypeMismatchException("MotionInovanceParameter required");
            }

            return new MotionInovance(id, name, (MotionInovanceParameter)parameter) { MotionMaster = (MotionMasterInovance)(this.MotionMaster) };
        }
    }
}
