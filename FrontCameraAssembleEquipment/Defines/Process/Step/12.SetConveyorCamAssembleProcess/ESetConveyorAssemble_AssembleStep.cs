using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVAssemble_AssembleStep
    {
        Start,

        Cyl_Align_On,
        Cyl_Align_On_Check,

        VisionConditionCheck,

        WaitCamHeadOutSignaltoFilmInspection,

        FilmInspect_Vision_Request_Set,
        FilmInspect_Vision_Response_Wait,
        FilmInspect_Vision_ResultHandle,

        CamAssemble_Request_Set,
        CamAssemble_Done_Wait,

        WaitCamHeadOutSignalToCamInspection,

        CamAssemble_Inspection_Request_Set,
        CamAssemble_Inspection_Response_Wait,
        CamAssemble_Inspection_ResultHandle,

        CylAlignOffToRetry,
        CylAlignOffToRetry_Check,
        CylAlignOnToRetry,
        CylAlignOnToRetry_Check,

        CyAlignOff_StopperDown,
        CylAlignOff_StopperDown_Wait,

        End,
    }
}
