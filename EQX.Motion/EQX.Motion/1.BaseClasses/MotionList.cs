using EQX.Core.Motion;

namespace EQX.Motion
{
    public class MotionList<TEnum> where TEnum : Enum
    {
        public List<IMotion> All { get; }

        public MotionList(IMotionFactory<IMotion> motionFactory, List<IMotionParameter> parameterList)
        {
            var motionList = Enum.GetNames(typeof(TEnum)).ToList();
            var motionIndex = (int[])Enum.GetValues(typeof(TEnum));

            if (parameterList.Count != motionList.Count)
            {
                throw new ArgumentOutOfRangeException("parameterList and Enum count not match");
            }

            All = new List<IMotion>();
            for (int i = 0; i < motionList.Count; i++)
            {
                All.Add(motionFactory.Create(motionIndex[i], motionList[i], parameterList[i]));
            }
        }
    }
}
