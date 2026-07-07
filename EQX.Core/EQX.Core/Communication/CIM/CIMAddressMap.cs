// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// 2026-03-03 CursorAI에 의한 주석 보강: WORD 주소는 D가 실제 주소 일부이므로 제거 금지, BIT만 B 표현자
// 참고: doc/CIM/SDC CIM 구현 컨셉.md (Bit read/write, Word read/write 변수)
// B/D 논리 주소(명세서 16진) ↔ MPLC.PLCReadValue_B/W, PLCWriteValue_B/W 배열 인덱스 변환

using log4net;
using System;
using System.Net;

namespace TOPENG_Device
{
    /// <summary>
    /// CIM 명세서 B/D 주소(16진)를 MPLC 배열 인덱스로 변환.
    /// Command: CIM Address = Master Bit(Read), PLC Address = Local Reply Bit(Write).
    /// Event: PLC Address = Local Bit(Write), CIM Address = Master Reply Bit(Read).
    /// </summary>
    public static class CIMAddressMap
    {
        /// <summary>Master(CIM) Bit 영역 Read 시 사용. B1001 → index 1 등. (B1000 기준 오프셋)</summary>
        private const int CIM_READ_B_BASE = 0x1000;
        /// <summary>Local(PLC) Bit 영역 Write 시 사용. B0001 → index 1.</summary>
        private const int PLC_WRITE_B_BASE = 0x0000;
        /// <summary>Word 영역 D 주소 기준. 통신 스펙에 따라 조정.</summary>
        private const int WORD_BASE = 0;
        private const int D_WORD_BASE = 0xD000;

        private static object _lock = new object();

        /// <summary>
        /// B 주소 문자열 파싱 (16진). 예: "B1001" → 0x1001, "B0001" → 0x0001
        /// </summary>
        public static int ParseBAddress(string bAddress)
        {
            if (string.IsNullOrEmpty(bAddress)) return 0;
            string s = bAddress.TrimStart('B', 'b').Trim();
            try
            {
                return Convert.ToInt32(s, 16);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Word 주소에서 16진 오프셋만 추출. WORD 주소는 표현자 없이 첫 글자 D가 실제 주소의 일부이므로, 호출부에서는 D를 제거하지 말고 "D011", "DCAD4" 형태로 유지할 것.
        /// 내부 인덱스 계산용으로만 D 다음 부분을 16진으로 파싱. 예: "D000" → 0, "D011" → 0x11, "DCAD4" → 0xCAD4
        /// </summary>
        public static int ParseWordAddress(string dAddress)
        {
            if (string.IsNullOrEmpty(dAddress)) return 0;
            // 첫 글자 D는 주소의 일부. 인덱스 계산을 위해 D 다음 16진 부분만 추출(주소 문자열 자체는 호출부에서 D 포함 유지)
            string s = dAddress.TrimStart('D', 'd', 'W', 'w').Trim();
            try
            {
                return Convert.ToInt32(s, 16);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Master(CIM) Bit 주소 → PLCReadValue_B[] 인덱스.
        /// B1001 등 CIM Address는 Read 영역에서 (addr - CIM_READ_B_BASE) 로 매핑.
        /// </summary>
        public static int GetReadBitIndexForCIMAddress(int bAddressHex)
        {
            int index = bAddressHex - CIM_READ_B_BASE;
            if (index < 0) index = bAddressHex;
            int maxLen = MPLC.PLCReadBaseAddressLength_B;
            if (index >= maxLen) index = index % maxLen;
            return index;
        }

        public static int GetReadBitIndexForCIMAddress(string bAddress)
        {
            return GetReadBitIndexForCIMAddress(ParseBAddress(bAddress));
        }

        /// <summary>
        /// Local(PLC) Bit 주소 → PLCWriteValue_B[] 인덱스.
        /// B0001 등 PLC Address는 Write 영역에서 (addr - PLC_WRITE_B_BASE) 로 매핑.
        /// </summary>
        public static int GetWriteBitIndexForPLCAddress(int bAddressHex)
        {
            int index = bAddressHex - PLC_WRITE_B_BASE;
            if (index < 0) index = 0;
            int maxLen = MPLC.PLCWriteBaseAddressLength_B;
            if (index >= maxLen) index = maxLen - 1;
            return index;
        }

        public static int GetWriteBitIndexForPLCAddress(string bAddress)
        {
            return GetWriteBitIndexForPLCAddress(ParseBAddress(bAddress));
        }

        /// <summary>
        /// Local(PLC) Bit 주소 → PLCReadValue_B[] 인덱스 (Event 시 Master가 쓴 Reply를 읽는 경우 등).
        /// </summary>
        public static int GetReadBitIndexForPLCAddress(int bAddressHex)
        {
            int index = bAddressHex - PLC_WRITE_B_BASE;
            if (index < 0) index = 0;
            int maxLen = MPLC.PLCReadBaseAddressLength_B;
            if (index >= maxLen) index = maxLen - 1;
            return index;
        }

        /// <summary>
        /// Master(CIM) Bit 주소 → PLCWriteValue_B[] 인덱스 (Local이 CIM 쪽에 쓸 때, 필요 시).
        /// </summary>
        public static int GetWriteBitIndexForCIMAddress(int bAddressHex)
        {
            int index = bAddressHex - CIM_READ_B_BASE;
            if (index < 0) index = bAddressHex;
            int maxLen = MPLC.PLCWriteBaseAddressLength_B;
            if (index >= maxLen) index = index % maxLen;
            return index;
        }

        /// <summary>
        /// Word 주소(Dxxxx) → PLCReadValue_W[] 인덱스.
        /// </summary>
        public static int GetReadWordIndex(int wordAddressHex)
        {
            int index = wordAddressHex - D_WORD_BASE;
            if (index < 0) index = 0;
            int maxLen = MPLC.PLCReadBaseAddressLength_W;
            if (index >= maxLen) index = maxLen - 1;
            return index;
        }

        public static int GetWordIndexFromAddress(string dAddress)
        {
            return GetReadWordIndex(Convert.ToInt32(dAddress, 16));
        }

        /// <summary>
        /// DO NOT USE
        /// </summary>
        /// <param name="dAddress"></param>
        /// <returns></returns>
        public static int GetReadWordIndexFromDAddress(string dAddress)
        {
            return GetReadWordIndex(ParseWordAddress(dAddress));
        }

        /// <summary>
        /// Word 주소(Dxxxx) → PLCWriteValue_W[] 인덱스.
        /// </summary>
        public static int GetWriteWordIndex(int wordAddressHex)
        {
            int index = wordAddressHex - WORD_BASE;
            if (index < 0) index = 0;
            int maxLen = MPLC.PLCWriteBaseAddressLength_W;
            if (index >= maxLen) index = maxLen - 1;
            return index;
        }

        public static int GetWriteWordIndexFromDAddress(string dAddress)
        {
            return GetWriteWordIndex(ParseWordAddress(dAddress));
        }

        /// <summary>
        /// Bit 값 읽기 (Read B 영역). CIM Address용.
        /// </summary>
        public static short ReadCIMBit(string bAddress)
        {
            int idx = GetReadBitIndexForCIMAddress(bAddress);
            if (idx < 0 || idx >= MPLC.PLCReadValue_B.Length)
                return 0;
            return MPLC.PLCReadValue_B[idx];
        }

        /// <summary>
        /// Bit 값 쓰기 (Write B 영역). PLC Address용.
        /// </summary>
        public static void WritePLCBit(string bAddress, short value)
        {
            int idx = GetWriteBitIndexForPLCAddress(bAddress);
            if (idx < 0 || idx >= MPLC.PLCWriteValue_B.Length)
                return;
            if (bAddress != "B0000") LogManager.GetLogger("CIM").Info($"TURN {(value == 1 ? "ON" : "OFF")} B[{bAddress}]");
            MPLC.PLCWriteValue_B[idx] = value;
        }

        // 2026-02-25 CursorAI에 의한 코드 생성 - CIM Test UI에서 Master Bit 시뮬레이션용. PLCReadValue_B(CIM 주소)에 씀.
        /// <summary>
        /// Master(CIM) Bit 시뮬레이션. 테스트 시 Master 요청을 흉내 내기 위해 PLCReadValue_B의 CIM 주소 위치에 값을 쓴다.
        /// </summary>
        public static void WriteCIMBitForTest(string bAddress, short value)
        {
            int idx = GetReadBitIndexForCIMAddress(bAddress);
            if (idx < 0 || idx >= MPLC.PLCReadValue_B.Length)
                return;
            MPLC.PLCReadValue_B[idx] = value;
        }

        /// <summary>
        /// Bit 값 읽기 (Read B 영역). PLC Address용.
        /// </summary>
        public static short ReadPLCBit(string bAddress)
        {
            int idx = GetReadBitIndexForPLCAddress(ParseBAddress(bAddress));
            if (idx < 0 || idx >= MPLC.PLCReadValue_B.Length)
                return 0;
            return MPLC.PLCReadValue_B[idx];
        }

        /// <summary>
        /// Word 영역에서 시작 인덱스부터 length(Word 단위)만큼 읽기. PLCReadValue_W 사용.
        /// </summary>
        public static void ReadWords(int startWordIndex, int lengthWords, short[] destination)
        {
            if (destination == null || lengthWords <= 0 || startWordIndex < 0) return;
            var src = MPLC.PLCReadValue_W;
            int maxLen = src.Length;
            for (int i = 0; i < lengthWords && (startWordIndex + i) < maxLen; i++)
                destination[i] = src[startWordIndex + i];
            LogManager.GetLogger("CIM").Info($"READ from W[{startWordIndex:X4}] | {lengthWords} words | {destination[0]}");
        }

        /// <summary>
        /// Word 영역에서 시작 인덱스부터 length(Word 단위)만큼 쓰기. PLCWriteValue_W 사용.
        /// </summary>
        public static void WriteWords(int startWordIndex, int lengthWords, short[] source, bool isLog = true)
        {
            lock (_lock)
            {
                if (isLog) LogManager.GetLogger("CIM").Info($"WRITE to W[{startWordIndex:X4}] | {lengthWords} words | {source[0]}");
                if (source == null || lengthWords <= 0 || startWordIndex < 0) return;
                var dst = MPLC.PLCWriteValue_W;
                int maxLen = dst.Length;
                for (int i = 0; i < lengthWords && (startWordIndex + i) < maxLen && i < source.Length; i++)
                    dst[startWordIndex + i] = source[i];
            }
        }

        // 2026-02-24 CursorAI에 의한 코드 생성 - CIM 시나리오 테스트 UI용 단일 Word 읽기/쓰기
        /// <summary>
        /// 단일 Word 읽기. D 주소 문자열로 지정. PLCReadValue_W 사용.
        /// </summary>
        public static short ReadWordSingle(string dAddress)
        {
            int idx = GetReadWordIndexFromDAddress(dAddress);
            if (idx < 0 || idx >= MPLC.PLCReadValue_W.Length)
                return 0;
            return MPLC.PLCReadValue_W[idx];
        }

        /// <summary>
        /// 단일 Word 쓰기. D 주소 문자열로 지정. PLCWriteValue_W 사용.
        /// </summary>
        public static void WriteWordSingle(string dAddress, short value)
        {
            int idx = GetWriteWordIndexFromDAddress(dAddress);
            if (idx < 0 || idx >= MPLC.PLCWriteValue_W.Length)
                return;
            MPLC.PLCWriteValue_W[idx] = value;
        }
    }
}
