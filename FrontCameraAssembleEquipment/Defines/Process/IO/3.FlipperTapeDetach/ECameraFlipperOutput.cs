using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECameraFlipperOutput
    {
        GRIPER_OFF_DONE,
        GRIPER_ON_DONE,
        CAM_OUT_REQUEST,
        CAM_PICKUP_DONE,
        CAM_ASSEMBLE_HEAD_XAXIS_HOME_DONE_RECEIVED,
        FLIPPER_MOVE_IN_DONE,
        FLIPPER_GRIPPER_OFF_DONE,
    }
}
