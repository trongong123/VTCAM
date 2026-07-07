using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInputElevator_OriginStep
    {
        Start,
        CheckOriginSelected,
        Stop_CV,
        TrayCenteringOff,
        TrayCenteringOff_Check,
        ZAxisElevatorHomeSearch,
        ZAxisElevatorHomeSearch_Check,
        End,
    }
}
