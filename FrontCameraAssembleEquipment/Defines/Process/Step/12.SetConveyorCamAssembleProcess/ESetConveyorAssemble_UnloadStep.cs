using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVAssemble_UnloadStep
    {
        Start,

        CVOut_Request_Wait,
        StopperDown,
        StopperDown_Check,
        CV_SetUnloadStart,
        ProductDataSet,
        CVOut_SetReceive_Wait,
        CV_UnloadStop,

        End,
    }
}
