using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPENG_Device
{
    public class CPLC_Base : IPLC
    {
        public virtual bool Connect()
        {
            return false;
        }

        public virtual bool Disconnect()
        {
            return false;
        }

        public virtual bool ReadAll(ref strPLCData[] data)
        {
            return false;
        }

        public virtual bool WriteAll(strPLCData[] data)
        {
            return false;
        }
        public virtual bool ReadSingle(ref strPLCData data)
        {
            return false;
        }

        public virtual bool WriteSingle(strPLCData data)
        {
            return false;
        }

        public virtual string Address { get; }
        public virtual int Port { get; }
        public virtual bool IsConnected { get; }
        public virtual int ReceiveTimeout { get; set; }

        public delegate void ConnectionChanged(DEVICE_ID device, bool bConnected);
        public virtual event ConnectionChanged OnConnectionChanged;

        public delegate void DataReceived(DEVICE_ID device, double[] recvData);
        public virtual event DataReceived OnDataReceived;

        public delegate void ErrorReceived(DEVICE_ID device, Exception ex);
        public virtual event ErrorReceived OnErrorReceived;

        public delegate void DataTransferred(DEVICE_ID device);
        public virtual event DataTransferred OnDataTransferred;
    }
}
