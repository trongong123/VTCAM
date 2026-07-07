using log4net;
using System;
using System.Diagnostics;
using TOPENG_Device;

namespace EQX.Core.Communication.CIM
{
    /// <summary>
    /// CREATE NEW OBJECT EVERY USE. IMPORTANT!!!
    /// </summary>
    public class EquipEventDetail
    {
        public static EquipEventDetail Create(EquipEvent equipEvent)
        {
            return new EquipEventDetail(new SDVCIMMapHeler())
            {
                Event = equipEvent
            };
        }

        public EquipEvent Event { get; set; }

        public string CIMAddress
        {
            get
            {
                _mapHelper.GetBitAddress(Event, out string cimAddress, out _);
                return cimAddress;
            }
        }

        public string PLCAddress
        {
            get
            {
                _mapHelper.GetBitAddress(Event, out _, out string plcAddress);
                return plcAddress;
            }
        }

        public string CIMWordAddress
        {
            get
            {
                _mapHelper.GetWordAddress(Event, out string cimAddress, out _, out _, out _);
                return cimAddress;
            }
        }

        public string PLCWordAddress
        {
            get
            {
                _mapHelper.GetWordAddress(Event, out _, out string plcAddress, out _, out _);
                return plcAddress;
            }
        }

        public int CIMWordLength
        {
            get
            {
                _mapHelper.GetWordAddress(Event, out _, out _, out int cimLength, out _);
                return cimLength;
            }
        }

        public int PLCWordLength
        {
            get
            {
                _mapHelper.GetWordAddress(Event, out _, out _, out _, out int plcLength);
                return plcLength;
            }
        }

        public EquipEventDetail()
        {
            _mapHelper = new SDVCIMMapHeler();
        }

        public EquipEventDetail(ICIMMapHelper mapHelper)
        {
            _mapHelper = mapHelper;
        }

        public void WriteAndBitOnOff(short[] buffer)
        {
            Write(buffer);
            SetPLCBitOnAutoOff();
        }

        public void Write(short[] buffer)
        {
            if (buffer.Length != PLCWordLength)
            {
                throw new Exception($"buffer Length [{buffer.Length}] is not match, excpect {PLCWordLength}");
            }

            CIMAddressMap.WriteWords(CIMAddressMap.GetWriteWordIndexFromDAddress(PLCWordAddress), PLCWordLength, buffer);
        }

        public bool IsCIMBitOn()
        {
            short v = CIMAddressMap.ReadCIMBit(CIMAddress);
            return v != 0;
        }

        /// <summary>Local PLC Bit On. PLCWriteValue_B 사용.</summary>
        public void SetPLCBitOn()
        {
            CIMAddressMap.WritePLCBit(PLCAddress, 1);
        }

        public void SetPLCBitOnAutoOff(int onTimeInMs = 500)
        {
            CIMAddressMap.WritePLCBit(PLCAddress, 1);
            Task.Run(async () =>
            {
                await Task.Delay(onTimeInMs);
                CIMAddressMap.WritePLCBit(PLCAddress, 0);
            });
        }

        /// <summary>Local Reply Bit Off.</summary>
        public void SetPLCBitOff()
        {
            CIMAddressMap.WritePLCBit(PLCAddress, 0);
        }

        /// <summary>CIM Bit가 On 될 때까지 대기. timeoutMs 내에 On 되면 true, 아니면 false(타임아웃 시 알람 발생).</summary>
        public bool WaitForCIMBitOn(int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (IsCIMBitOn())
                {
                    LogManager.GetLogger("CIM").Info($"BIT B[{CIMAddress}] ON");
                    return true;
                }
                System.Threading.Thread.Sleep(10);
            }
            LogManager.GetLogger("CIM").Error($"WAIT FOR BIT B[{CIMAddress}] ON TIMEOUT");
            //OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} Master Bit not On within {timeoutMs}ms");
            return false;
        }

        public bool WaitForCIMBitOff(int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (IsCIMBitOn() == false)
                {
                    LogManager.GetLogger("CIM").Info($"BIT B[{CIMAddress}] OFF");
                    return true;
                }
                System.Threading.Thread.Sleep(10);
            }
            LogManager.GetLogger("CIM").Error($"WAIT FOR BIT B[{CIMAddress}] OFF TIMEOUT");
            //OnCommandTimeoutAlarm?.Invoke("CIM_CMD_TIMEOUT", $"Command {cmd} Master Bit not On within {timeoutMs}ms");
            return false;
        }

        /// <summary>Master Command Bit가 On 된 후, 처리 완료 시 Local Reply를 3초 이내에 On 해야 함. 지연 시 알람.</summary>
        public bool EnsureReplyWithinTimeout(Func<bool> doWork, int timeoutMs = 3000)
        {
            var sw = Stopwatch.StartNew();
            bool done = doWork();
            if (done)
            {
                SetPLCBitOnAutoOff();
                return true;
            }
            if (sw.ElapsedMilliseconds >= timeoutMs)
            {
                //OnCommandTimeoutAlarm?.Invoke("CIM_CMD_RESP_TIMEOUT", $"Command {cmd} response not completed within {timeoutMs}ms");
                return false;
            }
            return false;
        }

        #region Privates
        private readonly ICIMMapHelper _mapHelper;
        #endregion
    }
}
