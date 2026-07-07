// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// Alive Bit: 0.1초 이내 On 유지, 5초 마다 동작 (Timing Chart Toggle Bit 규칙)
// 2026-02-25 CursorAI - 프로그램 실행 시 Local에서 자동 시작하여 계속 동작. Master가 Local PC Live 상태 확인.

namespace TOPENG_Device
{
    /// <summary>
    /// Alive Bit 5초 주기, 0.1초 동안 On 후 Off. EquipEvent.AliveBit 사용.
    /// Local 프로그램 실행 시 시작되어 계속 동작하며, Master에서 Local PC의 Live 상태 확인에 사용.
    /// </summary>
    public class CIMRealTimeReportTask
    {
        public const int AlivePeriodMs = 100;

        private Timer _timer;
        private readonly object _lock = new object();

        private static bool _excute = false;
        //====================================================================

        /// <summary>Alive 태스크 시작. 5초 주기로 Alive Bit 0.1초 On.</summary>
        public void Start()
        {
            lock (_lock)
            {
                Stop();
                _timer = new Timer(OnReportTick, null, AlivePeriodMs, AlivePeriodMs);
            }
        }

        /// <summary>Alive 태스크 정지.</summary>
        public void Stop()
        {
            lock (_lock)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        //====================================================================

        private void OnReportTick(object state)
        {
            if (_excute) return;

            _excute = true;

            lock (_lock)
            {
                MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.B, false);
                MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.W, false);

                // 인자로 받는 레시피 리스트는 현재 레시피 폴더 안에 파일을 순차적으로 넘버링해서 받는거로, 사용할 구조와 상이함.
                // 테스트를 위해 해당 부분으로 처리를 했지만, 이후 변경을 해야하고 다시 주석을 해제해야한다.
                //WritePPIDList(Equipment.Instance.Recipe.RecipeIdList);
                WritePPIDList();
                WriteCurrentPPID();
                WriteRmsItem();
                WriteEcmItem();
                WriteFdcItem();

                MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.W, false);
                MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.B, false);
            }

            _excute = false;
        }

        //====================================================================

        /// <summary>
        /// 전체 레시피 리스트 업데이트
        /// </summary>
        //private void WritePPIDList(IReadOnlyList<RecipeListItem> list)
        //{
        //    const int count = 2000;
        //    var buf = new short[count];

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].Number >= 1 && list[i].Number <= 99)
        //        {
        //            int index = list[i].Number * 20;
        //            CIMScenarioDispatcher.EncodeAsciiToWords(list[i].RecipeId, ref buf, index, 20);
        //        }
        //    }

        //    int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D8A54");
        //    CIMAddressMap.WriteWords(w, count, buf);
        //}
        private void WritePPIDList(List<CIM_RecipeList_Item> list = null)
        {
            const int count = 2000;
            var buf = new short[count];

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].No >= 1 && list[i].No <= 99)
                {
                    int index = (list[i].No-1) * 20;
                    CIMScenarioDispatcher.EncodeAsciiToWords(list[i].Name, ref buf, index, 20);
                }
            }

            if (list.Count != 0)
            {
                int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D8A54");
                CIMAddressMap.WriteWords(w, count, buf);
            }
        }
        private void WriteCurrentPPID(string name = null)
        {
            const int count = 20;
            var buf = new short[count];

            CIMScenarioDispatcher.EncodeAsciiToWords(name, ref buf, 0, 20);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("D14");
            CIMAddressMap.WriteWords(w, count, buf);
        }
        /// <summary>
        /// 전체 RMS 항목 업데이트, 단 Fix영역 제외
        /// </summary>
        private void WriteRmsItem()
        {
            // 사용여부 확인 필요.
        }
        /// <summary>
        /// 전체 ECM 항목 업데이트
        /// </summary>
        private void WriteEcmItem()
        {
            // 추후 이걸로 어드레스 가지고 오는거로 변경 필요.
            //CIMEventItemMap.GetEventItemWordRange(EquipEvent.CellIDReadingResult1, out string startD, out int lengthWords);
            const int count = 320;
            var buf = new short[count];

            // TEST
            CIMScenarioDispatcher.EncodeAsciiToWords("1000", ref buf, 0, 2);
            CIMScenarioDispatcher.EncodeAsciiToWords("2000", ref buf, 2, 2);
            CIMScenarioDispatcher.EncodeAsciiToWords("3000", ref buf, 4, 2);
            CIMScenarioDispatcher.EncodeAsciiToWords("4000", ref buf, 6, 2);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DA1C4");
            CIMAddressMap.WriteWords(w, count, buf);
        }
        /// <summary>
        /// 전체 FDC 항목 업데이트
        /// </summary>
        private void WriteFdcItem()
        {
            const int count = 1129;
            var buf = new short[count];

            // TEST
            // Cell ID
            CIMScenarioDispatcher.EncodeAsciiToWords("TEST_CELLID", ref buf, 0, 20);
            CIMScenarioDispatcher.EncodeAsciiToWords("0", ref buf, 22, 2);
            CIMScenarioDispatcher.EncodeAsciiToWords("100", ref buf, 24, 2);
            CIMScenarioDispatcher.EncodeAsciiToWords("0", ref buf, 26, 2);

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DABEC");
            CIMAddressMap.WriteWords(w, count, buf);
        }

        private void WriteDVItem()
        {
            const int count = 1000;
            var buf = new short[count];

            int w = CIMAddressMap.GetWriteWordIndexFromDAddress("DC104");
            CIMAddressMap.WriteWords(w, count, buf);
        }

        private void WriteEQPStatus()
            {
                // EQPID

                // EQP PPID
                // EQP Control
                // EQP 
            }
        }


    
}
