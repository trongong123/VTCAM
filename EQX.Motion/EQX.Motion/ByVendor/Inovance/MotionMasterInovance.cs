using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;
using System.Diagnostics;

namespace EQX.Motion.ByVendor.Inovance
{
    public class MotionMasterInovance : MotionMasterBase
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
                    ImcApi.TRsouresNum tResource = new ImcApi.TRsouresNum();
                    bool ret = ImcApi.IMC_GetCardResource(ControllerId, ref tResource) == ImcApi.EXE_SUCCESS;

                    lastConnectResult = ret & tResource.axNum == NumberOfDevices;
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
            lock (_lock)
            {
                Int32 nCardNum = 0;
                uint ret;
                ret = ImcApi.IMC_GetCardsNum(ref nCardNum);

                if (ret != 0)
                {
                    return false;
                }

                if (nCardNum <= 0)
                {
                    return false;
                }

                ret = ImcApi.IMC_OpenCardHandle(0, ref _DeviceId);

                if (ret != 0)
                {
                    return false;
                }

                ret = ImcApi.IMC_CloseCard((ulong)ControllerId);

                if (ret != 0)
                {
                    return false;
                }

                ret = ImcApi.IMC_OpenCardHandle(0, ref _DeviceId);

                if (ret != 0)
                {
                    return false;
                }

                uint masterStatus = 0;
                ret = ImcApi.IMC_GetECATMasterSts(ControllerId, ref masterStatus);
                if (ret != 0)
                {
                    return false;
                }

                if (masterStatus != ImcApi.EC_MASTER_OP)
                {
                    //retRtn = ImcApi.IMC_DownLoadDeviceConfig(cardHandle, "DeviceConfig_Terminal.xml");    //下载设备配置文件
                    ret = ImcApi.IMC_DownLoadDeviceConfig(ControllerId, "D:\\PIFilmAutoDetachCleanMC\\Config\\Inovance\\DeviceConfig.xml");    //下载设备配置文件
                    if (ret != ImcApi.EXE_SUCCESS)
                    {
                        return false;
                    }

                    ret = ImcApi.IMC_ScanCardECAT(ControllerId, 1);     // 建立EtherCAT通讯  0:不阻塞 1:阻塞
                    if (ret != ImcApi.EXE_SUCCESS)
                    {
                        return false;
                    }

                    Int16 loopTemp = 10;
                    while (loopTemp > 0)
                    {
                        ret = ImcApi.IMC_GetECATMasterSts(ControllerId, ref masterStatus);      // 获取主站状态
                        if (masterStatus == ImcApi.EC_MASTER_OP)
                        {
                            break;
                        }

                        loopTemp--;
                        if (loopTemp <= 0)
                        {
                            break;
                        }

                        Thread.Sleep(100);
                    }
                }

                // Error orcus here, if the card not closed befor connect
                ret = ImcApi.IMC_DownLoadSystemConfig(ControllerId, "D:\\PIFilmAutoDetachCleanMC\\Config\\Inovance\\SystemConfig.xml");
                if (ret != 0)
                {
                    return false;
                }

                ret = ImcApi.IMC_SetEmgTrigLevelInv(ControllerId, 1);
                if (ret != 0) return false;

                IsConnected = true;
                return true;
            }
        }

        public override bool Disconnect()
        {
            //uint ret = ImcApi.IMC_CloseCard(ControllerId);    // RESET CARD
            uint ret = ImcApi.IMC_CloseCardHandle(ControllerId);// DONT RESET CARD

            if (ret != 0)
            {
                return false;
            }

            IsConnected = false;
            ControllerId = 0;
            return true;
        }
    }
}
