using EQX.Core.Motion;
using EQX.ThirdParty.Fastech;
using System.Net;
using NativeLib = FASTECH.EziMOTIONPlusELib;

namespace EQX.Motion
{
    public class MotionEziPlusE : MotionBase
    {
        public MotionEziPlusE(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
            iPAddress = IPAddress.Parse($"192.168.0.{id}");
        }

        protected override bool ActualInitialization()
        {
            return true;
        }

        protected override bool ActualConnect()
        {
            bool result = NativeLib.FAS_Connect(iPAddress, Id);
            IsConnected = result;
            return result;
        }

        protected override bool ActualDisconnect()
        {
            NativeLib.FAS_Close(Id);
            IsConnected = false;
            return true;
        }

        protected override bool ActualMotionOff()
        {
            return NativeLib.FAS_ServoEnable(Id, 0) == NativeLib.FMM_OK;
        }

        protected override bool ActualMotionOn()
        {
            return NativeLib.FAS_ServoEnable(Id, 1) == NativeLib.FMM_OK;
        }

        protected override bool ActualSearchOrigin()
        {
            return NativeLib.FAS_MoveOriginSingleAxis(Id) == NativeLib.FMM_OK;
        }

        protected override bool ActualMoveAbs(double position, double velocity)
        {
            return NativeLib.FAS_MoveSingleAxisAbsPos(Id, MMtoPulse(position), (uint)MMtoPulse(velocity)) == NativeLib.FMM_OK;
        }

        protected override bool ActualMoveInc(double position, double velocity)
        {
            return NativeLib.FAS_MoveSingleAxisIncPos(Id, MMtoPulse(position), (uint)MMtoPulse(velocity)) == NativeLib.FMM_OK;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            NativeLib.FAS_MoveVelocity(Id, (uint)MMtoPulse(speed), isForward ? 1 : 0);
        }

        public override bool Stop(bool forceStop = true)
        {
            return forceStop ? NativeLib.FAS_EmergencyStop(Id) == NativeLib.FMM_OK
                             : NativeLib.FAS_MoveStop(Id) == NativeLib.FMM_OK;
        }

        public override bool AlarmReset()
        {
            if (Status.IsMotionOn == true)
            {
                MotionOff();
            }

            bool result = NativeLib.FAS_ServoAlarmReset(Id) == NativeLib.FMM_OK;

            if (result)
            {
                MotionOn();
            }

            return result;
        }

        public override bool ClearPosition()
        {
            return NativeLib.FAS_ClearPosition(Id) == NativeLib.FMM_OK;
        }
        protected override void UpdateAxisStatus()
        {
            uint inputStatus = 0, outputStatus = 0, axisStatus = 0;
            int cmdPosition = 0, actPosition = 0, posErr = 0, actVel = 0;
            ushort currentRunPT = 0;

            if (NativeLib.FAS_GetAllStatus(Id, ref inputStatus, ref outputStatus, ref axisStatus,
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

            byte alarmType = 0;
            if (NativeLib.FAS_GetAlarmType(Id, ref alarmType) == EziPlusEMotionLib.FMM_OK)
            {
                ((MotionStatus)Status).IsAlarm = alarmType != 0;
            }
            if (NativeLib.FAS_IsSlaveExist(Id) == EziPlusEMotionLib.FMM_OK)
            {
                ((MotionStatus)Status).IsConnected = true;
            }
            else
            {
                ((MotionStatus)Status).IsConnected = false;

            }
        }

        private IPAddress iPAddress;
    }
}
