using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECV
    {
        [Description("Tray IN External")]
        TraySup_TrayInExternal,

        [Description("Tray In Buffer")]
        TraySup_TrayInput,

        [Description("Tray In Lift")]
        TraySup_TrayInElevator,

        [Description("Tray OUT External")]
        TraySup_TrayOutExternal,

        [Description("Tray Out Buffer")]
        TraySup_TrayOutput,

        [Description("Tray Out Lift")]
        TraySup_TrayOutElevator,

        [Description("")]
        SetWork_FrontPreLoadCV,

        [Description("")]
        SetWork_FrontSetLoadInput,

        [Description("FrontCV_Vinyl Detach")]
        SetWork_FrontSetFilmDetach,

        [Description("FrontCV_Cam Assemble")]
        SetWork_FrontSetCamAssemble,

        [Description("FrontCV_Unload Output")]
        SetWork_FrontSetUnloadOutput,

        [Description("")]
        SetWork_RearPreLoadCV,

        [Description("")]
        SetWork_RearSetLoadInput,

        [Description("RearCV_Vinyl Detach")]
        SetWork_RearSetFilmDetach,

        [Description("RearCV_Cam Assemble")]
        SetWork_RearSetCamAssemble,

        [Description("RearCV_Unload Output")]
        SetWork_RearSetUnloadOutput,
    }
}
