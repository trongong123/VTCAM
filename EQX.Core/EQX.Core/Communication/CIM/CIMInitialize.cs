// 2026-03-04 CursorAI에 의한 코드 생성: 4.1 Initialize Sub 함수(AliveCheck, DateTimeSync). 외부 호출·시나리오 조합·테스트용.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace TOPENG_Device
{
    /// <summary>4.1 Initialize 시나리오 Sub 함수. Initialize.AliveCheck, Initialize.DateTimeSync.</summary>
    public static class CIMInitialize
    {
        private const int AliveOnDurationMs = 100;  // 명세: 0.1초 이내 On 유지
        private const int CommandTimeoutMs = 3000;

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear, wMonth, wDayOfWeek, wDay, wHour, wMinute, wSecond, wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);

        /// <summary>Initialize.AliveCheck — Step 1: Local Alive Bit 1회 On(0.1초) 후 Off. Step 2: Master Alive Bit On 확인.</summary>
        /// <returns>Master Alive Bit On 확인 시 true, 타임아웃 시 false</returns>
        public static bool AliveCheck()
        {
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.AliveBit);
            Thread.Sleep(AliveOnDurationMs);
            CIMEventHandler.SetLocalRequestBitOff(EquipEvent.AliveBit);
            return CIMCommandHandler.WaitForCIMBitOn(CIMCommand.AliveBit, CommandTimeoutMs);
        }

        // 260304, TEST 수정
        /// <summary>Initialize.DateTimeSync — Step 3: Master Datetime Bit Set On 확인. Step 4: D000 7 Word 읽어 PC 시간 적용. Step 5: Datetime Reply Bit On.</summary>
        /// <returns>Step 3~5 성공 시 true</returns>
        public static bool DateTimeSync(bool checkbit=true)
        {
            if (checkbit)
            {
                if (!CIMCommandHandler.WaitForCIMBitOn(CIMCommand.DatetimeSet, CommandTimeoutMs))
                    return false;
            }
            int startWord = CIMAddressMap.GetReadWordIndexFromDAddress("D000");
            const int len = 7;
            var buf = new short[len];
            CIMAddressMap.ReadWords(startWord, len, buf);
            DateTime? dt = ParseDatetimeFromD000Words(buf);
            if (dt.HasValue)
                TrySetPcLocalTime(dt.Value);
            CIMCommandHandler.SetLocalPLCBitOn(CIMCommand.DatetimeSet);
            CIMScenarioDispatcher.AddReplyBitItem(new CIMReplyBit(CIMCommand.DatetimeSet, DateTime.Now));
            return true;
        }

        private static DateTime? ParseDatetimeFromD000Words(short[] words)
        {
            if (words == null || words.Length < 7) return null;
            var sb = new System.Text.StringBuilder(14);
            for (int i = 0; i < 7; i++)
            {
                int w = words[i] & 0xFFFF;
                sb.Append((char)(w & 0xFF));
                sb.Append((char)((w >> 8) & 0xFF));
            }
            string s = sb.ToString();
            if (s.Length >= 14 && DateTime.TryParseExact(s.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                return dt;
            if (s.Length >= 12 && DateTime.TryParseExact(s.Substring(0, 12), "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt2))
                return dt2;
            return null;
        }

        private static bool TrySetPcLocalTime(DateTime dt)
        {
            var st = new SYSTEMTIME
            {
                wYear = (ushort)dt.Year,
                wMonth = (ushort)dt.Month,
                wDayOfWeek = (ushort)dt.DayOfWeek,
                wDay = (ushort)dt.Day,
                wHour = (ushort)dt.Hour,
                wMinute = (ushort)dt.Minute,
                wSecond = (ushort)dt.Second,
                wMilliseconds = (ushort)dt.Millisecond
            };
            return SetLocalTime(ref st);
        }
    }
}
