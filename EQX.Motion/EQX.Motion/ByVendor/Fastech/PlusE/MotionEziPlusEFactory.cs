using EQX.Core.Motion;

namespace EQX.Motion
{
    public class MotionEziPlusEFactory : IMotionFactory<IMotion>
    {
        public IMotion Create(int id, string name, IMotionParameter parameter)
        {
            return new MotionEziPlusE(id, name, parameter);
        }
    }

    public class MotionEziPlusECustomFactory : IMotionFactory<IMotion>
    {
        public IMotion Create(int id, string name, IMotionParameter parameter)
        {
            return new MotionEziPlusECustom(id, name, parameter);
        }
    }
}
