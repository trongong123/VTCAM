using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EUnitChangeReadyPreProcessStep
    {
        Start,
        CrurrentSequence_Check,
        //Wait_Signal_LightCurtain_Or_MuttingSW,
        // CurrentSequence is Run Auto -> Chanege Tray
        // Use Light Curtain
        //Light_Curtain_Detect,
        //Light_Curtain_Stop_Axis,
        //Light_Curtatin_Check_Time_Detect,
        //Light_Curtain_Alarm_TimeOut,
        // Use Mutting SW
        ChangeSW_PressDetect,
        ChangeSW_PressHold_Wait,
        ChangeSW_PressDetect_Confirm,
        ChangeTray_Enable,
        SW_Stop_Axis,
        // CurrentSequence is Change Tray -> Run Auto
        ReadySW_PressDetect,
        ReadySW_PressHold_Wait,
        Ready_Enable,

        End,
    }
}
