// 2026-03-03 CursorAI에 의한 코드 생성: CIM 설비 상태 보고(EQPStatus) 데이터 클래스를 PLC 폴더로 분리
// 참고: PLC2CIM.csv EQPStatus 블록(D0, 55 Word). CIM 관련 구조체/클래스는 PLC 폴더에 둔다(.cursor/rules/cim-plc-types.mdc).

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text;

namespace TOPENG_Device
{
    /// <summary>CIM 설비 상태 보고(EQPStatus) 데이터. PLC2CIM.csv EQPStatus 블록(D0, 55 Word) 항목.</summary>
    public class CIMEquipStatusData
    {
        /// <summary>EQPID, D0, 20 Word (40자 ASCII)</summary>
        public string EQPID { get; set; } = string.Empty;
        /// <summary>EQPPPID, D14, 20 Word (40자 ASCII)</summary>
        public string EQPPPID { get; set; } = string.Empty;
        /// <summary>EQPControl, D28, 1 Word (ASCII)</summary>
        public string EQPControl { get; set; } = "0";
        /// <summary>EQPAvailability, D2C, 1 Word (1: Down, 2: Up 등)</summary>
        public string EQPAvailability { get; set; } = "2";
        /// <summary>EQPInterlock, D2D, 1 Word (1: On, 2: Off)</summary>
        public string EQPInterlock { get; set; } = "2";
        /// <summary>EQPMove, D2E, 1 Word (1: Pause, 2: Running)</summary>
        public string EQPMove { get; set; } = "2";
        /// <summary>EQPRun, D2F, 1 Word (1: Idle, 2: Run)</summary>
        public string EQPRun { get; set; } = "1";
        /// <summary>EQPFront, D30, 1 Word (ASCII)</summary>
        public string EQPFront { get; set; } = "0";
        /// <summary>EQPRear, D31, 1 Word (ASCII)</summary>
        public string EQPRear { get; set; } = "0";
        /// <summary>EQPState_PP, D32, 1 Word (ASCII)</summary>
        public string EQPState_PP { get; set; } = "0";
        /// <summary>EQPMCR1, D35, 1 Word (ASCII)</summary>
        public string EQPMCR1 { get; set; } = "0";
        /// <summary>EQPMCR2, D36, 1 Word (ASCII)</summary>
        public string EQPMCR2 { get; set; } = "0";

        /// <summary>PLC2CIM EQPStatus 블록 55 Word 버퍼로 직렬화. CIM 보고 시 사용.</summary>
        public short[] ToWordBuffer()
        {
            const int lengthWords = 55;
            var buf = new short[lengthWords];
            int wi = 0;
            WriteStringToWords(buf, ref wi, EQPID, 20);   // D0, 20 Word
            wi = 0x14; // D14 EQPPPID
            WriteStringToWords(buf, ref wi, EQPPPID, 20);
            wi = 0x28; // D28 EQPControl
            WriteStringToWords(buf, ref wi, EQPControl, 1);
            wi = 0x2C; // D2C~D2F: ASCII 1 Word each (CIMUnitUnit ReportEquipStatusOnce와 동일)
            WriteStringToWords(buf, ref wi, EQPAvailability, 1);
            WriteStringToWords(buf, ref wi, EQPInterlock, 1);
            WriteStringToWords(buf, ref wi, EQPMove, 1);
            WriteStringToWords(buf, ref wi, EQPRun, 1);
            WriteStringToWords(buf, ref wi, EQPFront, 1);
            WriteStringToWords(buf, ref wi, EQPRear, 1);
            WriteStringToWords(buf, ref wi, EQPState_PP, 1);
            wi = 0x35;
            WriteStringToWords(buf, ref wi, EQPMCR1, 1);
            WriteStringToWords(buf, ref wi, EQPMCR2, 1);
            return buf;
        }

        private static void WriteStringToWords(short[] buf, ref int wordIndex, string value, int maxWords)
        {
            if (buf == null || value == null || wordIndex < 0) return;
            var enc = Encoding.ASCII;
            byte[] bytes = enc.GetBytes(value ?? string.Empty);
            int maxChars = maxWords * 2;
            for (int w = 0; w < maxWords && (wordIndex + w) < buf.Length; w++)
            {
                int lo = (w * 2) < bytes.Length ? bytes[w * 2] : 0;
                int hi = (w * 2 + 1) < bytes.Length ? bytes[w * 2 + 1] : 0;
                buf[wordIndex + w] = (short)(lo | (hi << 8));
            }
            wordIndex += maxWords;
        }
    }

    // 2026-03-03 CursorAI에 의한 코드 생성: doc/CIM/CIM Online Check List.md §4 Alarm 구조체. DCAD4, 400 Word.
    /// <summary>CIM Alarm 보고 데이터. doc §4: Start Word DCAD4, Length 400. ALCD(1=Light/Warning, 2=Heavy), ALID(0~9999), Alarm Description. MoldInspectorMachineData.dicErrorCode(ErrorData)와 연결 시 ErrorData.Type→ALCD, Id→ALID, Message[0]→Alarm Description.</summary>
    public class CIMAlarmData
    {
        /// <summary>ALCD: 1=Light Alarm(Warning), 2=Heavy Alarm. ErrorData.Type에 대응.</summary>
        public short ALCD { get; set; }
        /// <summary>ALID: 알람 번호 0~9999. ErrorData.Id에 대응.</summary>
        public int ALID { get; set; }
        /// <summary>Alarm Description. ErrorData.Message[0]에 대응.</summary>
        public string AlarmDescription { get; set; } = string.Empty;

        /// <summary>400 Word 버퍼 생성. Word 0=ALCD, Word 1=ALID, Word 2~399=AlarmDescription(ASCII, 최대 796자).</summary>
        public short[] ToWordBuffer(int startIndex)
        {
            const int lengthWords = 400;
            var buf = new short[lengthWords];

            // 전체 리드
            CIMAddressMap.ReadWords(startIndex, lengthWords, buf);

            int wordAddr = ALID / 16;
            int bitIndex = ALID % 16;

            if (wordAddr >= 0 && wordAddr < lengthWords)
            {
                buf[wordAddr] |= (short)(1 << bitIndex);
            }

            return buf;
        }

        private static void WriteStringToWords(short[] buf, ref int wordIndex, string value, int maxWords)
        {
            if (buf == null || wordIndex < 0) return;
            var enc = Encoding.ASCII;
            byte[] bytes = enc.GetBytes(value ?? string.Empty);
            for (int w = 0; w < maxWords && (wordIndex + w) < buf.Length; w++)
            {
                int lo = (w * 2) < bytes.Length ? bytes[w * 2] : 0;
                int hi = (w * 2 + 1) < bytes.Length ? bytes[w * 2 + 1] : 0;
                buf[wordIndex + w] = (short)(lo | (hi << 8));
            }
            wordIndex += maxWords;
        }
    }

    // 2026-03-03 CursorAI에 의한 코드 생성: doc/CIM/CIM Online Check List.md §5 MATERIAL 구조체.
    /// <summary>CIM Material 보고 데이터. doc §5: MATERIALID, MATERIALTYPE, MATERIALST(MOUNT), MATERIALPORTID, MATERIALUSAGE. Material Change / Material Location Update 시 D7B14 121 Word 등에 사용.</summary>
    public class CIMMaterialData
    {
        /// <summary>자재 ID</summary>
        public string MaterialId { get; set; } = string.Empty;
        /// <summary>자재 종류</summary>
        public string MaterialType { get; set; } = string.Empty;
        /// <summary>자재 상태(예: MOUNT)</summary>
        public string MaterialSt { get; set; } = "MOUNT";
        /// <summary>자재 장착 위치</summary>
        public string MaterialPortId { get; set; } = string.Empty;
        /// <summary>자재 수량(문자열 또는 숫자 표현)</summary>
        public string MaterialUsage { get; set; } = string.Empty;

        /// <summary>지정 Word 길이로 버퍼 생성. MATERIALID·TYPE·ST·PORTID·USAGE 순서로 ASCII 패킹(필드별 고정 길이).</summary>
        public short[] ToWordBuffer(int lengthWords)
        {
            var buf = new short[lengthWords];
            int wi = 0;
            int idWords = Math.Min(20, lengthWords - wi);     // 40자
            WriteStringToWords(buf, ref wi, MaterialId, idWords);
            int typeWords = Math.Min(10, lengthWords - wi);  // 20자
            WriteStringToWords(buf, ref wi, MaterialType, typeWords);
            int stWords = Math.Min(5, lengthWords - wi);     // 10자
            WriteStringToWords(buf, ref wi, MaterialSt, stWords);
            int portWords = Math.Min(10, lengthWords - wi);  // 20자
            WriteStringToWords(buf, ref wi, MaterialPortId, portWords);
            int usageWords = Math.Max(0, lengthWords - wi);  // 나머지
            WriteStringToWords(buf, ref wi, MaterialUsage, usageWords);
            return buf;
        }

        private static void WriteStringToWords(short[] buf, ref int wordIndex, string value, int maxWords)
        {
            if (buf == null || wordIndex < 0 || maxWords <= 0) return;
            var enc = Encoding.ASCII;
            byte[] bytes = enc.GetBytes(value ?? string.Empty);
            for (int w = 0; w < maxWords && (wordIndex + w) < buf.Length; w++)
            {
                int lo = (w * 2) < bytes.Length ? bytes[w * 2] : 0;
                int hi = (w * 2 + 1) < bytes.Length ? bytes[w * 2 + 1] : 0;
                buf[wordIndex + w] = (short)(lo | (hi << 8));
            }
            wordIndex += maxWords;
        }
    }

    public class CIMCellTrackInData
    {
        public string CellID { get; set; } = string.Empty;
        public string ProductID { get; set; } = string.Empty;
        public string StepID { get; set; } = string.Empty;
        public string ProcessJobID { get; set; } = string.Empty;
        public int PlanQuantity { get; set; } = 0;
        public int ProcessQuantity { get; set; } = 0;
        public string ReaderID { get; set; } = string.Empty;
        public string RRC { get; set; } = string.Empty;
        public string ReasonCode { get; set; } = string.Empty;

    }

    public class CIM_MCR_Read_Data
    {
        public string ReadID { get; set; } = string.Empty;
        public string ReadERCode { get; set; } = string.Empty;
        public string ReadERID { get; set; } = string.Empty;
    }

    public class CIMCellInfoDownloadData
    {
        public string CellID { get; set; } = string.Empty;
        public string ProductID { get; set; } = string.Empty;
        /// <summary>
        /// 0, 42 가 아닐시 ng 보고
        /// </summary>
        public string CellInfoResut { get; set; } = string.Empty;
    }

    public class CIMRecipeData
    {
        // TEST
        public List<CIM_RecipeList_Item> RecipeList = new List<CIM_RecipeList_Item>();
        public string IsRecipeName = "";

        public void ClearRecipeList() => RecipeList.Clear();
        public void AddRecipeList(int no, string name) => RecipeList.Add(new CIM_RecipeList_Item(no, name));
        /// <summary>
        /// filename : 번호_name
        /// </summary>
        /// <param name="no"></param>
        /// <param name="filename"></param>
        public void RemoveRecipeList(int no, string filename)
        {
            bool excute = false;
            string name = "";

            if (filename.Contains('_'))
            {
                string[] arr = filename.Split('_');
                name = arr[1];

                if(arr.Length >= 2) 
                { 
                    for(int i=2; i<arr.Length; i++)
                    {
                        name += "_";
                        name += arr[i];
                    }
                }
            }
            else name = filename;

            if (no != 0) { 
                if(RecipeList.FindIndex(x => x.No == no) != -1)
                    RecipeList.RemoveAt(RecipeList.FindIndex(x => x.No == no));
                excute = true;
            }
            if (name != "" && !excute) {
                if (RecipeList.FindIndex(x => x.Name == name) != -1) {
                    RecipeList.RemoveAt(RecipeList.FindIndex(x => x.Name == name)); }
                else RecipeList.RemoveAt(RecipeList.FindIndex(x => x.Name == name));
            }
        }
        public bool CheckExistRecipe(int no, string name)
        {
            bool ret = false;

            if (CheckExitRecipeNo(no)) ret = true;
            if (CheckExitRecipeName(name)) ret = true;

            return ret;
        }
        public bool CheckExitRecipeNo(int no) => RecipeList.Exists(x => x.No == no);
        public bool CheckExitRecipeName(string name) => RecipeList.Exists(x => x.Name == name);
    }

    public class CIMReplyBit
    {
        public CIMCommand Command;
        public DateTime Time;

        public CIMReplyBit(CIMReplyBit item)
        {
            this.Command = item.Command;
            this.Time = item.Time;
        }

        public CIMReplyBit(CIMCommand cmd, DateTime time)
        {
            this.Command = cmd;
            this.Time = time;
        }
    }
    
    public class CIM_Report_Item
    {
        public string Name;
        public string Value;
    }

    public class CIM_RecipeList_Item
    {
        public int No;
        public string Name;

        public CIM_RecipeList_Item(CIM_RecipeList_Item item)
        {
            this.No = item.No;
            this.Name = item.Name;
        }
        public CIM_RecipeList_Item(int no, string name)
        {
            this.No = no;
            this.Name = name;
        }
    }

    
}
