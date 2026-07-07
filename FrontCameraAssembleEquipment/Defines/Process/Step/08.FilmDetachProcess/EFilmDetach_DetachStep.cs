using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFilmDetach_DetachStep
    {
        Start,
        CheckFilmExsit,

        CylHeadUpDownSafety,
        CylHeadUpDownSafety_Check,

        CheckFlagFilmRequestCondition,

        MoveToReadyPos,
        MoveToReadyPos_Check,

        CylGripperOff,
        CylGripperOff_Check,

        CheckFilmDetachRequest,

        MoveToFilmDetachPos,
        MoveToFilmDetachPos_Check,

        WaitFilmDetachStartSignal,

        DirectOfSetCheck,
        DirectOfSetCheckWait,

        MoveToShiftDetachPos,
        MoveToShiftDetachPos_Check,

        MoveCylinderHeadDown,
        MoveCylinderHeadDown_Check,

        FilmGripperOn,
        FilmGripperOn_Check,

        MoveCylinderHeadUp,
        MoveCylinderHeadUp_Check,

        // TODO: REMOVE THIS WHEN RETURN SUCTION SEQUENCE
        IonizerOn,
        //----------------------------------------------

        // TODO: SUCTION SEQ - UNCOMMENT THIS

        MoveToVinylDetachCheckPos,
        MoveToVinylDetachCheckPos_Wait,

        VinylDetach_Check,

        MoveToGarbagePos,
        MoveToGarbagePos_Check,

        FilmDetachSignal_Reset,

        CylinderHeadDown,
        CylinderHeadDown_Check,

        TrashSuctionOnToRemoveFilm,

        FilmGripperOff,
        FilmGripperOff_Check,

        FilmGripperOnAgain,
        FilmGripperOnAgain_Check,

        MoveToCleanFilmPos,
        MoveToCleanFilmPos_Check,

        TrashSuctionDelay,

        CylinderHeadUp,
        CylinderHeadUp_Check,
        End,
    }
}
