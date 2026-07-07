using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_PickStep
    {
        Start,

        ConditionCheck,

        //MoveFlipperDown,
        //MoveFlipperDown_Check,

        MoveFlipperUp,
        MoveFlipperUp_Check,

        FlipperUngripAndRotateToPick,
        FlipperUngripAndRotateToPick_Check,

        //MoveFlipperUpAgain,
        //MoveFlipperUpAgain_Check,

        WaitSpongeDetachRequest,
        CheckTrayHeadOutOfPlaceArea,

        MoveFlipMoverToPickPos,
        MoveFlipMoverToPickPos_Check,

        WaitFlipperWorkRequestSignal,

        MovePickupDown,
        MovePickupDown_Check,
        MovePickupDownDone_Wait,

        CamGripperOn,
        CamGripperOn_Check,

        WaitSpongeDetachDoneSignal,

        CamGripperOff,
        CamGripperOff_Check,

        CamGripperOnAgain,
        CamGripperOnAgain_Check,

        Wait_Cylinder_SpongeRemoveBackward,

        MovePickupUp,
        MovePickupUp_Check,
        End
    }
}
