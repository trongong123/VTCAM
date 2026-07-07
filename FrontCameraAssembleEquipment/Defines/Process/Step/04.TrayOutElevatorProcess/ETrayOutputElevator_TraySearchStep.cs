using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayOutputElevator_TraySearchStep
    {
        Start,

        Check_CheckExist,

        ZAxis_Move_ReadyPosition,
        ZAxis_Move_ReadyPosition_Wait,

        Check_IsTraySearch,
        /// <summary>
        /// Incase Tray work possition is higher than require position.
        /// Check if Tray EndUp detected -> Move down until Tray EndUp not detect
        /// </summary>
        ZAxisTrayEndUp_PreCheck,
        ZAxisTrayDown_Move,

        /// <summary>
        /// Check if Tray End not detected -> Move up until Tray EndUp detected
        /// </summary>
        ZAxisTrayEndUp_PostCheck,
        ZAxisTrayUp_Move,

        ZAxis_Stop_Wait,


        ZAxis_MoveDownOffset,
        ZAxis_MoveDownOffset_Check,

        End,
    }
}
