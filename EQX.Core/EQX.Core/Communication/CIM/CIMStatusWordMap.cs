// 2026-02-25 CursorAI에 의한 코드 생성
// 2026-02-25 CursorAI에 의한 PLC2CIM.csv 반영 수정: AvailabilityLengthWords 20→30
// 2026-02-25 CursorAI에 의한 명세서 반영: ETC Alarm 블록(DCAD4, 400 Word) 추가 — 4.6/4.7 Alarm 발생·해제
// PLC2CIM.csv Bit Address 미할당 섹션(EQPStatus, PortStatus, Availability, MaterialPortState, UnitStatus, ETC Alarm)의 Word 블록 시작 D·길이.
// 시나리오(예: 4.42 Unit Status Change, 4.6/4.7 Alarm)에서 해당 상태 참조 시 사용.

namespace TOPENG_Device
{
    /// <summary>
    /// PLC2CIM.csv 「# 추가 Address: Bit Address는 미지정이며 Word Address만 추가합니다.」 섹션의 Status Word 블록.
    /// Event Bit 없이 Word(D) 주소만 정의된 블록. 시나리오 구현 시 해당 시작 D·길이로 PLCReadValue_W/PLCWriteValue_W 접근.
    /// </summary>
    public static class CIMStatusWordMap
    {
        /// <summary>EQPStatus 블록 시작 D 주소(hex)</summary>
        public const string EQPStatusStartD = "D0";
        /// <summary>EQPStatus 블록 Word 수 (EQPID~EQPMCR2, PLC2CIM.csv 기준)</summary>
        public const int EQPStatusLengthWords = 55;

        /// <summary>PortStatus 블록 시작 D 주소</summary>
        public const string PortStatusStartD = "D39";
        /// <summary>PortStatus 블록 Word 수 (PortNo1~PortProcessingState4)</summary>
        public const int PortStatusLengthWords = 24;

        /// <summary>Availability 블록 시작 D 주소</summary>
        public const string AvailabilityStartD = "D5D";
        /// <summary>Availability 블록 Word 수 (AvailabilityReasonCode 10 + AvailabilityDescription 20, PLC2CIM.csv 기준)</summary>
        public const int AvailabilityLengthWords = 30;

        /// <summary>MaterialPortState 블록 시작 D 주소</summary>
        public const string MaterialPortStateStartD = "D160";
        /// <summary>MaterialPortState 블록 Word 수 (1~8번 포트)</summary>
        public const int MaterialPortStateLengthWords = 424;

        /// <summary>UnitStatus 블록 시작 D 주소. 시나리오 4.42 Unit Status Change 참조 시 사용.</summary>
        public const string UnitStatusStartD = "D450";
        /// <summary>UnitStatus 블록 Word 수 (Unit 1~8 x 7 state)</summary>
        public const int UnitStatusLengthWords = 56;

        /// <summary>ETC Alarm 블록 시작 D 주소. 시나리오 4.6 Alarm 발생·4.7 Alarm 해제 시 사용(PLC2CIM.csv ETC 섹션).</summary>
        public const string EtcAlarmStartD = "DC55C";
        /// <summary>ETC Alarm 블록 Word 수 (명세서 §4.6/§4.7, PLC2CIM.csv)</summary>
        public const int EtcAlarmLengthWords = 400;

        /// <summary>
        /// 지정한 Status 블록의 시작 D 주소와 Word 길이를 반환한다.
        /// </summary>
        public static void GetStatusBlockRange(StatusBlockKind kind, out string startD, out int lengthWords)
        {
            switch (kind)
            {
                case StatusBlockKind.EQPStatus:
                    startD = EQPStatusStartD; lengthWords = EQPStatusLengthWords; break;
                case StatusBlockKind.PortStatus:
                    startD = PortStatusStartD; lengthWords = PortStatusLengthWords; break;
                case StatusBlockKind.Availability:
                    startD = AvailabilityStartD; lengthWords = AvailabilityLengthWords; break;
                case StatusBlockKind.MaterialPortState:
                    startD = MaterialPortStateStartD; lengthWords = MaterialPortStateLengthWords; break;
                case StatusBlockKind.UnitStatus:
                    startD = UnitStatusStartD; lengthWords = UnitStatusLengthWords; break;
                case StatusBlockKind.EtcAlarm:
                    startD = EtcAlarmStartD; lengthWords = EtcAlarmLengthWords; break;
                default:
                    startD = null; lengthWords = 0; break;
            }
        }

        /// <summary>
        /// Hàm lấy địa chỉ Word và giá trị bitmask tương ứng với ALARM ID
        /// </summary>
        /// <param name="alarmId">Mã Alarm ID (Ví dụ: 1, 17, 33...)</param>
        /// <param name="wordAddress">Địa chỉ Word trả về (Ví dụ: "CAD4", "CAD5")</param>
        /// <param name="wordValue">Giá trị của Word tương ứng với cờ bit của Alarm</param>
        public static void GetAlarmWordInfo(int alarmId, out string wordAddress, out short wordValue)
        {
            // Dải Alarm Word từ CAD4 ~ CC63 (400 words * 16 bit = 6400 Alarms tối đa)
            if (alarmId < 2 || alarmId > 6400)
            {
                throw new ArgumentOutOfRangeException("Alarm ID chỉ hỗ trợ từ 1 đến 6400.");
            }

            // Địa chỉ cơ sở (Base Address) của Alarm là CAD4 (Hệ Hex)
            int baseAddress = 0xCAD4;

            // Tính toán Index bù trừ (Do Alarm ID bắt đầu từ 1, còn Index bắt đầu từ 0)
            int index = alarmId - 1;

            // Tính khoảng cách Word (Word Offset)
            int wordOffset = index / 16;

            // Tính vị trí Bit trong Word đó (0 - 15)
            int bitOffset = index % 16;

            // Xác định địa chỉ đích (Target Address)
            int targetAddress = baseAddress + wordOffset;

            // Trả về địa chỉ dạng chuỗi Hexadecimal (Ví dụ: CAD4, CAD5)
            wordAddress = targetAddress.ToString("X4");

            // Trả về giá trị của Word ứng với cờ bit đó (Dịch bit 1 sang trái 'bitOffset' lần)
            wordValue = (short)(1 << bitOffset);
        }
    }

    /// <summary>PLC2CIM.csv Bit 미할당 Status Word 블록 종류</summary>
    public enum StatusBlockKind
    {
        EQPStatus,
        PortStatus,
        Availability,
        MaterialPortState,
        UnitStatus,
        EtcAlarm
    }
}
