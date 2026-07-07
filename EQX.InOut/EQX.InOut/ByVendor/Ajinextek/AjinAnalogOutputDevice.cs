using EQX.InOut.InOut.Analog;

namespace EQX.InOut
{
    public class AjinAnalogOutputDevice<TEnum> : AnalogOutputDeviceBase<TEnum> where TEnum : Enum
    {
        #region Properties
        public override bool IsConnected => AXL.AxlIsOpened() == 0x01;
        #endregion

        #region Public methods
        public override bool Connect()
        {
            if (IsConnected) return true;

            if (AXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return false;

            return true;
        }

        public override bool Disconnect()
        {
            return AXL.AxlClose() == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS;
        }

        protected override void ExtendInit()
        {
        }

        public override double GetVolt(int channel)
        {
            double volt = 0.0;
            AXA.AxaoReadVoltage(channel, ref volt);
            return volt;
        }

        public override void SetVolt(int channel, double voltage)
        {
            AXA.AxaoWriteVoltage(channel, voltage);
        }
        #endregion

    }
}
