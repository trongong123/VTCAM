using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECameraAssembleHeadOutput
    {
        CAM_PICKUP_DONE,
        VAC_ON_OK,
        CAM_ASSEMBLE_FRONT_DONE,
        CAM_ASSEMBLE_REAR_DONE,
        CAM_ASSEMBLE_FRONT_PLACE_DONE,
        CAM_ASSEMBLE_REAR_PLACE_DONE,

        CAM_ASSEMBLE_AVOID_TO_VISION_FRONT,
        CAM_ASSEMBLE_AVOID_TO_VISION_REAR,

        CAM_ASSEMBLE_HEAD_READY_DONE,
    }
}
