using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetConveyerIn_LoadAutoStep
    {
        Start,

        CV_Stop1,

        CV_ConditionCheck,

        CV_Wait_IF_PreMC_On,

        CV_Stop2,

        CV_Wait_IF_PreMC_Off,

        CV_Wait_LoadCvStart,

        CV_TransferSet_ToEnd,

        CV_EndDetect_Wait,

        CV_Stop3,

       
        End
    }
}
