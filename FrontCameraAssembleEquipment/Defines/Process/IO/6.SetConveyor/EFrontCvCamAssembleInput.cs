using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFrontCvCamAssembleInput
    {
        CAM_ASSEMBLE_FRONT_WAIT_PUSH,
        CAM_ASSEMBLE_DONE,
        FRONT_UNLOAD_REQUEST,

        VISION_INSPECTION_RUN,
        VISION_FRONT_FILM_INSPECTION_ERROR,
        VISION_FRONT_ASSEMBLE_INSPECTION_ERROR,

        CAM_ASSEMBLE_AVOID_TO_VISION
    }
}
