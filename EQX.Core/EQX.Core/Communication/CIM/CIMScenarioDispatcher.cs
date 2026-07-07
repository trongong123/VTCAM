// 2026-03-02 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// 시나리오 Enum 기반 switch-case, 47개 조건 판별 함수 기본형, 시나리오별 동작(구현/TODO)
// 2026-02-24 CursorAI - CIM명령어_명세서 §4 시나리오별 시작 조건 반영
// 2026-02-25 CursorAI - Word Write 임시값으로 실제 동작 코드 구현, 테스트 후 실제값 연결할 곳 주석 표시
// 2026-02-25 CursorAI - 4.1 Initialize: D000 Datetime 읽어 PC 시스템 시간 설정 기능 추가
// 2026-02-25 CursorAI - 4.2~4.5, 4.11, 4.14, 4.18 보류/4.19 유지, 4.26, 4.29 시나리오 보완; CIMScenarioContext 도입
// 2026-02-25 CursorAI에 의한 PLC2CIM.csv 반영 수정: 4.6/4.7 Alarm Word 주소 DCAD4, 400워드 적용
// 2026-03-01 CursorAI에 의한 보완: CIM Word 보고용 dummy 데이터 지정 및 전달 유무 제어
// 2026-03-02 CursorAI에 의한 코드 생성: 설비 상태 설정 ApplyEquipStatusReportValues / ApplyEquipReportState (CIMScenarioTestView와 동일 규칙)

using System.Globalization;
using System.Runtime.InteropServices;

namespace TOPENG_Device
{
    /// <summary>
    /// 47개 시나리오 분기 및 조건 판별. switch(CIMScenario) + IsScenario_XXX() 기본형.
    /// </summary>
    public static class CIMScenarioDispatcher
    {
        private static List<CIMReplyBit> ListReplyBit = new List<CIMReplyBit>();

        /// <summary>
        /// 2026-02-28 CursorAI에 의한 코드 생성: 시나리오 실행 완료(성공/실패) 시 발생. 구독하여 완료 확인·로깅·PLC 갱신 등에 사용.
        /// </summary>
        public static event Action<ScenarioCompletedArgs> ScenarioCompleted;

        /// <summary>
        /// 2026-03-03 CursorAI에 의한 코드 생성: Operator Call, Terminal Display 등에서 UI 메시지 표시를 요청할 때 발생.
        /// EquipApplication 등 UI 계층에서 구독하여 FormMessageBox를 띄운다.
        /// </summary>
        public static event Action<CIMMessageDisplayEventArgs> CIMMessageDisplayRequested;

        /// <summary>2026-03-04 CursorAI에 의한 코드 생성: Step 클래스 등 외부에서 메시지 표시 이벤트를 발생시킬 때 호출.</summary>
        internal static void RaiseCIMMessageDisplayRequested(CIMMessageDisplayEventArgs args) => CIMMessageDisplayRequested?.Invoke(args);

        public static event Action<CIMRecipeControlEventArgs> CIMRecipeControlRequested;
        internal static void RaiseCIMRecipeControlRequested(CIMRecipeControlEventArgs args) => CIMRecipeControlRequested.Invoke(args);

        /// <summary>Word 영역 임시 버퍼를 지정 값으로 채움. 시나리오 Step 클래스에서 사용. 테스트 후 실제값 연결할 것인 경우 호출 전에 버퍼를 실제 데이터로 채울 것.</summary>
        internal static void FillWordsTestValue(ref short[] buf, short value = 0)
        {
            if (buf == null) return;
            for (int i = 0; i < buf.Length; i++)
                buf[i] = value;
        }

        /// <summary>2026-03-01 CursorAI에 의한 코드 생성: true이면 CIM Word 보고 시 버퍼를 DummyWordValue로 채워 전달 유무만 검증.</summary>
        public static bool UseDummyWordDataForCIM { get; set; }

        /// <summary>2026-03-01 CursorAI에 의한 코드 생성: UseDummyWordDataForCIM일 때 Word 버퍼를 채울 dummy 값(기본 0x20).</summary>
        public static short DummyWordValue { get; set; } = 0x20;
        /// <summary> 상위에서 지령주는 것에 대한 응답코드 
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
        public static string PPIDControlAck { get; set; } = "0";

        /// <summary>2026-03-01 CursorAI에 의한 코드 생성: CIM 보고용 버퍼를 DummyWordValue로 채움. 시나리오 Step 클래스에서 사용.</summary>
        internal static void FillWordsWithDummyData(short[] buf)
        {
            if (buf == null) return;
            for (int i = 0; i < buf.Length; i++)
                buf[i] = DummyWordValue;
        }

        // 2026-02-25 CursorAI - PC 시스템 시간 설정용 P/Invoke (4.1 Initialize Datetime 적용)
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);

        /// <summary>D000 7워드(ASCII)를 날짜시간으로 파싱. 7워드 = 14바이트 ASCII (yyyyMMddHHmmss) 또는 7바이트 (yyMMddH 등).</summary>
        /// <returns>파싱 성공 시 DateTime, 실패 시 null</returns>
        private static DateTime? ParseDatetimeFromD000Words(short[] words)
        {
            if (words == null || words.Length < 7) return null;
            // 7 words: 각 word에서 low byte, high byte 순으로 ASCII (14 chars) → yyyyMMddHHmmss
            var sb = new System.Text.StringBuilder(14);
            for (int i = 0; i < 7; i++)
            {
                int w = words[i] & 0xFFFF;
                sb.Append((char)(w & 0xFF));
                sb.Append((char)((w >> 8) & 0xFF));
            }
            string s = sb.ToString();
            // yyyyMMddHHmmss (14자)
            if (s.Length >= 14)
            {
                if (DateTime.TryParseExact(s.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    return dt;
            }
            // yyMMddHHmmss (12자) 등 축약 형식 시도
            if (s.Length >= 12 && DateTime.TryParseExact(s.Substring(0, 12), "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt2))
                return dt2;
            return null;
        }

        /// <summary>PC 로컬 시간을 설정. 관리자 권한 필요.</summary>
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

        /// <summary>
        /// 2026-03-03 CursorAI에 의한 코드 생성: CIM Word 버퍼를 ASCII 문자열로 변환. 시나리오 Step 클래스에서 사용.
        /// 1 Word = 2바이트(LOW/HIGH) 순서로 해석하며, 널(0) 또는 비인쇄 문자는 무시하고 앞쪽 유효 문자만 사용한다.
        /// </summary>
        internal static string DecodeAsciiFromWords(short[] words, int startWordIndex, int wordLength)
        {
            if (words == null || words.Length == 0 || startWordIndex < 0 || wordLength <= 0) return string.Empty;
            if (startWordIndex + wordLength > words.Length) wordLength = words.Length - startWordIndex;

            var sb = new System.Text.StringBuilder(wordLength * 2);
            for (int i = 0; i < wordLength; i++)
            {
                int w = words[startWordIndex + i] & 0xFFFF;
                char cLow = (char)(w & 0xFF);
                char cHigh = (char)((w >> 8) & 0xFF);

                if (!char.IsControl(cLow) && cLow != '\0')
                    sb.Append(cLow);
                if (!char.IsControl(cHigh) && cHigh != '\0')
                    sb.Append(cHigh);
            }

            return sb.ToString().Trim();
        }
        /// <summary>
        /// 2026-03-04 CursorAI에 의한 코드 생성: ASCII 문자열을 Word 버퍼로 인코딩. DecodeAsciiFromWords의 역동작.
        /// 1 Word = 2바이트(LOW=첫 번째 문자, HIGH=두 번째 문자) 순서로 채움. 문자열이 짧으면 남는 바이트는 0으로 채움.
        /// </summary>
        /// <param name="ascii">인코딩할 ASCII 문자열. null이면 해당 구간을 0으로 채움.</param>
        /// <param name="words">쓸 Word 버퍼.</param>
        /// <param name="startWordIndex">쓰기 시작 Word 인덱스.</param>
        internal static short[]? EncodeAsciiToWords(string ascii)
        {
            int startWordIndex = 0;
            int wordLength = ascii.Length / 2;
            short[] words = new short[wordLength];

            if (startWordIndex + wordLength > words.Length) wordLength = words.Length - startWordIndex;

            int maxChars = wordLength * 2;
            for (int i = 0; i < wordLength; i++)
            {
                int idx = startWordIndex + i;
                int charIdx = i * 2;
                byte low = 0;
                byte high = 0;
                if (ascii != null && charIdx < ascii.Length)
                    low = (byte)(ascii[charIdx] & 0xFF);
                if (ascii != null && charIdx + 1 < ascii.Length)
                    high = (byte)(ascii[charIdx + 1] & 0xFF);
                words[idx] = (short)((high << 8) | low);
            }

            return words;
        }
        internal static void EncodeAsciiToWords(string ascii, ref short[] words, int startWordIndex, int wordLength)
        {
            if (words == null || startWordIndex < 0 || wordLength <= 0) return;
            if (startWordIndex + wordLength > words.Length) wordLength = words.Length - startWordIndex;

            int maxChars = wordLength * 2;
            for (int i = 0; i < wordLength; i++)
            {
                int idx = startWordIndex + i;
                int charIdx = i * 2;
                byte low = 0;
                byte high = 0;
                if (ascii != null && charIdx < ascii.Length)
                    low = (byte)(ascii[charIdx] & 0xFF);
                if (ascii != null && charIdx + 1 < ascii.Length)
                    high = (byte)(ascii[charIdx + 1] & 0xFF);
                words[idx] = (short)((high << 8) | low);
            }
        }
        internal static void EncodeInt32ToWords(int value, ref short[] words, int startWordIndex)
        {
            if (words == null || startWordIndex < 0 ) return;

            string data = Convert.ToString(value, 2).PadLeft(32, '0');
            string data1 = data.Substring(0, 16);
            string data2 = data.Substring(16, 16);

            words[startWordIndex] = (short)(value & 0xFFFF);;
            words[startWordIndex + 1] = (short)((value >> 16) & 0xFFFF);
        }

        /// <summary>
        /// 시나리오 실행. switch-case로 분기.
        /// 사용법: ExecuteScenario(CIMScenario.XXX) 또는 ExecuteScenario(CIMScenario.XXX, context).
        /// context 사용 시나리오: 4.2 TerminalDisplayMode(수신/송신), 4.4 InterlockReportStateRequested(설비 상태 보고), 4.11 CellInOutKind(CellIn/CellOut), 4.26 OperatorLogMode(LogIn/LogOut), 4.29 MaterialAlarmKind(Warning/Shortage). null이면 각 시나리오 기본 동작.
        /// 2026-02-28 CursorAI에 의한 코드 생성: 완료 시 ScenarioCompleted 이벤트 발생(성공/실패).
        /// </summary>
        public static void ExecuteScenario(CIMScenario scenario, CIMScenarioContext context = null)
        {
            try
            {
                switch (scenario)
                {
                    case CIMScenario.Initialize: RunScenario_Initialize(); break;
                    case CIMScenario.TerminalDisplay: RunScenario_TerminalDisplay(context); break;
                    case CIMScenario.OPCallOccurAndRelease: RunScenario_OPCallOccurAndRelease(context); break;
                    case CIMScenario.InterlockOccur: RunScenario_InterlockOccur(context); break;
                    case CIMScenario.InterlockRelease: RunScenario_InterlockRelease(context); break;
                    case CIMScenario.AlarmOccur: RunScenario_AlarmOccur(context); break;
                    case CIMScenario.AlarmRelease: RunScenario_AlarmRelease(context); break;
                    case CIMScenario.AlarmInquiry: RunScenario_AlarmInquiry(); break;
                    case CIMScenario.EquipStateChange: RunScenario_EquipStateChange(); break;
                    case CIMScenario.MaterialChange: RunScenario_MaterialChange(context); break;
                    case CIMScenario.CellInOut: RunScenario_CellInOut(context); break;
                    case CIMScenario.PPIDChange: RunScenario_PPIDChange(); break;
                    case CIMScenario.PPIDCreateDelete: RunScenario_PPIDCreateDelete(context); break;
                    case CIMScenario.PPIDDownload: RunScenario_PPIDDownload(); break;
                    case CIMScenario.ParameterChange: RunScenario_ParameterChange(context); break;
                    case CIMScenario.ParameterInquiry: RunScenario_ParameterInquiry(); break;
                    case CIMScenario.PPIDListInquiry: RunScenario_PPIDListInquiry(); break;
                    case CIMScenario.EquipConstantInquiry: RunScenario_EquipConstantInquiry(); break;
                    case CIMScenario.EquipConstantNameListInquiry: RunScenario_EquipConstantNameListInquiry(); break;
                    case CIMScenario.EquipPosition: RunScenario_EquipPosition(); break;
                    case CIMScenario.TPMLoss: RunScenario_TPMLoss(); break;
                    case CIMScenario.IQCPOLCellInOut: RunScenario_IQCPOLCellInOut(); break;
                    case CIMScenario.MaterialKittingCancel: RunScenario_MaterialKittingCancel(); break;
                    case CIMScenario.MaterialAssembleProcess: RunScenario_MaterialAssembleProcess(); break;
                    case CIMScenario.MaterialNGProcess: RunScenario_MaterialNGProcess(); break;
                    case CIMScenario.OperatorLogInOut: RunScenario_OperatorLogInOut(context); break;
                    case CIMScenario.MaterialSupplementRequest: RunScenario_MaterialSupplementRequest(); break;
                    case CIMScenario.MaterialSupplementComplete: RunScenario_MaterialSupplementComplete(); break;
                    case CIMScenario.MaterialAlarm: RunScenario_MaterialAlarm(context); break;
                    case CIMScenario.APNIDRequestProcess: RunScenario_APNIDRequestProcess(); break;
                    case CIMScenario.APNIDIssueConfirmProcess: RunScenario_APNIDIssueConfirmProcess(); break;
                    case CIMScenario.APNIDGrapeIDRequestProcess: RunScenario_APNIDGrapeIDRequestProcess(); break;
                    case CIMScenario.LoadRequest: RunScenario_LoadRequest(); break;
                    case CIMScenario.LoadComplete: RunScenario_LoadComplete(); break;
                    case CIMScenario.UnloadRequest: RunScenario_UnloadRequest(); break;
                    case CIMScenario.UnloadComplete: RunScenario_UnloadComplete(); break;
                    case CIMScenario.MaterialLocationUpdate: RunScenario_MaterialLocationUpdate(context); break;
                    case CIMScenario.CellTrackINValidationCheck: RunScenario_CellTrackINValidationCheck(); break;
                    case CIMScenario.CurrentProcessControlDataRequest: RunScenario_CurrentProcessControlDataRequest(); break;
                    case CIMScenario.ProcessControlRequestReport: RunScenario_ProcessControlRequestReport(); break;
                    case CIMScenario.ProcessControlResultReport: RunScenario_ProcessControlResultReport(); break;
                    case CIMScenario.UnitStatusChange: RunScenario_UnitStatusChange(); break;
                    case CIMScenario.CellLoaderPortLoadUnload: RunScenario_CellLoaderPortLoadUnload(); break;
                    case CIMScenario.CellUnloaderPortLoadUnload: RunScenario_CellUnloaderPortLoadUnload(); break;
                    case CIMScenario.JobStartCancel: RunScenario_JobStartCancel(); break;
                    case CIMScenario.ProcessJobEvent: RunScenario_ProcessJobEvent(); break;
                    case CIMScenario.PortStateChange: RunScenario_PortStateChange(); break;
                    case CIMScenario.EquipFunctionChangeCommand: RunScenario_EquipFunctionChangeCommand(context); break;
                    case CIMScenario.CellStartPort: RunScenario_CellStartPort(context); break;
                    case CIMScenario.CellIDReduingResult: RunScenario_CellIDReadingResult(context); break;
                    case CIMScenario.CellInformationRequest: RunScenario_CellInformationRequest(context); break;
                    default: break;
                }
                //ScenarioCompleted?.Invoke(new ScenarioCompletedArgs(scenario, true));
            }
            catch (Exception ex)
            {
                ScenarioCompleted?.Invoke(new ScenarioCompletedArgs(scenario, false, ex.Message, ex));
                throw;
            }
        }

        /// <summary>
        /// 2026-02-28 CursorAI에 의한 코드 생성: ExecuteScenario 비동기 실행. Task.Run으로 기존 동기 메서드 래핑.
        /// 사용법: await ExecuteScenarioAsync(CIMScenario.XXX, context) 또는 CancellationToken 전달.
        /// </summary>
        public static Task ExecuteScenarioAsync(CIMScenario scenario, CIMScenarioContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => ExecuteScenario(scenario, context), cancellationToken);
        }

        private static bool CheckExistReplyBit() => ListReplyBit.Count != 0 ? true : false;
        public static void AddReplyBitItem(CIMReplyBit item) => ListReplyBit.Add(new CIMReplyBit(item));
        public static void CheckTimeReplyBit(bool checktime = true)
        {
            if (!CheckExistReplyBit()) return;

            foreach (CIMReplyBit item in ListReplyBit)
            {
                if (checktime)
                {
                    // 일정시간 경과하면 끄기.
                    if (((TimeSpan)(DateTime.Now - item.Time)).TotalMilliseconds > 300)
                    {
                        CIMCommandHandler.SetLocalReplyBitOff(item.Command);
                        ListReplyBit.RemoveAt(ListReplyBit.FindIndex(x => x.Command == item.Command));
                    }
                }
                else
                {
                    // CIM 요청커맨드 off 되면 끄기
                    if (CIMCommandHandler.IsCIMBitOff(item.Command))
                    {
                        CIMCommandHandler.SetLocalReplyBitOff(item.Command);
                        ListReplyBit.RemoveAt(ListReplyBit.FindIndex(x => x.Command == item.Command));
                    }
                }
            }

        }
       
        // ----- 조건 판별 함수 47개 (시나리오 진입 조건) - CIM명령어_명세서 §4 시작 조건 반영 -----
        /// <summary>4.1 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_Initialize() { return true; }
        /// <summary>4.2 시작 조건: 수신=Master Terminal Display Bit On, 송신=Local 요청. (수신 시 진입 판별)</summary>
        public static bool IsScenario_TerminalDisplay() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.TerminalDisplay); }
        /// <summary>4.3 시작 조건: 발생=Master Operator Call Bit On, 해제=Local 요청. (발생 시 진입 판별)</summary>
        public static bool IsScenario_OPCallOccurAndRelease() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.OperatorCall); }
        /// <summary>4.4 시작 조건: Master에서 Interlock Bit가 On 될 때</summary>
        public static bool IsScenario_InterlockOccur() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.Interlock); }
        /// <summary>4.5 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_InterlockRelease() { return true; }
        /// <summary>4.6 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_AlarmOccur() { return true; }
        /// <summary>4.7 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_AlarmRelease() { return true; }
        /// <summary>4.8 시작 조건: 보류</summary>
        public static bool IsScenario_AlarmInquiry() { /* TODO: 보류 */ return false; }
        /// <summary>4.9 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_EquipStateChange() { return true; }
        /// <summary>4.10 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialChange() { return true; }
        /// <summary>4.11 시작 조건: Local 프로그램에서 요청을 받을 때(Cell In/Out 모두)</summary>
        public static bool IsScenario_CellInOut() { return true; }
        /// <summary>4.12 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_PPIDChange() { return true; }
        /// <summary>4.13 시작 조건: Local 프로그램에서 요청을 받을 때(PPID 생성/삭제 모두)</summary>
        public static bool IsScenario_PPIDCreateDelete() { return true; }
        /// <summary>4.14 시작 조건: Master의 Formatted Process Program Send Bit가 ON 될 때</summary>
        public static bool IsScenario_PPIDDownload() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.FormattedProcessProgramSend); }
        /// <summary>4.15 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_ParameterChange() { return true; }
        /// <summary>4.16 시작 조건: Master의 Process Program Request Bit가 On 될 때</summary>
        public static bool IsScenario_ParameterInquiry() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.FormattedProcessProgramRequest); }
        /// <summary>4.17 시작 조건: Master의 PPID List Request Bit가 On 될 때</summary>
        public static bool IsScenario_PPIDListInquiry() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.CurrentEquipPPIDListRequest); }
        /// <summary>4.18 시작 조건: 보류. 명세서는 Master의 Equipment Constant Request Bit 사용하나, 현재 CIMCommand에 해당 Bit 미정의. 4.19(Equipment Constant Name List)만 구현 유지.</summary>
        public static bool IsScenario_EquipConstantInquiry() { /* TODO: 보류 - Equipment Constant Request Bit 미정의 */ return false; }
        /// <summary>4.19 시작 조건: Master의 Equipment Constant Name List Request Bit가 On 될 때</summary>
        public static bool IsScenario_EquipConstantNameListInquiry() { return CIMCommandHandler.IsCIMBitOn(CIMCommand.EquipConstantNameList); }
        /// <summary>4.20 시작 조건: 미사용</summary>
        public static bool IsScenario_EquipPosition() { /* TODO: 미사용 */ return false; }
        /// <summary>4.21 시작 조건: Local 프로그램에서 요청 받을 때</summary>
        public static bool IsScenario_TPMLoss() { return true; }
        /// <summary>4.22 시작 조건: Local 요청. 보류</summary>
        public static bool IsScenario_IQCPOLCellInOut() { /* TODO: 보류 */ return false; }
        /// <summary>4.23 시작 조건: Local 요청(Material Kitting/Kitting Cancel). 보류</summary>
        public static bool IsScenario_MaterialKittingCancel() { /* TODO: 보류 */ return false; }
        /// <summary>4.24 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialAssembleProcess() { return true; }
        /// <summary>4.25 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialNGProcess() { return true; }
        /// <summary>4.26 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_OperatorLogInOut() { return true; }
        /// <summary>4.27 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialSupplementRequest() { return true; }
        /// <summary>4.28 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialSupplementComplete() { return true; }
        /// <summary>4.29 시작 조건: Local 프로그램에서 요청을 받을 때(부족 수량/수량 0 모두)</summary>
        public static bool IsScenario_MaterialAlarm() { return true; }
        /// <summary>4.30 시작 조건: Local 요청. 미사용</summary>
        public static bool IsScenario_APNIDRequestProcess() { /* TODO: 미사용 */ return false; }
        /// <summary>4.31 시작 조건: Local 요청. 미사용</summary>
        public static bool IsScenario_APNIDIssueConfirmProcess() { /* TODO: 미사용 */ return false; }
        /// <summary>4.32 시작 조건: Local 요청. 미사용</summary>
        public static bool IsScenario_APNIDGrapeIDRequestProcess() { /* TODO: 미사용 */ return false; }
        /// <summary>4.33 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_LoadRequest() { return true; }
        /// <summary>4.34 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_LoadComplete() { return true; }
        /// <summary>4.35 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_UnloadRequest() { return true; }
        /// <summary>4.36 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_UnloadComplete() { return true; }
        /// <summary>4.37 시작 조건: Local 프로그램에서 요청을 받을 때</summary>
        public static bool IsScenario_MaterialLocationUpdate() { return true; }
        /// <summary>4.38~4.42, 4.45~4.47 시작 조건: 보류</summary>
        public static bool IsScenario_CellTrackINValidationCheck() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_CurrentProcessControlDataRequest() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_ProcessControlRequestReport() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_ProcessControlResultReport() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_UnitStatusChange() { /* TODO: 보류 */ return false; }
        /// <summary>4.43, 4.44 시작 조건: 미사용</summary>
        public static bool IsScenario_CellLoaderPortLoadUnload() { /* TODO: 미사용 */ return false; }
        public static bool IsScenario_CellUnloaderPortLoadUnload() { /* TODO: 미사용 */ return false; }
        public static bool IsScenario_JobStartCancel() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_ProcessJobEvent() { /* TODO: 보류 */ return false; }
        public static bool IsScenario_PortStateChange() { /* TODO: 보류 */ return false; }
        /// <summary>16.7 Equipment Function Change</summary>
        public static bool IsScenario_EquipFunctionChangeCommand() {  return CIMCommandHandler.IsCIMBitOn(CIMCommand.EquipFunctionChangeCommand); }
        public static bool IsScenario_CellStartPort() { return true; }
        public static bool IsScenario_CellIDReadingResult() { return true; }
        public static bool IsScenario_CellInformationRequest() { return true; }


        // ----- 시나리오 동작 47개 (사용법은 각 /// summary 참고) -----
        /// <summary>4.1 Initialize. Sub 함수 조합: Initialize.AliveCheck → Initialize.DateTimeSync.</summary>
        private static void RunScenario_Initialize()
        {
            CIMInitialize.AliveCheck();
            CIMInitialize.DateTimeSync();
        }

        /// <summary>4.2 Terminal Display. CIMTerminalDisplay.Receive / Send 호출.</summary>
        private static void RunScenario_TerminalDisplay(CIMScenarioContext context)
        {
            // 260304, TEST 수정 
            bool doSend = context?.TerminalDisplayMode == TerminalDisplayMode.Send;
            //bool doReceive = context?.TerminalDisplayMode == TerminalDisplayMode.Receive;
            bool doReceive = !doSend;
            if (!doReceive && !doSend) { doReceive = CIMCommandHandler.IsCIMBitOn(CIMCommand.TerminalDisplay); doSend = !doReceive; }
            if (doReceive) { CIMTerminalDisplay.Receive(); return; }
            if (doSend) CIMTerminalDisplay.Send();
        }

        /// <summary>4.3 OP Call 발생/해제. CIMOPCall.Occur / Release 호출.</summary>
        private static void RunScenario_OPCallOccurAndRelease(CIMScenarioContext context)
        {
            // 260304, TEST 수정 
            bool doRelease = context?.OPCallOccurOnly == false;
            //bool doOccur = context?.OPCallOccurOnly == true;
            bool doOccur = !doRelease;
            if (!doOccur && !doRelease) doOccur = CIMCommandHandler.WaitForCIMBitOn(CIMCommand.OperatorCall, CIMCommandHandler.CommandResponseTimeoutMs);
            if (doOccur) { CIMOPCall.Occur(); return; }
            CIMOPCall.Release(context.OPCallID, context.OPCallMsg);
        }

        /// <summary>4.4 Interlock 발생. CIMInterlockOccur.ReadAndReply / ReportEquipState 호출.</summary>
        private static void RunScenario_InterlockOccur(CIMScenarioContext context)
        {
            // Read 
            // RCMD에 따라 동작진행.
            // 상태보고
            string RCMD = "";

            RCMD = CIMInterlockOccur.ReadAndReply();

            if(RCMD != "-1")
            {
                // RCMD별 동작처리시킬수 있는 구문 필요.
                // 완료 대기할 수 있는 구문 필요. 
                switch (RCMD)
                {
                    case "2": //Equipment Command -EQP Interlock Send 
                        break;
                    case "11": //Equipment Command(Unit Machine Control) - Transfer Stop)
                        break;
                    case "12": //Equipment Command(Unit Machine Control) - Loading Stop)
                        break;
                    case "13": //Equipment Command(Unit Machine Control) - Step Stop)
                        break;
                    case "14": //Equipment Command(Unit Machine Control) - OWN Stop)
                        break;
                }

                CIMInterlockOccur.ReportEquipState();
            }

            //if (context?.InterlockReportStateRequested == true) 
            //{
            //    CIMInterlockOccur.ReportEquipState(); return; 
            //}
        }

        /// <summary>4.5 Interlock 해제. CIMInterlockRelease.Run 호출.</summary>
        private static void RunScenario_InterlockRelease(CIMScenarioContext context)
        {
            CIMInterlockRelease.Run(context.InterlockID, context.InterlockMsg);
        }

        /// <summary>4.6 Alarm 발생. CIMAlarm.AlarmOccur 호출.</summary>
        private static void RunScenario_AlarmOccur(CIMScenarioContext context) { CIMAlarm.AlarmOccur(context); }

        /// <summary>4.7 Alarm 해제. CIMAlarm.AlarmRelease 호출.</summary>
        private static void RunScenario_AlarmRelease(CIMScenarioContext context) { CIMAlarm.AlarmRelease(context); }

        /// <summary>4.8 Alarm 조회. CIMAlarm.AlarmInquiry 호출.</summary>
        private static void RunScenario_AlarmInquiry() { CIMAlarm.AlarmInquiry(); }

        /// <summary>EQ 상태 보고: D2C~D2F에 ASCII 1 Word씩 기록. CIMUnitUnit ReportEquipStatusOnce(ToWordBuffer)와 동일하게 ASCII('1'=0x31, '2'=0x32)로 PLC 보고.</summary>
        private static void ApplyEquipStatusReportValues(short availabilityAscii, short interlockAscii, short moveStateAscii, short runStateAscii)
        {
            CIMAddressMap.WriteWordSingle("D2C", availabilityAscii);
            CIMAddressMap.WriteWordSingle("D2D", interlockAscii);
            CIMAddressMap.WriteWordSingle("D2E", moveStateAscii);
            CIMAddressMap.WriteWordSingle("D2F", runStateAscii);
        }

        /// <summary>ASCII 코드: '1'=0x31, '2'=0x32. 설비 상태 Word는 ASCII로 보고.</summary>
        private const short ASCII_1 = (short)'1';
        private const short ASCII_2 = (short)'2';

        /// <summary>설비 보고 상태 종류에 따라 D2C~D2F를 ASCII로 설정. IDLE/NORMAL, DOWN, INTERLOCK, RUNNING 등 시나리오에서 호출.</summary>
        public static void ApplyEquipReportState(EquipReportStateKind state)
        {
            switch (state)
            {
                case EquipReportStateKind.IdleNormal: ApplyEquipStatusReportValues(ASCII_2, ASCII_2, ASCII_2, ASCII_1); break;
                case EquipReportStateKind.Down:       ApplyEquipStatusReportValues(ASCII_1, ASCII_2, ASCII_1, ASCII_1); break;
                case EquipReportStateKind.Interlock:  ApplyEquipStatusReportValues(ASCII_2, ASCII_1, ASCII_1, ASCII_1); break;
                case EquipReportStateKind.Running:   ApplyEquipStatusReportValues(ASCII_2, ASCII_2, ASCII_2, ASCII_2); break;
                default: break;
            }
        }

        // 2026-03-02 CursorAI에 의한 코드 수정: PLC2CIM.csv EQPStatus(D0~) 블록 기준으로 설비 상태 보고
        // 2026-03-04 CursorAI에 의한 코드 보완: 설비 상태(state) 입력 시 D2C~D2F를 해당 상태에 맞게 보고
        /// <summary>설비 상태 보고 1회. 프로그램 시작 시 또는 상태 변경 시 호출. state 지정 시 D2C~D2F(EQPAvailability, EQPInterlock, EQPMove, EQPRun)를 해당 상태값으로 보고.</summary>
        /// <param name="buffer">EQPStatus 55 Word 버퍼. MoldInspectorMachineData.CIMEquipStatus.ToWordBuffer() 등에서 생성. null이면 UseDummyWordDataForCIM/테스트값 사용.</param>
        /// <param name="state">설비 보고 상태. 지정 시 IdleNormal/Down/Interlock/Running에 맞게 D2C~D2F ASCII 보고. null이면 버퍼 내용만 사용.</param>
        public static void ReportEquipStatusOnce(short[] buffer = null, EquipReportStateKind? state = null)
        {
            CIMStatusWordMap.GetStatusBlockRange(StatusBlockKind.EQPStatus, out string startD, out int lengthWords);
            if (string.IsNullOrEmpty(startD) || lengthWords <= 0) return;
            short[] buf;
            if (buffer != null && buffer.Length == lengthWords)
            {
                buf = buffer;
            }
            else
            {
                buf = new short[lengthWords];
                if (UseDummyWordDataForCIM) FillWordsWithDummyData(buf); else FillWordsTestValue(ref buf, 0);
            }
            int w = CIMAddressMap.GetWriteWordIndexFromDAddress(startD);
            CIMAddressMap.WriteWords(w, lengthWords, buf);
            if (state.HasValue)
                ApplyEquipReportState(state.Value);
        }

        /// <summary>4.9 Equip State Change. CIMEquipStateChange.Report 호출.</summary>
        private static void RunScenario_EquipStateChange() { CIMEquipStateChange.Report(); }

        /// <summary>4.10 Material Change. CIMMaterialChange.ReportPort 호출.</summary>
        private static void RunScenario_MaterialChange(CIMScenarioContext context) { CIMMaterialChange.ReportPort(context); }

        /// <summary>4.11 Cell IN/OUT. CIMCellInOut.CellIn / CellOut 호출.</summary>
        private static void RunScenario_CellInOut(CIMScenarioContext context)
        {
            if (context?.CellInOutKind == CellInOutKind.CellOut) { CIMCellInOut.CellOut(); return; }
            CIMCellInOut.CellIn();
        }

        /// <summary>4.12 PPID Change. CIMPPIDChange.Run 호출.</summary>
        private static void RunScenario_PPIDChange() { CIMPPIDChange.Run(); }

        /// <summary>4.13 PPID Create/Delete. CIMPPIDCreateDelete.Run 호출.</summary>
        private static void RunScenario_PPIDCreateDelete(CIMScenarioContext context) { CIMPPIDCreateDelete.Run(context); }

        /// <summary>4.14 PPID Download. CIMScenarioStepsPPIDParam.PPIDDownloadRun 호출.</summary>
        private static void RunScenario_PPIDDownload() { CIMScenarioStepsPPIDParam.PPIDDownloadRun(); }

        /// <summary>4.15 Parameter Change. CIMScenarioStepsPPIDParam.ParameterChangeRun 호출.</summary>
        private static void RunScenario_ParameterChange(CIMScenarioContext context) { CIMScenarioStepsPPIDParam.ParameterChangeRun(context); }

        /// <summary>4.16 Parameter 조회. CIMScenarioStepsPPIDParam.ParameterInquiryRun 호출.</summary>
        private static void RunScenario_ParameterInquiry() { CIMScenarioStepsPPIDParam.ParameterInquiryRun(); }

        /// <summary>4.17 PPID List 조회. CIMScenarioStepsPPIDParam.PPIDListInquiryRun 호출.</summary>
        private static void RunScenario_PPIDListInquiry() { CIMScenarioStepsPPIDParam.PPIDListInquiryRun(); }

        /// <summary>4.18 Equipment Constant 조회. CIMEquipConstantInquiry.Run 호출(보류 시 빈 구현).</summary>
        private static void RunScenario_EquipConstantInquiry() { CIMEquipConstantInquiry.Run(); }

        /// <summary>4.19 Equipment Constant Name List 조회. CIMScenarioStepsPPIDParam.EquipConstantNameListRun 호출.</summary>
        private static void RunScenario_EquipConstantNameListInquiry() { CIMScenarioStepsPPIDParam.EquipConstantNameListRun(); }

        /// <summary>4.20 Equipment Position. CIMEquipPosition.Run 호출(미사용 시 빈 구현).</summary>
        private static void RunScenario_EquipPosition() { CIMEquipPosition.Run(); }

        /// <summary>4.21 TPM Loss. CIMTPMLoss.Run 호출.</summary>
        private static void RunScenario_TPMLoss() { CIMTPMLoss.Run(); }

        /// <summary>4.22 IQC+POL Cell IN/OUT. CIMIQCPOLCellInOut.Run 호출(보류).</summary>
        private static void RunScenario_IQCPOLCellInOut() { CIMIQCPOLCellInOut.Run(); }

        /// <summary>4.23 Material Kitting Cancel. CIMMaterialKittingCancel.Run 호출(보류).</summary>
        private static void RunScenario_MaterialKittingCancel() { CIMMaterialKittingCancel.Run(); }

        /// <summary>4.24 Material Assemble Process. CIMMaterialAssembleProcess.Run 호출.</summary>
        private static void RunScenario_MaterialAssembleProcess() { CIMMaterialAssembleProcess.Run(); }

        /// <summary>4.25 Material NG Process. CIMMaterialNGProcess.Run 호출.</summary>
        private static void RunScenario_MaterialNGProcess() { CIMMaterialNGProcess.Run(); }

        /// <summary>4.26 Operator Log In/Out. CIMOperatorLogInOut.Run 호출.</summary>
        private static void RunScenario_OperatorLogInOut(CIMScenarioContext context) { CIMOperatorLogInOut.Run(context); }

        /// <summary>4.27 Material Supplement Request. CIMMaterialSupplementRequest.Run 호출.</summary>
        private static void RunScenario_MaterialSupplementRequest() { CIMMaterialSupplementRequest.Run(); }

        /// <summary>4.28 Material Supplement Complete. CIMMaterialSupplementComplete.Run 호출.</summary>
        private static void RunScenario_MaterialSupplementComplete() { CIMMaterialSupplementComplete.Run(); }

        /// <summary>4.29 Material Alarm. CIMMaterialAlarm.Run 호출.</summary>
        private static void RunScenario_MaterialAlarm(CIMScenarioContext context) { CIMMaterialAlarm.Run(context); }

        /// <summary>4.30 APN ID Request Process. CIMAPNIDRequestProcess.Run 호출(미사용).</summary>
        private static void RunScenario_APNIDRequestProcess() { CIMAPNIDRequestProcess.Run(); }

        /// <summary>4.31 APN ID 발행 확인 Process. CIMAPNIDIssueConfirmProcess.Run 호출(미사용).</summary>
        private static void RunScenario_APNIDIssueConfirmProcess() { CIMAPNIDIssueConfirmProcess.Run(); }

        /// <summary>4.32 APN ID(Grape ID) Request Process. CIMAPNIDGrapeIDRequestProcess.Run 호출(미사용).</summary>
        private static void RunScenario_APNIDGrapeIDRequestProcess() { CIMAPNIDGrapeIDRequestProcess.Run(); }

        /// <summary>4.33 Load Request. CIMLoadRequest.Run 호출.</summary>
        private static void RunScenario_LoadRequest() { CIMLoadRequest.Run(); }

        /// <summary>4.34 Load Complete. CIMLoadComplete.Run 호출.</summary>
        private static void RunScenario_LoadComplete() { CIMLoadComplete.Run(); }

        /// <summary>4.35 Unload Request. CIMUnloadRequest.Run 호출.</summary>
        private static void RunScenario_UnloadRequest() { CIMUnloadRequest.Run(); }

        /// <summary>4.36 Unload Complete. CIMUnloadComplete.Run 호출.</summary>
        private static void RunScenario_UnloadComplete() { CIMUnloadComplete.Run(); }

        /// <summary>4.37 Material Location Update. CIMMaterialLocationUpdate.Run 호출.</summary>
        private static void RunScenario_MaterialLocationUpdate(CIMScenarioContext context) { CIMMaterialLocationUpdate.Run(context); }

        /// <summary>4.38 Cell Track IN Validation Check. CIMCellTrackINValidationCheck.Run 호출(보류).</summary>
        private static void RunScenario_CellTrackINValidationCheck() { CIMCellTrackINValidationCheck.Run(); }

        /// <summary>4.39 Current Process Control Data Request. CIMCurrentProcessControlDataRequest.Run 호출(보류).</summary>
        private static void RunScenario_CurrentProcessControlDataRequest() { CIMCurrentProcessControlDataRequest.Run(); }

        /// <summary>4.40 Process Control Request Report. CIMProcessControlRequestReport.Run 호출(보류).</summary>
        private static void RunScenario_ProcessControlRequestReport() { CIMProcessControlRequestReport.Run(); }

        /// <summary>4.41 Process Control Result Report. CIMProcessControlResultReport.Run 호출(보류).</summary>
        private static void RunScenario_ProcessControlResultReport() { CIMProcessControlResultReport.Run(); }

        /// <summary>4.42 Unit Status Change. CIMUnitStatusChange.Run 호출(보류).</summary>
        private static void RunScenario_UnitStatusChange() { CIMUnitStatusChange.Run(); }

        /// <summary>4.43 Cell Loader Port Load/Unload. CIMCellLoaderPortLoadUnload.Run 호출(미사용).</summary>
        private static void RunScenario_CellLoaderPortLoadUnload() { CIMCellLoaderPortLoadUnload.Run(); }

        /// <summary>4.44 Cell Unloader Port Load/Unload. CIMCellUnloaderPortLoadUnload.Run 호출(미사용).</summary>
        private static void RunScenario_CellUnloaderPortLoadUnload() { CIMCellUnloaderPortLoadUnload.Run(); }

        /// <summary>4.45 Job Start/Cancel. CIMJobStartCancel.Run 호출(보류).</summary>
        private static void RunScenario_JobStartCancel() { CIMJobStartCancel.Run(); }

        /// <summary>4.46 Process Job Event. CIMProcessJobEvent.Run 호출(보류).</summary>
        private static void RunScenario_ProcessJobEvent() { CIMProcessJobEvent.Run(); }

        /// <summary>4.47 Port State Change. CIMPortStateChange.Run 호출(보류).</summary>
        private static void RunScenario_PortStateChange() { CIMPortStateChange.Run(); }

        /// <summary>16.7 Equipment Function Change </summary>
        private static void RunScenario_EquipFunctionChangeCommand(CIMScenarioContext context) { CIMEquipFunctionChange.RunCommand(context); }

        private static void RunScenario_CellStartPort(CIMScenarioContext context) { CIMCellProcessStart.Run(context); }
        private static void RunScenario_CellIDReadingResult(CIMScenarioContext context) { CIMCellIDReadingResult.Run(context); }
        private static void RunScenario_CellInformationRequest(CIMScenarioContext context) { CIMCellInfomationRequest.Run(context); }

    }
}
