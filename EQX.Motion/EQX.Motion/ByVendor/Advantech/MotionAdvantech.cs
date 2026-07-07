using System.Runtime.InteropServices;
using Advantech.Motion;

namespace EQX.Motion.ByVendor.AdvantechMotion
{
    public class MotionAdvantech : MotionBase
    {
        public MotionAdvantech(int id, string name, MotionAdvantechParameter parameter) : base(id, name, parameter)
        {
            this.parameter = parameter;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
            {
                throw new Exception("Only Windows OS supported");
            }
        }

        protected override bool ActualInitialization()
        {
            bool isInitSuccess = true;

            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.CFG_AxPPU, parameter.Pulse) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.CFG_AxPPUDenominator, parameter.Unit) == (uint)ErrorCode.SUCCESS;

            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxAcc, parameter.Acceleration) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxDec, parameter.Deceleration) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.CFG_AxMaxVel, parameter.MaxVelocity) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxVelLow, parameter.MinVelocity) == (uint)ErrorCode.SUCCESS;

            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogVelHigh, parameter.JogMaxVelocity) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogVelLow, parameter.JogMinVelocity) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogAcc, parameter.JogAcceleration) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogDec, parameter.JogDeceleration) == (uint)ErrorCode.SUCCESS;

            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetU32Property(AxisHandle, (uint)PropertyID.CFG_AxHomeDir, (uint)parameter.HomeDirect) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.CFG_AxHomeOffsetDistance, parameter.HomeOffset) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetU32Property(AxisHandle, (uint)PropertyID.PAR_AxHomeMode, parameter.HomeMode) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.CFG_AxHomeOffsetVel, parameter.HomeOffserVelocity) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxHomeVelHigh, parameter.HomeVelocity) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxHomeAcc, parameter.HomeAcceleration) == (uint)ErrorCode.SUCCESS;
            isInitSuccess &= Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxHomeDec, parameter.HomeDeceleration) == (uint)ErrorCode.SUCCESS;

            return isInitSuccess;
        }

        protected override bool ActualConnect()
        {
            if (IsConnected) return true;

            uint Result;
            uint deviceCount = 0;
            IntPtr m_DeviceHandle = IntPtr.Zero;
            uint DeviceNum;
            DEV_LIST[] CurAvailableDevs = new DEV_LIST[Advantech.Motion.Motion.MAX_DEVICES];

            Result = (uint)Advantech.Motion.Motion.mAcm_GetAvailableDevs(CurAvailableDevs, Advantech.Motion.Motion.MAX_DEVICES, ref deviceCount);
            if (Result != (int)ErrorCode.SUCCESS)
            {
                return false;
            }
            DeviceNum = CurAvailableDevs[0].DeviceNum;

            if (DeviceNum <= 0)
            {
                return false;
            }

            Result = Advantech.Motion.Motion.mAcm_DevOpen(DeviceNum, ref m_DeviceHandle);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }

            uint AxesPerDev = 0;
            Result = Advantech.Motion.Motion.mAcm_GetU32Property(m_DeviceHandle, (uint)PropertyID.FT_DevAxesCount, ref AxesPerDev);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }
            Result = Advantech.Motion.Motion.mAcm_AxOpen(m_DeviceHandle, (UInt16)Id, ref AxisHandle);
            if (Result != (uint)ErrorCode.SUCCESS)
            {
                return false;
            }

            IsConnected = true;
            return true;
        }

        protected override bool ActualDisconnect()
        {
            if(Advantech.Motion.Motion.mAcm_AxClose(ref AxisHandle) == (uint)ErrorCode.SUCCESS)
            {
                IsConnected = false;
            }
            return true;
        }

        protected override bool ActualMotionOn()
        {
            if (Advantech.Motion.Motion.mAcm_AxSetSvOn(AxisHandle, 1) == (uint)ErrorCode.SUCCESS)
            {
                ((MotionStatus)Status).IsMotionOn = true;
            }
            else
            {
                ((MotionStatus)Status).IsMotionOn = false;
            }
            return true;
        }

        protected override bool ActualMotionOff()
        {
            if (Advantech.Motion.Motion.mAcm_AxSetSvOn(AxisHandle, 0) == (uint)ErrorCode.SUCCESS)
            {
                ((MotionStatus)Status).IsMotionOn = false;
            }
            return true;
        }

        protected override bool ActualMoveAbs(double position, double speed)
        {
            SetMotionParams(speed, parameter.Acceleration, parameter.Deceleration);
            return Advantech.Motion.Motion.mAcm_AxMoveAbs(AxisHandle, position) == (uint)ErrorCode.SUCCESS;
        }

        protected override bool ActualMoveInc(double position, double speed)
        {
            SetMotionParams(speed, parameter.Acceleration, parameter.Deceleration);
            return Advantech.Motion.Motion.mAcm_AxMoveRel(AxisHandle, position) == (uint)ErrorCode.SUCCESS;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            SetMotionJogParams(speed, parameter.JogAcceleration, parameter.JogDeceleration);
            Advantech.Motion.Motion.mAcm_AxMoveVel(AxisHandle, isForward == true ? (ushort)1 : (ushort)0);
        }

        public override bool Stop(bool forceStop = true)
        {
            if (forceStop)
            {
                return Advantech.Motion.Motion.mAcm_AxStopEmg(AxisHandle) == (uint)ErrorCode.SUCCESS;
            }
            else
            {
                return Advantech.Motion.Motion.mAcm_AxStopDec(AxisHandle) == (uint)ErrorCode.SUCCESS;
            }
        }

        protected override bool ActualSearchOrigin()
        {
            return Advantech.Motion.Motion.mAcm_AxHome(AxisHandle, parameter.HomeMode, parameter.HomeDirect) == (uint)ErrorCode.SUCCESS;
        }

        public override bool AlarmReset()
        {
            return Advantech.Motion.Motion.mAcm_AxResetAlm(AxisHandle, 1) == (uint)ErrorCode.SUCCESS;
        }

        protected override void UpdateAxisStatus()
        {
            double actualPos = 0;
            double cmdPos = 0;
            double actualVel = 0;
            uint motionIOStatus = 0;
            ushort state = 0;

            Advantech.Motion.Motion.mAcm_AxGetActualPosition(AxisHandle, ref actualPos);
            ((MotionStatus)Status).ActualPosition = actualPos;
            Advantech.Motion.Motion.mAcm_AxGetCmdPosition(AxisHandle, ref cmdPos);
            ((MotionStatus)Status).CommandPosition = cmdPos;
            Advantech.Motion.Motion.mAcm_AxGetActVelocity(AxisHandle, ref actualVel);
            ((MotionStatus)Status).ActualVelocity = actualVel;

            Advantech.Motion.Motion.mAcm_AxGetMotionIO(AxisHandle, ref motionIOStatus);
            ((MotionStatus)Status).IsAlarm = (motionIOStatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_ALM) != 0;
            ((MotionStatus)Status).HwPosLimitDetect = (motionIOStatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_LMTP) != 0;
            ((MotionStatus)Status).HwNegLimitDetect = (motionIOStatus & (uint)Advantech.Motion.Ax_Motion_IO.AX_MOTION_IO_LMTN) != 0;

            Advantech.Motion.Motion.mAcm_AxGetState(AxisHandle, ref state);
            if ((Advantech.Motion.AxisState)state == Advantech.Motion.AxisState.STA_AX_READY)
            {
                ((MotionStatus)Status).IsMotioning = false;
            }
            else
            {
                ((MotionStatus)Status).IsMotioning = true;
            }
        }
        #region Private Functions
        private void SetMotionParams(double speed, double acc, double dec)
        {
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxVelHigh, speed);
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxAcc, acc);
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxDec, dec);
        }
        private void SetMotionJogParams(double speed, double acc, double dec)
        {
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogVelHigh, speed);
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogAcc, acc);
            Advantech.Motion.Motion.mAcm_SetF64Property(AxisHandle, (uint)PropertyID.PAR_AxJogDec, dec);
        }
        #endregion

        #region Privates
        private IntPtr AxisHandle = IntPtr.Zero;
        private readonly MotionAdvantechParameter parameter;
        #endregion

    }
}
