using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVAssemble_LoadStep
    {
        Start,       

        StopperUp,
        StopperUp_Check,

        CV_ConditionCheck,

        CVAssemble_LoadRequest_Set,

        CV_StartDetect_Wait,

        CV_Run,
        CV_EndDetect_Wait,
        CV_Stop,

        Cylinder_AlignOn,
        Cylinder_AlignOn_Wait,

        End,
    }
}
