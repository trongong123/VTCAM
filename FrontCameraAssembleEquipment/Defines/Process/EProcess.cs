using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Process
{
    public enum EProcess
    {
        Root,
        [Description("Tray IN Buffer")]
        TrayInCV,

        [Description("Tray IN Lift")]
        TrayInElevator,

        [Description("Tray OUT Buffer")]
        TrayOutCV,

        [Description("Tray OUT Lift")]
        TrayOutElevator,

        [Description("Tray CAM Loader")]
        TrayHead,

        [Description("CAM Sponge Detach")]
        SpongeDetach,

        [Description("CAM Rotator")]
        CameraRotator,

        [Description("Vinyl Detach")]
        FilmDetach,

        [Description("CAM Assemble")]
        CameraAssemble,

        [Description("Front IN CV")]
        FrontSetCVIn,

        [Description("Front OUT CV")]
        FrontSetCVOut,

        [Description("Front Detach CV")]
        FrontSetCVDetach,

        [Description("Front Assemble CV")]
        FrontSetCVAssemble,

        [Description("Rear IN CV")]
        RearSetCVIn,

        [Description("Rear OUT CV")]
        RearSetCVOut,

        [Description("Rear Detach CV")]
        RearSetCVDetach,

        [Description("Rear Assemble CV")]
        RearSetCVAssemble,
    }
}
