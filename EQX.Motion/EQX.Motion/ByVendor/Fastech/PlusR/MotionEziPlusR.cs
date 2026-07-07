using EQX.Core.Motion;
using EQX.Motion.ByVendor.Ajinextek;
using EQX.ThirdParty.Fastech;
using Newtonsoft.Json.Linq;
using System;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace EQX.Motion
{
    public class MotionEziPlusRServo : MotionEziPlusR
    {
        public MotionEziPlusRServo(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
        }

        public override bool AlarmReset()
        {
            if (Status.IsMotionOn == true)
            {
                MotionOff();
            }

            bool result = NativeLib.FAS_ServoAlarmReset((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK;

            if (result)
            {
                MotionOn();
            }

            return result;
        }

        protected override void UpdateAxisStatus()
        {
            uint inputStatus = 0, outputStatus = 0, axisStatus = 0;
            int cmdPosition = 0, actPosition = 0, posErr = 0, actVel = 0;
            ushort currentRunPT = 0;

            if (MotionMaster == null) return;

            // TODO: CHECK BUG
            if (NativeLib.FAS_GetAllStatus((byte)MotionMaster.ControllerId, (byte)Id, ref inputStatus, ref outputStatus, ref axisStatus,
                ref cmdPosition, ref actPosition, ref posErr, ref actVel, ref currentRunPT)
                    == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).IsMotioning =
                    (axisStatus & EziPlusRServoDefine.FFLAG_MOTIONING) > 0 ||
                    (axisStatus & EziPlusRServoDefine.FFLAG_MOTIONCONST) > 0 ||
                    (axisStatus & EziPlusRServoDefine.FFLAG_MOTIONDECEL) > 0 ||
                    (axisStatus & EziPlusRServoDefine.FFLAG_MOTIONACCEL) > 0;
                ((MotionStatus)Status).IsHomeDone =
                    (axisStatus & EziPlusRServoDefine.FFLAG_ORIGINRETOK) > 0 &&
                    (axisStatus & EziPlusRServoDefine.FFLAG_ORIGINRETURNING) == 0;
                ((MotionStatus)Status).IsMotionOn = (axisStatus & EziPlusRServoDefine.FFLAG_SERVOON) > 0;
                
                ((MotionStatus)Status).CommandPosition = PulseToMM(cmdPosition);
                ((MotionStatus)Status).ActualPosition = PulseToMM(actPosition);
                ((MotionStatus)Status).PositionError = PulseToMM(posErr);
                ((MotionStatus)Status).ActualVelocity = PulseToMM(actVel);

                ((MotionStatus)Status).HwPosLimitDetect = (axisStatus & EziPlusRServoDefine.FFLAG_HWPOSILMT) > 0;
                ((MotionStatus)Status).HwNegLimitDetect = (axisStatus & EziPlusRServoDefine.FFLAG_HWNEGALMT) > 0;
            }

            byte alarmType = 0;
            if (NativeLib.FAS_GetAlarmType((byte)MotionMaster.ControllerId, (byte)Id, ref alarmType) == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).IsAlarm = alarmType != 0;
            }
            if (NativeLib.FAS_IsSlaveExist((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).IsConnected = true;
            }
            else
            {
                ((MotionStatus)Status).IsConnected = false;

            }
        }
    }

    public class MotionEziPlusRStep : MotionEziPlusR
    {
        public MotionEziPlusRStep(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
        }

        public override bool AlarmReset()
        {
            if (Status.IsMotionOn == true)
            {
                MotionOff();
            }

            bool result = NativeLib.FAS_StepAlarmReset((byte)MotionMaster.ControllerId, (byte)Id, 1) == NativeLib.FMM_OK;

            Thread.Sleep(30);

            result &= NativeLib.FAS_StepAlarmReset((byte)MotionMaster.ControllerId, (byte)Id, 0) == NativeLib.FMM_OK;

            if (result)
            {
                MotionOn();
            }

            return result;
        }

        protected override void UpdateAxisStatus()
        {
            uint inputStatus = 0, outputStatus = 0, axisStatus = 0;
            int cmdPosition = 0, actPosition = 0, posErr = 0, actVel = 0;
            ushort currentRunPT = 0;

            if (NativeLib.FAS_GetAxisStatus((byte)MotionMaster.ControllerId, (byte)Id, ref axisStatus)
                    == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).IsMotionOn = (axisStatus & EziPlusRStepDefine.FFLAG_ALARMRESET) == 0;
                ((MotionStatus)Status).IsAlarm = (axisStatus & EziPlusRStepDefine.FFLAG_ERRORALL) > 0;
                ((MotionStatus)Status).IsMotioning =
                    (axisStatus & EziPlusRStepDefine.FFLAG_MOTIONING) > 0 ||
                    (axisStatus & EziPlusRStepDefine.FFLAG_MOTIONCONST) > 0 ||
                    (axisStatus & EziPlusRStepDefine.FFLAG_MOTIONDECEL) > 0 ||
                    (axisStatus & EziPlusRStepDefine.FFLAG_MOTIONACCEL) > 0;
                ((MotionStatus)Status).IsHomeDone =
                    (axisStatus & EziPlusRStepDefine.FFLAG_ORIGINRETOK) > 0 &&
                    (axisStatus & EziPlusRStepDefine.FFLAG_ORIGINRETURNING) == 0;

                ((MotionStatus)Status).HwPosLimitDetect = (axisStatus & EziPlusRStepDefine.FFLAG_HWPOSILMT) > 0;
                ((MotionStatus)Status).HwNegLimitDetect = (axisStatus & EziPlusRStepDefine.FFLAG_HWNEGALMT) > 0;
            }
            
            if (NativeLib.FAS_GetCommandPos((byte)MotionMaster.ControllerId, (byte)Id, ref cmdPosition) == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).CommandPosition = PulseToMM(cmdPosition);
                ((MotionStatus)Status).ActualPosition = PulseToMM(cmdPosition);
            }

            return;
            if (NativeLib.FAS_GetActualVel((byte)MotionMaster.ControllerId, (byte)Id, ref actVel) == NativeLib.FMM_OK)
            {
                ((MotionStatus)Status).ActualVelocity = PulseToMM(actVel);
            }
            if (NativeLib.FAS_GetPosError((byte)MotionMaster.ControllerId, (byte)Id, ref posErr) == EziPlusEMotionLib.FMM_OK)
            {
                ((MotionStatus)Status).PositionError = PulseToMM(posErr);
            }
        }
    }

    public class MotionEziPlusR : MotionBase
    {
        public MotionMasterEziPlusR MotionMaster { get; init; }

        public override bool IsConnected => MotionMaster.IsConnected;

        public MotionEziPlusR(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
        }

        protected override bool ActualInitialization()
        {
            return true;
        }

        protected override bool ActualMotionOff()
        {
            return NativeLib.FAS_ServoEnable((byte)MotionMaster.ControllerId, (byte)Id, 0) == NativeLib.FMM_OK;
        }

        protected override bool ActualMotionOn()
        {
            return NativeLib.FAS_ServoEnable((byte)MotionMaster.ControllerId, (byte)Id, 1) == NativeLib.FMM_OK;
        }

        protected override bool ActualSearchOrigin()
        {
            return NativeLib.FAS_MoveOriginSingleAxis((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK;
        }

        int retryCount = 0;

        protected override bool ActualMoveAbs(double position, double velocity)
        {
            int retCode = NativeLib.FAS_MoveSingleAxisAbsPos((byte)MotionMaster.ControllerId, (byte)Id, MMtoPulse(position), (uint)MMtoPulse(velocity));
            if (retCode == NativeLib.FMM_OK)
            {
                retryCount = 0;
                return true;
            }

            if (retryCount < 10)
            {
                bool isMotioning = Status.IsMotioning;
                retryCount++;
                Thread.Sleep(20);
                return MoveAbs(position, velocity);
            }

            retryCount = 0;
            return false;
        }

        protected override bool ActualMoveInc(double position, double velocity)
        {
            return NativeLib.FAS_MoveSingleAxisIncPos((byte)MotionMaster.ControllerId, (byte)Id, MMtoPulse(position), (uint)MMtoPulse(velocity)) == NativeLib.FMM_OK;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            NativeLib.FAS_MoveVelocity((byte)MotionMaster.ControllerId, (byte)Id, (uint)MMtoPulse(speed), isForward ? 1 : 0);
        }

        public override bool Stop(bool forceStop = true)
        {
            return forceStop ? NativeLib.FAS_EmergencyStop((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK
                             : NativeLib.FAS_MoveStop((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK;
        }

        public override bool ClearPosition()
        {
            return NativeLib.FAS_ClearPosition((byte)MotionMaster.ControllerId, (byte)Id) == NativeLib.FMM_OK;
        }

        //protected override void UpdateAxisStatus()
        //{
        //    uint inputStatus = 0, outputStatus = 0, axisStatus = 0;
        //    int cmdPosition = 0, actPosition = 0, posErr = 0, actVel = 0;
        //    ushort currentRunPT = 0;

        //    if (NativeLib.FAS_GetAxisStatus((byte)MotionMaster.ControllerId, (byte)Id, ref axisStatus)
        //            == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).IsHomeDone = (axisStatus & EziPlusEMotionLib.FFLAG_ORIGINRETOK) > 0;
        //        ((MotionStatus)Status).IsMotionOn = (axisStatus & EziPlusEMotionLib.FFLAG_SERVOON) > 0;
        //        ((MotionStatus)Status).IsMotioning = (axisStatus & EziPlusEMotionLib.FFLAG_MOTIONING) > 0;

        //        ((MotionStatus)Status).HwPosLimitDetect = (axisStatus & EziPlusEMotionLib.FFLAG_HWPOSILMT) > 0;
        //        ((MotionStatus)Status).HwNegLimitDetect = (axisStatus & EziPlusEMotionLib.FFLAG_HWNEGALMT) > 0;
        //    }

        //    if (NativeLib.FAS_GetCommandPos((byte)MotionMaster.ControllerId, (byte)Id, ref cmdPosition)
        //            == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).CommandPosition = PulseToMM(cmdPosition);
        //    }
        //    if (NativeLib.FAS_GetActualPos((byte)MotionMaster.ControllerId, (byte)Id, ref actPosition)
        //            == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).ActualPosition = PulseToMM(actPosition);
        //    }
        //    if (NativeLib.FAS_GetPosError((byte)MotionMaster.ControllerId, (byte)Id, ref posErr)
        //            == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).PositionError = PulseToMM(posErr);
        //    }
        //    if (NativeLib.FAS_GetActualVel((byte)MotionMaster.ControllerId, (byte)Id, ref actVel)
        //            == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).ActualVelocity = PulseToMM(actVel);
        //    }


        //    byte alarmType = 0;
        //    if (NativeLib.FAS_GetAlarmType((byte)MotionMaster.ControllerId, (byte)Id, ref alarmType) == EziPlusEMotionLib.FMM_OK)
        //    {
        //        ((MotionStatus)Status).IsAlarm = alarmType != 0;
        //    }
        //}

        #region Privates
        #endregion
    }
}
