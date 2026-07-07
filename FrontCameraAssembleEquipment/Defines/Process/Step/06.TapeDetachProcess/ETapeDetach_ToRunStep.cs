using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_ToRunStep
    {
        Start,

        InternalInOutSignal_Reset,

        MaterialDataMatching_VacOn,
        MaterialDataMatching_Check,

        ErrorCheck,

        End,
    }
}
