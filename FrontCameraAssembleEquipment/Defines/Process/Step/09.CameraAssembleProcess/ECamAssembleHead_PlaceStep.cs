using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECamAssembleHead_PlaceStep
    {
        Start,

        CamAssembleStateCheck,

        ZAXis_MoveToReadyPos,
        ZAXis_MoveToReadyPos_Check,

        CamExistVacCheck,

        CVAssemble_CamAssembleRequest_Wait,
        CamAssembleRequest_Check,

        XYAxis_MoveToReadyPlacePos,
        XYAxis_MoveToReadyPlacePos_Check,

        //RXZAxis_MoveToReadyPlacePos,
        //RXZAxis_MoveToReadyPlacePos_Check,

        MovetoFirstPlacePos,
        MovetoFirstPlacePos_Check,
        MovetoSecondPlacePos,
        MovetoSecondPlacePos_Check,

        RAxis_Back,
        RAxis_Back_Wait,

        CamPickVacOff,
        CamPickVacOff_Check,

        CamHead_CamPlaceDone_Set,

        YAxis_MoveToReadyPlacePosToPush,
        YAxis_MoveToReadyPlacePosToPush_Check,

        MoveToPrePushInUpPos,
        MoveToPrePushInUpPos_Check,
        MoveToPrePushInPos,
        MoveToPrePushInPos_Check,
        MoveToPushInPos,
        MoveToPushInPos_Check,

        MoveBackToPrePushInUpPos,
        MoveBackToPrePushInUpPos_Check,

        CamHead_CamAssembleDone_Set,

        XZRXAxis_MoveToReadyPick,
        XZRXAxis_MoveToReadyPick_Check,

        XYAxis_MoveOutVisionFrontWork,
        XYAxis_MoveOutVisionFrontWork_Check,
        XYAxis_MoveOutVisionFrontWorkDone_Check,

        Wait_CVAssembleRequest_Clear,

        End,
    }
}
