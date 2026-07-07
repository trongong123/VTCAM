using System;
// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// 참고: doc/CIM/SDC CIM 구현 컨셉.md, doc/CIM/CIM명령어_명세서.md
// §1 Command Type Pair, §2 Event Type Pair, §4 동작 시나리오(47개) 기준

namespace TOPENG_Device
{
    /// <summary>
    /// Command Type Pair - Master 동작. CIM Address = Master Bit(읽기), PLC Address = Local Reply Bit(쓰기).
    /// </summary>
    public enum CIMCommand
    {
        AliveBit = 0,  // 시나리오 4.1에서 Master Alive 확인용
        DatetimeSet,
        TerminalDisplay,
        OperatorCall,
        Interlock,
        JOBStart,
        OnlineRemote,
        OnlineLocal,
        OffLine,
        TCDisconnectAlarm,
        CellStartPort1,
        CellStartPort2,
        CellStartPort3,
        CellStartPort4,
        FormattedProcessProgramSend2,
        EquipConstantNameList,
        FormattedProcessProgramSend,
        FormattedProcessProgramRequest,
        CurrentEquipPPIDListRequest,
        BatchLotInforDownload,
        CellJobProcess1,
        CellJobProcess2,
        CellJobProcess3,
        CellJobProcess4,
        EquipApproveProecss,
        MaterialInfoSend1,
        MaterialInfoSend2,
        MaterialInfoSend3,
        LabelInformationSend,
        LabelValidationSend,
        LabelPrecheckSend,
        CurrentProcessControlDataReq,
        ProcessControlInformSend1,
        ProcessControlInformSend2,
        ProcessControlInformSend3,
        ProcessControlInformSend4,
        CarrierInformationSendCassette1,
        CarrierInformationSendCassette2,
        CarrierInformationSendCassette3,
        CellLotInformationSend1_1,
        CellLotInformationSend1_2,
        CellLotInformationSend2_1,
        CellLotInformationSend2_2,
        CellLotInformationSend3_1,
        CellLotInformationSend3_2,
        CarrierInformationSendLoader1,
        CarrierInformationSendUnloader1,
        CarrierInformationSendLoader2,
        CarrierInformationSendUnloader2,
        CarrierInformationSendLoader3,
        CarrierInformationSendUnloader3,
        CellInofrmationDownload1,
        CellInofrmationDownload2,
        SpecificValidationDataSend1,
        SpecificValidationDataSend2,
        SpecificValidationDataSend3,
        SpecificValidationDataSend4,
        SpecificValidationDataSend5,
        EquipFunctionChangeCommand,
        EquipMachineControl1,
        EquipMachineControl2,
        EquipMachineControl3,
        EquipMachineControl4,
        EquipMachineControl5,
        EquipMachineControl6,
        EquipMachineControl7,
        EquipMachineControl8,
        EquipControlInformation,
        EquipUnitOPCallSend,
        CarrierInformationSendCassetteBatch,
        EquipConfigFileChange1,
        EquipConfigFileChange2,
        InspectionCarrierReleaseInfoSend1,
        InspectionCarrierReleaseInfoSend2,
        InspectionCarrierAssignInfoSend1,
        InspectionCarrierAssignInfoSend2,
        CarrierInformationSendLoader1_CEID262,
        CarrierInformationSendLoader1_CEID256,
        CarrierInformationSendLoader2_CEID262,
        CarrierInformationSendLoader2_CEID256,
        CarrierInformationSendUnloader1_CEID262,
        CarrierInformationSendUnloader1_CEID260Sub,
        CarrierInformationSendUnloader2_CEID262,
        CarrierInformationSendUnloader2_CEID260Sub,
        APCWordTotal,
        EquipApproveProcess,
    }

    /// <summary>
    /// Event Type Pair - Local 동작. PLC Address = Local 호출 Bit(쓰기), CIM Address = Master Reply Bit(읽기).
    /// </summary>
    public enum EquipEvent
    {
        AliveBit,
        JobEvent1,
        TPMLoss,
        JobEvent2,
        TPMLossReady,
        PPIDChange,
        ParameterTotal,
        ParameterChange,
        ParameterChange2,
        EquipmentConstantParameterChangeEvent,
        CellStartPort1,
        CellStartPort2,
        CellStartPort3,
        CellStartPort4,
        CellCompPort1,
        CellCompPort2,
        CellCompPort3,
        CellCompPort4,
        CellCompPort5,
        CellCompPort6,
        OPCallConfirm,
        InterlockConfirm,
        MaterialIDReadingResult1,
        MaterialIDReadingResult2,
        MaterialIDReadingResult3,
        BatchLotInforRequest,
        CellIDReadingResult1,
        StartCellLot1,
        CellIDReadingResult2,
        StartCellLot2,
        CarrierLoaderEvent,
        CarrierUnloadEvent,
        OperatorLogInfomation,
        InspectionResultReport,
        PairCellProcessStart,
        PairCellProcessStart_2,
        PairCellProcessComplete,
        PairCellProcessComplete_2,
        PairCellProcessComplete_3,
        MaterialAssembleProcess1_1,
        MaterialAssembleProcess1_2,
        MaterialAssembleProcess1_3,
        MaterialAssembleProcess2_1,
        MaterialAssembleProcess2_2,
        MaterialAssembleProcess2_3,
        MaterialAssembleProcess3_1,
        MaterialAssembleProcess3_2,
        MaterialAssembleProcess4_1,
        MaterialAssembleProcess4_2,
        MaterialAssembleProcess5_1,
        MaterialAssembleProcess5_2,
        MaterialNGProcess1_1,
        MaterialNGProcess2_1,
        MaterialNGProcess3_1,
        MaterialNGProcess4_1,
        MaterialNGProcess5_1,
        MaterialNGProcess6_1,
        PerformanceLossEvent1,
        PerformanceLossEvent2,
        PerformanceLossEvent3,
        MaterialSupplementRequest1,
        MaterialSupplementRequest2,
        MaterialSupplementRequest3,
        MaterialSupplementComplete1,
        MaterialSupplementComplete2,
        MaterialSupplementComplete3,
        KittingorCancel1,
        KittingorCancel2,
        KittingorCancel3,
        MaterialWarning1,
        MaterialShortage1,
        MaterialWarning2,
        MaterialShortage2,
        MaterialWarning3,
        MaterialShortage3,
        MaterialLocationUpdate1,
        MaterialLocationUpdate2,
        MaterialLocationUpdate3,
        LabelInformationRequest,
        LabelValidationRequest,
        LabelPrecheckRequest,
        ProcessControlResult1,
        ProcessControlResult2,
        ProcessControlResult3,
        ProcessControlResult4,
        ProcessControlDataCreateDelete1,
        ProcessControlDataCreateDelete2,
        ProcessControlDataCreateDelete3,
        ProcessControlDataCreateDelete4,
        CassetteStateChangeEvent1_A,
        CassetteStateChangeEvent1_B,
        CassetteStateChangeEvent1_C,
        CassetteStateChangeEvent1_D,
        CassetteStateChangeEvent2_A,
        CassetteStateChangeEvent2_B,
        CassetteStateChangeEvent2_C,
        CassetteStateChangeEvent2_D,
        CassetteStateChangeEvent3_A,
        CassetteStateChangeEvent3_B,
        CassetteStateChangeEvent3_C,
        CassetteStateChangeEvent3_D,
        CassetteStateChangeEvent4_A,
        CassetteStateChangeEvent4_B,
        CassetteStateChangeEvent4_C,
        CassetteStateChangeEvent4_D,
        CassetteStateChangeEvent5_A,
        CassetteStateChangeEvent5_B,
        CassetteStateChangeEvent5_C,
        CassetteStateChangeEvent5_D,
        CellLotInformationRequest1_1,
        CellLotInformationRequest1_2,
        CellLotInformationRequest2_1,
        CellLotInformationRequest2_2,
        CellLotInformationRequest3_1,
        CellLotInformationRequest3_2,
        GetAttributeRequest1,
        GetAttributeRequest2,
        GetAttributeRequest3,
        TRSProcessJobLocationUpdate,
        CarrierProcessChangeLoader1,
        CarrierProcessChangeUnloader1,
        CarrierProcessChangeLoader2,
        CarrierProcessChangeUnloader2,
        CarrierProcessChangeLoader3,
        CarrierProcessChangeUnloader3,
        SpecificValidationRequest1,
        SpecificValidationRequest2,
        SpecificValidationRequest3,
        SpecificValidationRequest4,
        SpecificValidationRequest5,
        EquipFunctionChangeEvent,
        EquipFunctionChangeCMDHcack,
        UnitInterlockConfirm1,
        UnitInterlockConfirm2,
        UnitInterlockConfirm3,
        UnitInterlockConfirm4,
        UnitInterlockConfirm5,
        UnitInterlockConfirm6,
        UnitInterlockConfirm7,
        UnitInterlockConfirm8,
        UnitOPCallConfirm,
        UnitTPMLossReady1,
        UnitTPMLossReady2,
        UnitTPMLossReady3,
        UnitTPMLossReady4,
        UnitTPMLossReady5,
        UnitTPMLossReady6,
        UnitTPMLossReady7,
        UnitTPMLossReady8,
        UnitTPMLoss1,
        UnitTPMLoss2,
        UnitTPMLoss3,
        UnitTPMLoss4,
        UnitTPMLoss5,
        UnitTPMLoss6,
        UnitTPMLoss7,
        UnitTPMLoss8,
        CarrierProcessChangeCassetteBatch,
        PackingInformation,
        PackingJobStart,
        InnerLabelAttach,
        CartonLabelAttach,
        PackingLabelReading,
        JigAssembleProcess1_1,
        JigAssembleProcess1_2,
        JigAssembleProcess2_1,
        JigAssembleProcess2_2,
        JigAssembleProcess3_1,
        JigAssembleProcess3_2,
        JigAssembleProcess4_1,
        JigAssembleProcess4_2,
        JigAssembleProcess5_1,
        JigAssembleProcess5_2,
        JigAssembleProcess6_1,
        JigAssembleProcess6_2,
        JigAssembleProcess7_1,
        JigAssembleProcess7_2,
        JigAssembleProcess8_1,
        JigAssembleProcess8_2,
        CellInformationRequest1,
        CellInformationRequest2,
        InspectionCarrierReleaseRequest1,
        InspectionCarrierReleaseRequest2,
        InspectionCarrierAssignRequest1,
        InspectionCarrierAssignRequest2,
        CarrierProcessChangeLoader1_CEID262,
        CarrierProcessChangeLoader1_CEID256,
        CarrierProcessChangeLoader2_CEID262,
        CarrierProcessChangeLoader2_CEID256,
        CarrierProcessChangeUnloader1_CEID262,
        CarrierProcessChangeUnloader1_CEID260Sub,
        CarrierProcessChangeUnloader2_CEID262,
        CarrierProcessChangeUnloader2_CEID260Sub,
        MaterialPreHandCheck1,
        MaterialPreHandCheck2,
        MaterialPreHandCheck3,
        NormalDataCollection1,
        NormalDataCollection2,
        SpecificStepEvent1_1,
        SpecificStepEvent1_2,
        SpecificStepEvent2_1,
        SpecificStepEvent2_2,


        // --- Nhóm Pseudo-Event (Chỉ có WORD Area, không có BIT) ---
        MaterialPortState,  // W 160 ~ W 307
        MaterialPortState1,  // W 160 ~ W 194
        MaterialPortState2,  // W 195 ~ W 1C9
        MaterialPortState3,  // W 1CA ~ W 1FE
        MaterialPortState4,  // W 1FF ~ W 233
        MaterialPortState5,  // W 234 ~ W 268
        MaterialPortState6,  // W 268 ~ W 29D
        MaterialPortState7,  // W 29D ~ W 2D2
        MaterialPortState8,  // W 2D3 ~ W 307
        PortStatus,         // W 2C ~ W 5C
        UnitStatus,         // W 450 ~ W 487
        JigPortState,       // W 59A4 ~ W 5AFF

        // CUSTOM
        EQPPIDUpdate,
        PPIDModeAndPPIDUpdate,
        CurrentEquipPPIDListResponse,
        FormattedProcessProgramSend,
        CurrentProcessControlDataReply,
        APCWordTotal,
    }

    /// <summary>
    /// 동작 시나리오 47개 - CIM명령어_명세서 §4와 1:1 대응.
    /// </summary>
    public enum CIMScenario
    {
        Initialize = 0,              // 4.1
        TerminalDisplay,            // 4.2
        OPCallOccurAndRelease,      // 4.3
        InterlockOccur,             // 4.4
        InterlockRelease,           // 4.5
        AlarmOccur,                 // 4.6
        AlarmRelease,               // 4.7
        AlarmInquiry,               // 4.8
        EquipStateChange,           // 4.9
        MaterialChange,             // 4.10
        CellInOut,                  // 4.11
        PPIDChange,                 // 4.12
        PPIDCreateDelete,           // 4.13
        PPIDDownload,               // 4.14
        ParameterChange,            // 4.15
        ParameterInquiry,           // 4.16
        PPIDListInquiry,            // 4.17
        EquipConstantInquiry,       // 4.18
        EquipConstantNameListInquiry, // 4.19
        EquipPosition,              // 4.20 미사용
        TPMLoss,                    // 4.21
        IQCPOLCellInOut,            // 4.22 보류
        MaterialKittingCancel,      // 4.23 보류
        MaterialAssembleProcess,    // 4.24
        MaterialNGProcess,          // 4.25
        OperatorLogInOut,           // 4.26
        MaterialSupplementRequest,  // 4.27
        MaterialSupplementComplete, // 4.28
        MaterialAlarm,              // 4.29
        APNIDRequestProcess,       // 4.30 미사용
        APNIDIssueConfirmProcess,   // 4.31 미사용
        APNIDGrapeIDRequestProcess,// 4.32 미사용
        LoadRequest,                // 4.33
        LoadComplete,               // 4.34
        UnloadRequest,              // 4.35
        UnloadComplete,             // 4.36
        MaterialLocationUpdate,     // 4.37
        CellTrackINValidationCheck, // 4.38 보류
        CurrentProcessControlDataRequest, // 4.39 보류
        ProcessControlRequestReport,     // 4.40 보류
        ProcessControlResultReport,      // 4.41 보류
        UnitStatusChange,           // 4.42 보류
        CellLoaderPortLoadUnload,    // 4.43 미사용
        CellUnloaderPortLoadUnload, // 4.44 미사용
        JobStartCancel,             // 4.45 보류
        ProcessJobEvent,            // 4.46 보류
        PortStateChange,            // 4.47 보류
        EquipFunctionChangeCommand,        // 16.7
        CellStartPort,
        CellIDReduingResult,
        CellInformationRequest,
    }

    // 2026-03-04 CursorAI에 의한 코드 생성: 47개 시나리오 Sub 함수(XXX.YYY) 식별자. UI 테스트 버튼·Runner switch에서 사용.
    /// <summary>시나리오별 Sub 함수. 표기: Initialize.AliveCheck → Initialize_AliveCheck. 외부 호출·단위 테스트용.</summary>
    public enum CIMSubFunction
    {
        Initialize_AliveCheck,
        Initialize_DateTimeSync,
        TerminalDisplay_Receive,
        TerminalDisplay_Send,
        OPCallOccurAndRelease_Occur,
        OPCallOccurAndRelease_Release,
        InterlockOccur_ReadAndReply,
        InterlockOccur_ReportEquipState,
        InterlockRelease_Run,
        AlarmOccur_Run,
        AlarmRelease_Run,
        AlarmInquiry_Run,
        EquipStateChange_Report,
        MaterialChange_ReportPort,
        CellInOut_CellIn,
        CellInOut_CellOut,
        PPIDChange_Run,
        PPIDCreateDelete_Create,
        PPIDCreateDelete_Delete,
        PPIDDownload_Run,
        ParameterChange_Run,
        ParameterInquiry_Run,
        PPIDListInquiry_Run,
        EquipConstantInquiry_Run,
        EquipConstantNameListInquiry_Run,
        EquipPosition_Run,
        TPMLoss_Run,
        IQCPOLCellInOut_Run,
        MaterialKittingCancel_Run,
        MaterialAssembleProcess_Run,
        MaterialNGProcess_Run,
        OperatorLogInOut_Run,
        MaterialSupplementRequest_Run,
        MaterialSupplementComplete_Run,
        MaterialAlarm_Warning,
        MaterialAlarm_Shortage,
        APNIDRequestProcess_Run,
        APNIDIssueConfirmProcess_Run,
        APNIDGrapeIDRequestProcess_Run,
        LoadRequest_Run,
        LoadComplete_Run,
        UnloadRequest_Run,
        UnloadComplete_Run,
        MaterialLocationUpdate_Run,
        CellTrackINValidationCheck_Run,
        CurrentProcessControlDataRequest_Run,
        ProcessControlRequestReport_Run,
        ProcessControlResultReport_Run,
        UnitStatusChange_Run,
        CellLoaderPortLoadUnload_Run,
        CellUnloaderPortLoadUnload_Run,
        JobStartCancel_Run,
        ProcessJobEvent_Run,
        PortStateChange_Run,
        EquipFunctionChangeCommand, // 260305, CIM TEST
        CellStartPort,
        CellIDReadingResult,
        CellInformationRequest,
    }

    // 2026-02-25 CursorAI - 시나리오 호출 시 구분 옵션(4.2 수신/송신, 4.11 Cell In/Out, 4.26 Log In/Out, 4.29 부족/수량0 등)
    /// <summary>4.2 Terminal Display: 수신(Master→Local) 또는 송신(Local→Master) 구분</summary>
    public enum TerminalDisplayMode { Receive, Send }
    /// <summary>4.11 Cell IN/OUT: Cell In 또는 Cell Out 구분</summary>
    public enum CellInOutKind { CellIn, CellOut }
    /// <summary>4.26 Operator Log: Log In 또는 Log Out 구분</summary>
    public enum OperatorLogMode { LogIn, LogOut }
    /// <summary>4.29 Material Alarm: 부족 수량(Warning) 또는 수량 0(Shortage) 구분</summary>
    public enum MaterialAlarmKind { Warning, Shortage }
    /// <summary>4.13 PPID 생성/삭제 Sub 함수 구분</summary>
    public enum PPIDCreateDeleteKind { Create, Delete }

    // 2026-03-02 CursorAI에 의한 코드 생성 - CIMScenarioDispatcher 설비 상태 설정 함수용. doc/CIM/CIM Online Check List.md §3
    /// <summary>설비 상태 보고 종류. D2C(EQPAvailability)·D2D(EQPInterlock)·D2E(EQPMove)·D2F(EQPRun) 조합. IDLE/NORMAL, DOWN, INTERLOCK 등 시나리오에서 사용.</summary>
    public enum EquipReportStateKind
    {
        /// <summary>§3.1 초기화(Normal, Idle): AVAILABILITY=2(UP), INTERLOCK=2(OFF), MOVESTATE=2(RUNNING), RUNSTATE=1(IDLE)</summary>
        IdleNormal,
        /// <summary>§3.2 Down: AVAILABILITY=1(DOWN), INTERLOCK=2(OFF), MOVESTATE=1(PAUSE), RUNSTATE=1(IDLE)</summary>
        Down,
        /// <summary>§3.3 Interlock: AVAILABILITY=2(UP), INTERLOCK=1(ON), MOVESTATE=1(PAUSE), RUNSTATE=1(IDLE)</summary>
        Interlock,
        /// <summary>가동 중: AVAILABILITY=2(UP), INTERLOCK=2(OFF), MOVESTATE=2(RUNNING), RUNSTATE=2(RUN)</summary>
        Running
    }

    /// <summary>
    /// 시나리오별 호출 옵션. ExecuteScenario(scenario, context)로 전달하여 수신/송신, Cell In/Out, Log In/Out, Material Warning/Shortage, Interlock 설비 상태 보고 여부 등을 구분.
    /// </summary>
    public class CIMScenarioContext
    {
        /// <summary>4.2 Terminal Display: 수신만/송신만 실행. null이면 기존 동작(Master Bit On이면 수신, 아니면 송신)</summary>
        public TerminalDisplayMode? TerminalDisplayMode { get; set; }
        /// <summary>4.11 Cell IN/OUT: CellIn 또는 CellOut만 실행. null이면 CellIn</summary>
        public CellInOutKind? CellInOutKind { get; set; }
        /// <summary>4.26 Operator Log: LogIn 또는 LogOut만 실행. null이면 LogIn</summary>
        public OperatorLogMode? OperatorLogMode { get; set; }
        /// <summary>4.29 Material Alarm: Warning(부족 수량) 또는 Shortage(수량 0). null이면 Warning</summary>
        public MaterialAlarmKind? MaterialAlarmKind { get; set; }
        /// <summary>4.4 Interlock 발생: true면 Step 5(Interlock State/Motion State Word Write)만 실행. false/null이면 Step 1~3(읽기, Reply On)만 실행. 설비 상태 보고 요청 시 true로 호출</summary>
        public bool InterlockReportStateRequested { get; set; }
        /// <summary>4.3 OP Call: true=발생(Occur)만, false=해제(Release)만. null이면 Master Bit 대기 후 분기.</summary>
        public bool? OPCallOccurOnly { get; set; }

        // OP Call Info
        public string OPCallID { get; set; }
        public string OPCallMsg { get; set; }

        // Interlock Info
        public string InterlockID { get; set; }
        public string InterlockMsg { get; set; }
        /// <summary>4.13 PPID 생성/삭제: Create=1, Delete=2. null이면 Create.</summary>
        public PPIDCreateDeleteKind? PPIDCreateDeleteKind { get; set; }
        /// <summary>4.6/4.7 Alarm: Alarm 발생·해제 시 사용할 CIM Alarm 데이터. null이면 기본값(ALCD=0, ALID=0, Description="")으로 버퍼 생성.</summary>
        public CIMAlarmData CIMAlarmData { get; set; }
        /// <summary>4.10 Material Change / 4.37 Material Location Update: 자재 데이터. null이면 기본값으로 버퍼 생성.</summary>
        public CIMMaterialData CIMMaterialData { get; set; }

        /// <summary>
        /// 1~4
        /// </summary>
        public int CellStartPortNo { get; set; }
        public CIMCellTrackInData CIMCellTrackInData1 { get; set; }
        public CIMCellTrackInData CIMCellTrackInData2 { get; set; }
        public CIMCellTrackInData CIMCellTrackInData3 { get; set; }
        public CIMCellTrackInData CIMCellTrackInData4 { get; set; }

        // MCR ID
        public CIM_MCR_Read_Data MCRReadData1 { get; set; }
        public CIM_MCR_Read_Data MCRReadData2 { get; set; }

        // Recipe Info
        public int PPID_No { get; set; }
        public string PPID_Name { get; set; }
        /// <summary>
        /// 0: NEW (생성), 1:OLD (설비 운영 이력이 있는 Recipe)
        /// </summary>
        public int PPID_CreateType { get; set; }
        /// <summary>
        /// 0: DEL (삭제 / PPID+ RMS + Off-Set), 1:General (삭제 / PPID + RMS)
        /// </summary>
        public int PPID_DeleteType { get; set; }
    }

    /// <summary>
    /// 2026-02-28 CursorAI에 의한 코드 생성: 시나리오 완료 시 ScenarioCompleted 이벤트에 전달되는 인자.
    /// </summary>
    public class ScenarioCompletedArgs
    {
        public CIMScenario Scenario { get; }
        public bool Success { get; }
        public string Message { get; }
        public Exception Error { get; }

        public ScenarioCompletedArgs(CIMScenario scenario, bool success, string message = null, Exception error = null)
        {
            Scenario = scenario;
            Success = success;
            Message = message ?? (success ? "Completed" : (error?.Message ?? "Failed"));
            Error = error;
        }
    }

    /// <summary>
    /// 2026-03-03 CursorAI에 의한 코드 생성: CIM 시나리오 실행 중 UI 메시지 표시용 이벤트 인자.
    /// Operator Call, Terminal Display 등에서 Word 버퍼를 문자열로 변환하여 전달한다.
    /// </summary>
    public class CIMMessageDisplayEventArgs
    {
        public CIMScenario Scenario { get; }
        public string Title { get; }
        public string MsgID { get; }
        public string Message { get; }
        public int IsMsgType => GetMsgType(Scenario);

        public CIMMessageDisplayEventArgs(CIMScenario scenario, string title, string message)
        {
            Scenario = scenario;
            Title = title;
            Message = message;
        }
        public CIMMessageDisplayEventArgs(CIMScenario scenario, string title, string msgID, string message)
        {
            Scenario = scenario;
            Title = title;
            MsgID = msgID;
            Message = message;
        }

        public int GetMsgType(CIMScenario scenario)
        {
            int ret = 0;

            switch (scenario)
            {
                case CIMScenario.TerminalDisplay: 
                    ret = 0;
                    break;

                case CIMScenario.OPCallOccurAndRelease:
                    ret = 1; 
                    break;

                case CIMScenario.InterlockOccur:
                case CIMScenario.InterlockRelease:
                    ret = 2; 
                    break;
            }

            return ret;
        }
    }

    public class CIMRecipeControlEventArgs
    {
        public CIMScenario Scenario { get; }
        public string CCODE { get; }
        public string Name { get; }
        public string No { get; }
        public int IsRecipeNo => Convert.ToInt32(No.Substring(0, 2));
        /// <summary>
        /// 0: New Create / Delete, 1:SaveAS / Delete
        /// </summary>
        public int IsType => GetType();

        public CIMRecipeControlEventArgs(CIMScenario scenario, string ccode, string name, string no)
        {
            this.Scenario = scenario;
            this.CCODE = ccode;
            this.Name = name;
            this.No = no;
        }

        private int GetType()
        {
            int ret = 0;

            switch (No.Substring(2, 1))
            {
                case "N": ret = 0; break;
                case "O": ret = 1; break;
                case "D": ret = 0; break;
                case "G": ret = 1; break;
                case "C": ret = 0; break;
            }

            return ret;
        }
    }
}
