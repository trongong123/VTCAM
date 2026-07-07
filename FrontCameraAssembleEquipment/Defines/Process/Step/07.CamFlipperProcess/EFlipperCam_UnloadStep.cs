using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_UnloadStep
    {
        Start,

        FlipperConditionCheck,

        MoveFlipperUp,
        MoveFlipperUp_Check,

        MoveFlipperToUnloadAndPosRotate,
        MoveFlipperToUnloadPosAndRotate_Check,

        SpongeExisCheck,

        CamExistCheck,

        Wait_RemoveSpongeDoneClear,

        RequestCamUnload,
        CheckCamUnload,
        CamGripperOff,
        CamGripperOff_Check,
        CheckCamUnloadComplete,
        End,
    }
}
