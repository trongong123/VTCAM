using EQX.Core.Common;

namespace EQX.Core.Device.SpeedController
{
    public interface ISpeedController : IHandleConnection , IIdentifier
    {
        void Start();
        void Stop();
        void SetDirection(bool isCW);
        void SetSpeed(int speed);
        void SetAcceleration(int acceleration);
        void SetDeceleration(int deceleration);
    }
}
