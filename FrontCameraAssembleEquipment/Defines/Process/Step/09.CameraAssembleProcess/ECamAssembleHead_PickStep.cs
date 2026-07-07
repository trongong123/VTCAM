using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECamAssembleHead_PickStep
    {
        Start,

        ZAxis_MoveToReadyPickPos,
        ZAxis_MoveToReadyPickPos_Check,
        RXAxis_MoveToPickPos,
        RXAxis_MoveToPickPos_Check,

        CamHeadPositionCheck,

        //XYAxis_MoveToReadyPickPos,
        //XYAxis_MoveToReadyPickPos_Check,

        XYAixs_MoveToPickPos,
        XYAixs_MoveToPickPos_Check,

        Flipper_CamUnloadRequest_Wait,

        ZAixs_MoveToPickPos,
        ZAixs_MoveToPickPos_Check,

        CamPickVacOn,
        CamPickVacOn_Check,

        ZAixs_MoveToPickPosRetry,
        ZAixs_MoveToPickPosRetry_Check,
        CamHead_VacOnDone_Set,

        Flipper_GripOffDone_Wait,

        ZAxis_MoveToReadyPickPosAgain,
        ZAxis_MoveToReadyPickPosAgain_Check,

        Set_Flag_CameraOutDone,

        Wait_FlagOut_GripOffDone_Clear,

        AssembleRequestCheck,

        XYAxis_MoveToReadyPickPosAgain,
        XYAxis_MoveToReadyPickPosAgain_Check,

        CamHead_PickOutDone_Set,

        End,
    }
}
