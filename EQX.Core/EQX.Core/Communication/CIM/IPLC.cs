using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPENG_Device
{
    interface IPLC
    {
        bool Connect();
        bool Disconnect();
        bool ReadAll(ref strPLCData[] data);
        bool WriteAll(strPLCData[] data);
        bool ReadSingle(ref strPLCData data);
        bool WriteSingle(strPLCData data);
    }
}
