using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_SpongeRemoveStep
    {
        Start,
        ConditionCheck,

        InterlockConditionCheck,

        CenteringOn,
        CenteringOn_Wait,

        PrealignVacOn,

        Set_Status,

        CenteringOff,
        CenteringOff_Wait,

        PrealignVacOn_Check,

        FlipperInRequest,

        WaitFlipperGripOnDoneSignal,

        SpongePosition_Check,
        SpongeRemoverUnGrip_Blow,
        SpongeRemoverUnGrip_Blow_Check,

        SpongeRemoverMoveUp,
        SpongeRemoverUp_Check,

        SafetyConditionCheck,

        SpongeRemoverMoveIn,
        SpongeRemoverMoveIn_Check,

        SpongeRemoverGripOffBeforeDown,
        SpongeRemoverGripOffBeforeDown_Wait,

        SpongeRemoverMoveDown,
        SpongeRemoverDown_Check,
        SpongeRemoverDownDone_Wait,

        SpongeRemoverVacOn,
        SpongeRemoverVacOn_Check,

        SpongeRemoverGripOn,
        SpongeRemoverGripOn_Check,
        SpongeRemoverGripOnDone_Wait,
        SpongeRemoverGripOffToRetry,
        SpongeRemoverGripOffToRetry_Check,

        SpongeRemoverMoveUpAgain,
        SpongeRemoverMoveUpAgain_Check,

        Set_FlagOut_SpongeRemoveDone,

        WaitFlipperGripOffToCheckCamExist,
        CheckCameraPrealignExist,

        PreAlignVac_Off,
        PreAlignVac_Off_Check,

        SpongeRemoverMoveOut,
        SpongeRemoverMoveOut_Check,

        SpongeRemoverDoneSignal_Set,

        SpongeRemoverDownAgain,
        SpongeRemoverDownAgain_Check,

        SpongeHoldVacCheck,

        SpongeRemoverGripOff,
        SpongeRemoverGripOff_Check,

        SpongeRemoverVacOff,
        SpongeRemoverVacOff_Check,

        TrashSuctionDelay,
        CheckCameraOutDone,

        Wait_GripperRemoveSpongeDone,

        End,
    }
}
