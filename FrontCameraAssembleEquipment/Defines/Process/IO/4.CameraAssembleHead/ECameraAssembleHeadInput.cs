using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
  public enum ECameraAssembleHeadInput
    {
        GRIPPER_OFF_DONE,
        FLIPPER_CAM_OUT_REQUEST,
        REAR_CAM_ASSEMBLE_REQUEST,
        FRONT_CAM_ASSEMBLE_REQUEST,
        CAM_ASSEMBLE_HEAD_XAXIS_HOME_DONE_RECEIVED,

        VISION_RUNNING,
    }
}
