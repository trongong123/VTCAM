using EQX.Core.Motion;

namespace EQX.Motion
{
    public interface IMotionFactory<IMotion>
    {
        IMotion Create(int id, string name, IMotionParameter parameter);
    }

    public interface IMotionFactoryWithMaster<IMotion> : IMotionFactory<IMotion>
    {
        IMotionMaster MotionMaster { get; }
    }
}
