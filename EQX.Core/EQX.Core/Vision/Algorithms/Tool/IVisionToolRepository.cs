namespace EQX.Core.Vision.Algorithms
{
    public interface IVisionToolRepository
    {
        IEnumerable<IVisionTool> GetAll();
    }
}
