using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHeadInput
    {
        TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ,
        TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ,
        TAPE_DETACH_CAM_IN_REQ,
        TRAY_OUT_ELEVATOR_READY_PLACE,

        TRAY_IN_ELEVATOR_UNALIGN_DONE,
        TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED,
        TRAY_HEAD_SCAN_BARCODE_RUN,
        TRAY_HEAD_SCAN_BARCODE_ERROR,
    }
}
