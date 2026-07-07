using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInPutElevator_TrayUnloadStep
    {
        Start,
        Tray_Input_Elevator_Cv_Out_Request,
        Tray_In_Elevator_Wait_TrayHead_VacOn,
        Tray_In_Elevator_UnAlign,
        Tray_In_Elevator_UnAlign_Check,
        SetFlag_Tray_In_Elevator_UnAlign_Done,
        Tray_Input_Elevator_Cv_Out_Done_Check,
        Tray_In_Elevator_Reset,
        Tray_in_Elevator_Check_Status_Exist,
        End,
    }
}
