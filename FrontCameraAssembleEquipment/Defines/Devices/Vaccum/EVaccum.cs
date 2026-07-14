using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EVaccum
    {
        [Description("Tray Picker VAC")]
        TrayHead_TrayPickerVac,
        [Description("CAM Picker VAC")]
        TrayHead_CamPickerVac,
        [Description("CAM Align VAC")]
        Prealign_CamHoldVac,
        [Description("Sponge Detach VAC")]
        SpongeDetach_SpongeHoldVac,
        VinylDetach_VinylSuctionVac,
        [Description("CAM Assemble Picker VAC")]
        CamHead_CamPickerVac,
    }
}
