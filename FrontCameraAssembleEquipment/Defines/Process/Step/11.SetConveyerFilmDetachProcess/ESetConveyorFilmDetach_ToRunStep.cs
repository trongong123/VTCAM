using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVFilmDetach_ToRunStep
    {
        Start,

        InternalInOutSignal_Reset,

        EndSensorSetDetect,   
        EndSensorSetConfirm,

        End
    }
}
