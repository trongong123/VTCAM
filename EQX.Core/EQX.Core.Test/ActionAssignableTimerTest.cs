using EQX.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Test
{
    [TestClass]
    public class ActionAssignableTimerTest
    {
        [TestMethod]
        public void TestActionAssignableTimer()
        {
            ActionAssignableTimer timer = new ActionAssignableTimer(500);

            timer.EnableAction(
                "console1",
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase1 Hello #1"); },
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase2 Hello #1"); }
                );

            timer.EnableAction(
                "console2",
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase1 Hello #2"); },
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase2 Hello #2"); }
                );

            timer.EnableAction(
                "console1",
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase1 Hello #3"); },
                () => { Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Phase2 Hello #3"); }
                );

            Thread.Sleep(10000);

            timer.DisableAction("console1");

            Thread.Sleep(10000);

            timer.StopTimer();
        }
    }
}
