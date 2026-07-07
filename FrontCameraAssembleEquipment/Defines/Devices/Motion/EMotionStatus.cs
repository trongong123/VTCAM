using System.ComponentModel;

namespace FrontCameraAssembleEquipment.Defines
{
    public partial class Motions
    {
        public enum EMotionStatus
        {
            None = 0,

            [Description("Power Off")]
            PowerOff,

            [Description("Move Time Out")]
            MoveTimeOut,

            [Description("Move Fail")]
            MoveFail,

            [Description("Move Done")]
            MotionDone,
        }
    }
}
