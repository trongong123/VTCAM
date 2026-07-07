using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayOutputCV_LoadStep
    {
        Start,

        Check_Tray_Out_CV_Detect_Start,
        Tray_Out_CV_Run,
        Wait_Tray_Out_CV_Detect_End,
        Tray_Out_CV_Stop,
        Tray_Out_CV_Request,
        Tray_Out_CV_Request_Check,
        Tray_Out_CV_Run_Again,
        Wait_Tray_Out_CV_Detect_Sensor_End,
        Tray_Out_CV_Stop_Again,

        End,
    }
}
