using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
   public enum ETrayInPutElevator_TraySearchStep
    {
        Start,
        Check_Status_IsTraySearch,
        Tray_In_Elevator_UnAlign,
        Tray_In_Elevator_UnAlign_Check,

        /// <summary>
        /// Incase Tray work possition is higher than require position.
        /// Check if Tray EndUp detected -> Move down until Tray EndUp not detect
        /// </summary>
        ZAxisTrayEndUp_PreCheck,
        ZAxisTrayDown_Move,

        /// <summary>
        /// Check if Tray End not detected -> Move up until Tray EndUp detected
        /// </summary>
        ZAxisTrayEndUp_PostCheck_1st,
        ZAxisTrayUp_Move_1st,

        ZAxis_Stop_Wait_1st,

        ZAxis_Move_DownOffset,
        ZAxis_Move_DownOffset_Wait,

        ZAxisTrayEndUp_PostCheck_2nd,
        ZAxisTrayUp_Move_2nd,

        ZAxis_Stop_Wait_2nd,

        ZAxis_Stop_Wait_Before_MoveZLimitUp,
        ZAxis_MoveZLimitUp,
        ZAxis_MoveZLimitUp_Wait,

        TrayEndUp_Detect_Check,

        Tray_In_Elevator_Align,
        Tray_In_Elevator_Align_Check,   
        Tray_In_Elevator_Check_Warning_Empty_Material,          

        End,
    }
}
