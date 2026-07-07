using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHeadOutput
    {
        TRAY_IN_ELEVATOR_TRAYHEAD_VAC_ON,
        TRAY_IN_ELEVATOR_UNLOAD_TRAY_DONE,
        TRAY_IN_ELEVATOR_CAM_UNLOAD_DONE,
        TRAYHEAD_CAM_UNLOAD_PICK_FAIL,
        TAPE_DETACH_CAM_IN_DONE,
        TRAY_OUT_ELEVATOR_PLACE_DONE,
        TRAYHEAD_OUT_OF_PLACE_AREA,
        TRAYHEAD_Z_UP_DONE,
        TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED,
    }
}
