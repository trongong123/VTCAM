using EQX.Core.Common;

namespace EQX.Core.Vision.Grabber
{
    public interface ICamera : IHandleConnection, IIdentifier
    {
        bool Initialization();

        GrabData GrabSingle();

        event EventHandler<GrabData> ContinuousImageGrabbed;
        void ContinuousImageGrabStart();
        void ContinuousImageGrabStop();

        double ExposureTime { get; set; }
    }
}
