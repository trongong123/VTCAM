using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_AutoRunStep
    {
        Start,

        CheckPreAlginCamExist_VacOn,

        CheckPreAlginCamExist,

        CheckSpongeDetachDone,

        End,
    }
}
