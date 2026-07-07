using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESearchZPos
    {

        Idle,
        SafetyCheck,
        ZReady,
        MoveXYAxis,
        MoveZAxis,
        Done,
        Error
    }
}
