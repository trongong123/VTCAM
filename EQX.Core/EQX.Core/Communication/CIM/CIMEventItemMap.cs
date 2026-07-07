// 2026-02-24 CursorAI에 의한 코드 생성
// 2026-02-25 CursorAI에 의한 PLC2CIM.csv 반영 수정: KittingorCancel1 lengthWords 3894→121
// 2026-02-25 CursorAI에 의한 보완 설계 반영: 불일치 10시나리오 Event Word 주소 추가(PLC2CIM B8A/B4E/B5E/B76/B79/B82/B83/BA3~BA6)
// 2. Event Type Pair의 Items(Local 측 Word 주소) 매핑. Local(PLC)에서 보고하는 Items는 명세서 §2 Local 측 Item에 있는 Word 주소 사용.

using System;

namespace TOPENG_Device
{
    /// <summary>
    /// Event Type Pair의 Items(Local 측 Word) 주소·길이. 명세서 §2 기준. Local 보고 시 PLCWriteValue_W에 이 주소로 쓴다.
    /// </summary>
    public static class CIMEventItemMap
    {
        /// <summary>
        /// 해당 Event에 대한 Local 측 Item Word 범위(시작 D 주소, 길이 Word 수). Items 없으면 startDAddress=null, lengthWords=0.
        /// </summary>
        public static void GetEventItemWordRange(EquipEvent evt, out string startDAddress, out int lengthWords)
        {
            startDAddress = null;
            lengthWords = 0;
            switch (evt)
            {
                case EquipEvent.JobEvent1:
                    startDAddress = "D1140"; lengthWords = 1 + 20 + 20 + 2 + 2 + 2; break; // ProcessJobEventCEID1~ProcessJobEventDQTY1
                case EquipEvent.TPMLoss:
                    startDAddress = "D120F"; lengthWords = 2 + 20; break; // TMPLossCode, TMPLossDescp
                case EquipEvent.JobEvent2:
                    startDAddress = "D116F"; lengthWords = 1 + 20 + 2 + 2 + 2; break;
                case EquipEvent.ParameterChange2:
                    startDAddress = "D23D9"; lengthWords = 3; break;
                case EquipEvent.CellStartPort1:
                    startDAddress = "D4A0"; lengthWords = 40 + 20 + 20 + 20 + 2 + 2 + 1 + 1 + 1; break;
                case EquipEvent.CellStartPort2:
                    startDAddress = "D510"; lengthWords = 40 + 20 + 20 + 20 + 2 + 2 + 1 + 1 + 1; break;
                case EquipEvent.CellStartPort3:
                    startDAddress = "D580"; lengthWords = 40 + 20 + 20 + 20 + 2 + 2 + 1 + 1 + 1; break;
                case EquipEvent.CellStartPort4:
                    startDAddress = "D5F0"; lengthWords = 40 + 20 + 20 + 20 + 2 + 2 + 1 + 1 + 1; break;
                case EquipEvent.CellCompPort1:
                    startDAddress = "D760"; lengthWords = 40 + 20 + 20 + 20 + 2 + 2 + 1 + 1 + 10 + 10 + 10 + 1 + 10 + 20; break;
                case EquipEvent.OPCallConfirm:
                    startDAddress = "DF30"; lengthWords = 40 + 20 + 20 + 10 + 60; break;
                case EquipEvent.InterlockConfirm:
                    startDAddress = "DFF0"; lengthWords = 40 + 20 + 20 + 10 + 60; break;
                case EquipEvent.BatchLotInforRequest:
                    startDAddress = "D40"; lengthWords = 20; break;
                case EquipEvent.CellIDReadingResult1:
                    startDAddress = "D70"; lengthWords = 40 + 1 + 1; break;
                case EquipEvent.StartCellLot1:
                    startDAddress = "DE30"; lengthWords = 1 + 1 + 40 + 20; break;
                case EquipEvent.CellIDReadingResult2:
                    startDAddress = "D9A"; lengthWords = 40 + 1 + 1; break;
                case EquipEvent.StartCellLot2:
                    startDAddress = "DE6E"; lengthWords = 1 + 1 + 40 + 20; break;
                case EquipEvent.CarrierLoaderEvent:
                    startDAddress = "D10F0"; lengthWords = 20 + 20 + 2 + 20 + 1; break;
                case EquipEvent.CarrierUnloadEvent:
                    startDAddress = "D11D0"; lengthWords = 20 + 20 + 2 + 20 + 1; break;
                case EquipEvent.OperatorLogInfomation:
                    startDAddress = "D1226"; lengthWords = 5 + 20 + 10 + 20; break;
                case EquipEvent.InspectionResultReport:
                    startDAddress = "D23E0"; lengthWords = 10 + 200 + 2 + 1 + 10 + 10 + 100 + 100; break;
                case EquipEvent.LabelInformationRequest:
                    startDAddress = "D1280"; lengthWords = 5 + 40 + 20 + 40; break;
                case EquipEvent.LabelValidationRequest:
                    startDAddress = "D12E9"; lengthWords = 5 + 40; break;
                case EquipEvent.LabelPrecheckRequest:
                    startDAddress = "D1340"; lengthWords = 5 + 40 + 20 + 40 + 200; break;
                case EquipEvent.ProcessControlResult1:
                    startDAddress = "D1A24"; lengthWords = 1 + 40 + 2 + 5; break;
                case EquipEvent.ProcessControlResult2:
                    startDAddress = "D1A54"; lengthWords = 1 + 40 + 2 + 5; break;
                case EquipEvent.ProcessControlResult3:
                    startDAddress = "D1A84"; lengthWords = 1 + 40 + 2 + 5; break;
                case EquipEvent.ProcessControlResult4:
                    startDAddress = "D1AB4"; lengthWords = 1 + 40 + 2 + 5; break;
                case EquipEvent.KittingorCancel1:
                    startDAddress = "D7B14"; lengthWords = 121; break; // MT_Kitting1_CEID~Reserve(7B86 7), PLC2CIM.csv B8A(MaterialLocationUpdate1) 항목 기준
                // 2026-02-25 CursorAI에 의한 보완 설계: 불일치 10시나리오 Event Word(PLC2CIM.csv 기준)
                case EquipEvent.MaterialLocationUpdate1:
                    startDAddress = "D7B14"; lengthWords = 121; break; // PLC2CIM B8A
                case EquipEvent.MaterialAssembleProcess1_1:
                    startDAddress = "D7F4F"; lengthWords = 226; break; // PLC2CIM B4E
                case EquipEvent.MaterialNGProcess1_1:
                    startDAddress = "D7C7F"; lengthWords = 120; break; // PLC2CIM B5E
                case EquipEvent.MaterialSupplementRequest1:
                case EquipEvent.MaterialSupplementComplete1:
                case EquipEvent.MaterialWarning1:
                case EquipEvent.MaterialShortage1:
                    startDAddress = "D7B14"; lengthWords = 121; break; // PLC2CIM B76/B79/B82/B83
                case EquipEvent.CassetteStateChangeEvent1_A:
                    startDAddress = "D1490"; lengthWords = 37; break;  // PLC2CIM BA3
                case EquipEvent.CassetteStateChangeEvent1_B:
                    startDAddress = "D14B5"; lengthWords = 37; break;  // PLC2CIM BA4
                case EquipEvent.CassetteStateChangeEvent1_C:
                    startDAddress = "D14DA"; lengthWords = 37; break;  // PLC2CIM BA5
                case EquipEvent.CassetteStateChangeEvent1_D:
                    startDAddress = "D14FF"; lengthWords = 37; break;  // PLC2CIM BA6
                case EquipEvent.CellInformationRequest1:
                    startDAddress = "D5F94"; lengthWords = 20; break;

                default:
                    break;
            }
        }

        /// <summary>해당 Event가 §2 Items를 가지면 true.</summary>
        public static bool HasEventItems(EquipEvent evt)
        {
            GetEventItemWordRange(evt, out string start, out int len);
            return !string.IsNullOrEmpty(start) && len > 0;
        }
    }
}
