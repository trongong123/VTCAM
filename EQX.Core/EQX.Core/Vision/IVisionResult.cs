using OpenCvSharp;

namespace EQX.Core.Vision
{
    public enum EVisionJudge
    {
        NG = -1,
        OK = 1
    }
    public interface IVisionResult
    {
        EVisionJudge Judge { get; }
        Action<Mat> DrawAction { get; set; }
        string ToString();
        void Pixel2mm(double pixelSize);
    }
}
