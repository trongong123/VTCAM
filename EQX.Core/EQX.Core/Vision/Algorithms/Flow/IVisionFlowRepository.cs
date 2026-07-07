namespace EQX.Core.Vision.Algorithms
{
    public interface IVisionFlowRepository
    {
        event Action VisionFlowsInitialized;

        void Init(List<IVisionFlow> visionFlows);
        IEnumerable<IVisionFlow> GetAll();
    }
}
