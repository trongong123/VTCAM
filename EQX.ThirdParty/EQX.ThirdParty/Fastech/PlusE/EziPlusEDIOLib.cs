using System.Net;

namespace EQX.ThirdParty.Fastech
{
    public class EziPlusEDIOLib : EziPlusEEventCommunicator
    {
        #region Return Codes
        public const int FMM_OK = 0;

        public const int FMM_NOT_OPEN = 1;
        public const int FMM_INVALID_PORT_NUM = 2;
        public const int FMM_INVALID_SLAVE_NUM = 3;

        public const int FMC_DISCONNECTED = 5;
        public const int FMC_TIMEOUT_ERROR = 6;
        public const int FMC_CRCFAILED_ERROR = 7;
        public const int FMC_RECVPACKET_ERROR = 8;

        public const int FMM_POSTABLE_ERROR = 9;

        public const int FMP_FRAMETYPEERROR = 0x80;
        public const int FMP_DATAERROR = 0x81;
        public const int FMP_PACKETERROR = 0x82;

        public const int FMP_RUNFAIL = 0x85;
        public const int FMP_RESETFAIL = 0x86;
        public const int FMP_SERVOONFAIL1 = 0x87;
        public const int FMP_SERVOONFAIL2 = 0x88;
        public const int FMP_SERVOONFAIL3 = 0x89;

        public const int FMP_SERVOOFF_FAIL = 0x8A;
        public const int FMP_ROMACCESS = 0x8B;

        public const int FMP_PACKETCRCERROR = 0xAA;

        public const int FMM_UNKNOWN_ERROR = 0xFF;
        #endregion

        #region Constructors
        public EziPlusEDIOLib(int index, string name)
            : base(index, name, IPAddress.Parse($"192.168.0.{index}"), 2002)
        {
        }
        #endregion

        #region Functions
        #region IO Board
        public int GetInput(ref uint inputStatus, ref uint latchStatus)
        {
            byte[] bInputStatus = new byte[4];
            byte[] bLatchStatus = new byte[4];

            SetTransmitFrameData(0xC0);
            SendTransmitFrameData();

            ReadResponseFrameData();

            for (int i = 6; i < 10; i++)
            {
                bInputStatus[i - 6] = responseFrameData[i];
            }
            for (int i = 10; i < 14; i++)
            {
                bLatchStatus[i - 10] = responseFrameData[i];
            }
            inputStatus = BitConverter.ToUInt32(bInputStatus);
            latchStatus = BitConverter.ToUInt32(bLatchStatus);
            return CommunicationStatus;
        }

        public int ClearLatch(uint latchStatus)
        {
            byte[] bLatchStatus = BitConverter.GetBytes(latchStatus);

            SetTransmitFrameData(0xC1, bLatchStatus.Length, bLatchStatus);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetLatchCount(byte latchCount, ref uint latchStatus)
        {
            byte[] bLatchStatus = new byte[4];

            byte[] bLatchCount = new byte[1] { latchCount };

            SetTransmitFrameData(0xC2, bLatchCount.Length, bLatchCount);
            SendTransmitFrameData();
            ReadResponseFrameData();
            for (int i = 6; i < 10; i++)
            {
                bLatchStatus[i - 6] = responseFrameData[i];
            }

            latchStatus = BitConverter.ToUInt32(bLatchStatus);

            return CommunicationStatus;
        }

        public int GetLatchCountAll(ref uint[] data)
        {
            data = new uint[16];

            SetTransmitFrameData(0xC3);
            SendTransmitFrameData();
            ReadResponseFrameData();

            for (int i = 0; i < 16; i++)
            {
                byte[] bLatchData = new byte[4];
                for (int j = 6 + 4 * i; j < 10 + 4 * i; j++)
                {
                    bLatchData[j - (6 + 4 * i)] = responseFrameData[j];
                }
                data[i] = BitConverter.ToUInt32(bLatchData);
            }

            return CommunicationStatus;
        }

        public int ClearLatchCount(uint latchCount)
        {
            byte[] bLatchCount = BitConverter.GetBytes(latchCount);
            SetTransmitFrameData(0xC4, bLatchCount.Length, bLatchCount);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetOutput(ref ulong outputStatus)
        {
            byte[] bOutputStatus = new byte[8];
            SetTransmitFrameData(0xC5);
            SendTransmitFrameData();
            ReadResponseFrameData();

            for (int i = 6; i < 14; i++)
            {
                bOutputStatus[i - 6] = responseFrameData[i];
            }

            outputStatus = BitConverter.ToUInt64(bOutputStatus);

            return CommunicationStatus;
        }

        public int SetOutput(uint setOutputData, uint resetOutputData)
        {
            byte[] bSetOutputData = BitConverter.GetBytes(setOutputData);
            byte[] bResetOutputData = BitConverter.GetBytes(resetOutputData);

            byte[] data = bSetOutputData.Concat(bResetOutputData).ToArray();

            SetTransmitFrameData(0xC6, data.Length, data);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int SetTrigger(uint count, ushort blank1, ushort ontime, ushort blank2, ushort period, byte outputNo)
        {
            byte[] bCount = BitConverter.GetBytes(count);
            byte[] bBlank1 = BitConverter.GetBytes(blank1);
            byte[] bOnTime = BitConverter.GetBytes(ontime);
            byte[] bBlank2 = BitConverter.GetBytes(blank2);
            byte[] bPeriod = BitConverter.GetBytes(period);
            byte[] bOutputNo = new byte[1] { outputNo };

            byte[] data = bCount.Concat(bBlank1).Concat(bOnTime).Concat(bBlank2).Concat(bPeriod).Concat(bOutputNo).ToArray();

            SetTransmitFrameData(0xC7, data.Length, data);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetTrigger(byte outputCount, ref uint triggerOutput)
        {
            byte[] bOutputCount = new byte[1] { outputCount };

            SetTransmitFrameData(0xC9, bOutputCount.Length, bOutputCount);
            SendTransmitFrameData();
            ReadResponseFrameData();

            byte[] bTriggerOutput = new byte[4];

            for (int i = 6; i < 10; i++)
            {
                bTriggerOutput[i - 6] = responseFrameData[i];
            }

            triggerOutput = BitConverter.ToUInt32(bTriggerOutput);

            return CommunicationStatus;
        }

        public int GetIOLevel(ref uint ioLevel)
        {
            SetTransmitFrameData(0xCA);
            SendTransmitFrameData();
            ReadResponseFrameData();

            byte[] bIOLevel = new byte[4];
            for (int i = 6; i < 10; i++)
            {
                bIOLevel[i - 6] = responseFrameData[i];
            }

            ioLevel = BitConverter.ToUInt32(bIOLevel);

            return CommunicationStatus;
        }

        public int SetIOLevel(uint ioLevel)
        {
            byte[] bIOLevel = BitConverter.GetBytes(ioLevel);
            SetTransmitFrameData(0xCB, bIOLevel.Length, bIOLevel);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int LoadIOLevel()
        {
            SetTransmitFrameData(0xCC);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int SaveIOLevel()
        {
            SetTransmitFrameData(0xCD);
            SendTransmitFrameData();
            ReadResponseFrameData();

            return CommunicationStatus;
        }
        #endregion
        #endregion
    }

}
