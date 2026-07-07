using EQX.Core.Common;

namespace EQX.Core.Motion
{
    public interface IMotion : IHandleConnection, IIdentifier
    {
        Dictionary<string, Func<bool>>? PositionIncreaseInterlocks { get; set; }
        Dictionary<string, Func<bool>>? PositionDecreaseInterlocks { get; set; }

        IMotionStatus Status { get; }
        IMotionParameter Parameter { get; }

        bool Initialization();

        bool MotionOn();
        bool MotionOff();

        bool SearchOrigin();

        bool MoveInc(double position);
        /// <summary>
        /// Move increase position
        /// </summary>
        /// <param name="position">Unit: mm</param>
        /// <param name="speed">Unit: mm/s</param>
        /// <returns></returns>
        bool MoveInc(double position, double speed);
        bool MoveAbs(double position);
        /// <summary>
        /// Move absolute position
        /// </summary>
        /// <param name="position">Unit: mm</param>
        /// <param name="speed">Unit: mm/s</param>
        /// <returns></returns>
        bool MoveAbs(double position, double speed);

        void MoveJog(double speed, bool isForward);

        bool Stop(bool forceStop = false);

        bool AlarmReset();
        /// <summary>
        /// Is Axis On Position
        /// </summary>
        /// <param name="dPosition">Unit: mm</param>
        /// <returns></returns>
        bool IsOnPosition(double dPosition);

        bool ClearPosition();
    }
}
