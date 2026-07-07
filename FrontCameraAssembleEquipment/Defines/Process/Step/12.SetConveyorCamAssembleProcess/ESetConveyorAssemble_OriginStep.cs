using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVAssemble_OriginStep
    {
        Start,

        CV_Stop,
        Cyl_Align_Off,
        Cyl_Align_Off_Check,
        Cyl_Stopper_Down,
        Cyl_Stopper_Down_Check,

        End,
    }
}
