using EQX.Core.Motion;
using EQX.Motion;
using EQX.ThirdParty.Fastech;
using FASTECH;
using System.Net;
using NativeLib = FASTECH.EziMOTIONPlusRLib;

namespace EQX.InOut
{
    public class PlusRInputDevice<TEnum> : InputDeviceBase<TEnum> where TEnum : Enum
    {
        // TODO : InOut tham chieu den MOTION????
        public MotionMasterEziPlusR MotionMaster { get; init; }

        public override bool IsConnected => MotionMaster.IsConnected;

        #region Constructor(s)
        public PlusRInputDevice()
            : base()
        {
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        protected override bool ActualGetInput(int index)
        {
            if (index > inputMask.Count() - 1) return false;

            return (MotionMaster.GetIOInput(Id) & inputMask[index]) > 0;
        }
        #endregion

        private uint[] inputMask = new uint[]
        {
            0x04000000,
            0x08000000,
            0x10000000,
            0x20000000,
            0x40000000,
            0x80000000,
            0x00000200,
            0x00000400,
            0x00000800
        };

        #region Privates
        #endregion
    }
}