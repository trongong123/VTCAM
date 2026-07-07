using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum ECylinder
    {
        //Unit: Tray Supplier

        [Description("Stopper CYL")]
        TraySupplier_TrayInStopper,

        [Description("Centering CYL")]
        TraySupplier_TrayCentering1,

        [Description("Centering CYL")]
        TraySupplier_TrayCentering2,

        //Unit: Picker Head
        [Description("Picker CYL")]
        TrayPicker_UpDown,

        //Unit: Flipper Sponge Detach
        [Description("PreAlign CYL")]
        SpongeDetach_VtCamCentering,

        [Description("Sponge FW/BW CYL")]
        SpongeDetach_SpongePickupMoverFwBw,

        [Description("Sponge UP/DOWN CYL")]
        SpongeDetach_SpongePickupMoverUpDn,

        [Description("Sponge Gripper CYL")]
        SpongeDetach_SpongeHoldGripper,

        [Description("Rotator FW/BW CYL")]
        Flipper_VtCamRotatorMoverFwBw,

        [Description("Rotator UP/DOWN CYL")]
        Flipper_VtCamRotatorMoverUpDn,

        [Description("Rotator Gripper CYL")]
        Flipper_VtCamRotatorGripper,

        [Description("Rotator CYL")]
        Flipper_VtCamRotatorFlipper,

        //Unit: Film Detach Head
        [Description("Vinyl UP/DOWN CYL")]
        FilmDetach_MoverUpDn,

        [Description("Suction UP/DOWN CYL")]
        FilmDetach_SuctionUpDn,

        [Description("Vinyl Gripper CYL")]
        FilmDetach_GripperOnOff,

        //Unit: Cam Head
        [Description("CAM Head UP/DOWN CYL")]
        CamHead_MoverUpDn,

        //Unit: Case Load CV
        [Description("Stopper CYL")]
        SetCV_FrontDetachStopperUpDn,

        [Description("Align CYL")]
        SetCV_FrontDetachCentering,

        [Description("Stopper CYL")]
        SetCV_FrontAssembleStopperUpDn,

        [Description("Align CYL")]
        SetCV_FrontAssembleCentering,

        [Description("Stopper CYL")]
        SetCV_RearDetachStopperUpDn,

        [Description("Align CYL")]
        SetCV_RearDetachCentering,

        [Description("Stopper CYL")]
        SetCV_RearAssembleStopperUpDn,

        [Description("Align CYL")]
        SetCV_RearAssembleCentering,

        [Description("UP/DOWN CYL")]
        SetCV_FrontUnloadMoverUpDn,

        [Description("UP/DOWN CYL")]
        SetCV_RearUnloadMoverUpDn,
    }
}
