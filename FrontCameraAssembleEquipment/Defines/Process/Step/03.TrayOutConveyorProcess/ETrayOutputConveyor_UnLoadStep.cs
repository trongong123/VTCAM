using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayOutputCV_UnLoadStep
    {
        Start,
        Tray_Out_CV_Cv_Stop,

        Tray_Out_CV_Request_Agv,
        Tray_Out_CV_Wait_Agv_Ready,

        Tray_Out_CV_Run,
        Tray_Out_CV_Wait_Unload_Done,
        Tray_Out_CV_Stop_End,

        Wait_AGV_UnloadTray,
        
        End,
    }
}
