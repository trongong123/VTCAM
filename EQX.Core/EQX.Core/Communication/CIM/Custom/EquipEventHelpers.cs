using EQX.Core.Common;
using EQX.Core.Communication.CIM.Custom.WordArea;
using log4net;
using Newtonsoft.Json.Linq;
using OpenCvSharp.Flann;
using System.Security.Cryptography;
using System.Text;
using TOPENG_Device;
using static EQX.Core.Communication.CIM.Custom.WordArea.PPIDListArea;

namespace EQX.Core.Communication.CIM.Custom
{
    public class EquipEventHelpers
    {
        public static bool TPMLostReport(ETPMLossDesciption tpmLoss)
        {
            TPMLossArea tmpLossArea = new TPMLossArea
            {
                TPMLossCode = (int)tpmLoss,
                TPMLossDescp = tpmLoss.ToString()
            };

            EquipEventDetail.Create(EquipEvent.TPMLoss).Write(tmpLossArea.ToCIMData());
            EquipEventDetail.Create(EquipEvent.TPMLoss).SetPLCBitOn();

            EquipEventDetail.Create(EquipEvent.TPMLoss).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.TPMLoss).SetPLCBitOff();

            EquipEventDetail.Create(EquipEvent.TPMLossReady).SetPLCBitOff();

            return true;
        }

        public static bool PPIDChange(string ppidName)
        {
            bool result;

            EQPPPIDArea ppipArea = new EQPPPIDArea
            {
                EQPPPID = ppidName,
            };
            EquipEventDetail.Create(EquipEvent.PPIDChange).Write(ppipArea.ToCIMData());
            EquipEventDetail.Create(EquipEvent.PPIDChange).SetPLCBitOn();
            result = EquipEventDetail.Create(EquipEvent.PPIDChange).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.PPIDChange).SetPLCBitOff();

            return result;
        }

        public static bool PPIDCreate(string ppidName, string cimRecipeNumber = "")
        {
            bool result = true;

            PPIDModeAndPPIDArea ppipArea = new PPIDModeAndPPIDArea
            {
                PPIDName = ppidName,
                PPIDMode = 1,
            };
            bool result1 = CIMHelpers.TryParseRecipeNumber(ppidName, out int index);
            if (result1 == false)
            {
                throw new Exception("PPID Name format is not match");
            }

            EquipEventDetail.Create(EquipEvent.PPIDModeAndPPIDUpdate).Write(ppipArea.ToCIMData());
            ParameterChange2Area parameterChangeArea = new ParameterChange2Area()
            {
                RecipeNumber = cimRecipeNumber
            };
            if (string.IsNullOrEmpty(cimRecipeNumber)) parameterChangeArea.RecipeNumber = $"{index}O";

            EquipEventDetail.Create(EquipEvent.ParameterChange2).Write(parameterChangeArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOff();

            return result;
        }

        public static bool PPIDDelete(string ppidName, string fromCIMRecipeNumber = "")
        {
            bool result = true;

            PPIDModeAndPPIDArea ppipArea = new PPIDModeAndPPIDArea
            {
                PPIDName = ppidName,
                PPIDMode = 2,
            };
            bool result1 = CIMHelpers.TryParseRecipeNumber(ppidName, out int index);
            if (result1 == false)
            {
                throw new Exception("PPID Name format is not match");
            }
            EquipEventDetail.Create(EquipEvent.PPIDModeAndPPIDUpdate).Write(ppipArea.ToCIMData());

            ParameterChange2Area parameterChangeArea = new ParameterChange2Area
            {
                RecipeNumber = $"{index}G"
            };
            if (string.IsNullOrEmpty(fromCIMRecipeNumber) == false) parameterChangeArea.RecipeNumber = fromCIMRecipeNumber;
            EquipEventDetail.Create(EquipEvent.ParameterChange2).Write(parameterChangeArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOff();

            return result;
        }

        public static bool ParameterRequestResponse(FormattedProcessProgramRequestArea fpprArea, string data)
        {
            short[] buf = new short[2];
            CIMScenarioDispatcher.EncodeAsciiToWords(data, ref buf, 0, 2);
            return ParameterInquiryResponse(fpprArea, buf);
        }
        public static bool ParameterInquiryResponse(FormattedProcessProgramRequestArea fpprArea, short[] data)
        {
            bool result = true;

            ParameterWordSingleParameterUpdate(fpprArea.ReqPPIDIndex, data);
            PPIDModeAndPPIDArea ppipArea = new PPIDModeAndPPIDArea
            {
                PPIDName = fpprArea.ReqPPID,
                PPIDMode = 0,
            };
            EquipEventDetail.Create(EquipEvent.PPIDModeAndPPIDUpdate).Write(ppipArea.ToCIMData());
            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramRequest).SetPLCBitOn();
            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramRequest).WaitForCIMBitOff();
            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramRequest).SetPLCBitOff();

            return result;
        }

        /// <summary>
        /// Update single parameter to PARAMETER WORD / SET DATA ONLY
        /// </summary>
        /// <returns></returns>
        public static bool ParameterWordSingleParameterUpdate(int index, int value)
        {
            short[] buffer = new short[2];
            CIMScenarioDispatcher.EncodeInt32ToWords(value, ref buffer, 0);

            CIMAddressMap.WriteWords(CIMHelpers.GetParameterWordAddressInt(index), 2, buffer);
            return true;
        }

        /// <summary>
        /// Update single parameter to PARAMETER WORD / SET DATA ONLY
        /// </summary>
        /// <returns></returns>
        public static bool ParameterWordSingleParameterUpdate(int index, short[] buffer)
        {
            if (buffer.Count() != 2)
            {
                throw new Exception("buffer size must be 2 short");
            }

            CIMAddressMap.WriteWords(CIMHelpers.GetParameterWordAddressInt(index), 2, buffer);
            return true;
        }

        /// <summary>
        /// After write parameter 
        /// </summary>
        public static void ParameterChange(ParameterWordArea parameterArea, int recipeNumber)
        {
            parameterArea.PPIDMode = 3;
            EquipEventDetail.Create(EquipEvent.ParameterTotal).Write(parameterArea.ToCIMData());

            ParameterChange2Area parameterChangeArea = new ParameterChange2Area
            {
                RecipeNumber = $"{recipeNumber}C"
            };
            EquipEventDetail.Create(EquipEvent.ParameterChange2).Write(parameterChangeArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOff();
        }

        public static void PPIDListInquiryResponse(string[] ppidList)
        {
            //if (ppidList.Count() > 100)
            //{
            //    throw new Exception("ppidList size must have exact 100 member");
            //}

            //for (int i = 0; i < ppidList.Length; i++)
            //{
            //    PPIDListSinglePPIDWrite(ppidList[i]);
            //}

            EquipEventDetail.Create(EquipEvent.CurrentEquipPPIDListResponse).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.CurrentEquipPPIDListResponse).WaitForCIMBitOff();
            EquipEventDetail.Create(EquipEvent.CurrentEquipPPIDListResponse).SetPLCBitOff();
        }

        public static bool PPIDListSinglePPIDWrite(string ppid, bool isClear = false)
        {
            bool result = CIMHelpers.TryParseRecipeNumber(ppid, out int index);
            if (result == false)
            {
                return false;
                throw new Exception("PPID Name format is not match");
            }

            short[] buf = new short[20];
            if (isClear == false) CIMScenarioDispatcher.EncodeAsciiToWords(ppid, ref buf, 0, 20);

            CIMAddressMap.WriteWords(CIMHelpers.GetPpidListWordAddressInt(index), 20, buf, false);
            return true;
        }

        public static bool SpecificValidation(int jigIndex, string cellID, ref string outCellID)
        {
            EquipEvent validRequest = EquipEvent.SpecificValidationRequest1 + jigIndex - 1;
            CIMCommand validData = CIMCommand.SpecificValidationDataSend1 + jigIndex - 1;

            // 1. Request SpecificValidation
            SpecificValidationRequestArea validationRequestArea = new()
            {
                OptionCode = "CHKBIMJIG",
                CellID = cellID,
            };

            bool result = true;

            EquipEventDetail.Create(validRequest).Write(validationRequestArea.ToCIMData());
            EquipEventDetail.Create(validRequest).SetPLCBitOn();
            result = EquipEventDetail.Create(validRequest).WaitForCIMBitOn(30000);
            EquipEventDetail.Create(validRequest).SetPLCBitOff();

            var validDataCommand = CIMCommandDetail.Create(validData);

            bool validTimeout = validDataCommand.WaitForCIMBitOn(30000) == false;
            if (validTimeout)
            {
                LogManager.GetLogger("CIM").Error("SpecificValidationDataSend1 TIMEOUT");
                return false;
            }

            validDataCommand.ReadCIMWords();

            // 3. Data processing
            SpecificValidationDataSendArea dataSendArea = new SpecificValidationDataSendArea();
            dataSendArea.FromCIMData(validDataCommand.FromCIMDataBuffer);
            outCellID = dataSendArea.CellID;

            if (dataSendArea.ReplyStatus.ToUpper() == "FAIL")
            {
                LogManager.GetLogger("CIM").Error("SpecificValidationDataSend1 FAIL");
                return false;
            }

            return result;
        }

        public static bool CellTrackIn(int jigIndex, string cellID)
        {
            bool result = true;

            EquipEvent cellStartPortEvent = EquipEvent.CellStartPort1 + jigIndex - 1;

            CellStartPortArea cellStartPortArea = new CellStartPortArea()
            {
                TrackInCellID = cellID,
                TrackInReaderID = jigIndex.ToString(),
                TrackInRRC = "0",
            };

            EquipEventDetail.Create(cellStartPortEvent).Write(cellStartPortArea.ToCIMData());
            EquipEventDetail.Create(cellStartPortEvent).SetPLCBitOn();
            EquipEventDetail.Create(cellStartPortEvent).WaitForCIMBitOn(30000);
            EquipEventDetail.Create(cellStartPortEvent).SetPLCBitOff();


            return result;
        }

        public static CellJobProcessCimToPlcArea CellJobProcessConfirm(int jigIndex, string cellID)
        {
            EquipEvent cellCompPort = EquipEvent.CellCompPort1 + jigIndex - 1;
            CIMCommand cellJobProc = CIMCommand.CellJobProcess1 + jigIndex - 1;

            bool result = true;

            result = CIMCommandDetail.Create(cellJobProc).WaitForCIMBitOn(30000);

            if (result == false)
            {
                CellCompPortArea compPortTimeout = new CellCompPortArea()
                {
                    TrackOutCellID = cellID,
                    TrackOutJudge = "O",
                    TrackOutDescription = "CELL_VALIDATION_TIMEOUT",
                };
                EquipEventDetail.Create(cellCompPort).Write(compPortTimeout.ToCIMData());
                EquipEventDetail.Create(cellCompPort).SetPLCBitOn();
                EquipEventDetail.Create(cellCompPort).WaitForCIMBitOn(30000);
                EquipEventDetail.Create(cellCompPort).SetPLCBitOff();

                return new CellJobProcessCimToPlcArea()
                {
                    UserDefine_TrackOutDescription = compPortTimeout.TrackOutDescription,
                    UserDefine_TrackOutJudge = compPortTimeout.TrackOutJudge,
                    CellJobProcessCellID = compPortTimeout.TrackOutCellID,
                    CellJobProcessRCMD = ((int)ECellJobProcessRCMD.CellJobProcessCancel).ToString(),
                };
            }

            var cellProcCommand = CIMCommandDetail.Create(cellJobProc);
            cellProcCommand.ReadCIMWords();

            CellJobProcessCimToPlcArea cellJobProcess = new CellJobProcessCimToPlcArea();
            cellJobProcess.FromCIMData(cellProcCommand.FromCIMDataBuffer);

            return cellJobProcess;
        }

        public static bool CellTrackOut(int jigIndex, CellJobProcessCimToPlcArea cellJobProcess, bool isFail = false)
        {
            bool result = true;
            CellCompPortArea compPortArea = new CellCompPortArea()
            {
                TrackOutJudge = string.IsNullOrEmpty(cellJobProcess.UserDefine_TrackOutJudge) == false ? cellJobProcess.UserDefine_TrackOutJudge : "G", // GOOD
                TrackOutDescription = string.IsNullOrEmpty(cellJobProcess.UserDefine_TrackOutDescription) == false ? cellJobProcess.UserDefine_TrackOutDescription : "",
                TrackOutCellID = cellJobProcess.CellJobProcessCellID,
                TrackOutProductID = cellJobProcess.CellJobProcessProductID,
                TrackOutStepID = cellJobProcess.CellJobProcessStepID
            };

            EquipEvent cellCompPort = EquipEvent.CellCompPort1 + jigIndex - 1;
            EquipEventDetail.Create(cellCompPort).WriteAndBitOnOff(compPortArea.ToCIMData());

            if (cellJobProcess.CellJobProcessRCMD == $"{(int)ECellJobProcessRCMD.CellJobProcessCancel}")
            {
                result = false;
            }

            return result;
        }

        public static void MaterialKitting(MaterialKittingPlcToCimArea materialKittingPlcToCimArea)
        {
            materialKittingPlcToCimArea.CEID = (short)EMaterialKittingCEID.KITTING;

            EquipEventDetail.Create(EquipEvent.KittingorCancel1).Write(materialKittingPlcToCimArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.KittingorCancel1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.KittingorCancel1).WaitForCIMBitOn(30000);
            EquipEventDetail.Create(EquipEvent.KittingorCancel1).SetPLCBitOff();
        }

        public static void MaterialCancel(MaterialKittingPlcToCimArea materialKittingPlcToCimArea)
        {
            materialKittingPlcToCimArea.CEID = (short)EMaterialKittingCEID.KITTING_CANCEL;

            EquipEventDetail.Create(EquipEvent.KittingorCancel1).WriteAndBitOnOff(materialKittingPlcToCimArea.ToCIMData());
        }

        public static void MaterialInfoSendKitting(MaterialKittingPlcToCimArea materialKittingPlcToCimArea)
        {
            EquipEventDetail.Create(EquipEvent.MaterialLocationUpdate1).Write(materialKittingPlcToCimArea.ToCIMData());
            EquipEventDetail.Create(EquipEvent.MaterialLocationUpdate1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialLocationUpdate1).WaitForCIMBitOn(30000);
            EquipEventDetail.Create(EquipEvent.MaterialLocationUpdate1).SetPLCBitOff();

            MaterialPortStateItemArea area = new MaterialPortStateItemArea()
            {
                Type = materialKittingPlcToCimArea.Type,
                LST = "1",  // MOUNT
                ID = materialKittingPlcToCimArea.BatchID,
                LoaderNo = Convert.ToInt16(materialKittingPlcToCimArea.PortID),
                Usage = (short)materialKittingPlcToCimArea.TotalQty,
            };

            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).Write(area.ToCIMData());
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).SetPLCBitOff();

            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).SetPLCBitOn();
            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).WaitForCIMBitOff(30000);
            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).SetPLCBitOff();
        }

        public static void MaterialInfoSendCancel(MaterialKittingPlcToCimArea materialKittingPlcToCimArea)
        {
            EquipEventDetail.Create(EquipEvent.MaterialShortage1).Write(materialKittingPlcToCimArea.ToCIMData());
            EquipEventDetail.Create(EquipEvent.MaterialShortage1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialShortage1).WaitForCIMBitOn(30000);
            EquipEventDetail.Create(EquipEvent.MaterialShortage1).SetPLCBitOff();

            MaterialPortStateItemArea area = new MaterialPortStateItemArea()
            {
                Type = "",
                LST = "3",  // UNMOUNT
                ID = "",
                LoaderNo = Convert.ToInt16(materialKittingPlcToCimArea.PortID),
                Usage = 0,
            };

            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).Write(area.ToCIMData());
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.MaterialPortState1 + Convert.ToInt16(materialKittingPlcToCimArea.PortID) - 1).SetPLCBitOff();

            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).SetPLCBitOn();
            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).WaitForCIMBitOff(30000);
            CIMCommandDetail.Create(CIMCommand.MaterialInfoSend1).SetPLCBitOff();
        }

        public static void WriteEcmSingle(int index, int value)
        {
            short[] buffer = new short[2];
            CIMScenarioDispatcher.EncodeInt32ToWords(value, ref buffer, 0);
            CIMAddressMap.WriteWords(CIMHelpers.GetEcmWordAddressInt(index), 2, buffer, false);
        }

        public static void EquipmentConstantParameterChanged(int index, int value)
        {
            WriteEcmSingle(index, value);

            EquipEventDetail.Create(EquipEvent.EquipmentConstantParameterChangeEvent).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.EquipmentConstantParameterChangeEvent).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.EquipmentConstantParameterChangeEvent).SetPLCBitOff();
        }

        public static void WriteEcmAll()
        {
            // 추후 이걸로 어드레스 가지고 오는거로 변경 필요.
            //CIMEventItemMap.GetEventItemWordRange(EquipEvent.CellIDReadingResult1, out string startD, out int lengthWords);
            const int count = 320;
            var buf = new short[count];

            // TEST
            CIMScenarioDispatcher.EncodeInt32ToWords(1000, ref buf, 0);
            CIMScenarioDispatcher.EncodeInt32ToWords(2000, ref buf, 2);
            CIMScenarioDispatcher.EncodeInt32ToWords(3000, ref buf, 4);
            CIMScenarioDispatcher.EncodeInt32ToWords(4000, ref buf, 6);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DA1C4");
            CIMAddressMap.WriteWords(w, count, buf);
        }

        /// <summary>
        /// 전체 FDC 항목 업데이트
        /// </summary>
        public static void WriteFdcItem()
        {
            const int count = 1129;
            var buf = new short[count];

            // TEST
            // Cell ID
            CIMScenarioDispatcher.EncodeAsciiToWords("TEST_CELLID", ref buf, 0, 20);
            CIMScenarioDispatcher.EncodeInt32ToWords(123, ref buf, 20);
            CIMScenarioDispatcher.EncodeInt32ToWords(456, ref buf, 22);
            CIMScenarioDispatcher.EncodeInt32ToWords(1123, ref buf, 24);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DAFD4");
            CIMAddressMap.WriteWords(w, count, buf, false);
        }

        public static void EquipFunctionChange(EquipFunctionChangeEventArea functionChangeArea)
        {
            EquipEventDetail.Create(EquipEvent.EquipFunctionChangeEvent).Write(functionChangeArea.ToCIMData());
            EquipEventDetail.Create(EquipEvent.EquipFunctionChangeEvent).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.EquipFunctionChangeEvent).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.EquipFunctionChangeEvent).SetPLCBitOff();
        }

        public static void EquipStateReport(EquipState equipState)
        {
            CIMAddressMap.WriteWordSingle("D2C", equipState.IsAvailable ? (short)'2' : (short)'1');
            CIMAddressMap.WriteWordSingle("D2D", equipState.IsInterlock ? (short)'1' : (short)'2');
            CIMAddressMap.WriteWordSingle("D2E", equipState.IsRunning & equipState.IsInterlock == false & equipState.IsAvailable ? (short)'2' : (short)'1');
            CIMAddressMap.WriteWordSingle("D2F", equipState.IsCellInEquip ? (short)'2' : (short)'1');
        }

        public static void ProcessControlDTOReply(int[] apcValues, int portId, string byWHO, string moduleID, int recipeNumber)
        {
            if (portId <= 0)
                throw new ArgumentException($"portId không hợp lệ. Yêu cầu >= 1.");
            if (apcValues.Length > APCWordArea.TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu <= {APCWordArea.TOTAL_WORDS} Words.");

            ProcessControlDataCreateDeleteArea pcdcdArea = new ProcessControlDataCreateDeleteArea();
            pcdcdArea.ByWho = byWHO;
            pcdcdArea.ModuleID = moduleID;

            if(byWHO == "EQP")
            {
                APCWordArea apcArea = new APCWordArea();
                apcArea.APCs = apcValues;

                EquipEventDetail.Create(EquipEvent.APCWordTotal).Write(apcArea.ToCIMData());
            }

            EquipEventDetail.Create(EquipEvent.ProcessControlDataCreateDelete1 + portId - 1).Write(pcdcdArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.ProcessControlDataCreateDelete1 + portId - 1).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.ProcessControlDataCreateDelete1 + portId - 1).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.ProcessControlDataCreateDelete1 + portId - 1).SetPLCBitOff();

            int apcStartIndex = 2; // Start index trên RMS TOTAL WORD
            for (int i = 0; i < apcValues.Length; i++)
            {
                ParameterWordSingleParameterUpdate(apcStartIndex + i, apcValues[i]);
            }

            ParameterChange2Area parameterChangeArea = new ParameterChange2Area()
            {
            };
            parameterChangeArea.RecipeNumber = $"{recipeNumber}C";

            EquipEventDetail.Create(EquipEvent.ParameterChange2).Write(parameterChangeArea.ToCIMData());

            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.ParameterChange2).SetPLCBitOff();
        }

        public static bool OperatorLogin(string opID, string opPW)
        {
            return OperatorLoginOut(opID, opPW, true);
        }

        public static bool OperatorLogout(string opID, string opPW)
        {
            return OperatorLoginOut(opID, opPW, false);
        }

        private static bool OperatorLoginOut(string opID, string opPW, bool isLogin)
        {
            bool result = true;
            OperatorLogInformationArea loginInfor = new OperatorLogInformationArea()
            {
                OPID = opID,
                OPPW = opPW,
                Option = isLogin ? "LOGIN" : "LOGOUT"
            };

            EquipEventDetail.Create(EquipEvent.OperatorLogInfomation).Write(loginInfor.ToCIMData());
            EquipEventDetail.Create(EquipEvent.OperatorLogInfomation).SetPLCBitOn();
            EquipEventDetail.Create(EquipEvent.OperatorLogInfomation).WaitForCIMBitOn();
            EquipEventDetail.Create(EquipEvent.OperatorLogInfomation).SetPLCBitOff();

            var approveCommand = CIMCommandDetail.Create(CIMCommand.EquipApproveProcess);
            result = approveCommand.WaitForCIMBitOn(10000);
            if (!result) return result;

            approveCommand.ReadCIMWords();

            EquipApproveProcessReceiveArea approveArea = new EquipApproveProcessReceiveArea();
            approveArea.FromCIMData(approveCommand.FromCIMDataBuffer);

            return approveArea.ApproveCode == "31"; // "31" : PASS | "32" : FAIL
        }
    }
}
