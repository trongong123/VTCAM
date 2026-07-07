using EQX.Core.Motion;
using EQX.ThirdParty.Fastech;

namespace EQX.Motion
{
    public class MotionEziPlusECustom : MotionBase
    {
        public MotionEziPlusECustom(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
            nativeLib = new EziPlusEMotionLib(id, name);
        }

        protected override bool ActualInitialization()
        {
            return true;
        }

        protected override bool ActualConnect()
        {
            bool result = nativeLib.Connect();
            IsConnected = result;
            return result;
        }

        protected override bool ActualDisconnect()
        {
            bool result = nativeLib.Disconnect();
            IsConnected = false;
            return result;
        }

        protected override bool ActualMotionOff()
        {
            return nativeLib.MotionOff() == EziPlusEMotionLib.FMM_OK;
        }

        protected override bool ActualMotionOn()
        {
            return nativeLib.MotionOn() == EziPlusEMotionLib.FMM_OK;
        }

        protected override bool ActualSearchOrigin()
        {
            return nativeLib.SearchOrigin() == EziPlusEMotionLib.FMM_OK;
        }

        protected override bool ActualMoveAbs(double position, double velocity)
        {
            return nativeLib.MoveAbs((int)(position * 1000), (uint)(velocity * 1000)) == EziPlusEMotionLib.FMM_OK;
        }

        protected override bool ActualMoveInc(double position, double velocity)
        {
            return nativeLib.MoveInc((int)(position * 1000), (uint)(velocity * 1000)) == EziPlusEMotionLib.FMM_OK;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            nativeLib.MoveJog((uint)speed, isForward);
        }

        public override bool Stop(bool forceStop = true)
        {
            return forceStop ? nativeLib.EMGStop() == EziPlusEMotionLib.FMM_OK
                             : nativeLib.SoftStop() == EziPlusEMotionLib.FMM_OK;
        }

        public override bool AlarmReset()
        {
            return nativeLib.AlarmReset() == EziPlusEMotionLib.FMM_OK;
        }

        protected override void UpdateAxisStatus()
        {
            uint inputStatus = 0, outputStatus = 0, axisStatus = 0;
            int cmdPosition = 0, actPosition = 0, posErr = 0, actVel = 0, currentRunPT = 0;

            if (nativeLib.GetAllStatus(ref inputStatus, ref outputStatus, ref axisStatus,
                ref cmdPosition, ref actPosition, ref posErr, ref actVel, ref currentRunPT)
                    == EziPlusEMotionLib.FMM_OK)
            {
                ((MotionStatus)Status).IsHomeDone = (axisStatus & EziPlusEMotionLib.FFLAG_ORIGINRETOK) > 0;
                ((MotionStatus)Status).IsMotionOn = (axisStatus & EziPlusEMotionLib.FFLAG_SERVOON) > 0;
                ((MotionStatus)Status).IsMotioning = (axisStatus & EziPlusEMotionLib.FFLAG_MOTIONING) > 0;

                ((MotionStatus)Status).CommandPosition = PulseToMM(cmdPosition);
                ((MotionStatus)Status).ActualPosition = PulseToMM(actPosition);
                ((MotionStatus)Status).PositionError = PulseToMM(posErr);
                ((MotionStatus)Status).ActualVelocity = PulseToMM(actVel);

                ((MotionStatus)Status).HwPosLimitDetect = (axisStatus & EziPlusEMotionLib.FFLAG_HWPOSILMT) > 0;
                ((MotionStatus)Status).HwNegLimitDetect = (axisStatus & EziPlusEMotionLib.FFLAG_HWNEGALMT) > 0;
            }
            IsConnected = true;
            byte alarmType = 0;
            if (nativeLib.GetAlarmType(ref alarmType) == EziPlusEMotionLib.FMM_OK)
            {
                ((MotionStatus)Status).IsAlarm = alarmType != 0;
            }
        }

        //public bool ClearPosition()
        //{
        //    return nativeLib.ClearPosition() == EziPlusEMotionLib.FMM_OK;
        //}

        #region Privates
        private EziPlusEMotionLib nativeLib;
        #endregion
    }
}
