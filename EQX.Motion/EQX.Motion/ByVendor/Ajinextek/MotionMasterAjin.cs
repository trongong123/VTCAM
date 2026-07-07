using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;
using System.Diagnostics;

namespace EQX.Motion.ByVendor.Ajinextek
{
    public class MotionMasterAjin : MotionMasterBase
    {
        public override bool IsConnected
        {
            get
            {
                if (lastConnectTime + 500 > Environment.TickCount)
                {
                    return lastConnectResult;
                }

                lock (_lock)
                {
                    int axisCount = -1;

                    bool ret = AXM.AxmInfoGetAxisCount(ref axisCount) == (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS;
                    lastConnectResult = AXL.AxlIsOpened() == 0x01 & ret & axisCount == NumberOfDevices;

                    lastConnectTime = Environment.TickCount;

                    return lastConnectResult;
                }
            }
        }

        private object _lock = new object();

        private bool lastConnectResult = false;
        private int lastConnectTime = Environment.TickCount;

        public override bool Connect()
        {
            if (IsConnected == true) return true;

            if (AXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return false;

            return true;
        }

        public override bool Disconnect()
        {
            AXL.AxlClose();

            return true;
        }
    }
}
