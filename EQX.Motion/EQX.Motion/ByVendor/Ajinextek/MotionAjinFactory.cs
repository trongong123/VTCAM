using EQX.Core.Motion;
using EQX.Motion.ByVendor.Ajinextek;
using EQX.Motion.ByVendor.Inovance;

namespace EQX.Motion
{
    public class MotionAjinFactory : IMotionFactoryWithMaster<IMotion>
    {
        public IMotionMaster MotionMaster { get; set; }

        public MotionAjinFactory(IMotionMaster motionMaster)
        {
            MotionMaster = motionMaster;
        }

        public IMotion Create(int id, string name, IMotionParameter parameter)
        {
            if (parameter.GetType() != typeof(MotionAjinParameter))
            {
                throw new ArrayTypeMismatchException("MotionAjinParameter required");
            }

            return new MotionAjin(id, name, (MotionAjinParameter)parameter) { MotionMaster = (MotionMasterAjin)(this.MotionMaster) };
        }
    }
}
