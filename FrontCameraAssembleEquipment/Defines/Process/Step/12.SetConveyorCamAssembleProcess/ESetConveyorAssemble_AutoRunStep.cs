using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVAssemble_AutoRunStep
    {
        Start,

        ConditionCheck1,

        StopperUp,
        CV_Run1,
        CV_Stop1,

        ConditionCheck2,

        End
    }
}
