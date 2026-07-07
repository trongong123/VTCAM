using EQX.Motion.ByVendor.Ajinextek;
using EQX.Motion.ByVendor.Inovance;
using System.Runtime.InteropServices;

namespace EQX.Motion
{
    public class MotionAjin : MotionBase
    {
        public MotionMasterAjin MotionMaster { get; init; }

        public override bool IsConnected => MotionMaster.IsConnected;

        public MotionAjin(int id, string name, MotionAjinParameter parameter)
            : base(id, name, parameter)
        {
            this.parameter = parameter;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
            {
                throw new Exception("Only Windows OS supported");
            }
        }

        protected override bool ActualInitialization()
        {
            AXM.AxmMotSetPulseOutMethod(Id, parameter.PulseOutput);
            AXM.AxmMotSetEncInputMethod(Id, parameter.EncoderInput);
            AXM.AxmContiSetAbsRelMode(Id, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);
            AXM.AxmMotSetProfileMode(Id, parameter.ProfileMode);//
            AXM.AxmMotSetMinVel(Id, parameter.MinVelocity);
            AXM.AxmMotSetMaxVel(Id, parameter.MaxVelocity);
            AXM.AxmMotSetAccelUnit(Id, parameter.AccelUnit);
            AXM.AxmMotSetAccelJerk(Id, parameter.Acceleration);
            AXM.AxmMotSetDecelJerk(Id, parameter.Deceleration);
            AXM.AxmMotSetMoveUnitPerPulse(Id, parameter.Unit, parameter.Pulse);//
            AXM.AxmSignalSetServoOnLevel(Id, parameter.ServoOnLevel);
            AXM.AxmSignalSetServoAlarm(Id, parameter.ServoAlarmLevel);
            AXM.AxmSignalSetInpos(Id, parameter.ServoInposLevel);
            AXM.AxmSignalSetLimit(Id, (int)AXT_MOTION_STOPMODE.EMERGENCY_STOP, parameter.PositiveLevel, parameter.NegativeLevel);
            AXM.AxmSignalSetStop(Id, (int)AXT_MOTION_STOPMODE.EMERGENCY_STOP, (int)AXT_MOTION_LEVEL_MODE.HIGH);
            AXM.AxmSignalSetZphaseLevel(Id, parameter.ZPhaseLevel);

            AXM.AxmHomeSetMethod(Id, parameter.HomeDirect, parameter.HomeSignal, parameter.HomeZPhaseUse, parameter.HomeClearTime, parameter.HomeOffset);
            AXM.AxmHomeSetVel(Id, parameter.HomeVelFirst, parameter.HomeVelSecond, parameter.HomeVelThird, parameter.HomeVelLast, parameter.HomeAccFirst, parameter.HomeAccSecond);

            return true;
        }

        protected override bool ActualMotionOff()
        {
            uint retCode = AXM.AxmSignalServoOn(Id, 0);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override bool ActualMotionOn()
        {
            uint retCode = AXM.AxmSignalServoOn(Id, 1);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override bool ActualMoveAbs(double position, double speed)
        {
            uint uMode = 0;
            AXM.AxmMotGetAbsRelMode(Id, ref uMode);
            if (uMode != (uint)AXT_MOTION_ABSREL.POS_ABS_MODE)
            {
                AXM.AxmMotSetAbsRelMode(Id, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);
            }

            uint retCode = AXM.AxmMoveStartPos(Id, position, speed, Parameter.Acceleration, Parameter.Deceleration);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override bool ActualMoveInc(double position, double speed)
        {
            uint uMode = 0;
            AXM.AxmMotGetAbsRelMode((int)Id, ref uMode);
            if (uMode != (uint)AXT_MOTION_ABSREL.POS_REL_MODE)
            {
                AXM.AxmMotSetAbsRelMode(Id, (uint)AXT_MOTION_ABSREL.POS_REL_MODE);
            }

            uint retCode = AXM.AxmMoveStartPos(Id, position, speed, parameter.Acceleration, parameter.Deceleration);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            uint uMode = 0;
            AXM.AxmMotGetAbsRelMode((int)Id, ref uMode);
            if (uMode != (uint)AXT_MOTION_ABSREL.POS_REL_MODE)
            {
                AXM.AxmMotSetAbsRelMode(Id, (uint)AXT_MOTION_ABSREL.POS_REL_MODE);
            }

            AXM.AxmMoveVel(Id, speed * (isForward ? 1 : -1), parameter.Acceleration, parameter.Deceleration);
        }

        protected override bool ActualSearchOrigin()
        {
            uint retCode = AXM.AxmHomeSetStart(Id);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        public override bool Stop(bool forceStop = true)
        {
            uint retCode = forceStop ? AXM.AxmMoveEStop(Id) : AXM.AxmMoveSStop(Id);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        public override bool AlarmReset()
        {
            uint retCode = AXM.AxmSignalServoAlarmReset(Id, 1);

            return (AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override void UpdateAxisStatus()
        {
            uint retCode;
            uint uintStatus = 0;

            uint homeResult = 0;
            retCode = AXM.AxmHomeGetResult(Id, ref homeResult);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                ((MotionStatus)Status).IsHomeDone = homeResult == (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS;
            }

            uint upOnOff = 0;
            retCode = AXM.AxmSignalIsServoOn(Id, ref upOnOff);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                ((MotionStatus)Status).IsMotionOn = upOnOff == 0x01;
            }

            retCode = AXM.AxmStatusReadInMotion(Id, ref uintStatus);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                ((MotionStatus)Status).IsMotioning = uintStatus == 0x01;
            }

            retCode = AXM.AxmStatusReadMechanical(Id, ref uintStatus);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                ((MotionStatus)Status).HwPosLimitDetect =
                    (uintStatus & (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PELM_LEVEL) > 0 |
                    (uintStatus & (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_PSLM_LEVEL) > 0;

                ((MotionStatus)Status).HwNegLimitDetect =
                    (uintStatus & (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NELM_LEVEL) > 0 |
                    (uintStatus & (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_NSLM_LEVEL) > 0;

                ((MotionStatus)Status).IsAlarm =
                    (uintStatus & (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_ALARM_LEVEL) > 0;
            }

            MOTION_INFO motioninfo = new MOTION_INFO();
            retCode = AXM.AxmStatusReadMotionInfo(Id, ref motioninfo);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                // TODO: APPLY PulseToMM
                ((MotionStatus)Status).CommandPosition = motioninfo.dCmdPos;
                ((MotionStatus)Status).ActualPosition = motioninfo.dActPos;
            }

            double actualVelocity = 0.0;
            retCode = AXM.AxmStatusReadVel(Id, ref actualVelocity);
            if ((AXT_FUNC_RESULT)retCode == AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                ((MotionStatus)Status).ActualVelocity = actualVelocity;
            }
        }

        #region Privates
        private readonly MotionAjinParameter parameter;
        #endregion
    }
}
