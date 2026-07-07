// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// 참고: doc/CIM/SDC CIM 구현 컨셉.md (Command Type, 3초 타임아웃)
// Command Type: CIM Address(Master Bit) 읽기 → Item Word 읽기 → PLC Address(Local Reply Bit) 쓰기. 3초 타임아웃 시 알람.

using System;
using System.Diagnostics;
using System.Threading;

namespace TOPENG_Device
{
    /// <summary>
    /// Command Type 처리. Master Command Bit 확인, Word 읽기, Local Reply Bit 쓰기, 3초 타임아웃 알람.
    /// </summary>
    public static class CIMCommandHandler
    {
        /// <summary>Master Bit 확인 3초 초과 시 알람. (설비 알람 연동 시 사용)</summary>
        public const int CommandResponseTimeoutMs = 3000;
        /// <summary>
        /// [ms], On 유지타임
        /// </summary>
        private const int _replyTime = 1000;

        /// <summary>CIM Command 타임아웃 발생 시 알람 전달용. (코드, 메시지)</summary>
        public static event Action<string, string> OnCommandTimeoutAlarm;        

        /// <summary>CIMCommand에 해당하는 CIM Address(Master), PLC Address(Local Reply) 반환. 명세서 §1 기준.</summary>
        public static void GetAddresses(CIMCommand cmd, out string cimAddress, out string plcAddress)
        {
            switch (cmd)
            {
                case CIMCommand.AliveBit: cimAddress = "B1000"; plcAddress = "B0000"; break;
                case CIMCommand.DatetimeSet: cimAddress = "B1001"; plcAddress = "B0001"; break;
                case CIMCommand.TerminalDisplay: cimAddress = "B1002"; plcAddress = "B0002"; break;
                case CIMCommand.OperatorCall: cimAddress = "B1003"; plcAddress = "B0003"; break;
                case CIMCommand.Interlock: cimAddress = "B1004"; plcAddress = "B0004"; break;
                case CIMCommand.JOBStart: cimAddress = "B1005"; plcAddress = "B0005"; break;
                case CIMCommand.OnlineRemote: cimAddress = "B1007"; plcAddress = "B0007"; break;
                case CIMCommand.OnlineLocal: cimAddress = "B1008"; plcAddress = "B0008"; break;
                case CIMCommand.OffLine: cimAddress = "B1009"; plcAddress = "B0009"; break;
                case CIMCommand.TCDisconnectAlarm: cimAddress = "B100A"; plcAddress = "B000A"; break;
                case CIMCommand.CellStartPort1: cimAddress = "B101B"; plcAddress = "B001B"; break;
                case CIMCommand.CellStartPort2: cimAddress = "B101C"; plcAddress = "B001C"; break;
                case CIMCommand.CellStartPort3: cimAddress = "B101D"; plcAddress = "B001D"; break;
                case CIMCommand.CellStartPort4: cimAddress = "B101E"; plcAddress = "B001E"; break;
                // 260305, FormattedProcessProgramSend -> FormattedProcessProgramSend2 로 대체
                case CIMCommand.FormattedProcessProgramSend:/* cimAddress = "B1035"; plcAddress = "B0035"; break;*/
                case CIMCommand.FormattedProcessProgramSend2: cimAddress = "B1033"; plcAddress = "B0033"; break;
                case CIMCommand.EquipConstantNameList: cimAddress = "B1034"; plcAddress = "B0034"; break;
                case CIMCommand.FormattedProcessProgramRequest: cimAddress = "B1036"; plcAddress = "B0036"; break;
                case CIMCommand.CurrentEquipPPIDListRequest: cimAddress = "B1037"; plcAddress = "B0037"; break;
                case CIMCommand.BatchLotInforDownload: cimAddress = "B1039"; plcAddress = "B0039"; break;
                case CIMCommand.CellJobProcess1: cimAddress = "B1049"; plcAddress = "B0049"; break;
                case CIMCommand.CellJobProcess2: cimAddress = "B104A"; plcAddress = "B004A"; break;
                case CIMCommand.CellJobProcess3: cimAddress = "B104B"; plcAddress = "B004B"; break;
                case CIMCommand.CellJobProcess4: cimAddress = "B104C"; plcAddress = "B004C"; break;
                case CIMCommand.EquipApproveProecss: cimAddress = "B104D"; plcAddress = "B004D"; break;
                case CIMCommand.MaterialInfoSend1: cimAddress = "B1071"; plcAddress = "B0071"; break;
                case CIMCommand.MaterialInfoSend2: cimAddress = "B1072"; plcAddress = "B0072"; break;
                case CIMCommand.MaterialInfoSend3: cimAddress = "B1073"; plcAddress = "B0073"; break;
                case CIMCommand.LabelInformationSend: cimAddress = "B108F"; plcAddress = "B008F"; break;
                case CIMCommand.LabelValidationSend: cimAddress = "B1091"; plcAddress = "B0091"; break;
                case CIMCommand.LabelPrecheckSend: cimAddress = "B1093"; plcAddress = "B0093"; break;
                case CIMCommand.CurrentProcessControlDataReq: cimAddress = "B1095"; plcAddress = "B0095"; break;
                case CIMCommand.ProcessControlInformSend1: cimAddress = "B109A"; plcAddress = "B009A"; break;
                case CIMCommand.ProcessControlInformSend2: cimAddress = "B109B"; plcAddress = "B009B"; break;
                case CIMCommand.ProcessControlInformSend3: cimAddress = "B109C"; plcAddress = "B009C"; break;
                case CIMCommand.ProcessControlInformSend4: cimAddress = "B109D"; plcAddress = "B009D"; break;
                case CIMCommand.EquipFunctionChangeCommand: cimAddress = "B10E4"; plcAddress = "B00E4"; break;
                case CIMCommand.CellInofrmationDownload1: cimAddress = "B10D7"; plcAddress = "B00D7"; break;
                case CIMCommand.CellInofrmationDownload2: cimAddress = "B10D8"; plcAddress = "B00D8"; break;

                default: cimAddress = "B1000"; plcAddress = "B0000"; break;
            }
        }
   
        /// <summary>CIM Bit가 On인지 확인. PLCReadValue_B 사용.</summary>
        public static bool IsCIMBitOn(CIMCommand cmd)
        {
            GetAddresses(cmd, out string cimAddr, out _);
            short v = CIMAddressMap.ReadCIMBit(cimAddr);
            return v != 0;
        }
        public static bool IsCIMBitOff(CIMCommand cmd)
        {
            GetAddresses(cmd, out string cimAddr, out _);
            short v = CIMAddressMap.ReadCIMBit(cimAddr);
            return v == 0;
        }
        public static bool IsPLCBitOn(CIMCommand cmd)
        {
            GetAddresses(cmd, out _, out string plcAddr);
            short v = CIMAddressMap.ReadPLCBit(plcAddr);
            return v != 0;
        }

        // 260304, 우선 메소드 내에서 처리로 구현. 다중처리가 필요할 시 별도처리로 구현 필요.
        /// <summary>Local PLC Bit On. PLCWriteValue_B 사용.</summary>
        public static void SetLocalPLCBitOn(CIMCommand cmd, bool useReply = false)
        {
            GetAddresses(cmd, out _, out string plcAddr);
            CIMAddressMap.WritePLCBit(plcAddr, 1);

            if (useReply)
            {
                DateTime time = DateTime.Now;
                while (true)
                {
                    if (((TimeSpan)(DateTime.Now - time)).TotalMilliseconds >= _replyTime) break;
                    Thread.Sleep(1);
                }

                SetLocalReplyBitOff(cmd);
            }
        }

        /// <summary>Local Reply Bit Off.</summary>
        public static void SetLocalReplyBitOff(CIMCommand cmd)
        {
            GetAddresses(cmd, out _, out string plcAddr);
            CIMAddressMap.WritePLCBit(plcAddr, 0);
        }

        /// <summary>CIM Bit가 On 될 때까지 대기. timeoutMs 내에 On 되면 true, 아니면 false(타임아웃 시 알람 발생).</summary>
        public static bool WaitForCIMBitOn(CIMCommand cmd, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (IsCIMBitOn(cmd))
                    return true;
                System.Threading.Thread.Sleep(10);
            }
            OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} Master Bit not On within {timeoutMs}ms");
            return false;
        }
        /// <summary>CIM Bit가 Off 될 때까지 대기. timeoutMs 내에 On 되면 true, 아니면 false(타임아웃 시 알람 발생).</summary>
        public static bool WaitForCIMBitOff(CIMCommand cmd, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (!IsCIMBitOn(cmd))
                    return true;
                System.Threading.Thread.Sleep(10);
            }
            OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} Master Bit not Off within {timeoutMs}ms");
            return false;
        }
        /// <summary>PLC Bit가 Off 될 때까지 대기. timeoutMs 내에 On 되면 true, 아니면 false(타임아웃 시 알람 발생).</summary>
        public static bool WaitForPLCBitOff(CIMCommand cmd, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (!IsPLCBitOn(cmd))
                    return true;
                System.Threading.Thread.Sleep(10);
            }
            OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} PLC Bit not Off within {timeoutMs}ms");
            return false;
        }
        /// <summary>PLC Bit가 On 될 때까지 대기. timeoutMs 내에 On 되면 true, 아니면 false(타임아웃 시 알람 발생).</summary>
        public static bool WaitForPLCBitOn(CIMCommand cmd, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (IsPLCBitOn(cmd))
                    return true;
                System.Threading.Thread.Sleep(10);
            }
            OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} PLC Bit not Off within {timeoutMs}ms");
            return false;
        }

        /// <summary>Master Command Bit가 On 된 후, 처리 완료 시 Local Reply를 3초 이내에 On 해야 함. 지연 시 알람.</summary>
        public static bool EnsureReplyWithinTimeout(CIMCommand cmd, Func<bool> doWork, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            bool done = doWork();
            if (done)
            {
                SetLocalPLCBitOn(cmd);
                return true;
            }
            if (sw.ElapsedMilliseconds >= timeoutMs)
            {
                OnCommandTimeoutAlarm?.Invoke("CIM_CMD_RESP_TIMEOUT", $"Command {cmd} response not completed within {timeoutMs}ms");
                return false;
            }
            return false;
        }

    }
}
