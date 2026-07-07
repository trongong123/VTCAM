using EQX.Core.Communication.CIM.Custom.WordArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TOPENG_Device;

namespace EQX.Core.Communication.CIM.Custom
{
    public class CIMCommandHelpers
    {
        // After received FormattedProcessProgramSend BIT ON
        public static FormattedProcessProgramSendArea PPIDDownload()
        {
            return CIMCommandHelpers.PPIDDownloadRun();
        }
        
        // After received FormattedProcessProgramRequest BIT ON
        public static FormattedProcessProgramRequestArea GetParameterRequestInfor()
        {
            FormattedProcessProgramRequestArea fpprArea = new FormattedProcessProgramRequestArea();
            var fpprCommand = CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramRequest);
            fpprCommand.ReadCIMWords();

            fpprArea.FromCIMData(fpprCommand.FromCIMDataBuffer);

            return fpprArea;
        }

        public static FormattedProcessProgramSendArea PPIDDownloadRun()
        {
            var ppidDownloadCommand = CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramSend2);
            ppidDownloadCommand.ReadCIMWords();

            FormattedProcessProgramSendArea fppsArea = new FormattedProcessProgramSendArea();
            fppsArea.FromCIMData(ppidDownloadCommand.FromCIMDataBuffer);

            EquipEventDetail.Create(EquipEvent.ParameterTotal).Write(fppsArea.RmsParameterList);

            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramSend2).SetPLCBitOn();
            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramSend2).WaitForCIMBitOff();
            CIMCommandDetail.Create(CIMCommand.FormattedProcessProgramSend2).SetPLCBitOff();

            // Check 구문
            //CIMScenarioDispatcher.PPIDControlAck
            return fppsArea;
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
    }
}