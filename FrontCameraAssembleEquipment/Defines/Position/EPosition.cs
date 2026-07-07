using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EPosition
    {
        // Tray In Elevator Positions
        [Description("Limit Up")]
        TrayInElevator_LimitUpPos,

        [Description("Input Tray")]
        TrayInElevator_InputTrayPos,


        // Tray Out Elevator Positions
        [Description("Ready Place")]
        TrayOutElevator_ReadyPlacePos,

        [Description("Output Tray")]
        TrayOutElevator_OutputTrayPos,

        [Description("Limit Down Tray Search")]
        TrayOutElevator_LimitDownTraySearchPos,

        // Tray Head Positions
        [Description("Ready")]
        TrayHead_ReadyPos,

        [Description("Camera Pick")]
        TrayHead_CamPickPos,

        [Description("Camera Ready Pick")]
        TrayHead_CamReadyPickPos,

        [Description("Camera Scan")]
        TrayHead_CamScanPos,

        [Description("Camera Place")]
        TrayHead_CamPlacePos,

        [Description("Camera Ready Place")]
        TrayHead_CamReadyPlacePos,

        [Description("Tray Pick")]
        TrayHead_TrayPickPos,

        [Description("Tray Place")]
        TrayHead_TrayPlacePos,

        // Film Detach Head Positions
        [Description("Ready")]
        FilmDetachHead_ReadyPos,

        [Description("Front - Vinyl Detach")]
        FilmDetachHead_FrontDetachPos,

        [Description("Rear - Vinyl Detach")]
        FilmDetachHead_RearDetachPos,

        [Description("Front - Vinyl Detach Check")]
        FilmDetachHead_FrontSuctionPos,

        [Description("Rear - Vinyl Detach Check")]
        FilmDetachHead_RearSuctionPos,

        [Description("Front - Clean Vinyl")]
        FilmDetachHead_FrontCleanPos,

        [Description("Rear - Clean Vinyl")]
        FilmDetachHead_RearCleanPos,

        // Camera Head Positions
        [Description("Ready Pick")]
        CamHead_ReadyPickPos,

        [Description("Cam Pick")]
        CamHead_PickPos,

        [Description("Front Ready Place")]
        CamHead_FrontReadyPlacePos,

        [Description("Rear Ready Place")]
        CamHead_RearReadyPlacePos,

        [Description("Front Place 1st")]
        CamHead_FrontPlace1stPos,

        [Description("Rear Place 1st")]
        CamHead_RearPlace1stPos,

        [Description("Front Place 2nd")]
        CamHead_FrontPlace2ndPos,

        [Description("Rear Place 2nd")]
        CamHead_RearPlace2ndPos,

        [Description("Front Pre Push In")]
        CamHead_FrontPrePushInPlacePos,

        [Description("Rear Pre Push In")]
        CamHead_RearPrePushInPlacePos,
    }
}
