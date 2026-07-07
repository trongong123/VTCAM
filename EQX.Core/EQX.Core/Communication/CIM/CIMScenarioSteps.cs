// 2026-03-04 CursorAI에 의한 코드 생성: 시나리오별 별도 함수(Step) 클래스. RunScenario_XXX와 동일 동작, 외부 호출 가능.

namespace TOPENG_Device
{
    /// <summary>4.2 Terminal Display. Receive = Master Bit 대기 → Word 읽기 → Display 이벤트 → Reply. Send = Word Write → Bit On → Reply 대기.</summary>
    public static class CIMTerminalDisplay
    {
        public static void Receive()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.TerminalDisplay, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("DD011");
            var buf = new short[60];
            CIMAddressMap.ReadWords(startWord, 60, buf);
            string terminalMessage = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, buf.Length);
            if (!string.IsNullOrWhiteSpace(terminalMessage))
                CIMScenarioDispatcher.RaiseCIMMessageDisplayRequested(new CIMMessageDisplayEventArgs(CIMScenario.TerminalDisplay, "Terminal Display", terminalMessage));
            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.TerminalDisplay);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.TerminalDisplay, DateTime.Now));
        }

        public static void Send()
        {
            const int terminalDisplaySndWords = 60;
            var bufSnd = new short[terminalDisplaySndWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufSnd); else CIMScenarioDispatcher.FillWordsTestValue(ref bufSnd, 0x20);
            int wSnd = CIMAddressMap.GetWriteWordIndexFromDAddress("D1086");
            CIMAddressMap.WriteWords(wSnd, terminalDisplaySndWords, bufSnd);
            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.TerminalDisplay);
            CIMCommandHandler.WaitForCIMBitOn(CIMCommand.TerminalDisplay, CIMCommandHandler.CommandResponseTimeoutMs);
        }
    }

    /// <summary>4.3 OP Call 발생/해제. Occur = Master Bit 대기 후 Word 읽기·표시·Reply. Release = Confirm Word Write → Bit On → Reply 대기.</summary>
    public static class CIMOPCall
    {
        public static void Occur()
        {
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("DD058");
            var buf = new short[70];
            CIMAddressMap.ReadWords(startWord, 70, buf);
            string opCallID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, 10);
            string opCallMessage = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 10, 60);
            if (!string.IsNullOrWhiteSpace(opCallMessage))
                CIMScenarioDispatcher.RaiseCIMMessageDisplayRequested(new CIMMessageDisplayEventArgs(CIMScenario.OPCallOccurAndRelease, "Operator Call", opCallID, opCallMessage));
            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.OperatorCall);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.OperatorCall, DateTime.Now));
        }

        public static void Release(string id, string msg)
        {
            const int opCallConfirmWords = 150;
            var bufConfirm = new short[opCallConfirmWords];
            // 260304, CIM
            //if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufConfirm); else CIMScenarioDispatcher.FillWordsTestValue(ref bufConfirm, 0);

            // Cell ID : 0, 40
            // Product ID : 40, 20
            // Setp ID : 60, 20
            // Call ID : 80, 10
            CIMScenarioDispatcher.EncodeAsciiToWords(id, ref bufConfirm, 80, 10);
            // Call MSG : 90, 60
            CIMScenarioDispatcher.EncodeAsciiToWords(msg, ref bufConfirm, 90, 60);

            int wConfirm = CIMAddressMap.GetWriteWordIndexFromDAddress("DF30");
            CIMAddressMap.WriteWords(wConfirm, opCallConfirmWords, bufConfirm);

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.OPCallConfirm);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.OPCallConfirm, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.OPCallConfirm);
        }
    }

    /// <summary>4.4 Interlock 발생. ReadAndReply = Master Bit 대기 → Word 읽기 → Reply. ReportEquipState = D2C~D2F 설비 상태 Write.</summary>
    public static class CIMInterlockOccur
    {
        public static string ReadAndReply()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.Interlock, CIMCommandHandler.CommandResponseTimeoutMs)) return "-1";
            const int lenInterlockID = 10, lenInterlockMessage = 60, lenInterlockRCMD = 1;
            int totalWords = lenInterlockID + lenInterlockMessage + lenInterlockRCMD;
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("DD09E");
            var buf = new short[totalWords];
            CIMAddressMap.ReadWords(startWord, totalWords, buf);
            string interlockID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, lenInterlockID);
            string interlockMsg = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 10, lenInterlockMessage);
            string interlockRCMD = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 70, 1);
            if (!string.IsNullOrWhiteSpace(interlockMsg))
                CIMScenarioDispatcher.RaiseCIMMessageDisplayRequested(new CIMMessageDisplayEventArgs(CIMScenario.InterlockOccur, "Interlock Occur", interlockID, interlockMsg));

            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.Interlock);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.Interlock, DateTime.Now));

            return interlockRCMD;
        }

        public static void ReportEquipState()
        {
            CIMScenarioDispatcher.ApplyEquipReportState(EquipReportStateKind.Interlock);
        }
    }

    /// <summary>4.5 Interlock 해제. Confirm Word Write → Bit On → Reply 대기 → 설비 상태 IdleNormal → Bit Off.</summary>
    public static class CIMInterlockRelease
    {
        public static void Run(string id, string msg)
        {
            const int interlockConfirmWords = 150;
            var bufConfirm = new short[interlockConfirmWords];
            //if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufConfirm); else CIMScenarioDispatcher.FillWordsTestValue(ref bufConfirm, 0);
            
            // Cell ID : 0, 40
            // Product ID : 40, 20
            // Setp ID : 60, 20
            // Interlock ID : 80, 10
            CIMScenarioDispatcher.EncodeAsciiToWords(id, ref bufConfirm, 80, 10);
            // Interlock MSG : 90, 60
            CIMScenarioDispatcher.EncodeAsciiToWords(msg, ref bufConfirm, 90, 60);

            int wConfirm = CIMAddressMap.GetWriteWordIndexFromDAddress("DFF0");
            CIMAddressMap.WriteWords(wConfirm, interlockConfirmWords, bufConfirm);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.InterlockConfirm);
            if (!CIMEventHandler.WaitForCIMBitOn(EquipEvent.InterlockConfirm, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            CIMEventHandler.SetLocalRequestBitOff(EquipEvent.InterlockConfirm);
        }
    }

    /// <summary>4.6 Alarm 발생, 4.7 Alarm 해제, 4.8 Alarm 조회.</summary>
    public static class CIMAlarm
    {
        public static void AlarmOccur(CIMScenarioContext context)
        {
            CIMStatusWordMap.GetStatusBlockRange(StatusBlockKind.EtcAlarm, out string alarmStartD, out int alarmWords);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(alarmStartD);
            short[] buf = context?.CIMAlarmData != null ? context.CIMAlarmData.ToWordBuffer(w) : new CIMAlarmData { ALCD = 1, ALID = 0, AlarmDescription = "Test" }.ToWordBuffer(w);
            CIMAddressMap.WriteWords(w, alarmWords, buf);
        }

        public static void AlarmRelease(CIMScenarioContext context)
        {
            CIMStatusWordMap.GetStatusBlockRange(StatusBlockKind.EtcAlarm, out string alarmStartD, out int alarmWords);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(alarmStartD);
            short[] buf = new CIMAlarmData { ALCD = 0, ALID = 0, AlarmDescription = string.Empty }.ToWordBuffer(w);
            CIMAddressMap.WriteWords(w, alarmWords, buf);
        }

        public static void AlarmInquiry()
        {
            // TODO: 보류 (명세 §4.8 Alarm 조회)
        }
    }

    /// <summary>4.9 Equip State Change. 설비 상태 보고 1회.</summary>
    public static class CIMEquipStateChange
    {
        public static void Report()
        {
            CIMScenarioDispatcher.ReportEquipStatusOnce();
        }
    }

    /// <summary>4.10 Material Change. Word Write → Bit On → Reply 대기.</summary>
    public static class CIMMaterialChange
    {
        public static void ReportPort(CIMScenarioContext context)
        {
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.MaterialLocationUpdate1, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            short[] buf = context?.CIMMaterialData != null ? context.CIMMaterialData.ToWordBuffer(lengthWords) : new CIMMaterialData().ToWordBuffer(lengthWords);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.MaterialLocationUpdate1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.MaterialLocationUpdate1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.MaterialLocationUpdate1);
        }
    }

    /// <summary>4.11 Cell IN/OUT. CellIn = Track In Word Write → CellStart Bit → Reply. CellOut = Track Out Word Write → CellComplete Bit → Reply.</summary>
    public static class CIMCellInOut
    {
        // CellStartPort1 로 대체 사용
        public static void CellIn()
        {
            const int trackInWords = 107;
            var bufTrackIn = new short[trackInWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufTrackIn); else CIMScenarioDispatcher.FillWordsTestValue(ref bufTrackIn, 0);
            int wTrackIn = CIMAddressMap.GetWriteWordIndexFromDAddress("D4A0");
            CIMAddressMap.WriteWords(wTrackIn, trackInWords, bufTrackIn);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.CellStartPort1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.CellStartPort1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.CellStartPort1);
        }

        public static void CellOut()
        {
            const int trackOutWords = 167;
            var bufTrackOut = new short[trackOutWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufTrackOut); else CIMScenarioDispatcher.FillWordsTestValue(ref bufTrackOut, 0);
            int wTrackOut = CIMAddressMap.GetWriteWordIndexFromDAddress("D760");
            CIMAddressMap.WriteWords(wTrackOut, trackOutWords, bufTrackOut);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.CellCompPort1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.CellCompPort1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.CellCompPort1);
        }
    }

    /// <summary>4.12~4.19 PPID/Parameter/ECM 단계 구현.</summary>
    internal static class CIMScenarioStepsPPIDParam
    {
        public static void PPIDChangeRun()
        {
            const int paramPpidWords = 30;
            var buf = new short[paramPpidWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(buf); else CIMScenarioDispatcher.FillWordsTestValue(ref buf, 0);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DF7D");
            CIMAddressMap.WriteWords(w, paramPpidWords, buf);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.PPIDChange);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.PPIDChange, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.PPIDChange);
        }

        public static void PPIDCreateDeleteRun(CIMScenarioContext context)
        {
            switch (context.PPIDCreateDeleteKind)
            {
                case PPIDCreateDeleteKind.Create: PPIDCreateRun(context); break;
                case PPIDCreateDeleteKind.Delete: PPIDDeleteRun(context); break;
            }
                 
        }
        public static void PPIDCreateRun(CIMScenarioContext context)
        {
            const int ppidMode = 1;
            string no = "";

            // 실제 비트는 꺼졌는데, 내부비트가 살아있음. 디버그 필요.
            //if (!CIMCommandHandler.WaitForPLCBitOff(CIMCommand.FormattedProcessProgramSend, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            if (context.PPID_CreateType != 1) no = string.Format("{0:D2}N", context.PPID_No);
            else no = string.Format("{0:D2}O", context.PPID_No);

            writePPIDNo(no);
            writeRMSFixInfo(ppidMode, context.PPID_Name);
            writePPIDNameList(context.PPID_No, context.PPID_Name);

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.ParameterChange);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.ParameterChange, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.ParameterChange);

            // 260305, 계속 켜져있는 상황 때문에, 기다리고 나서도 안되있으면 그냥 off
            CIMEventHandler.SetLocalRequestBitOff(EquipEvent.ParameterChange);
        }
        public static void PPIDDeleteRun(CIMScenarioContext context)
        {
            const int ppidMode = 2;
            string no = "";

            if (context.PPID_DeleteType != 1) no = string.Format("{0:D2}D", context.PPID_No);
            else no = string.Format("{0:D2}G", context.PPID_No);

            writePPIDNo(no);
            writeRMSFixInfo(ppidMode, context.PPID_Name);
            writePPIDNameList(context.PPID_No, "");

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.ParameterChange);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.ParameterChange, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.ParameterChange);
        }

        public static void PPIDDownloadRun()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.FormattedProcessProgramSend2, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("DDF7D");
            const int masterParamWords = 4006;
            var buf = new short[masterParamWords];
            var paraList = new short[masterParamWords - 3];
            CIMAddressMap.ReadWords(startWord, masterParamWords, buf);

            string ppidType = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, 1);
            string ccode = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 1, 2);
            Array.Copy(buf, 3, paraList, 0, 4000);
            string name = CIMScenarioDispatcher.DecodeAsciiFromWords(paraList, 2, 20);
            string no = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 4003, 3);

            // Check 구문
            //CIMScenarioDispatcher.PPIDControlAck
            writePPIDControlAck(CheckPPIDControlEnable(ccode, no, name));

            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.FormattedProcessProgramSend);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.FormattedProcessProgramSend, DateTime.Now));

            if (CheckPPIDControlEnable(ccode, no, name) == "0")
            {
                if (!string.IsNullOrWhiteSpace(ccode))
                    CIMScenarioDispatcher.RaiseCIMRecipeControlRequested(new CIMRecipeControlEventArgs(CIMScenario.PPIDDownload, ccode, name, no));
            }
        }
        /// <summary> 상위에서 지령주는 것에 대한 응답코드, 7,8번만 장비에서 처리.
        /// </summary>
        /// <remarks>
        /// <code>
        /// 0 = Accepted, 가능하다
        /// 1 = Permission not granted, 권한이 없어 변경 할 수 없다
        /// 2 = Length Error, 길이 오류로 변경 할 수 없다
        /// 3 = Mode unsupported, 지원 되지 않는 모드로 변경 할 수 없다
        /// 4 = Target module is Offline, Off Line 상태로 변경 할 수 없다
        /// 5 = Target module is Down, 설비 Down 상태로 변경 할 수 없다
        /// 6 = Time Out Happened, Time Out 발생으로 변경 할 수 없다
        /// 7 = EQPID is not exist, 설비에 해당 EQPID 가 존재하지 않아 변경 할 수 없다
        /// 8 = MODULE ID is not exist, MODULE ID 가 존재하지 않아 변경 할 수 없다
        /// 9 = PPID_TYPE is not match, PPID_TYPE 맞지 않아 변경 할 수 없다
        /// </code>
        /// </remarks>
        public static string CheckPPIDControlEnable(string ccode, string no, string name)
        {
            string ret = "0";
            /*
            if (data.CIMRecipeData == null) return ret;
            int nno = Convert.ToInt32(no.Substring(0,2));
            string str = name.Substring(0, 3);

            // 커맨드별 확인사항
            switch (ccode)
            {
                case "1": // Create
                    // TT_ 시작하는 PPID로 01~90 번에 생성요청할때 ack 7
                    // TT_ 시작하지 않는 PPID로 91~99번에 생성요청할때. ack 7
                    // 이미 존재하는 번호에 생성하라고 할때 ack 8
                    if (nno >= 1 && nno <= 90)
                    {
                        if (str == "TT_") ret = "7";
                    }
                    if (nno >= 91 && nno <= 99)
                    {
                        if (str != "TT_") ret = "7";
                    }
                    if (data.CIMRecipeData.CheckExistRecipe(nno, name)) ret = "8";

                    break;

                case "2": // Delete
                    // 장비에 없는 PPID 번호로 요청할 때, ack 7 
                    if(!data.CIMRecipeData.CheckExitRecipeNo(nno)) ret = "7";
                    break;

                case "3": // Modify
                    // 이름하고, 번호가 맞지 않을 경우, ack 7
                    if(data.CIMRecipeData.CheckExistRecipe(nno, name))
                    {
                        if (data.CIMRecipeData.RecipeList.FindIndex(x => x.No == nno)
                            != data.CIMRecipeData.RecipeList.FindIndex(x => x.Name == name)) ret = "7";
                    }
                    break;
            }
            */
            return ret;
        }

        public static void ParameterChangeRun(CIMScenarioContext context)
        {
            const int ppidMode = 3;
            string no = "";

            no = string.Format("{0:D2}C", context.PPID_No);

            writePPIDNo(no);
            writeRMSFixInfo(ppidMode, context.PPID_Name);

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.ParameterChange);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.ParameterChange, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.ParameterChange);
        }

        public static void ParameterInquiryRun()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.FormattedProcessProgramRequest, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("DD1F0");
            var buf = new short[23];
            CIMAddressMap.ReadWords(startWord, 23, buf);
            string id = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, 20);

            writeRMSFixInfo(0, id);

            var bufWrite = new short[410];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(bufWrite); else CIMScenarioDispatcher.FillWordsTestValue(ref bufWrite, 0);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D923A");
            CIMAddressMap.WriteWords(w, 410, bufWrite);
            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.FormattedProcessProgramRequest);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.FormattedProcessProgramRequest, DateTime.Now));
        }

        public static void PPIDListInquiryRun()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.CurrentEquipPPIDListRequest, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            //const int ppidListWords = 100;
            //var buf = new short[ppidListWords];
            //if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(buf); else CIMScenarioDispatcher.FillWordsTestValue(ref buf, 0);
            //int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D500");
            //CIMAddressMap.WriteWords(w, ppidListWords, buf);

            //// 값은 실시간 보고 개념

            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.CurrentEquipPPIDListRequest);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.CurrentEquipPPIDListRequest, DateTime.Now));
        }

        public static void EquipConstantNameListRun()
        {
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.EquipConstantNameList, CIMCommandHandler.CommandResponseTimeoutMs)) return;

            //// 값은 실시간 보고 개념

            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.EquipConstantNameList);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.EquipConstantNameList, DateTime.Now));
        }

        private static void writePPIDControlAck(string ack = "0")
        {
            const int count = 1;
            var buf = new short[count];

            string str = ack.Substring(0, 1);

            buf[0] = Convert.ToInt16(Convert.ToChar(str)); 

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D125D");
            CIMAddressMap.WriteWords(w, count, buf);
        }
        private static void writePPIDNameList()
        {
            const int count = 2000;
            var buf = new short[count];

            // 전체 레시피 100ea 에 대한 이름 업데이트 -> buf

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D8A54");
            CIMAddressMap.WriteWords(w, count, buf);
        }
        private static void writePPIDNameList(int no, string name)
        {
            const int count = 20;
            int addr = 0x8A54 + ((no - 1) * count);
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(name, ref buf, 0, count);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(string.Format("D{0:X}", addr));
            CIMAddressMap.WriteWords(w, count, buf);
        }
        private static void writePPIDNo(string no)
        {
            const int count = 3;
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(no, ref buf, 0, 3);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D23D9");
            CIMAddressMap.WriteWords(w, count, buf);
        }
        private static void writeRMSFixInfo(int ppidMode, string name)
        {
            const int count = 22;
            var buf = new short[count];

            // PPID Mode 2word
            // PPID Name 20Word
            buf[0] = (short)ppidMode;
            CIMScenarioDispatcher.EncodeAsciiToWords(name, ref buf, 2, 20);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D9224");
            CIMAddressMap.WriteWords(w, count, buf);
        }
    }

    /// <summary>4.12 PPID Change. Parameter Word Write → PPIDChange Bit → Reply 대기.</summary>
    public static class CIMPPIDChange { public static void Run() { CIMScenarioStepsPPIDParam.PPIDChangeRun(); } }

    /// <summary>4.13 PPID 생성/삭제. Parameter Word(PPID Mode 1/2) Write → ParameterChange Bit → Reply 대기.</summary>
    public static class CIMPPIDCreateDelete
    {
        public static void Create(CIMScenarioContext context) { CIMScenarioStepsPPIDParam.PPIDCreateRun(context); }
        public static void Delete(CIMScenarioContext context) { CIMScenarioStepsPPIDParam.PPIDDeleteRun(context); }
        public static void Run(CIMScenarioContext context) { CIMScenarioStepsPPIDParam.PPIDCreateDeleteRun(context); }
    }

    /// <summary>4.14 PPID Download. Master Bit 대기 → Read → Reply → Parameter Write → ParameterChange Bit → Reply.</summary>
    public static class CIMPPIDDownload { public static void Run() { CIMScenarioStepsPPIDParam.PPIDDownloadRun(); } }

    /// <summary>4.15 Parameter Change. Parameter Word(Mode=3) Write → ParameterChange Bit → Reply.</summary>
    public static class CIMParameterChange { public static void Run(CIMScenarioContext context) { CIMScenarioStepsPPIDParam.ParameterChangeRun(context); } }

    /// <summary>4.16 Parameter 조회. Master Bit 대기 → Request PPID Read → DF7D Write(Mode=0) → Reply.</summary>
    public static class CIMParameterInquiry { public static void Run() { CIMScenarioStepsPPIDParam.ParameterInquiryRun(); } }

    /// <summary>4.17 PPID List 조회. Master Bit 대기 → PPID List Word Write → Reply.</summary>
    public static class CIMPPIDListInquiry { public static void Run() { CIMScenarioStepsPPIDParam.PPIDListInquiryRun(); } }

    /// <summary>4.18 Equipment Constant 조회. 보류.</summary>
    public static class CIMEquipConstantInquiry { public static void Run() { /* TODO: 보류 */ } }

    /// <summary>4.19 Equipment Constant Name List 조회. Master Bit 대기 → ECM Word Write → Reply.</summary>
    public static class CIMEquipConstantNameListInquiry { public static void Run() { CIMScenarioStepsPPIDParam.EquipConstantNameListRun(); } }

    /// <summary>4.20 Equipment Position. 미사용.</summary>
    public static class CIMEquipPosition { public static void Run() { /* TODO: 미사용 */ } }

    /// <summary>4.21 TPM Loss. TPMLossReady On → Word Write → TPMLoss Bit → Reply → Ready Off.</summary>
    public static class CIMTPMLoss
    {
        public static void Run()
        {
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.TPMLossReady);
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.TPMLoss, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            var buf = new short[lengthWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(buf); else CIMScenarioDispatcher.FillWordsTestValue(ref buf, 0);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.TPMLoss);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.TPMLoss, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.TPMLossReady);
        }
    }

    /// <summary>4.22~4.47 시나리오 공통: Event Word Write → Bit On → Reply 대기 (또는 TODO).</summary>
    public static class CIMScenarioStepsEventRun
    {
        public static void RunEventWordBitReply(EquipEvent evt, CIMScenarioContext context, short[] bufOverride = null)
        {
            CIMEventItemMap.GetEventItemWordRange(evt, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            short[] buf = bufOverride;
            if (buf == null || buf.Length != lengthWords)
            {
                buf = new short[lengthWords];
                if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(buf); else CIMScenarioDispatcher.FillWordsTestValue(ref buf, 0);
            }
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            CIMEventHandler.SetLocalRequestBitOn(evt);
            if (CIMEventHandler.WaitForCIMBitOn(evt, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(evt);
        }
    }

    /// <summary>4.22 IQC+POL Cell IN/OUT. 보류.</summary>
    public static class CIMIQCPOLCellInOut { public static void Run() { /* TODO: 보류 */ } }

    /// <summary>4.23 Material Kitting Cancel. 보류.</summary>
    public static class CIMMaterialKittingCancel { public static void Run() { /* TODO: 보류 */ } }

    /// <summary>4.24 Material Assemble Process.</summary>
    public static class CIMMaterialAssembleProcess { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.MaterialAssembleProcess1_1, null, null); } }

    /// <summary>4.25 Material NG Process.</summary>
    public static class CIMMaterialNGProcess { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.MaterialNGProcess1_1, null, null); } }

    /// <summary>4.26 Operator Log In/Out. Bit On → Word Write → Reply 대기 → Bit Off.</summary>
    public static class CIMOperatorLogInOut
    {
        public static void Run(CIMScenarioContext context)
        {
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.OperatorLogInfomation);
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.OperatorLogInfomation, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            var buf = new short[lengthWords];
            if (CIMScenarioDispatcher.UseDummyWordDataForCIM) CIMScenarioDispatcher.FillWordsWithDummyData(buf); else CIMScenarioDispatcher.FillWordsTestValue(ref buf, 0);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.OperatorLogInfomation, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.OperatorLogInfomation);
        }
    }

    /// <summary>4.27 Material Supplement Request.</summary>
    public static class CIMMaterialSupplementRequest { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.MaterialSupplementRequest1, null, null); } }

    /// <summary>4.28 Material Supplement Complete.</summary>
    public static class CIMMaterialSupplementComplete { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.MaterialSupplementComplete1, null, null); } }

    /// <summary>4.29 Material Alarm. Warning 또는 Shortage.</summary>
    public static class CIMMaterialAlarm
    {
        public static void Run(CIMScenarioContext context)
        {
            var kind = context?.MaterialAlarmKind ?? MaterialAlarmKind.Warning;
            var evt = kind == MaterialAlarmKind.Shortage ? EquipEvent.MaterialShortage1 : EquipEvent.MaterialWarning1;
            CIMScenarioStepsEventRun.RunEventWordBitReply(evt, context, null);
        }
    }

    /// <summary>4.30~4.32 APN ID. 미사용.</summary>
    public static class CIMAPNIDRequestProcess { public static void Run() { /* TODO: 미사용 */ } }
    public static class CIMAPNIDIssueConfirmProcess { public static void Run() { /* TODO: 미사용 */ } }
    public static class CIMAPNIDGrapeIDRequestProcess { public static void Run() { /* TODO: 미사용 */ } }

    /// <summary>4.33~4.36 Load/Unload Request/Complete.</summary>
    public static class CIMLoadRequest { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.CassetteStateChangeEvent1_A, null, null); } }
    public static class CIMLoadComplete { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.CassetteStateChangeEvent1_B, null, null); } }
    public static class CIMUnloadRequest { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.CassetteStateChangeEvent1_C, null, null); } }
    public static class CIMUnloadComplete { public static void Run() { CIMScenarioStepsEventRun.RunEventWordBitReply(EquipEvent.CassetteStateChangeEvent1_D, null, null); } }

    /// <summary>4.37 Material Location Update.</summary>
    public static class CIMMaterialLocationUpdate
    {
        public static void Run(CIMScenarioContext context)
        {
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.MaterialLocationUpdate1, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            short[] buf = context?.CIMMaterialData != null ? context.CIMMaterialData.ToWordBuffer(lengthWords) : new CIMMaterialData().ToWordBuffer(lengthWords);
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.MaterialLocationUpdate1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.MaterialLocationUpdate1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.MaterialLocationUpdate1);
        }
    }

    /// <summary>4.38~4.47 보류/미사용.</summary>
    public static class CIMCellTrackINValidationCheck { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMCurrentProcessControlDataRequest { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMProcessControlRequestReport { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMProcessControlResultReport { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMUnitStatusChange { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMCellLoaderPortLoadUnload { public static void Run() { /* TODO: 미사용 */ } }
    public static class CIMCellUnloaderPortLoadUnload { public static void Run() { /* TODO: 미사용 */ } }
    public static class CIMJobStartCancel { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMProcessJobEvent { public static void Run() { /* TODO: 보류 */ } }
    public static class CIMPortStateChange { public static void Run() { /* TODO: 보류 */ } }



    public static class CIMCellProcessStart
    {
        private static object _lock = new object();

        public static void Run(CIMScenarioContext context)
        {
            switch (context.CellStartPortNo)
            {
                case 1: Start(context.CIMCellTrackInData1, EquipEvent.CellStartPort1); break;
                case 2: Start(context.CIMCellTrackInData2, EquipEvent.CellStartPort2); break;
                case 3: Start(context.CIMCellTrackInData3, EquipEvent.CellStartPort3); break;
                case 4: Start(context.CIMCellTrackInData4, EquipEvent.CellStartPort4); break;
            }
        }

        public static void Start(CIMCellTrackInData data, EquipEvent evt)
        {
            lock (_lock)
            {
                CIMEventItemMap.GetEventItemWordRange(evt, out string startD, out int lengthWords);
                if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
                if (data.CellID == "") return;
                // 데이터 없을 때 생성구문 미작성. 무조건 넣어서 실행해야함.

                WriteTrackInData(data, startD, lengthWords);

                CIMEventHandler.SetLocalRequestBitOn(evt);
                if (CIMEventHandler.WaitForCIMBitOn(evt, CIMCommandHandler.CommandResponseTimeoutMs))
                    CIMEventHandler.SetLocalRequestBitOff(evt);
            }
        }

        private static void WriteTrackInData(CIMCellTrackInData data, string addr, int count)
        {
            //const int count = 107;
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(data.CellID, ref buf, 0, 40);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ProductID, ref buf, 40, 20);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.StepID, ref buf, 60, 20);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ProductID, ref buf, 80, 20);
            CIMScenarioDispatcher.EncodeInt32ToWords(data.PlanQuantity, ref buf, 100);
            CIMScenarioDispatcher.EncodeInt32ToWords(data.ProcessQuantity, ref buf, 102);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReaderID, ref buf, 104, 1);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.RRC, ref buf, 105, 1);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReasonCode, ref buf, 106, 1);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(addr);
            CIMAddressMap.WriteWords(w, count, buf);
        }
    }
    public static class CIMCellIDReadingResult
    {
        public static void Run(CIMScenarioContext context)
        {
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.CellIDReadingResult1, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;

            WriteData(context.MCRReadData1, startD, lengthWords);

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.CellIDReadingResult1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.CellIDReadingResult1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.CellIDReadingResult1);
        }


        private static void WriteData(CIM_MCR_Read_Data data, string addr, int count)
        {
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReadID, ref buf, 0, 40);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReadERCode, ref buf, 40, 1);
            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReadERID, ref buf, 41, 1);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(addr);
            CIMAddressMap.WriteWords(w, count, buf);
        }
    }
    public static class CIMCellInfomationRequest
    {
        public static void Run(CIMScenarioContext context)
        {
            // Send S6F207
            CIMEventItemMap.GetEventItemWordRange(EquipEvent.CellInformationRequest1, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;

            WriteData(context.MCRReadData1, startD, lengthWords);

            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.CellInformationRequest1);
            if (CIMEventHandler.WaitForCIMBitOn(EquipEvent.CellInformationRequest1, CIMCommandHandler.CommandResponseTimeoutMs))
                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.CellInformationRequest1);

            // Wait S2F103
            // TEST 위해 기다리는 시간 변경 3sec -> 30sec
            //if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.CellInformationRequest1, CIMCommandHandler.CommandResponseTimeoutMs)) return;
            if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.CellInofrmationDownload1, 30000)) return;

            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("D111C6");
            var buf = new short[61];
            CIMAddressMap.ReadWords(startWord, 61, buf);

            // 이거 밖으로 리턴시켜서 관리해야될듯?
            //CIMCellInfoDownloadData data = new CIMCellInfoDownloadData
            //{
            //    CellID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, 40),
            //    ProductID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 40, 20),
            //    CellInfoResut = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 60, 1),
            //};

            //if (data.CIMCellData != null)
            //{
            //    data.CIMCellData.CellID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 0, 40);
            //    data.CIMCellData.ProductID = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 40, 20);
            //    data.CIMCellData.CellInfoResut = CIMScenarioDispatcher.DecodeAsciiFromWords(buf, 60, 1);
            //}

            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.CellInofrmationDownload1);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.CellInofrmationDownload1, DateTime.Now));

            // ack 응답에 따른 장비 프로세스 진행 or 배출작업 결정될 수 있어야함.
            //bool ack = CheckCellInfoAck(context.MCRReadData1.ReadID, data.CIMCellData);
        }

        private static void WriteData(CIM_MCR_Read_Data data, string addr, int count)
        {
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(data.ReadID, ref buf, 0, 20);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(addr);
            CIMAddressMap.WriteWords(w, count, buf);
        }
        /// <summary>
        /// true : Pass, false : ng
        /// </summary>
        /// <param name="readID"></param>
        /// <param name="downData"></param>
        /// <returns></returns>
        private static bool CheckCellInfoAck(string readID, CIMCellInfoDownloadData downData)
        {
            bool ret = false;

            if(downData.CellInfoResut == "0" || downData.CellInfoResut == "42")
            {
                ret = true;
            }

            return ret;
        }
       
    }
    public static class CIMEquipFunctionChange 
    {
        public static void RunCommand(CIMScenarioContext context)
        {

        } 
    }
}
