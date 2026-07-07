using EQX.Core.Common;
using EQX.Core.Communication.CIM;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.Text;
using TOPENG_Device;

namespace EQX.Core.Test
{
    [TestClass]
    public class TimerTest
    {
        [TestMethod]
        public async Task NonOverlapingTimerTest()
        {
            NonOverlappingTimer timer = new NonOverlappingTimer(10);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            await Task.Delay(5000);
            timer.Pause();
            await Task.Delay(5000);
            timer.Start();
            await Task.Delay(5000);
            timer.Stop();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine($"[Timer_Elapsed] {DateTime.Now:HH:mm:ss.fff}");
            Thread.Sleep(1000);
        }
    }

    [TestClass]
    public class CIMTest
    {
        [TestMethod]
        public async Task InitializeScenarioTest()
        {
            #region 1. CONNECT
            MCCLinkIE.mCCLinkIEUse = true;

            if (MCCLinkIE.mCCLinkIEUse == true)
            {
                int iCCLinkIEOpen = MCCLinkIE.Open();
                if (iCCLinkIEOpen == 0)  //OK
                {
                    MCCLinkIE.Initial();
                    MCCLinkIE.mRDeviceNo = MPLC.PLCReadBaseAddress_B;
                    MCCLinkIE.mRDeviceNoLength = MPLC.PLCReadBaseAddressLength_B;
                    MCCLinkIE.mSDeviceNo = MPLC.PLCWriteBaseAddressLength_B;
                    MCCLinkIE.mSDeviceNoLength = MPLC.PLCReadBaseAddressLength_B;
                }
                else
                {

                }
            }
            #endregion

            #region 2. PING & DATA UPDATE
            System.Timers.Timer rwTask = new System.Timers.Timer(100);
            rwTask.Elapsed += (s, e) =>
            {
                MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.B, false);
                MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.W, false);

                MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.B, false);
                MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.W, false);
            };
            rwTask.Start();

            CIMAliveTask task = new CIMAliveTask();
            task.Start();
            #endregion

            CommandHandling = new Dictionary<CIMCommand, bool>();
            ICIMMapHelper mapHelper = new SDVCIMMapHeler();

            CIMCommand[] eventCommands = new CIMCommand[]
            {
                CIMCommand.TerminalDisplay,
                CIMCommand.DatetimeSet,
                CIMCommand.OperatorCall,
                CIMCommand.Interlock,
                CIMCommand.FormattedProcessProgramSend,
                CIMCommand.FormattedProcessProgramRequest,
                CIMCommand.CurrentEquipPPIDListRequest, // NO WORD
                CIMCommand.EquipConstantNameList, // NO WORD
            };


            var sw = Stopwatch.StartNew();
            while (true/*sw.ElapsedMilliseconds < 30000*/)
            {
                foreach (var command in eventCommands)
                {
                    CIMCommandDetail cimMap = new CIMCommandDetail(mapHelper)
                    {
                        Command = command
                    };

                    bool bitOn = cimMap.IsCIMBitOn();
                    if (bitOn)
                    {
                        if (CommandHandling.ContainsKey(cimMap.Command)) return;

                        CommandHandling.Add(cimMap.Command, true);
                        var buf = new short[cimMap.CIMWordLength];

                        if (cimMap.CIMWordLength > 0)
                        {
                            CIMAddressMap.ReadWords(CIMAddressMap.GetReadWordIndexFromDAddress(cimMap.CIMWordAddress), cimMap.CIMWordLength, buf);
                        }

                        cimMap.SetPLCBitOn();

                        HandleData(cimMap.Command, buf);

                        cimMap.WaitForCIMBitOff();
                        CommandHandling.Remove(cimMap.Command);
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        private Dictionary<CIMCommand, bool> CommandHandling;

        private bool HandleData(CIMCommand command, short[] buffer)
        {
            switch (command)
            {
                case CIMCommand.DatetimeSet:
                    break;
                case CIMCommand.TerminalDisplay:
                    {
                        string message = "";
                        for (int i = 0; i < buffer.Length - 1; i++)
                        {
                            message += System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(buffer[i]));
                        }
                        Debug.WriteLine($"[{command}] {message}");
                        return true;
                    }
                case CIMCommand.OperatorCall:
                    {
                        string message = "";
                        for (int i = 10; i < buffer.Length; i++)
                        {
                            message += System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(buffer[i]));
                        }
                        Debug.WriteLine($"[{command}] {message}");
                        break;
                    }
                case CIMCommand.Interlock:
                    {
                        string message = "";
                        for (int i = 10; i < buffer.Length - 1; i++)
                        {
                            message += System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(buffer[i]));
                        }
                        Debug.WriteLine($"[{command}] {message}");
                        break;
                    }
            }
            return false;
        }
    }
}
