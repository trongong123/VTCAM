using EQX.Core.Common;

namespace EQX.Core.TorqueController
{
    public enum TorqueControlMode
    {
        SpeedMode = 0,
        TorqueMode = 1
    }

    public interface ITorqueController : IIdentifier, IHandleConnection
    {
        void SetTorque(int torque);
        void SetSpeed(int speed);

        int GetValue();

        void Jog(int speed, bool isForward);

        void Run(bool isForward = true);
        void Stop();

        void ResetAlarm();
    }
}
