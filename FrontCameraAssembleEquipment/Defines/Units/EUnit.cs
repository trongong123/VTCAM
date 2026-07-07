using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Define
{
    public enum EUnit
    {
        [Description("Tray Supplier")]
        TraySupplier,

        [Description("Tray CAM Loader")]
        TrayHead,

        [Description("CAM Sponge Detach")]
        SpongeDetach,

        [Description("Vinyl Detach")]
        FilmDetach,

        [Description("CAM Assemble")]
        CameraAssemble,

        [Description("CV")]
        SetCV,

        [Description("Vision")]
        Vision,
    }
}
