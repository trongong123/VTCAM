using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
  public enum ETrayHead_ZAxisToReadyStep
    {
        Start,
        ZAxis_Move_To_Ready,
        ZAxis_Move_To_Ready_Check,
        TrayPicker_Move_Up,
        TrayPicker_Move_Up_Check,
        End,

    }
}
