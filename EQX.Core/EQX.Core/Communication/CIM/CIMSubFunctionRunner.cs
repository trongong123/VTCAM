// 2026-03-04 CursorAI에 의한 코드 생성: 47개 시나리오 Sub 함수(XXX.YYY) 실행. 외부 호출·UI 테스트·Unit Test용.

using System;

namespace TOPENG_Device
{
    /// <summary>Sub 함수(XXX.YYY) 단위 실행. Execute(sub, context)로 해당 Sub만 실행.</summary>
    public static class CIMSubFunctionRunner
    {
        /// <summary>지정한 Sub 함수 1개 실행. context는 시나리오별 옵션(수신/송신, CellIn/CellOut 등).</summary>
        /// <returns>성공 여부. 일부 Sub는 void 동작이면 true 반환.</returns>
        public static bool Execute(CIMSubFunction sub, CIMScenarioContext context = null)
        {
            switch (sub)
            {
                case CIMSubFunction.Initialize_AliveCheck:
                    return CIMInitialize.AliveCheck();
                case CIMSubFunction.Initialize_DateTimeSync:
                    // 260304, TEST 수정
                    return CIMInitialize.DateTimeSync(false);
                case CIMSubFunction.TerminalDisplay_Receive:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.TerminalDisplay, new CIMScenarioContext { TerminalDisplayMode = TerminalDisplayMode.Receive });
                    return true;
                case CIMSubFunction.TerminalDisplay_Send:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.TerminalDisplay, new CIMScenarioContext { TerminalDisplayMode = TerminalDisplayMode.Send });
                    return true;
                case CIMSubFunction.OPCallOccurAndRelease_Occur:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.OPCallOccurAndRelease, new CIMScenarioContext { OPCallOccurOnly = true });
                    return true;
                case CIMSubFunction.OPCallOccurAndRelease_Release:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.OPCallOccurAndRelease, new CIMScenarioContext { OPCallOccurOnly = false });
                    return true;
                case CIMSubFunction.InterlockOccur_ReadAndReply:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.InterlockOccur, new CIMScenarioContext { InterlockReportStateRequested = false });
                    return true;
                case CIMSubFunction.InterlockOccur_ReportEquipState:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.InterlockOccur, new CIMScenarioContext { InterlockReportStateRequested = true });
                    return true;
                case CIMSubFunction.InterlockRelease_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.InterlockRelease, context);
                    return true;
                case CIMSubFunction.AlarmOccur_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.AlarmOccur, context);
                    return true;
                case CIMSubFunction.AlarmRelease_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.AlarmRelease, context);
                    return true;
                case CIMSubFunction.AlarmInquiry_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.AlarmInquiry, context);
                    return true;
                case CIMSubFunction.EquipStateChange_Report:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.EquipStateChange, context);
                    return true;
                case CIMSubFunction.MaterialChange_ReportPort:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialChange, context);
                    return true;
                case CIMSubFunction.CellInOut_CellIn:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellInOut, new CIMScenarioContext { CellInOutKind = CellInOutKind.CellIn });
                    return true;
                case CIMSubFunction.CellInOut_CellOut:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellInOut, new CIMScenarioContext { CellInOutKind = CellInOutKind.CellOut });
                    return true;
                case CIMSubFunction.PPIDChange_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDChange, context);
                    return true;
                case CIMSubFunction.PPIDCreateDelete_Create:
                    if (context == null)
                        CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDCreateDelete, new CIMScenarioContext { PPIDCreateDeleteKind = PPIDCreateDeleteKind.Create });
                    else CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDCreateDelete, context);

                    return true;
                case CIMSubFunction.PPIDCreateDelete_Delete:
                    if(context == null)
                        CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDCreateDelete, new CIMScenarioContext { PPIDCreateDeleteKind = PPIDCreateDeleteKind.Delete });
                    else CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDCreateDelete, context);

                    return true;
                case CIMSubFunction.PPIDDownload_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDDownload, context);
                    return true;
                case CIMSubFunction.ParameterChange_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.ParameterChange, context);
                    return true;
                case CIMSubFunction.ParameterInquiry_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.ParameterInquiry, context);
                    return true;
                case CIMSubFunction.PPIDListInquiry_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PPIDListInquiry, context);
                    return true;
                case CIMSubFunction.EquipConstantInquiry_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.EquipConstantInquiry, context);
                    return true;
                case CIMSubFunction.EquipConstantNameListInquiry_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.EquipConstantNameListInquiry, context);
                    return true;
                case CIMSubFunction.EquipPosition_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.EquipPosition, context);
                    return true;
                case CIMSubFunction.TPMLoss_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.TPMLoss, context);
                    return true;
                case CIMSubFunction.IQCPOLCellInOut_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.IQCPOLCellInOut, context);
                    return true;
                case CIMSubFunction.MaterialKittingCancel_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialKittingCancel, context);
                    return true;
                case CIMSubFunction.MaterialAssembleProcess_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialAssembleProcess, context);
                    return true;
                case CIMSubFunction.MaterialNGProcess_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialNGProcess, context);
                    return true;
                case CIMSubFunction.OperatorLogInOut_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.OperatorLogInOut, context);
                    return true;
                case CIMSubFunction.MaterialSupplementRequest_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialSupplementRequest, context);
                    return true;
                case CIMSubFunction.MaterialSupplementComplete_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialSupplementComplete, context);
                    return true;
                case CIMSubFunction.MaterialAlarm_Warning:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialAlarm, new CIMScenarioContext { MaterialAlarmKind = MaterialAlarmKind.Warning });
                    return true;
                case CIMSubFunction.MaterialAlarm_Shortage:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialAlarm, new CIMScenarioContext { MaterialAlarmKind = MaterialAlarmKind.Shortage });
                    return true;
                case CIMSubFunction.APNIDRequestProcess_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.APNIDRequestProcess, context);
                    return true;
                case CIMSubFunction.APNIDIssueConfirmProcess_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.APNIDIssueConfirmProcess, context);
                    return true;
                case CIMSubFunction.APNIDGrapeIDRequestProcess_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.APNIDGrapeIDRequestProcess, context);
                    return true;
                case CIMSubFunction.LoadRequest_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.LoadRequest, context);
                    return true;
                case CIMSubFunction.LoadComplete_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.LoadComplete, context);
                    return true;
                case CIMSubFunction.UnloadRequest_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.UnloadRequest, context);
                    return true;
                case CIMSubFunction.UnloadComplete_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.UnloadComplete, context);
                    return true;
                case CIMSubFunction.MaterialLocationUpdate_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.MaterialLocationUpdate, context);
                    return true;
                case CIMSubFunction.CellTrackINValidationCheck_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellTrackINValidationCheck, context);
                    return true;
                case CIMSubFunction.CurrentProcessControlDataRequest_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CurrentProcessControlDataRequest, context);
                    return true;
                case CIMSubFunction.ProcessControlRequestReport_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.ProcessControlRequestReport, context);
                    return true;
                case CIMSubFunction.ProcessControlResultReport_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.ProcessControlResultReport, context);
                    return true;
                case CIMSubFunction.UnitStatusChange_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.UnitStatusChange, context);
                    return true;
                case CIMSubFunction.CellLoaderPortLoadUnload_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellLoaderPortLoadUnload, context);
                    return true;
                case CIMSubFunction.CellUnloaderPortLoadUnload_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellUnloaderPortLoadUnload, context);
                    return true;
                case CIMSubFunction.JobStartCancel_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.JobStartCancel, context);
                    return true;
                case CIMSubFunction.ProcessJobEvent_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.ProcessJobEvent, context);
                    return true;
                case CIMSubFunction.PortStateChange_Run:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.PortStateChange, context);
                    return true;
                case CIMSubFunction.EquipFunctionChangeCommand:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.EquipFunctionChangeCommand, context);
                    return true;
                case CIMSubFunction.CellStartPort:
                    if (context == null) return false;
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellStartPort, context);
                    return true;
                case CIMSubFunction.CellIDReadingResult:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellIDReduingResult, context);
                    return true;
                case CIMSubFunction.CellInformationRequest:
                    CIMScenarioDispatcher.ExecuteScenario(CIMScenario.CellInformationRequest, context);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>Sub 함수 표시명(Initialize.AliveCheck 형식). UI·로그용.</summary>
        public static string GetDisplayName(CIMSubFunction sub)
        {
            return sub.ToString().Replace("_", ".");
        }
    }
}
