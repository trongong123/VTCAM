using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFilmDetach_OriginStep
    {
        Start,
        CheckOriginSelected,

        FilmGripperOff,
        FilmGripperOff_Check,
        FilmDetachUp,
        FilmDetachUp_Check,
        YAxisFilmOrigin,
        YAxisFilmOrigin_Check,

        YAxisFilmHeadMoveToReadyPos,
        YAxisFilmHeadMoveToReadyPos_Check,
        End
    }
}
