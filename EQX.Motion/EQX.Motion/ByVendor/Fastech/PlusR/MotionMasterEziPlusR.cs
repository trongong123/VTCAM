using EQX.Core.Common;
using EQX.Core.Motion;
using FASTECH;
using OpenCvSharp.Flann;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace EQX.Motion
{
    public class MotionMasterEziPlusR : MotionMasterBase
    {
        public uint Baudrate { get; set; }

        public MotionMasterEziPlusR(uint port, uint baudrate)
        {
            ControllerId = port;
            Baudrate = baudrate;

            inputStatus = new uint[100];
            for (int i = 0; i < inputStatus.Length; i++)
            {
                inputStatus[i] = 0;
            }
            outputStatus = new uint[100];
            for (int i = 0; i < outputStatus.Length; i++)
            {
                outputStatus[i] = 0;
            }
            validInputSlave = new List<int>();
            validOutputSlave = new List<int>();

            NonOverlappingTimer timer = new NonOverlappingTimer(50);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public uint GetIOInput(int slaveId)
        {
            if (validInputSlave.Contains(slaveId) == false)
            {
                validInputSlave.Add(slaveId);
                uint inStatus = 0;
                int result = NativeLib.FAS_GetIOInput((byte)ControllerId, (byte)slaveId, ref inStatus);
                inputStatus[slaveId] = inStatus;
            }

            return inputStatus[slaveId];
        }

        public uint GetIOOutput(int slaveId)
        {
            if (validOutputSlave.Contains(slaveId) == false)
            {
                validOutputSlave.Add(slaveId);
                uint outStatus = 0;
                int result = NativeLib.FAS_GetIOOutput((byte)ControllerId, (byte)slaveId, ref outStatus);
                outputStatus[slaveId] = outStatus;
            }

            return outputStatus[slaveId];
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsConnected == false) return;

            for (int i = 0; i < validInputSlave.Count; i++)
            {
                uint inStatus = 0;
                int result = NativeLib.FAS_GetIOInput((byte)ControllerId, (byte)validInputSlave[i], ref inStatus);
                if (result == EziMOTIONPlusRLib.FMM_OK)
                {
                    inputStatus[validInputSlave[i]] = inStatus;
                }
                else
                {
                    inputStatus[validInputSlave[i]] = 0;
                }
            }

            for (int i = 0; i < validOutputSlave.Count; i++)
            {
                uint outStatus = 0;
                int result = NativeLib.FAS_GetIOOutput((byte)ControllerId, (byte)validOutputSlave[i], ref outStatus);
                if (result == EziMOTIONPlusRLib.FMM_OK)
                {
                    outputStatus[validOutputSlave[i]] = outStatus;
                }
                else
                {
                    outputStatus[validOutputSlave[i]] = 0;
                }
            }
        }

        public override bool Connect()
        {
            bool result = NativeLib.FAS_Connect((byte)ControllerId, Baudrate) != 0;
            IsConnected = result;
            return result;
        }

        override public bool Disconnect()
        {
            NativeLib.FAS_Close((byte)ControllerId);
            IsConnected = false;
            return true;
        }

        private uint[] inputStatus;
        private uint[] outputStatus;

        private List<int> validInputSlave;
        private List<int> validOutputSlave;
    }
}
