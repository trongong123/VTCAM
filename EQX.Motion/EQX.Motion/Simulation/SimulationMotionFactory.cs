using EQX.Core.Motion;

namespace EQX.Motion
{
    public class SimulationMotionFactory : IMotionFactory<IMotion>
    {
        public IMotion Create(int id, string name, IMotionParameter parameter)
        {
            return new SimulationMotion(id, name, parameter);
        }
    }
}
