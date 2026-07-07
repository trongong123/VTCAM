using TOPENG_Device;

namespace EQX.Core.Communication.CIM
{
    public enum EMaterialKittingCEID
    {
        KITTING_CANCEL = 219,
        KITTING = 221,
        WARNING = 223,
        SHORTAGE = 224,
        LOCATION_UPDAT = 225
    }

    public class SDVCIMMapHeler : ICIMMapHelper
    {
        public void GetBitAddress(CIMCommand cmd, out string cimAddress, out string plcAddress)
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
                case CIMCommand.FormattedProcessProgramRequest: cimAddress = "B1036"; plcAddress = "B0036"; break;
                case CIMCommand.FormattedProcessProgramSend2: cimAddress = "B1033"; plcAddress = "B0033"; break;
                case CIMCommand.EquipConstantNameList: cimAddress = "B1034"; plcAddress = "B0034"; break;
                case CIMCommand.FormattedProcessProgramSend: cimAddress = "B1035"; plcAddress = "B0035"; break;
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
                // Bổ sung vào bên trong cấu trúc switch-case của hàm GetBitAddress
                case CIMCommand.SpecificValidationDataSend1:
                    cimAddress = "B10DE"; plcAddress = "B00DE"; break;

                case CIMCommand.SpecificValidationDataSend2:
                    cimAddress = "B10DF"; plcAddress = "B00DF"; break;

                case CIMCommand.SpecificValidationDataSend3:
                    cimAddress = "B10E0"; plcAddress = "B00E0"; break;

                case CIMCommand.SpecificValidationDataSend4:
                    cimAddress = "B10E1"; plcAddress = "B00E1"; break;

                case CIMCommand.SpecificValidationDataSend5:
                    cimAddress = "B10E2"; plcAddress = "B00E2"; break;

                case CIMCommand.EquipFunctionChangeCommand:
                    cimAddress = "B10E4"; // CIM Request Bit
                    plcAddress = "B00E4"; // PLC Reply Bit (ACK/NACK)
                    break;

                case CIMCommand.EquipApproveProcess:
                    cimAddress = "B104D"; // CIM Request Bit
                    plcAddress = "B004D"; // PLC Reply Bit (Map gốc ghi B 4D)
                    break;

                default: cimAddress = string.Empty; plcAddress = string.Empty; break;
            }
        }

        public void GetWordAddress(CIMCommand cmd, out string cimAddress, out string plcAddress, out int cimLength, out int plcLength)
        {
            // Khởi tạo mặc định
            cimAddress = string.Empty;
            plcAddress = string.Empty;
            cimLength = 0;
            plcLength = 0;

            switch (cmd)
            {
                case CIMCommand.DatetimeSet:
                    cimAddress = "D000"; cimLength = 7;
                    break;

                case CIMCommand.TerminalDisplay:
                    cimAddress = "D011"; cimLength = 61;
                    plcAddress = "1086"; plcLength = 60; // TerminalDisplaySnd
                    break;

                case CIMCommand.OperatorCall:
                    cimAddress = "D058"; cimLength = 70;
                    plcAddress = "F30"; plcLength = 150; // OPCall Confirm WORD (F30 ~ FC5)
                    break;

                case CIMCommand.Interlock:
                    cimAddress = "D09E"; cimLength = 71;
                    plcAddress = "FF0"; plcLength = 150; // Interlock Confirm WORD (FF0 ~ 1085)
                    break;

                case CIMCommand.JOBStart:
                    cimAddress = "D0EB"; cimLength = 124;
                    break;

                case CIMCommand.FormattedProcessProgramRequest:
                    cimAddress = "D1F0"; cimLength = 23; // Request PPID WORD
                    plcAddress = "9224"; plcLength = 4000; // RMS PARAMETER WORD (PLC trả Recipe Body lên CIM)
                    break;

                case CIMCommand.CurrentEquipPPIDListRequest:
                    cimAddress = string.Empty; cimLength = 0;
                    plcAddress = "8A54"; plcLength = 2000; // PPID LIST WORD (W8A54 ~ W9223)
                    break;

                case CIMCommand.EquipConstantNameList:
                    cimAddress = string.Empty; cimLength = 0;
                    plcAddress = "A1C4"; plcLength = 3600; // ECM PARAMETER WORD (WA1C4 ~ WAFD3)
                    break;

                case CIMCommand.BatchLotInforDownload:
                    cimAddress = "D285"; cimLength = 170;
                    break;

                case CIMCommand.CellJobProcess1:
                    cimAddress = "D339"; cimLength = 112;
                    break;

                case CIMCommand.CellJobProcess2:
                    cimAddress = "D3A9"; cimLength = 112;
                    break;

                case CIMCommand.CellJobProcess3:
                    cimAddress = "D419"; cimLength = 112;
                    break;

                case CIMCommand.CellJobProcess4:
                    cimAddress = "D489"; cimLength = 112;
                    break;

                case CIMCommand.EquipApproveProecss:
                    cimAddress = "D7D3"; cimLength = 94;
                    plcAddress = "1225"; plcLength = 1;
                    break;

                case CIMCommand.MaterialInfoSend1:
                    cimAddress = "D503"; cimLength = 230;
                    break;

                case CIMCommand.MaterialInfoSend2:
                    cimAddress = "D5F3"; cimLength = 230;
                    break;

                case CIMCommand.MaterialInfoSend3:
                    cimAddress = "D6E3"; cimLength = 230;
                    break;

                case CIMCommand.LabelInformationSend:
                    cimAddress = "DAE0"; cimLength = 332;
                    break;

                case CIMCommand.LabelValidationSend:
                    cimAddress = "DC31"; cimLength = 332;
                    break;

                case CIMCommand.LabelPrecheckSend:
                    cimAddress = "DD82"; cimLength = 92;
                    break;

                case CIMCommand.ProcessControlInformSend1:
                    cimAddress = "DE5A"; cimLength = 70;
                    plcAddress = "1950"; plcLength = 2;
                    break;

                case CIMCommand.ProcessControlInformSend2:
                    cimAddress = "DEA0"; cimLength = 70;
                    plcAddress = "1952"; plcLength = 2;
                    break;

                case CIMCommand.ProcessControlInformSend3:
                    cimAddress = "DEE6"; cimLength = 70;
                    plcAddress = "1954"; plcLength = 2;
                    break;

                case CIMCommand.ProcessControlInformSend4:
                    cimAddress = "DF2C"; cimLength = 70;
                    plcAddress = "1956"; plcLength = 2;
                    break;

                case CIMCommand.FormattedProcessProgramSend:
                    cimAddress = "DF7D"; cimLength = 4006;
                    plcAddress = "125D"; plcLength = 1;
                    break;

                case CIMCommand.FormattedProcessProgramSend2:
                    cimAddress = "DF7D"; cimLength = 4006;
                    plcAddress = "125D"; plcLength = 1;
                    break;

                case CIMCommand.CurrentProcessControlDataReq:
                    plcAddress = "1910"; plcLength = 64;
                    break;

                case CIMCommand.SpecificValidationDataSend1:
                    cimAddress = "11256"; cimLength = 172; break; // W 11256 ~ W 11301

                case CIMCommand.SpecificValidationDataSend2:
                    cimAddress = "11302"; cimLength = 172; break; // W 11302 ~ W 113AD

                case CIMCommand.SpecificValidationDataSend3:
                    cimAddress = "113AE"; cimLength = 172; break; // W 113AE ~ W 11459

                case CIMCommand.SpecificValidationDataSend4:
                    cimAddress = "1145A"; cimLength = 172; break; // W 1145A ~ W 11505

                case CIMCommand.SpecificValidationDataSend5:
                    cimAddress = "11506"; cimLength = 172; break; // W 11506 ~ W 115B1

                case CIMCommand.EquipFunctionChangeCommand:
                    cimAddress = "D211";  // Vùng nhớ CIM ghi thông số cấu hình muốn đổi (Start: W D211)
                    cimLength = 66;       // Chiều dài 66 Words (EFID + EFST + Message)

                    plcAddress = "2970";  // Vùng nhớ PLC ghi kết quả chấp nhận hay từ chối HCACK (Start: W 2970)
                    plcLength = 1;        // Chiều dài 1 Word
                    break;
                case CIMCommand.APCWordTotal:
                    plcAddress = "CA64"; plcLength = 112;
                    cimAddress = "DDE0"; cimLength = 112;
                    break;

                case CIMCommand.EquipApproveProcess:
                    cimAddress = "D7D3";  // Vùng nhớ C# đọc thông tin Approval từ Host
                    cimLength = 94;       // Chiều dài 94 Words

                    // Ở sự kiện này, theo Map hiện tại không định nghĩa vùng Word PLC trả về cho lệnh này (Không có HCACK riêng). 
                    // Do đó PLC chỉ cần xác nhận bằng cách bật Bit B004D.
                    plcAddress = "";
                    plcLength = 0;
                    break;

                default:
                    break;
            }
        }

        public void GetAddress(CIMCommand cmd, out string cimBitAddress, out string plcBitAddress, out string cimWordAddress, out string plcWordAddress, out int cimWordLength, out int plcWordLength)
        {
            // 1. Gọi hàm trích xuất địa chỉ BIT (Cờ tín hiệu)
            GetBitAddress(cmd, out cimBitAddress, out plcBitAddress);

            // 2. Gọi hàm trích xuất địa chỉ WORD và Chiều dài dữ liệu (Data block)
            GetWordAddress(cmd, out cimWordAddress, out plcWordAddress, out cimWordLength, out plcWordLength);
        }

        public void GetAddress(EquipEvent evt, out string cimBitAddress, out string plcBitAddress, out string cimWordAddress, out string plcWordAddress, out int cimWordLength, out int plcWordLength)
        {
            // C# gửi lên CIM nên thông tin nhận (CIM Word) luôn trống
            cimWordAddress = string.Empty;
            cimWordLength = 0;

            // 1. Lấy địa chỉ BIT
            GetBitAddress(evt, out cimBitAddress, out plcBitAddress);

            // 2. Lấy địa chỉ WORD và Length
            GetWordAddress(evt, out _, out plcWordAddress, out _, out plcWordLength);
        }

        public void GetBitAddress(EquipEvent evt, out string cimAddress, out string plcAddress)
        {
            // Khởi tạo mặc định
            cimAddress = string.Empty;
            plcAddress = string.Empty;

            switch (evt)
            {
                case EquipEvent.AliveBit: cimAddress = "B1000"; plcAddress = "B0000"; break;
                case EquipEvent.JobEvent1: cimAddress = "B100D"; plcAddress = "B000D"; break;
                case EquipEvent.TPMLoss: cimAddress = "B100E"; plcAddress = "B000E"; break;

                case EquipEvent.EQPPIDUpdate: cimAddress = "B1014"; plcAddress = "B0014"; break;
                case EquipEvent.PPIDChange: cimAddress = "B1014"; plcAddress = "B0014"; break;
                case EquipEvent.ParameterChange:
                case EquipEvent.PPIDModeAndPPIDUpdate: cimAddress = "B1014"; plcAddress = "B0014"; break;
                case EquipEvent.CurrentEquipPPIDListResponse: cimAddress = "B1037"; plcAddress = "B0037"; break;

                case EquipEvent.ParameterChange2: cimAddress = "B1016"; plcAddress = "B0016"; break;

                case EquipEvent.CellStartPort1: cimAddress = "B101B"; plcAddress = "B1B"; break;
                case EquipEvent.CellStartPort2: cimAddress = "B101C"; plcAddress = "B1C"; break;
                case EquipEvent.CellStartPort3: cimAddress = "B101D"; plcAddress = "B1D"; break;
                case EquipEvent.CellStartPort4: cimAddress = "B101E"; plcAddress = "B1E"; break;
                case EquipEvent.CellCompPort1: cimAddress = "B1021"; plcAddress = "B0021"; break;
                case EquipEvent.CellCompPort2: cimAddress = "B1022"; plcAddress = "B0022"; break;
                case EquipEvent.CellCompPort3: cimAddress = "B1023"; plcAddress = "B0023"; break;
                case EquipEvent.CellCompPort4: cimAddress = "B1024"; plcAddress = "B0024"; break;

                // Nhóm Material Kitting & Status 1
                case EquipEvent.KittingorCancel1: cimAddress = "B107E"; plcAddress = "B007E"; break;
                case EquipEvent.MaterialSupplementRequest1: cimAddress = "B1076"; plcAddress = "B0076"; break;
                case EquipEvent.MaterialSupplementComplete1: cimAddress = "B1079"; plcAddress = "B0079"; break;
                case EquipEvent.MaterialWarning1: cimAddress = "B1082"; plcAddress = "B0082"; break;
                case EquipEvent.MaterialShortage1: cimAddress = "B1083"; plcAddress = "B0083"; break;
                case EquipEvent.MaterialLocationUpdate1: cimAddress = "B108A"; plcAddress = "B008A"; break;

                // Nhóm Material Kitting & Status 2
                case EquipEvent.KittingorCancel2: cimAddress = "B107F"; plcAddress = "B007F"; break;
                case EquipEvent.MaterialSupplementRequest2: cimAddress = "B1077"; plcAddress = "B0077"; break;
                case EquipEvent.MaterialSupplementComplete2: cimAddress = "B107A"; plcAddress = "B007A"; break;
                case EquipEvent.MaterialWarning2: cimAddress = "B1084"; plcAddress = "B0084"; break;
                case EquipEvent.MaterialShortage2: cimAddress = "B1085"; plcAddress = "B0085"; break;
                case EquipEvent.MaterialLocationUpdate2: cimAddress = "B108B"; plcAddress = "B008B"; break;

                // Nhóm Material Kitting & Status 3
                case EquipEvent.KittingorCancel3: cimAddress = "B1080"; plcAddress = "B0080"; break;
                case EquipEvent.MaterialSupplementRequest3: cimAddress = "B1078"; plcAddress = "B0078"; break;
                case EquipEvent.MaterialSupplementComplete3: cimAddress = "B107B"; plcAddress = "B007B"; break;
                case EquipEvent.MaterialWarning3: cimAddress = "B1086"; plcAddress = "B0086"; break;
                case EquipEvent.MaterialShortage3: cimAddress = "B1087"; plcAddress = "B0087"; break;
                case EquipEvent.MaterialLocationUpdate3: cimAddress = "B108C"; plcAddress = "B008C"; break;

                // Nhóm Pair Cell Process
                case EquipEvent.PairCellProcessStart: cimAddress = "B1044"; plcAddress = "B0044"; break;
                case EquipEvent.PairCellProcessStart_2: cimAddress = "B1045"; plcAddress = "B0045"; break;
                case EquipEvent.PairCellProcessComplete: cimAddress = "B1046"; plcAddress = "B0046"; break;
                case EquipEvent.PairCellProcessComplete_2: cimAddress = "B1047"; plcAddress = "B0047"; break;
                case EquipEvent.PairCellProcessComplete_3: cimAddress = "B1048"; plcAddress = "B0048"; break;

                // Nhóm Specific Validation & Inspection
                case EquipEvent.SpecificValidationRequest1: cimAddress = "B10D9"; plcAddress = "B00D9"; break;
                case EquipEvent.SpecificValidationRequest2: cimAddress = "B10DA"; plcAddress = "B00DA"; break;
                case EquipEvent.SpecificValidationRequest3: cimAddress = "B10DB"; plcAddress = "B00DB"; break;
                case EquipEvent.SpecificValidationRequest4: cimAddress = "B10DC"; plcAddress = "B00DC"; break;
                case EquipEvent.SpecificValidationRequest5: cimAddress = "B10DD"; plcAddress = "B00DD"; break;

                // Nhóm Label
                case EquipEvent.LabelInformationRequest: cimAddress = "B108E"; plcAddress = "B008E"; break;
                case EquipEvent.LabelValidationRequest: cimAddress = "B1090"; plcAddress = "B0090"; break;
                case EquipEvent.LabelPrecheckRequest: cimAddress = "B1092"; plcAddress = "B0092"; break;

                // Nhóm Process Control
                case EquipEvent.ProcessControlResult1: cimAddress = "B1096"; plcAddress = "B0096"; break;
                case EquipEvent.ProcessControlResult2: cimAddress = "B1097"; plcAddress = "B0097"; break;
                case EquipEvent.ProcessControlResult3: cimAddress = "B1098"; plcAddress = "B0098"; break;

                // Các event quan trọng khác
                case EquipEvent.EquipFunctionChangeEvent: cimAddress = "B10E3"; plcAddress = "B00E3"; break;
                case EquipEvent.EquipFunctionChangeCMDHcack: cimAddress = "B10E4"; plcAddress = "B00E4"; break;
                case EquipEvent.FormattedProcessProgramSend: cimAddress = "B1035"; plcAddress = "B0035"; break;
                case EquipEvent.TRSProcessJobLocationUpdate: cimAddress = "B10C9"; plcAddress = "B00C9"; break;
                case EquipEvent.NormalDataCollection1: cimAddress = "B1140"; plcAddress = "B0140"; break;
                case EquipEvent.NormalDataCollection2: cimAddress = "B1141"; plcAddress = "B0141"; break;
                case EquipEvent.EquipmentConstantParameterChangeEvent: cimAddress = "B1017"; plcAddress = "B0017"; break;

                case EquipEvent.CurrentProcessControlDataReply: cimAddress = "B1095"; plcAddress = "B0095"; break;
                case EquipEvent.APCWordTotal: cimAddress = "B1095"; plcAddress = "B0095"; break;

                // Nhóm Assemble Process
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

                // Nhóm NG Process
                case EquipEvent.MaterialNGProcess1_1: cimAddress = "B105E"; plcAddress = "B005E"; break;
                case EquipEvent.MaterialNGProcess2_1: cimAddress = "B105F"; plcAddress = "B005F"; break;
                case EquipEvent.MaterialNGProcess3_1: cimAddress = "B1060"; plcAddress = "B0060"; break;
                case EquipEvent.MaterialNGProcess4_1: cimAddress = "B1061"; plcAddress = "B0061"; break;
                case EquipEvent.MaterialNGProcess5_1: cimAddress = "B1062"; plcAddress = "B0062"; break;
                case EquipEvent.MaterialNGProcess6_1: cimAddress = "B1063"; plcAddress = "B0063"; break;

                // Nhóm ID Reading Result
                case EquipEvent.MaterialIDReadingResult1: cimAddress = "B102C"; plcAddress = "B002C"; break;
                case EquipEvent.MaterialIDReadingResult2: cimAddress = "B102D"; plcAddress = "B002D"; break;
                case EquipEvent.MaterialIDReadingResult3: cimAddress = "B102E"; plcAddress = "B002E"; break;

                // Nhóm Process Control Create/Delete
                case EquipEvent.ProcessControlDataCreateDelete1: cimAddress = "B109E"; plcAddress = "B009E"; break;
                case EquipEvent.ProcessControlDataCreateDelete2: cimAddress = "B109F"; plcAddress = "B009F"; break;
                case EquipEvent.ProcessControlDataCreateDelete3: cimAddress = "B10A0"; plcAddress = "B00A0"; break;
                case EquipEvent.ProcessControlDataCreateDelete4: cimAddress = "B10A1"; plcAddress = "B00A1"; break;

                // Nhóm Unit TPM Ready & Loss
                case EquipEvent.TPMLossReady: cimAddress = "B1012"; plcAddress = "B0012"; break;
                case EquipEvent.UnitTPMLossReady1: cimAddress = "B10F8"; plcAddress = "B00F8"; break;
                case EquipEvent.UnitTPMLossReady2: cimAddress = "B10F9"; plcAddress = "B00F9"; break;
                case EquipEvent.UnitTPMLossReady3: cimAddress = "B10FA"; plcAddress = "B00FA"; break;
                case EquipEvent.UnitTPMLossReady4: cimAddress = "B10FB"; plcAddress = "B00FB"; break;
                case EquipEvent.UnitTPMLossReady5: cimAddress = "B10FC"; plcAddress = "B00FC"; break;
                case EquipEvent.UnitTPMLossReady6: cimAddress = "B10FD"; plcAddress = "B00FD"; break;
                case EquipEvent.UnitTPMLossReady7: cimAddress = "B10FE"; plcAddress = "B00FE"; break;
                case EquipEvent.UnitTPMLossReady8: cimAddress = "B10FF"; plcAddress = "B00FF"; break;

                case EquipEvent.UnitTPMLoss1: cimAddress = "B1100"; plcAddress = "B0100"; break;
                case EquipEvent.UnitTPMLoss2: cimAddress = "B1101"; plcAddress = "B0101"; break;
                case EquipEvent.UnitTPMLoss3: cimAddress = "B1102"; plcAddress = "B0102"; break;
                case EquipEvent.UnitTPMLoss4: cimAddress = "B1103"; plcAddress = "B0103"; break;
                case EquipEvent.UnitTPMLoss5: cimAddress = "B1104"; plcAddress = "B0104"; break;
                case EquipEvent.UnitTPMLoss6: cimAddress = "B1105"; plcAddress = "B0105"; break;
                case EquipEvent.UnitTPMLoss7: cimAddress = "B1106"; plcAddress = "B0106"; break;
                case EquipEvent.UnitTPMLoss8: cimAddress = "B1107"; plcAddress = "B0107"; break;

                // Nhóm Unit Interlock Confirm
                case EquipEvent.UnitInterlockConfirm1: cimAddress = "B10ED"; plcAddress = "B00ED"; break;
                case EquipEvent.UnitInterlockConfirm2: cimAddress = "B10EE"; plcAddress = "B00EE"; break;
                case EquipEvent.UnitInterlockConfirm3: cimAddress = "B10EF"; plcAddress = "B00EF"; break;
                case EquipEvent.UnitInterlockConfirm4: cimAddress = "B10F0"; plcAddress = "B00F0"; break;
                case EquipEvent.UnitInterlockConfirm5: cimAddress = "B10F1"; plcAddress = "B00F1"; break;
                case EquipEvent.UnitInterlockConfirm6: cimAddress = "B10F2"; plcAddress = "B00F2"; break;
                case EquipEvent.UnitInterlockConfirm7: cimAddress = "B10F3"; plcAddress = "B00F3"; break;
                case EquipEvent.UnitInterlockConfirm8: cimAddress = "B10F4"; plcAddress = "B00F4"; break;
                case EquipEvent.OperatorLogInfomation: cimAddress = "B1040"; plcAddress = "B0040"; break;

                // Vùng nhớ không có BIT (Trả về rỗng)
                case EquipEvent.MaterialPortState:
                case EquipEvent.PortStatus:
                case EquipEvent.UnitStatus:
                case EquipEvent.JigPortState:
                    cimAddress = string.Empty; plcAddress = string.Empty; break;

                default: break;
            }
        }

        public void GetWordAddress(EquipEvent evt, out string cimAddress, out string plcAddress, out int cimLength, out int plcLength)
        {
            cimAddress = string.Empty;
            cimLength = 0;
            plcAddress = string.Empty;
            plcLength = 0;

            switch (evt)
            {
                // --- Nhóm Job & Standard Port ---
                case EquipEvent.JobEvent1: plcAddress = "1140"; plcLength = 47; break;
                case EquipEvent.JobEvent2: plcAddress = "116F"; plcLength = 27; break;
                case EquipEvent.TPMLoss: plcAddress = "120F"; plcLength = 22; break;
                case EquipEvent.PPIDChange: plcAddress = "14"; plcLength = 20; break;
                case EquipEvent.EQPPIDUpdate: plcAddress = "14"; plcLength = 20; break;
                case EquipEvent.PPIDModeAndPPIDUpdate: plcAddress = "9224"; plcLength = 22; break;

                case EquipEvent.ParameterTotal: plcAddress = "9224"; plcLength = 4000; break;
                case EquipEvent.ParameterChange2: plcAddress = "23D9"; plcLength = 3; break;

                case EquipEvent.CurrentEquipPPIDListResponse:
                    cimAddress = string.Empty; cimLength = 0;
                    plcAddress = "8A54"; plcLength = 2000; // PPID LIST WORD (W8A54 ~ W9223)
                    break;

                case EquipEvent.CellStartPort1: plcAddress = "4A0"; plcLength = 107; break;
                case EquipEvent.CellStartPort2: plcAddress = "510"; plcLength = 107; break;
                case EquipEvent.CellStartPort3: plcAddress = "580"; plcLength = 107; break;
                case EquipEvent.CellStartPort4: plcAddress = "5F0"; plcLength = 107; break;
                case EquipEvent.CellCompPort1: plcAddress = "760"; plcLength = 177; break;
                case EquipEvent.CellCompPort2: plcAddress = "820"; plcLength = 177; break;
                case EquipEvent.CellCompPort3: plcAddress = "8E0"; plcLength = 177; break;
                case EquipEvent.CellCompPort4: plcAddress = "9A0"; plcLength = 177; break;

                // --- Nhóm Confirm (Màn hình) ---
                case EquipEvent.OPCallConfirm: plcAddress = "F30"; plcLength = 150; break;
                case EquipEvent.InterlockConfirm: plcAddress = "FF0"; plcLength = 150; break;

                // --- Nhóm KITTING & MATERIAL EVENT (Dùng chung Area cho từng luồng) ---
                // Luồng 1 (W7B14 ~ W7B85 : 114 Words)
                case EquipEvent.KittingorCancel1:
                case EquipEvent.MaterialSupplementRequest1:
                case EquipEvent.MaterialSupplementComplete1:
                case EquipEvent.MaterialWarning1:
                case EquipEvent.MaterialShortage1:
                case EquipEvent.MaterialLocationUpdate1:
                    plcAddress = "7B14"; plcLength = 114; break;

                // Luồng 2 (W7B8D ~ W7BFE : 114 Words)
                case EquipEvent.KittingorCancel2:
                case EquipEvent.MaterialSupplementRequest2:
                case EquipEvent.MaterialSupplementComplete2:
                case EquipEvent.MaterialWarning2:
                case EquipEvent.MaterialShortage2:
                case EquipEvent.MaterialLocationUpdate2:
                    plcAddress = "7B8D"; plcLength = 114; break;

                // Luồng 3 (W7C06 ~ W7C77 : 114 Words)
                case EquipEvent.KittingorCancel3:
                case EquipEvent.MaterialSupplementRequest3:
                case EquipEvent.MaterialSupplementComplete3:
                case EquipEvent.MaterialWarning3:
                case EquipEvent.MaterialShortage3:
                case EquipEvent.MaterialLocationUpdate3:
                    plcAddress = "7C06"; plcLength = 114; break;

                // --- Nhóm Pair Cell Process Start (251 Words) ---
                case EquipEvent.PairCellProcessStart: plcAddress = "1BF0"; plcLength = 251; break;
                case EquipEvent.PairCellProcessStart_2: plcAddress = "1CEB"; plcLength = 251; break;

                // --- Nhóm Pair Cell Process Complete (292 Words) ---
                case EquipEvent.PairCellProcessComplete: plcAddress = "1DE6"; plcLength = 292; break;
                case EquipEvent.PairCellProcessComplete_2: plcAddress = "1F0A"; plcLength = 292; break;
                case EquipEvent.PairCellProcessComplete_3: plcAddress = "202E"; plcLength = 292; break;

                // --- Nhóm Specific Validation Request (45 Words) ---
                case EquipEvent.SpecificValidationRequest1: plcAddress = "2980"; plcLength = 45; break;
                case EquipEvent.SpecificValidationRequest2: plcAddress = "29AD"; plcLength = 45; break;
                case EquipEvent.SpecificValidationRequest3: plcAddress = "29DA"; plcLength = 45; break;
                case EquipEvent.SpecificValidationRequest4: plcAddress = "2A07"; plcLength = 45; break;
                case EquipEvent.SpecificValidationRequest5: plcAddress = "2A34"; plcLength = 45; break;

                // --- Nhóm Process Control Result (48 Words) ---
                case EquipEvent.ProcessControlResult1: plcAddress = "1A24"; plcLength = 48; break;
                case EquipEvent.ProcessControlResult2: plcAddress = "1A55"; plcLength = 48; break;
                case EquipEvent.ProcessControlResult3: plcAddress = "1A84"; plcLength = 48; break;
                case EquipEvent.ProcessControlResult4: plcAddress = "1AB4"; plcLength = 48; break;

                // --- Nhóm khác ---
                case EquipEvent.TRSProcessJobLocationUpdate: plcAddress = "5F0E"; plcLength = 60; break;
                case EquipEvent.EquipFunctionChangeCMDHcack: plcAddress = "2970"; plcLength = 1; break;
                case EquipEvent.FormattedProcessProgramSend: plcAddress = "125D"; plcLength = 1; break;
                case EquipEvent.EquipFunctionChangeEvent: plcAddress = "5952"; plcLength = 77; break;
                case EquipEvent.PackingInformation: plcAddress = "6664"; plcLength = 50; break;
                case EquipEvent.PackingJobStart: plcAddress = "6696"; plcLength = 78; break;
                case EquipEvent.NormalDataCollection1: plcAddress = "6876"; plcLength = 60; break;
                case EquipEvent.CurrentProcessControlDataReply: plcAddress = "1910"; plcLength = 64; break;
                case EquipEvent.APCWordTotal:
                    plcAddress = "C4EC"; plcLength = 112;
                    cimAddress = "DDE0"; cimLength = 112;
                    break;

                // --- Nhóm Status / State (Không có Bit) ---
                case EquipEvent.MaterialPortState: plcAddress = "160"; plcLength = 424; break;
                case EquipEvent.MaterialPortState1: plcAddress = "160"; plcLength = 53; break;
                case EquipEvent.MaterialPortState2: plcAddress = "195"; plcLength = 53; break;
                case EquipEvent.MaterialPortState3: plcAddress = "1CA"; plcLength = 53; break;
                case EquipEvent.MaterialPortState4: plcAddress = "1FF"; plcLength = 53; break;
                case EquipEvent.MaterialPortState5: plcAddress = "234"; plcLength = 53; break;
                case EquipEvent.MaterialPortState6: plcAddress = "269"; plcLength = 53; break;
                case EquipEvent.MaterialPortState7: plcAddress = "29E"; plcLength = 53; break;
                case EquipEvent.MaterialPortState8: plcAddress = "2D3"; plcLength = 53; break;
                case EquipEvent.PortStatus: plcAddress = "2C"; plcLength = 49; break;
                case EquipEvent.UnitStatus: plcAddress = "450"; plcLength = 56; break;
                case EquipEvent.JigPortState: plcAddress = "59A4"; plcLength = 352; break;

                // --- Nhóm Material Assemble Process (Length: 220) ---
                case EquipEvent.MaterialAssembleProcess1_1: plcAddress = "7F4F"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess1_2: plcAddress = "802B"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess1_3: plcAddress = "8107"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess2_1: plcAddress = "81E3"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess2_2: plcAddress = "82BF"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess2_3: plcAddress = "839B"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess3_1: plcAddress = "8477"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess3_2: plcAddress = "8553"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess4_1: plcAddress = "862F"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess4_2: plcAddress = "870B"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess5_1: plcAddress = "87E7"; plcLength = 220; break;
                case EquipEvent.MaterialAssembleProcess5_2: plcAddress = "88C3"; plcLength = 220; break;

                // --- Nhóm Material NG Process (Length: 120) ---
                case EquipEvent.MaterialNGProcess1_1: plcAddress = "7C7F"; plcLength = 120; break;
                case EquipEvent.MaterialNGProcess2_1: plcAddress = "7D47"; plcLength = 120; break;
                case EquipEvent.MaterialNGProcess3_1: plcAddress = "7DC1"; plcLength = 120; break;
                case EquipEvent.MaterialNGProcess4_1: plcAddress = "7DE7"; plcLength = 120; break;
                case EquipEvent.MaterialNGProcess5_1: plcAddress = "7E5F"; plcLength = 120; break;
                case EquipEvent.MaterialNGProcess6_1: plcAddress = "7ED7"; plcLength = 120; break;

                // --- Nhóm ID Reading Result (Length: 51) ---
                case EquipEvent.MaterialIDReadingResult1: plcAddress = "899F"; plcLength = 51; break;
                case EquipEvent.MaterialIDReadingResult2: plcAddress = "89D2"; plcLength = 51; break;
                case EquipEvent.MaterialIDReadingResult3: plcAddress = "8A05"; plcLength = 51; break;

                // --- Nhóm Process Control Create/Delete (Length: 49) ---
                case EquipEvent.ProcessControlDataCreateDelete1: plcAddress = "1958"; plcLength = 51; break;
                case EquipEvent.ProcessControlDataCreateDelete2: plcAddress = "1989"; plcLength = 51; break;
                case EquipEvent.ProcessControlDataCreateDelete3: plcAddress = "19BA"; plcLength = 51; break;
                case EquipEvent.ProcessControlDataCreateDelete4: plcAddress = "19EB"; plcLength = 51; break;

                // --- Nhóm Unit TPM Loss (Length: 22) ---
                case EquipEvent.UnitTPMLoss1: plcAddress = "2A61"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss2: plcAddress = "2A77"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss3: plcAddress = "2A8D"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss4: plcAddress = "2AA3"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss5: plcAddress = "2AB9"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss6: plcAddress = "2ACF"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss7: plcAddress = "2AE5"; plcLength = 22; break;
                case EquipEvent.UnitTPMLoss8: plcAddress = "2AFB"; plcLength = 22; break;
                case EquipEvent.OperatorLogInfomation: plcAddress = "1226"; plcLength = 55; break;
                
                default: break;
            }
        }
    }
}
