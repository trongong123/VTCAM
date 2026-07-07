using System.Net;
using System.Text;

namespace EQX.ThirdParty.Fastech
{
    public partial class EziPlusEMotionLib
    {
        /// <summary>
        /// One or more error occurs
        /// </summary>
        public const uint FFLAG_ERRORALL = 0x00000001;
        /// <summary>
        /// + direction Limit sensor turns ON
        /// </summary>
        public const uint FFLAG_HWPOSILMT = 0x00000002;
        /// <summary>
        /// - direction Limit sensor turns ON
        /// </summary>
        public const uint FFLAG_HWNEGALMT = 0x00000004;
        /// <summary>
        /// + direction program Limit is exceeded
        /// </summary>
        public const uint FFLAG_SWPOGILMT = 0x00000008;
        /// <summary>
        /// - direction program Limit is exceeded
        /// </summary>
        public const uint FFLAG_SWNEGALMT = 0x00000010;
        public const uint Reserved1 = 0x00000020;
        public const uint Reserved2 = 0x00000040;
        /// <summary>
        /// Position error is higher than "Pos Error Overflow Limit" parameter after position command.
        /// </summary>
        public const uint FFLAG_ERRPOSOVERFLOW = 0x00000080;
        /// <summary>
        /// Alarm occurs when an overcurrent occurs in the motor drive element
        /// </summary>
        public const uint FFLAG_ERROVERCURRENT = 0x00000100;
        /// <summary>
        /// Alarm occurs when the motor speed exceeds 3000 [rpm]
        /// </summary>
        public const uint FFLAG_ERROVERSPEED = 0x00000200;
        /// <summary>
        /// Position error is higher than "Pos Tracking Limit" parameter during position command run
        /// </summary>
        public const uint FFLAG_ERRPOSTRACKING = 0x00000400;
        /// <summary>
        /// Alarm occurs when a load exceeding the maximum torque of the motor is applied for a distance
        /// of more than 5 seconds or more than 10 turns
        /// </summary>
        public const uint FFLAG_ERROVERLOAD = 0x00000800;
        /// <summary>
        /// The internal temperature of the drive exceeds 85°C
        /// </summary>
        public const uint FFLAG_ERROVERHEAT = 0x00001000;
        /// <summary>
        /// A counter electromotive force of the motor exceeds 70V
        /// </summary>
        public const uint FFLAG_ERRBACKEMF = 0x00002000;
        /// <summary>
        /// Alarm occurs when it is the motor voltage error
        /// </summary>
        public const uint FFLAG_ERRMOTORPOWER = 0x00004000;
        /// <summary>
        /// Alarm occurs when it is In-position error
        /// </summary>
        public const uint FFLAG_ERRINPOSITION = 0x00008000;
        /// <summary>
        /// If the motor is in an emergency stop
        /// </summary>
        public const uint FFLAG_EMGSTOP = 0x00010000;
        /// <summary>
        /// If the motor is in an slow stop
        /// </summary>
        public const uint FFLAG_SLOWSTOP = 0x00020000;
        /// <summary>
        /// During homing operation
        /// </summary>
        public const uint FFLAG_ORIGINRETURNING = 0x00040000;
        /// <summary>
        /// In-position is complete
        /// </summary>
        public const uint FFLAG_INPOSITION = 0x00080000;
        /// <summary>
        /// The motor is under Servo On
        /// </summary>
        public const uint FFLAG_SERVOON = 0x00100000;
        /// <summary>
        /// Alarm Reset has run
        /// </summary>
        public const uint FFLAG_ALARMRESET = 0x00200000;
        /// <summary>
        /// Position Table operation has been finished
        /// </summary>
        public const uint FFLAG_PTSTOPED = 0x00400000;
        /// <summary>
        /// The origin sensor is ON
        /// </summary>
        public const uint FFLAG_ORIGINSENSOR = 0x00800000;
        /// <summary>
        /// In case of z-pulse type operation during homing operaiton
        /// </summary>
        public const uint FFLAG_ZPULSE = 0x01000000;
        /// <summary>
        /// Origin return operation has been finished
        /// </summary>
        public const uint FFLAG_ORIGINRETOK = 0x02000000;
        /// <summary>
        /// Motor operating direction (+ :OFF, - :ON) 
        /// </summary>
        public const uint FFLAG_MOTIONDIR = 0x04000000;
        /// <summary>
        /// The motor is running
        /// </summary>
        public const uint FFLAG_MOTIONING = 0x08000000;
        /// <summary>
        /// The motor in running is stopped by Pause command
        /// </summary>
        public const uint FFLAG_MOTIONPAUSE = 0x10000000;
        /// <summary>
        /// The motor is operating to the acceleration section
        /// </summary>
        public const uint FFLAG_MOTIONACCEL = 0x20000000;
        /// <summary>
        /// The motor is operating to the deceleration section
        /// </summary>
        public const uint FFLAG_MOTIONDECEL = 0x40000000;
        /// <summary>
        /// The motor is operating to the normal speed, not acceleration / deceleration sections
        /// </summary>
        public const uint FFLAG_MOTIONCONST = 0x80000000;

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
    }

    public partial class EziPlusEMotionLib : EziPlusEEventCommunicator
    {
        #region Constructors
        public EziPlusEMotionLib(int index, string name)
            : base(index, name, IPAddress.Parse($"192.168.0.{index}"), 2002)
        {
        }
        #endregion

        #region Functions
        #region Driver Board
        public int EMGStop()
        {
            SetTransmitFrameData(0x32);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int SearchOrigin()
        {
            SetTransmitFrameData(0x33);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int MotionOff()
        {
            byte[] data = new byte[] { 0x00 };
            SetTransmitFrameData(0x2A, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int MotionOn()
        {
            byte[] data = new byte[] { 0x01 };
            SetTransmitFrameData(0x2A, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int MoveAbs(int pulse, uint velocity)
        {
            byte[] dataPulse = BitConverter.GetBytes(pulse);
            byte[] dataVelocity = BitConverter.GetBytes(velocity);
            byte[] data = dataPulse.Concat(dataVelocity).ToArray();

            SetTransmitFrameData(0x34, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int AlarmReset()
        {
            SetTransmitFrameData(0x2B);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int MoveInc(int pulse, uint velocity)
        {
            byte[] dataPulse = BitConverter.GetBytes(pulse);
            byte[] dataVelocity = BitConverter.GetBytes(velocity);
            byte[] data = dataPulse.Concat(dataVelocity).ToArray();

            SetTransmitFrameData(0x35, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int MoveJog(uint velocity, bool direction)
        {
            byte[] dataVelocity = BitConverter.GetBytes(velocity);
            byte[] dataDirection = BitConverter.GetBytes(direction);
            byte[] data = dataVelocity.Concat(dataDirection).ToArray();

            SetTransmitFrameData(0x37, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int SoftStop()
        {
            SetTransmitFrameData(0x31);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int ClearPosition()
        {
            SetTransmitFrameData(0x56);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetBoardInfo(ref string sBoardInfo)
        {
            SetTransmitFrameData(0x01);
            SendTransmitFrameData();

            ReadResponseFrameData();

            byte[] bBoardInfo = new byte[260];
            for (int i = 7; i <= responseFrameData[1] + 1; i++)
            {
                bBoardInfo[i - 7] = responseFrameData[i];
            }
            sBoardInfo = Encoding.ASCII.GetString(bBoardInfo);

            return CommunicationStatus;
        }

        public int GetMotorInfo(ref string sMotorInfo)
        {
            SetTransmitFrameData(0x05);
            SendTransmitFrameData();

            ReadResponseFrameData();

            byte[] bMotorInfo = new byte[260];
            for (int i = 7; i <= responseFrameData[1] + 1; i++)
            {
                bMotorInfo[i - 7] = responseFrameData[i];
            }
            sMotorInfo = Encoding.ASCII.GetString(bMotorInfo);

            return CommunicationStatus;
        }

        public int SetIOOutput(uint setMask, uint clearMask)
        {
            byte[] dataSetMask = BitConverter.GetBytes(setMask);
            byte[] dataClearMask = BitConverter.GetBytes(clearMask);
            byte[] data = dataSetMask.Concat(dataClearMask).ToArray();

            SetTransmitFrameData(0x20, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();
            return CommunicationStatus;
        }

        public int SetIOInput(uint setMask, uint clearMask)
        {
            byte[] dataSetMask = BitConverter.GetBytes(setMask);
            byte[] dataClearMask = BitConverter.GetBytes(clearMask);
            byte[] data = dataSetMask.Concat(dataClearMask).ToArray();

            SetTransmitFrameData(0x21, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();
            return CommunicationStatus;
        }

        public int GetIOInput(ref uint inputStatus)
        {
            byte[] bInputStatus = new byte[4];
            SetTransmitFrameData(0x22);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bInputStatus[i - 6] = responseFrameData[i];
            }
            inputStatus = BitConverter.ToUInt32(bInputStatus);

            return CommunicationStatus;
        }

        public int GetIOOutput(ref uint outputStatus)
        {
            byte[] bOutputStatus = new byte[4];
            SetTransmitFrameData(0x23);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bOutputStatus[i - 6] = responseFrameData[i];
            }
            outputStatus = BitConverter.ToUInt32(bOutputStatus);

            return CommunicationStatus;
        }

        public int GetAlarmType(ref byte alarmType)
        {
            SetTransmitFrameData(0x2E);
            SendTransmitFrameData();

            ReadResponseFrameData();
            alarmType = responseFrameData[6];

            return CommunicationStatus;
        }

        public int GetAxisStatus(ref uint axisStatus)
        {
            byte[] bAxisStatus = new byte[4];
            SetTransmitFrameData(0x40);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bAxisStatus[i - 6] |= responseFrameData[i];
            }
            axisStatus = BitConverter.ToUInt32(bAxisStatus);

            return CommunicationStatus;
        }

        public void SetOutput(int pinNumber, bool value)
        {
            uint setMask = 0;
            uint clearMask = 0;

            if (value == true)
            {
                setMask = (uint)(0x00008000 << pinNumber);
            }
            else
            {
                clearMask = (uint)(0x00008000 << pinNumber);
            }

            SetIOOutput(setMask, clearMask);
        }

        public bool GetOutput(int pinNumber)
        {
            uint ioOutput = 0;
            GetIOOutput(ref ioOutput);

            uint pinBitMask = (uint)(0x00008000 << pinNumber);

            return (ioOutput & pinBitMask) == pinBitMask;
        }

        public int GetIOAxisStatus(ref uint inputStatus, ref uint outputStatus, ref uint axisStatus)
        {
            byte[] bInputStatus = new byte[4];
            byte[] bOutputStatus = new byte[4];
            byte[] bAxisStatus = new byte[4];
            SetTransmitFrameData(0x41);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i < 10; i++)
            {
                bInputStatus[i - 6] = responseFrameData[i];
            }
            for (int i = 10; i < 14; i++)
            {
                bOutputStatus[i - 10] = responseFrameData[i];
            }
            for (int i = 14; i <= responseFrameData[1] + 1; i++)
            {
                bAxisStatus[i - 14] = responseFrameData[i];
            }

            inputStatus = BitConverter.ToUInt32(bInputStatus);
            outputStatus = BitConverter.ToUInt32(bOutputStatus);
            axisStatus = BitConverter.ToUInt32(bAxisStatus);

            return CommunicationStatus;
        }

        public int GetMotionStatus(ref int cmdPos, ref int actPos, ref int posErr, ref int actVel, ref int currentRunPT)
        {
            byte[] bCmdPos = new byte[4];
            byte[] bActPos = new byte[4];
            byte[] bPosErr = new byte[4];
            byte[] bActVel = new byte[4];
            byte[] bCurrentRunPT = new byte[4];
            SetTransmitFrameData(0x42);
            SendTransmitFrameData();

            ReadResponseFrameData();

            for (int i = 6; i < 10; i++)
            {
                bCmdPos[i - 6] = responseFrameData[i];
            }
            for (int i = 10; i < 14; i++)
            {
                bActPos[i - 10] = responseFrameData[i];
            }
            for (int i = 14; i < 18; i++)
            {
                bPosErr[i - 14] = responseFrameData[i];
            }
            for (int i = 18; i < 22; i++)
            {
                bActVel[i - 18] = responseFrameData[i];
            }
            for (int i = 22; i <= responseFrameData[1] + 1; i++)
            {
                bCurrentRunPT[i - 22] = responseFrameData[i];
            }

            cmdPos = BitConverter.ToInt32(bCmdPos);
            actPos = BitConverter.ToInt32(bActPos);
            posErr = BitConverter.ToInt32(bPosErr);
            actVel = BitConverter.ToInt32(bActVel);
            currentRunPT = BitConverter.ToInt32(bCurrentRunPT);

            return CommunicationStatus;

        }

        public int GetAllStatus(ref uint inputStatus, ref uint outputStatus, ref uint axisStatus, ref int cmdPos, ref int actPos, ref int posErr, ref int actVel, ref int currentRunPT)
        {
            byte[] bInputStatus = new byte[4];
            byte[] bOutputStatus = new byte[4];
            byte[] bAxisStatus = new byte[4];
            byte[] bCmdPos = new byte[4];
            byte[] bActPos = new byte[4];
            byte[] bPosErr = new byte[4];
            byte[] bActVel = new byte[4];
            byte[] bCurrentRunPT = new byte[4];
            SetTransmitFrameData(0x43);
            SendTransmitFrameData();

            ReadResponseFrameData();

            for (int i = 6; i < 10; i++)
            {
                bInputStatus[i - 6] = responseFrameData[i];
            }
            for (int i = 10; i < 14; i++)
            {
                bOutputStatus[i - 10] = responseFrameData[i];
            }
            for (int i = 14; i < 18; i++)
            {
                bAxisStatus[i - 14] = responseFrameData[i];
            }
            for (int i = 18; i < 22; i++)
            {
                bCmdPos[i - 18] = responseFrameData[i];
            }
            for (int i = 22; i < 26; i++)
            {
                bActPos[i - 22] = responseFrameData[i];
            }
            for (int i = 26; i < 30; i++)
            {
                bPosErr[i - 26] = responseFrameData[i];
            }
            for (int i = 30; i < 34; i++)
            {
                bActVel[i - 30] = responseFrameData[i];
            }
            for (int i = 34; i <= responseFrameData[1] + 1; i++)
            {
                bCurrentRunPT[i - 34] = responseFrameData[i];
            }

            inputStatus = BitConverter.ToUInt32(bInputStatus);
            outputStatus = BitConverter.ToUInt32(bOutputStatus);
            axisStatus = BitConverter.ToUInt32(bAxisStatus);
            cmdPos = BitConverter.ToInt32(bCmdPos);
            actPos = BitConverter.ToInt32(bActPos);
            posErr = BitConverter.ToInt32(bPosErr);
            actVel = BitConverter.ToInt32(bActVel);
            currentRunPT = BitConverter.ToInt32(bCurrentRunPT);

            return CommunicationStatus;
        }

        public int SetCommandPos(int cmdPos)
        {
            byte[] data = BitConverter.GetBytes(cmdPos);
            SetTransmitFrameData(0x50, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetCommandPos(ref int cmdPos)
        {
            byte[] bCmdPos = new byte[4];
            SetTransmitFrameData(0x51);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bCmdPos[i - 6] = responseFrameData[i];
            }
            cmdPos = BitConverter.ToInt32(bCmdPos);

            return CommunicationStatus;
        }

        public int SetActualPos(int actPos)
        {
            byte[] data = BitConverter.GetBytes(actPos);
            SetTransmitFrameData(0x52, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();
            return CommunicationStatus;
        }

        public int GetActualPos(ref int actPos)
        {
            byte[] bActPos = new byte[4];
            SetTransmitFrameData(0x53);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bActPos[i - 6] = responseFrameData[i];
            }
            actPos = BitConverter.ToInt32(bActPos);

            return CommunicationStatus;
        }

        public int GetPosError(ref int posErr)
        {
            byte[] bPosErr = new byte[4];
            SetTransmitFrameData(0x54);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bPosErr[i - 6] = responseFrameData[i];
            }
            posErr = BitConverter.ToInt32(bPosErr);

            return CommunicationStatus;
        }

        public int GetActualVel(ref int actVel)
        {
            byte[] bActVel = new byte[4];
            SetTransmitFrameData(0x55);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bActVel[i - 6] = responseFrameData[i];
            }
            actVel = BitConverter.ToInt32(bActVel);

            return CommunicationStatus;
        }

        public int SetAllParameters()
        {
            SetTransmitFrameData(0x10);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetROMParameter(byte paraNumber, ref int paraValue)
        {
            byte[] bParaValue = new byte[4];
            byte[] bParaNumber = new byte[1];
            bParaNumber[0] = paraNumber;
            SetTransmitFrameData(0x11, 1, bParaNumber);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                bParaValue[i - 6] |= responseFrameData[i];
            }
            paraValue = BitConverter.ToInt32(bParaValue);

            return CommunicationStatus;
        }

        public int SetParameter(byte paraNumber, int paraValue)
        {
            byte[] dataParaNumber = new byte[1];

            dataParaNumber[0] = paraNumber;
            byte[] dataParaValue = BitConverter.GetBytes(paraValue);
            byte[] data = dataParaNumber.Concat(dataParaValue).ToArray();

            SetTransmitFrameData(0x12, data.Length, data);
            SendTransmitFrameData();

            ReadResponseFrameData();

            return CommunicationStatus;
        }

        public int GetParameter(byte paraNumber, ref int paraValue)
        {
            byte[] dataParaValue = new byte[4];

            byte[] dataParaNumber = new byte[1];
            dataParaNumber[0] = paraNumber;

            SetTransmitFrameData(0x13, dataParaNumber.Length, dataParaNumber);
            SendTransmitFrameData();

            ReadResponseFrameData();
            for (int i = 6; i <= responseFrameData[1] + 1; i++)
            {
                dataParaValue[i - 6] = responseFrameData[i];
            }
            paraValue = BitConverter.ToInt32(dataParaValue);

            return CommunicationStatus;
        }
        #endregion

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
