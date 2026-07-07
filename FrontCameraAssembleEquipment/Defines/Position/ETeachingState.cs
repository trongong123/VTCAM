using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETeachingState
    {
        Idle,
        SafetyCheck,
        ZReady,
        MoveAxisFirst,
        MoveAxisSecond,
        Done,
        Error
    }
}
