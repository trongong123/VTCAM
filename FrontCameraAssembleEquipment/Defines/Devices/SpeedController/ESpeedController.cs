using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment
{
    public enum ESpeedController
    {
        [Description("Tray In Buffer")]
        TRAYIN_CV_ROLLER = 5,

        [Description("Tray In Lift")]
        TRAYIN_ELEVATOR_ROLLER = 6,

        [Description("Tray Out Buffer")]
        TRAYOUT_CV_ROLLER = 7,

        [Description("Tray Out Lift")]
        TRAYOUT_ELEVATOR_ROLLER = 8,
    }
}
