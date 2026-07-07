// 2026-02-24 CursorAI - SDC CIM 구현 컨셉 기반 코드 생성
// Alive Bit: 0.1초 이내 On 유지, 5초 마다 동작 (Timing Chart Toggle Bit 규칙)
// 2026-02-25 CursorAI - 프로그램 실행 시 Local에서 자동 시작하여 계속 동작. Master가 Local PC Live 상태 확인.

using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace TOPENG_Device
{
    /// <summary>
    /// Alive Bit 5초 주기, 0.1초 동안 On 후 Off. EquipEvent.AliveBit 사용.
    /// Local 프로그램 실행 시 시작되어 계속 동작하며, Master에서 Local PC의 Live 상태 확인에 사용.
    /// </summary>
    public class CIMAliveTask
    {
        /// <summary>Alive Bit On 유지 시간 (ms). 명세: 0.1초 이내.</summary>
        /// 260304, 테스트위해 주기 변경 (100 -> 500)
        public const int AliveOnDurationMs = 500;//100;
        /// <summary>Alive 주기 (ms). 명세: 5초 마다.</summary>
        public const int AlivePeriodMs = 5000;

        private Timer _aliveTimer;
        private Timer _dataUpdateTimer;
        private readonly object _lock = new object();

        /// <summary>Alive 태스크 시작. 5초 주기로 Alive Bit 0.1초 On.</summary>
        public void Start()
        {
            lock (_lock)
            {
                Stop();

                MCCLinkIE.mCCLinkIEUse = true;
                if (MCCLinkIE.mCCLinkIEUse == true)
                {
                    int iCCLinkIEOpen = MCCLinkIE.Open();
                    if (iCCLinkIEOpen == 0)  //OK
                    {
                        MCCLinkIE.Initial();
                        MCCLinkIE.mRDeviceNo = MPLC.PLCReadBaseAddress_B;
                        MCCLinkIE.mRDeviceNoLength = MPLC.PLCReadBaseAddressLength_B;
                        MCCLinkIE.mSDeviceNo = MPLC.PLCWriteBaseAddressLength_B;
                        MCCLinkIE.mSDeviceNoLength = MPLC.PLCReadBaseAddressLength_B;
                    }
                    else
                    {

                    }
                }

                _aliveTimer = new Timer(OnAliveTick, null, AlivePeriodMs, AlivePeriodMs);
                tokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        OnDataUpdateTick();
                        await Task.Delay(30);
                    }
                }, tokenSource.Token);
            }
        }

        /// <summary>Alive 태스크 정지.</summary>
        public void Stop()
        {
            lock (_lock)
            {
                _aliveTimer?.Dispose();
                _aliveTimer = null;

                CIMEventHandler.SetLocalRequestBitOff(EquipEvent.AliveBit);

                Thread.Sleep(200);

                tokenSource?.Cancel();

                _dataUpdateTimer?.Dispose();
                _dataUpdateTimer = null;
            }
        }

        private void OnAliveTick(object state)
        {
            CIMEventHandler.SetLocalRequestBitOn(EquipEvent.AliveBit);
            Thread.Sleep(AliveOnDurationMs);
            CIMEventHandler.SetLocalRequestBitOff(EquipEvent.AliveBit);
        }

        private void OnDataUpdateTick()
        {
            MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.B, false);
            MPLC.PLCRead(false, 0, (int)MPLC.enumDeviceType.W, false);
            
            MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.B, false);
            MPLC.PLCWrite(false, 0, (int)MPLC.enumDeviceType.W, false);
        }

        private CancellationTokenSource tokenSource;
    }
}
