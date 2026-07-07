using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECamAssembleHead_OriginStep
    {
        Start,

        CheckOriginSelected,

        ZAXisAssembleHeadOrigin,
        ZAXisAssembleHeadOrigin_Check,

        CylinderUp,
        CylinderUp_Check,

        RXAisAssembleHeadOrigin,
        RXAisAssembleHeadOrigin_Check,

        XYAisAssembleHeadOrigin,
        XYAisAssembleHeadOrigin_Check,

        ZAxis_MoveToReadyPickPos,
        ZAxis_MoveToReadyPickPos_Check,
        RXAxis_MoveToReadyPickPos,
        RXAxis_MoveToReadyPickPos_Check,

        XYAxis_MoveToReadyPickPos,
        XYAxis_MoveToReadyPickPos_Check,

        Set_FlagCamAssembleHeadHomeDone,

        End
    }
}
