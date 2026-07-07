using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayOutputElevator_UnloadStep
    {
        Start,
        Tray_Output_Elevator_Cv_Stop,
        Move_Tray_Output_Elevator_to_Unload_Position,
        Move_Tray_Output_Elevator_to_Unload_Position_Check,
        Tray_Out_Elevator_Request,
        Tray_Out_Elevator_Request_Check,
        CV_Tray_Output_Elevator_Run,
        Wait_Tray_Output_Elevator_Cv_NotDetect_End,
        Tray_Output_Elevator_Cv_Stop_Again,
        End,
    }
}
