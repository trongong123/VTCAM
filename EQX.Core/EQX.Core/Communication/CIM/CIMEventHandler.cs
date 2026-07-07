// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// 2026-02-24 CursorAI에 의한 주석 처리 - 명세서 §2 Event Type Pair Items 변경 반영: GetAddresses(Bit)를 CIM명령어_명세서.md §2와 일치하도록 수정
// 참고: doc/CIM/SDC CIM 구현 컨셉.md (Event Type Pair = PLC Address 호출, CIM Address Reply). Local 보고 시 Word는 §2 Items(Local 측 Item) 주소 사용.
// Event Type: Local이 Word 쓰기 → PLC Address(Request) Bit On → CIM Address(Master Reply) 확인 후 Request Bit Off

using System;

namespace TOPENG_Device
{
    /// <summary>
    /// Event Type 처리. Local Request Bit 쓰기, Word 쓰기, Master Reply Bit 확인.
    /// </summary>
    public static class CIMEventHandler
    {
        /// <summary>EquipEvent에 해당하는 CIM Address(Master Reply), PLC Address(Local Request) 반환. 명세서 §2 기준.</summary>
        public static void GetAddresses(EquipEvent evt, out string cimAddress, out string plcAddress)
        {
            switch (evt)
            {
                case EquipEvent.AliveBit: cimAddress = "B1000"; plcAddress = "B0000"; break;
                case EquipEvent.JobEvent1: cimAddress = "B100D"; plcAddress = "B000D"; break;
                case EquipEvent.TPMLoss: cimAddress = "B100E"; plcAddress = "B000E"; break;
                case EquipEvent.JobEvent2: cimAddress = "B1010"; plcAddress = "B0010"; break;
                case EquipEvent.TPMLossReady: cimAddress = "B1012"; plcAddress = "B0012"; break;
                case EquipEvent.PPIDChange: cimAddress = "B1014"; plcAddress = "B0014"; break;
                // 260304, B1015 사용하지 않고, B1016으로 사용한다고 함.
                //case EquipEvent.ParameterChange: cimAddress = "B1015"; plcAddress = "B0015"; break;
                case EquipEvent.ParameterChange:
                case EquipEvent.ParameterChange2: cimAddress = "B1016"; plcAddress = "B0016"; break;
                case EquipEvent.EquipmentConstantParameterChangeEvent: cimAddress = "B1017"; plcAddress = "B0017"; break;
                case EquipEvent.CellStartPort1: cimAddress = "B101B"; plcAddress = "B001B"; break;
                case EquipEvent.CellStartPort2: cimAddress = "B101C"; plcAddress = "B001C"; break;
                case EquipEvent.CellStartPort3: cimAddress = "B101D"; plcAddress = "B001D"; break;
                case EquipEvent.CellStartPort4: cimAddress = "B101E"; plcAddress = "B001E"; break;
                case EquipEvent.CellCompPort1: cimAddress = "B1021"; plcAddress = "B0021"; break;
                case EquipEvent.CellCompPort2: cimAddress = "B1022"; plcAddress = "B0022"; break;
                case EquipEvent.CellCompPort3: cimAddress = "B1023"; plcAddress = "B0023"; break;
                case EquipEvent.CellCompPort4: cimAddress = "B1024"; plcAddress = "B0024"; break;
                case EquipEvent.CellCompPort5: cimAddress = "B1025"; plcAddress = "B0025"; break;
                case EquipEvent.CellCompPort6: cimAddress = "B1026"; plcAddress = "B0026"; break;
                case EquipEvent.OPCallConfirm: cimAddress = "B1028"; plcAddress = "B0028"; break;
                case EquipEvent.InterlockConfirm: cimAddress = "B1029"; plcAddress = "B0029"; break;
                case EquipEvent.MaterialIDReadingResult1: cimAddress = "B102C"; plcAddress = "B002C"; break;
                case EquipEvent.MaterialIDReadingResult2: cimAddress = "B102D"; plcAddress = "B002D"; break;
                case EquipEvent.MaterialIDReadingResult3: cimAddress = "B102E"; plcAddress = "B002E"; break;
                case EquipEvent.BatchLotInforRequest: cimAddress = "B1038"; plcAddress = "B0038"; break;
                case EquipEvent.CellIDReadingResult1: cimAddress = "B103A"; plcAddress = "B003A"; break;
                case EquipEvent.StartCellLot1: cimAddress = "B103B"; plcAddress = "B003B"; break;
                case EquipEvent.CellIDReadingResult2: cimAddress = "B103C"; plcAddress = "B003C"; break;
                case EquipEvent.StartCellLot2: cimAddress = "B103D"; plcAddress = "B003D"; break;
                case EquipEvent.CarrierLoaderEvent: cimAddress = "B103E"; plcAddress = "B003E"; break;
                case EquipEvent.CarrierUnloadEvent: cimAddress = "B103F"; plcAddress = "B003F"; break;
                case EquipEvent.OperatorLogInfomation: cimAddress = "B1040"; plcAddress = "B0040"; break;
                case EquipEvent.InspectionResultReport: cimAddress = "B1042"; plcAddress = "B0042"; break;
                case EquipEvent.PairCellProcessStart: cimAddress = "B1044"; plcAddress = "B0044"; break;
                case EquipEvent.PairCellProcessStart_2: cimAddress = "B1045"; plcAddress = "B0045"; break;
                case EquipEvent.PairCellProcessComplete: cimAddress = "B1046"; plcAddress = "B0046"; break;
                case EquipEvent.PairCellProcessComplete_2: cimAddress = "B1047"; plcAddress = "B0047"; break;
                case EquipEvent.PairCellProcessComplete_3: cimAddress = "B1048"; plcAddress = "B0048"; break;
                case EquipEvent.MaterialAssembleProcess1_1: cimAddress = "B104E"; plcAddress = "B004E"; break;
                case EquipEvent.MaterialAssembleProcess1_2: cimAddress = "B104F"; plcAddress = "B004F"; break;
                case EquipEvent.MaterialAssembleProcess1_3: cimAddress = "B1050"; plcAddress = "B0050"; break;
                case EquipEvent.MaterialAssembleProcess2_1: cimAddress = "B1051"; plcAddress = "B0051"; break;
                case EquipEvent.MaterialAssembleProcess2_2: cimAddress = "B1052"; plcAddress = "B0052"; break;
                case EquipEvent.MaterialAssembleProcess2_3: cimAddress = "B1053"; plcAddress = "B0053"; break;
                case EquipEvent.MaterialAssembleProcess3_1: cimAddress = "B1054"; plcAddress = "B0054"; break;
                case EquipEvent.MaterialAssembleProcess3_2: cimAddress = "B1055"; plcAddress = "B0055"; break;
                case EquipEvent.MaterialAssembleProcess4_1: cimAddress = "B1057"; plcAddress = "B0057"; break;
                case EquipEvent.MaterialAssembleProcess4_2: cimAddress = "B1058"; plcAddress = "B0058"; break;
                case EquipEvent.MaterialAssembleProcess5_1: cimAddress = "B105A"; plcAddress = "B005A"; break;
                case EquipEvent.MaterialAssembleProcess5_2: cimAddress = "B105B"; plcAddress = "B005B"; break;
                case EquipEvent.MaterialNGProcess1_1: cimAddress = "B105E"; plcAddress = "B005E"; break;
                case EquipEvent.MaterialNGProcess2_1: cimAddress = "B105F"; plcAddress = "B005F"; break;
                case EquipEvent.MaterialNGProcess3_1: cimAddress = "B1060"; plcAddress = "B0060"; break;
                case EquipEvent.MaterialNGProcess4_1: cimAddress = "B1061"; plcAddress = "B0061"; break;
                case EquipEvent.MaterialNGProcess5_1: cimAddress = "B1062"; plcAddress = "B0062"; break;
                case EquipEvent.MaterialNGProcess6_1: cimAddress = "B1063"; plcAddress = "B0063"; break;
                case EquipEvent.PerformanceLossEvent1: cimAddress = "B1066"; plcAddress = "B0066"; break;
                case EquipEvent.PerformanceLossEvent2: cimAddress = "B1067"; plcAddress = "B0067"; break;
                case EquipEvent.PerformanceLossEvent3: cimAddress = "B1068"; plcAddress = "B0068"; break;
                case EquipEvent.MaterialSupplementRequest1: cimAddress = "B1076"; plcAddress = "B0076"; break;
                case EquipEvent.MaterialSupplementRequest2: cimAddress = "B1077"; plcAddress = "B0077"; break;
                case EquipEvent.MaterialSupplementRequest3: cimAddress = "B1078"; plcAddress = "B0078"; break;
                case EquipEvent.MaterialSupplementComplete1: cimAddress = "B1079"; plcAddress = "B0079"; break;
                case EquipEvent.MaterialSupplementComplete2: cimAddress = "B107A"; plcAddress = "B007A"; break;
                case EquipEvent.MaterialSupplementComplete3: cimAddress = "B107B"; plcAddress = "B007B"; break;
                case EquipEvent.KittingorCancel1: cimAddress = "B107E"; plcAddress = "B007E"; break;
                case EquipEvent.KittingorCancel2: cimAddress = "B107F"; plcAddress = "B007F"; break;
                case EquipEvent.KittingorCancel3: cimAddress = "B1080"; plcAddress = "B0080"; break;
                case EquipEvent.MaterialWarning1: cimAddress = "B1082"; plcAddress = "B0082"; break;
                case EquipEvent.MaterialShortage1: cimAddress = "B1083"; plcAddress = "B0083"; break;
                case EquipEvent.MaterialWarning2: cimAddress = "B1084"; plcAddress = "B0084"; break;
                case EquipEvent.MaterialShortage2: cimAddress = "B1085"; plcAddress = "B0085"; break;
                case EquipEvent.MaterialWarning3: cimAddress = "B1086"; plcAddress = "B0086"; break;
                case EquipEvent.MaterialShortage3: cimAddress = "B1087"; plcAddress = "B0087"; break;
                case EquipEvent.MaterialLocationUpdate1: cimAddress = "B108A"; plcAddress = "B008A"; break;
                case EquipEvent.MaterialLocationUpdate2: cimAddress = "B108B"; plcAddress = "B008B"; break;
                case EquipEvent.MaterialLocationUpdate3: cimAddress = "B108C"; plcAddress = "B008C"; break;
                case EquipEvent.LabelInformationRequest: cimAddress = "B108E"; plcAddress = "B008E"; break;
                case EquipEvent.LabelValidationRequest: cimAddress = "B1090"; plcAddress = "B0090"; break;
                case EquipEvent.LabelPrecheckRequest: cimAddress = "B1092"; plcAddress = "B0092"; break;
                case EquipEvent.ProcessControlResult1: cimAddress = "B1096"; plcAddress = "B0096"; break;
                case EquipEvent.ProcessControlResult2: cimAddress = "B1097"; plcAddress = "B0097"; break;
                case EquipEvent.ProcessControlResult3: cimAddress = "B1098"; plcAddress = "B0098"; break;
                case EquipEvent.ProcessControlResult4: cimAddress = "B1099"; plcAddress = "B0099"; break;
                case EquipEvent.CassetteStateChangeEvent1_A: cimAddress = "B10A3"; plcAddress = "B00A3"; break;
                case EquipEvent.CassetteStateChangeEvent1_B: cimAddress = "B10A4"; plcAddress = "B00A4"; break;
                case EquipEvent.CassetteStateChangeEvent1_C: cimAddress = "B10A5"; plcAddress = "B00A5"; break;
                case EquipEvent.CassetteStateChangeEvent1_D: cimAddress = "B10A6"; plcAddress = "B00A6"; break;
                case EquipEvent.CassetteStateChangeEvent2_A: cimAddress = "B10A7"; plcAddress = "B00A7"; break;
                case EquipEvent.CassetteStateChangeEvent2_B: cimAddress = "B10A8"; plcAddress = "B00A8"; break;
                case EquipEvent.CassetteStateChangeEvent2_C: cimAddress = "B10A9"; plcAddress = "B00A9"; break;
                case EquipEvent.CassetteStateChangeEvent2_D: cimAddress = "B10AA"; plcAddress = "B00AA"; break;
                case EquipEvent.CassetteStateChangeEvent3_A: cimAddress = "B10AB"; plcAddress = "B00AB"; break;
                case EquipEvent.CassetteStateChangeEvent3_B: cimAddress = "B10AC"; plcAddress = "B00AC"; break;
                case EquipEvent.CassetteStateChangeEvent3_C: cimAddress = "B10AD"; plcAddress = "B00AD"; break;
                case EquipEvent.CassetteStateChangeEvent3_D: cimAddress = "B10AE"; plcAddress = "B00AE"; break;
                case EquipEvent.CassetteStateChangeEvent4_A: cimAddress = "B10AF"; plcAddress = "B00AF"; break;
                case EquipEvent.CassetteStateChangeEvent4_B: cimAddress = "B10B0"; plcAddress = "B00B0"; break;
                case EquipEvent.CassetteStateChangeEvent4_C: cimAddress = "B10B1"; plcAddress = "B00B1"; break;
                case EquipEvent.CassetteStateChangeEvent4_D: cimAddress = "B10B2"; plcAddress = "B00B2"; break;
                case EquipEvent.CassetteStateChangeEvent5_A: cimAddress = "B10B3"; plcAddress = "B00B3"; break;
                case EquipEvent.CassetteStateChangeEvent5_B: cimAddress = "B10B4"; plcAddress = "B00B4"; break;
                case EquipEvent.CassetteStateChangeEvent5_C: cimAddress = "B10B5"; plcAddress = "B00B5"; break;
                case EquipEvent.CassetteStateChangeEvent5_D: cimAddress = "B10B6"; plcAddress = "B00B6"; break;
                case EquipEvent.CellInformationRequest1: cimAddress = "B1123"; plcAddress = "B0123"; break;
                case EquipEvent.CellInformationRequest2: cimAddress = "B1124"; plcAddress = "B0124"; break;

                default: cimAddress = "B1000"; plcAddress = "B0000"; break;
            }
        }

        /// <summary>Local Request(Event) Bit On. PLCWriteValue_B 사용.</summary>
        public static void SetLocalRequestBitOn(EquipEvent evt)
        {
            GetAddresses(evt, out _, out string plcAddr);
            CIMAddressMap.WritePLCBit(plcAddr, 1);
        }

        /// <summary>Local Request Bit Off.</summary>
        public static void SetLocalRequestBitOff(EquipEvent evt)
        {
            GetAddresses(evt, out _, out string plcAddr);
            CIMAddressMap.WritePLCBit(plcAddr, 0);
        }

        /// <summary>Master Reply Bit가 On인지 확인. PLCReadValue_B(CIM Address) 사용.</summary>
        public static bool IsMasterReplyBitOn(EquipEvent evt)
        {
            GetAddresses(evt, out string cimAddr, out _);
            short v = CIMAddressMap.ReadCIMBit(cimAddr);
            return v != 0;
        }

        /// <summary>CIM Bit가 On 될 때까지 대기. timeoutMs 내에 On 되면 true.</summary>
        public static bool WaitForCIMBitOn(EquipEvent evt, int timeoutMs = 3000)
        {
            var deadline = DateTime.Now.AddMilliseconds(timeoutMs);
            while (DateTime.Now < deadline)
            {
                if (IsMasterReplyBitOn(evt))
                    return true;
                System.Threading.Thread.Sleep(10);
            }
            return false;
        }
    }
}
