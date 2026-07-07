using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_CamLoadStep
    {
        Start,
        //SpongeGripper_Off,
        //SpongeGripper_Off_Wait,
        CamExistCheck_VacOn,
        CamExistCheck_Check,
        CamCenteringOff,
        CamCenteringOff_Check,
        //SpongeDetach_Up,
        //SpongeDetach_Up_Check,
        SpongeDetach_Bw,
        SpongeDetach_Bw_Check,
        RequestCamIn,
        CheckCamInComplete,
        WaitTrayHeadZUpDone,
        End,
    }
}
