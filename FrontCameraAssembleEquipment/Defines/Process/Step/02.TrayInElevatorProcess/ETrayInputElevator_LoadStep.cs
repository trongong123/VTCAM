using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInputElevator_LoadStep
    {
        Start,
      
        Tray_Input_Elevator_Cv_Exist_Check,
        Elevator_Input_Position_Move,      
        Elevator_Input_Position_Move_Check, 
        Elevator_Input_Request_Input_Tray,
        

        Tray_Start_Sensor_Check,
        Input_Elevator_Cv_Start,
        Tray_End_Sensor_Check,
        Input_Elevator_Cv_Stop,   
        Reset_Status_Camera,
        End,

    }
}
   
