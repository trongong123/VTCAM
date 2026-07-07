using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_AutoRunStep
    {
        Start,

        FlipperConditionCheck,

        FlipperMoveUp,
        FlipperMoveUp_Check,

        MoveToReadyPos,
        MoveToReadyPos_Check,

        DelayToCheckCamExist,
        CheckCamExist,
        CheckSpongeExist,
        End,
    }
}
