using EQX.Core.Motion;
using EQX.Motion;
using EQX.ThirdParty.Fastech;
using FASTECH;
using System.Net;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace EQX.InOut
{
    public class PlusROutputDevice<TEnum> : OutputDeviceBase<TEnum> where TEnum : Enum
    {
        public MotionMasterEziPlusR MotionMaster { get; init; }

        public override bool IsConnected => MotionMaster.IsConnected;

        public PlusROutputDevice()
            : base()
        {
        }

        #region Public methods
        #endregion

        protected override bool SetOutput(int index, bool value)
        {
            uint setOutputData = 0x00, resetOutputData = 0x00;

            if (value) setOutputData = outputMask[index];
            else resetOutputData = outputMask[index];

            NativeLib.FAS_SetIOOutput((byte)MotionMaster.ControllerId, (byte)Id, setOutputData, resetOutputData);

            return true;
        }

        protected override bool GetOutput(int index)
        {
            return (MotionMaster.GetIOOutput(Id) & outputMask[index]) > 0;
        }

        private uint[] outputMask = new uint[]
        {
            0x00008000,
            0x00010000,
            0x00020000,
            0x00040000,
            0x00080000,
            0x00100000,
            0x00200000,
            0x00400000,
            0x00800000
        };
    }
}