using EQX.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Interlock
{
    public class InterlockEventAgrs : EventArgs
    {
        public InterlockEventAgrs(IIdentifier obj, string action, string message)
        {
            Obj = obj;
            Action = action;
            Message = message;
        }

        public IIdentifier Obj { get; }
        public string Action { get; }
        public string Message { get; }
    }

    public class InterlockMonitor
    {
        public static event EventHandler<InterlockEventAgrs> OnInterlockBlocked;

        public static void InterlockBlocked(IIdentifier obj, string action, string message)
        {
            // Gán vào biến tạm để đảm bảo an toàn luồng
            EventHandler<InterlockEventAgrs> handler = OnInterlockBlocked;
            handler?.Invoke(null, new InterlockEventAgrs(obj, action, message));
        }
    }
}
