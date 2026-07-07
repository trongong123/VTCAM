using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInElevatorOutput
    {
        TRAY_INPUT_ELEVATOR_REQUEST,
        TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ,
        TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ,

        TRAY_IN_ELEVATOR_UNALIGN_DONE,
        TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED,
    }
}
