using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECamAssembleHead_ToRunStep
    {
        Start,

        InternalInOutSignal_Reset,

        MaterialDataMatching_VacOn,
        MaterialDataMatching_Check,

        End,
    }
}
