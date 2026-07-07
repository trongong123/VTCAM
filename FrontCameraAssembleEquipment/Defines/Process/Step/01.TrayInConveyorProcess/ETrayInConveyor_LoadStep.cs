using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInCV_LoadStep
    {
        Start,

        Stopper_Up,
        Stopper_Up_Check,

        AGV_Call,
        AGV_Arrival_Check,
        AGV_SendWorkPermission,
        AGV_Wait_StartWorkSignal,

        Wait_TrayDetect,
        Enable_LightCurtainMuting,

        CV_Run,
        TrayInCV_DetectEnd_Wait,
        CV_Stop,
        
        AGV_SendGoPermission,
        AGV_GoPermissionConfirm_Wait,
        
        End,
    }
}
 