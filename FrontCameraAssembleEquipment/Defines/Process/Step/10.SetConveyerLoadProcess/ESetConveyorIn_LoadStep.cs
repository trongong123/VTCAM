using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVIn_LoadStep
    {
        Start,

        CV_Stop1,

        CV_ConditionCheck,
        
        CV_TransferSet_ToEnd,

        CV_EndDetect_Wait,

        CV_Stop2,

        End
    }
}
