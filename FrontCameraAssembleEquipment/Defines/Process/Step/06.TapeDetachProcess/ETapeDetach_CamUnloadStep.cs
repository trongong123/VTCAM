using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_CamUnloadStep
    {
        Start,
        CamCenteringOff,
        CamCenteringOff_Check,
        
        CamCemteringRetryOn,
        CamCemteringRetryOn_Check,
        CamCemteringRetryOff,
        CamCemteringRetryOff_Check,

        PrealignVacCheck,

        RequestCameraOut,
        CamPrealignVacOff,
        CamPrealignVacOff_Check,
        CheckCameraOutDone,
        End
    }
}
