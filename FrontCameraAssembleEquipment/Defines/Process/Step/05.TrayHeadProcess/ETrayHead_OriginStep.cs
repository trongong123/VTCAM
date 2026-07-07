using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHead_OriginStep
    {
        Start,
        CheckOriginSelected,
        Cyl_TrayPicker_Up,
        Cyl_TrayPicker_Up_Wait,
        ZAxis_Origin,
        ZAxis_Origin_Wait,
        Set_FlagTrayHeadZAxisHomeDone,
        XYAxis_Origin,
        XYAxis_Origin_Wait,
        ZAxis_MoveToWaitPos,
        ZAxis_MoveToWaitPos_Check,
        XYAxis_MoveToWaitPos,
        XYAxis_MoveToWaitPos_Check,
        End,
    }
}
