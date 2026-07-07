using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ERearCvCamAssembleInput
    {
        CAM_ASSEMBLE_REAR_WAIT_PUSH,
        CAM_ASSEMBLE_DONE,
        REAR_UNLOAD_REQUEST,

        VISION_INSPECTION_RUN,
        VISION_REAR_FILM_INSPECTION_ERROR,
        VISION_REAR_ASSEMBLE_INSPECTION_ERROR,

        CAM_ASSEMBLE_AVOID_TO_VISION
    }
}
