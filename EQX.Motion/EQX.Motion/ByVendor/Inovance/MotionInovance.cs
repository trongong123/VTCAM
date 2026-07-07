using Advantech.Motion;
using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace EQX.Motion.ByVendor.Inovance
{
    internal enum EInovanceAxisStatus : uint
    {
        AX_ALARM = 0x01,
        AX_MOTION_ON = 0x02,
        AX_BUSY = 0x04,
        AX_ARRIVED = 0x08,
        AX_POS_LIMIT = 0x10,
        AX_NEG_LIMIT = 0x20,
        AX_SOFT_POS_LIMIT = 0x40,
        AX_SOFT_NEG_LIMIT = 0x80,
        AX_ERRPOS = 0x100,
        AX_EMG_STOP = 0x200,
        AX_ECAT = 0x400,
        AX_SW_ABNOR = 0x800,
        AX_WARNING = 0x1000,
        AX_HOME = 0x2000,
    }

    public static class MotionHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoderResolution">Total of resolution</param>
        /// <param name="gearRatio">Reduce gear ratio</param>
        /// <param name="pitch">um</param>
        /// <returns></returns>
        private static double PositionFactor(int encoderResolution, int gearRatio, int pitch)
        {
            return (1.0 * encoderResolution * gearRatio) / (pitch * 1.0);
        }

        public static int MotorResolution(int encoderResolution, int gearRatio, int pitch)
        {
            double positionFactor = PositionFactor(encoderResolution, gearRatio, pitch);
            return (int)(positionFactor * GetMultiplier(positionFactor.ToString()));
        }

        public static int ShaftResolution(int encoderResolution, int gearRatio, int pitch)
        {
            double positionFactor = PositionFactor(encoderResolution, gearRatio, pitch);
            return (int)(GetMultiplier(positionFactor.ToString()));
        }

        static int GetMultiplier(string numStr)
        {
            // Chuẩn hóa: bỏ ký tự '+' hoặc khoảng trắng
            numStr = numStr.Trim().TrimStart('+');

            // Nếu không có dấu phẩy hoặc dấu chấm → là số nguyên
            if (!numStr.Contains('.') && !numStr.Contains(','))
                return 1;

            // Dùng dấu '.' làm chuẩn
            numStr = numStr.Replace(',', '.');

            // Tách phần nguyên và phần thập phân
            string[] parts = numStr.Split('.');
            if (parts.Length < 2)
                return 1;

            string decimalPart = parts[1].TrimEnd('0'); // Bỏ các số 0 vô nghĩa

            if (decimalPart.Length == 0)
                return 1;

            int decimalPlaces = Math.Min(decimalPart.Length, 5);
            int multiplier = (int)Math.Pow(10, decimalPlaces);
            return multiplier;
        }
    }

    public class MotionInovance : MotionBase
    {
        internal MotionMasterInovance MotionMaster { get; init; }

        public override bool IsConnected => MotionMaster.IsConnected;

        public MotionInovance(int id, string name, MotionInovanceParameter parameter) : base(id, name, parameter)
        {
            this.parameter = parameter;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
            {
                throw new Exception("Only Windows OS supported");
            }
        }

        protected override bool ActualInitialization()
        {
            uint abortCode = 0;
            bool bRet = true;

            short nphyStationId = 0, nphySlotId = 0;
            bRet &= ImcApi.IMC_GetAxEcatStation(MotionMaster.ControllerId, (ushort)Id, ref nphyStationId, ref nphySlotId) == ImcApi.EXE_SUCCESS;

            int motorResolution = MotionHelpers.MotorResolution(parameter.EncoderResolution, (int)parameter.ReduceGearRatio, (int)(parameter.Unit * 1000));
            int shaftResolution = MotionHelpers.ShaftResolution(parameter.EncoderResolution, (int)parameter.ReduceGearRatio, (int)(parameter.Unit * 1000));

            bRet &= ImcApi.IMC_SetEcatSdo(MotionMaster.ControllerId, nphyStationId, 0x6091, 0x01, BitConverter.GetBytes(motorResolution), 4, ref abortCode) == ImcApi.EXE_SUCCESS;
            bRet &= ImcApi.IMC_SetEcatSdo(MotionMaster.ControllerId, nphyStationId, 0x6091, 0x02, BitConverter.GetBytes(shaftResolution), 4, ref abortCode) == ImcApi.EXE_SUCCESS;

            return bRet;
        }
        protected override bool ActualMotionOn()
        {
            return ImcApi.IMC_AxServoOn(MotionMaster.ControllerId, (short)Id) == ImcApi.EXE_SUCCESS;
        }

        protected override bool ActualMotionOff()
        {
            return ImcApi.IMC_AxServoOff(MotionMaster.ControllerId, (short)Id) == ImcApi.EXE_SUCCESS;
        }

        public override bool AlarmReset()
        {
            return ImcApi.IMC_ClrAxSts(MotionMaster.ControllerId, (short)Id) == ImcApi.EXE_SUCCESS;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            ImcApi.IMC_SetSingleAxMvPara(MotionMaster.ControllerId, (short)Id, MMtoPulse(speed),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Acceleration) : MMtoPulse(parameter.Acceleration),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Deceleration) : MMtoPulse(parameter.Deceleration));
            ImcApi.IMC_StartJogMove(MotionMaster.ControllerId, (short)Id, MMtoPulse(speed) * (isForward ? 1 : -1));
        }

        protected override bool ActualMoveAbs(double position, double speed)
        {
            ImcApi.IMC_SetSingleAxMvPara(MotionMaster.ControllerId, (short)Id, MMtoPulse(speed),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Acceleration) : MMtoPulse(parameter.Acceleration),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Deceleration) : MMtoPulse(parameter.Deceleration));
            return ImcApi.IMC_StartPtpMove(MotionMaster.ControllerId, (short)Id, MMtoPulse(position), 0) == ImcApi.EXE_SUCCESS;
        }

        protected override bool ActualMoveInc(double position, double speed)
        {
            ImcApi.IMC_SetSingleAxMvPara(MotionMaster.ControllerId, (short)Id, MMtoPulse(speed),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Acceleration) : MMtoPulse(parameter.Acceleration),
                parameter.AccelUnit == 1 ? MMtoPulse(speed / parameter.Deceleration) : MMtoPulse(parameter.Deceleration));
            return ImcApi.IMC_StartPtpMove(MotionMaster.ControllerId, (short)Id, MMtoPulse(position), 1) == ImcApi.EXE_SUCCESS;
        }

        public override bool Stop(bool forceStop = true)
        {
            return ImcApi.IMC_AxMoveStop(MotionMaster.ControllerId, (short)Id, (short)(forceStop ? 1 : 0)) == ImcApi.EXE_SUCCESS;
        }

        protected override bool ActualSearchOrigin()
        {
            ImcApi.THomingPara homingPara = new ImcApi.THomingPara();
            homingPara.homeMethod = (short)parameter.HomeMethod;
            homingPara.offset = MMtoPulse(parameter.HomeOffset);

            homingPara.lowVel = (uint)MMtoPulse(parameter.HomeLowVelocity);
            homingPara.highVel = (uint)MMtoPulse(parameter.HomeHighVelocity);
            homingPara.acc = (uint)(parameter.AccelUnit == 1 ? MMtoPulse(parameter.HomeHighVelocity / parameter.HomeAcceleration) : MMtoPulse(parameter.HomeAcceleration));
            homingPara.overtime = 60000;

            ImcApi.IMC_StartHoming(MotionMaster.ControllerId, (short)Id, ref homingPara);

            return true;
        }

        protected override void UpdateAxisStatus()
        {
            if (MotionMaster == null) return;
            if (IsConnected == false) return;

            int[] status = new int[1];
            ImcApi.IMC_GetAxSts(MotionMaster.ControllerId, (short)Id, status);

            ((MotionStatus)Status).IsAlarm = (status[0] & (int)EInovanceAxisStatus.AX_ALARM) != 0;
            ((MotionStatus)Status).IsMotionOn = (status[0] & (int)EInovanceAxisStatus.AX_MOTION_ON) != 0;
            ((MotionStatus)Status).IsMotioning = (status[0] & (int)EInovanceAxisStatus.AX_BUSY) != 0;
            ((MotionStatus)Status).HwPosLimitDetect = (status[0] & (int)EInovanceAxisStatus.AX_POS_LIMIT) != 0;
            ((MotionStatus)Status).HwNegLimitDetect = (status[0] & (int)EInovanceAxisStatus.AX_NEG_LIMIT) != 0;

            short homingSts = 0;
            ImcApi.IMC_GetHomingStatus(MotionMaster.ControllerId, (short)Id, ref homingSts);

            ((MotionStatus)Status).IsHomeDone = homingSts == ImcApi.HOME_SUCESS;

            double[] actualPos = new double[1];
            ImcApi.IMC_GetAxEncPos(MotionMaster.ControllerId, (short)Id, actualPos);
            ((MotionStatus)Status).ActualPosition = PulseToMM((int)actualPos[0]);

            int errorPos = 0;
            ImcApi.IMC_GetAxErrorPos(MotionMaster.ControllerId, (short)Id, ref errorPos);
            ((MotionStatus)Status).PositionError = PulseToMM(errorPos);
        }

        public override bool ClearPosition()
        {
            if (MotionMaster == null) return false;
            if (IsConnected == false) return false;

            ImcApi.IMC_SetAxCurPos(MotionMaster.ControllerId, (short)Id, 0);

            return true;
        }

        #region Privates
        private readonly MotionInovanceParameter parameter;
        #endregion
    }
}
