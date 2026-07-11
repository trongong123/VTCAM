using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOneConveyorFrontLoadStep
    {
        Start,
        SetLoadRequest,
        WaitStartSensor,
        CheckStartSensor,
        ClearLoadRequest,
        RunUnitToEnd,
        CheckEndSensor,
        End
    }
}
