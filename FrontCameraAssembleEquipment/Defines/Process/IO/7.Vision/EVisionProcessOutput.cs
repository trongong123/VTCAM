using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EVisionProcessOutput
    {
        VISION_INSPECTION_RUN,
        VISION_FRONT_FILM_INSPECTION_ERROR,
        VISION_REAR_FILM_INSPECTION_ERROR,
        VISION_FRONT_ASSEMBLE_INSPECTION_ERROR,
        VISION_REAR_ASSEMBLE_INSPECTION_ERROR,

        SCAN_BARCODE_RUN,
        SCAN_BARCODE_ERROR,
    }
}
