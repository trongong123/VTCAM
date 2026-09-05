using System.ComponentModel;

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

        [Description("FrontCV Pre-Load")]
        SetWork_FrontPreLoadCV,

        [Description("FrontCV Load Input")]
        SetWork_FrontSetLoadInput,

        [Description("FrontCV_Vinyl Detach")]
        SetWork_FrontSetFilmDetach,

        [Description("FrontCV_Cam Assemble")]
        SetWork_FrontSetCamAssemble,

        [Description("FrontCV_Unload Output")]
        SetWork_FrontSetUnloadOutput,

        [Description("RearCV Pre-Load")]
        SetWork_RearPreLoadCV,

        [Description("RearCV Load Input")]
        SetWork_RearSetLoadInput,

        [Description("RearCV_Vinyl Detach")]
        SetWork_RearSetFilmDetach,

        [Description("RearCV_Cam Assemble")]
        SetWork_RearSetCamAssemble,

        [Description("RearCV_Unload Output")]
        SetWork_RearSetUnloadOutput,
    }
}
