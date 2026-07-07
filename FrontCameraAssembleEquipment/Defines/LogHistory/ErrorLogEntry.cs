using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.LogHistory
{
    public class ErrorLogEntry
    {
        public string Timestamp { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }

        public bool IsHightlight { get; set; }
    }

    public class ErrorLogCount
    {
        public int Count { get; set; }
        public int ErrorCode { get; set;}
        public string Message { get;set;}
    }
}
