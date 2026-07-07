using FrontCameraAssembleEquipment.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class VaccumList
    {
        public Vaccum TrayHead_TrayPickerVac { get; set; }
        public Vaccum TrayHead_CamPickerVac { get; set; }
        public Vaccum Prealign_CamHoldVac { get; set; }
        public Vaccum SpongeDetach_SpongeHoldVac { get; set; }
        public Vaccum VinylDetach_VinylSuctionVac { get; set; }
        public Vaccum CamHead_CamPickerVac { get; set; }
        public Vaccum FrontUnload_CvVac { get; set; }

        public VaccumList(Devices devices)
        {
            _devices = devices;

            TrayHead_TrayPickerVac = new(_devices.Inputs.TrayPickerVacOn) { Id = (int)EVaccum.TrayHead_TrayPickerVac, Name = EVaccum.TrayHead_TrayPickerVac.GetDescription(), VacOnOutput = _devices.Outputs.TrayPickerVacOn, VacOffOutput = _devices.Outputs.TrayPickerVacOff };
            TrayHead_CamPickerVac = new(_devices.Inputs.VtCamSupplyPnPVacOn) { Id = (int)EVaccum.TrayHead_CamPickerVac, Name = EVaccum.TrayHead_CamPickerVac.GetDescription(), VacOnOutput = _devices.Outputs.VtCamSupplyPnPVacOn, VacOffOutput = _devices.Outputs.VtCamSupplyPnPVacOff };
            Prealign_CamHoldVac = new(_devices.Inputs.VtCamPrealignVacOn) { Id = (int)EVaccum.Prealign_CamHoldVac, Name = EVaccum.Prealign_CamHoldVac.GetDescription(), VacOnOutput = _devices.Outputs.VtCamPrealignVacOn, VacOffOutput = _devices.Outputs.VtCamPrealignVacOff };
            SpongeDetach_SpongeHoldVac = new(_devices.Inputs.SpongeHoldDetect) { Id = (int)EVaccum.SpongeDetach_SpongeHoldVac, Name = EVaccum.SpongeDetach_SpongeHoldVac.GetDescription(), VacOnOutput = _devices.Outputs.SpongeHoldVacOn, VacOffOutput = _devices.Outputs.SpongeHoldVacOff };
            VinylDetach_VinylSuctionVac = new(_devices.Inputs.FilmDetachSuctionVacOn) { Id = (int)EVaccum.VinylDetach_VinylSuctionVac, Name = EVaccum.VinylDetach_VinylSuctionVac.GetDescription(), VacOnOutput = _devices.Outputs.FilmDetachSuctionVacOn, VacOffOutput = _devices.Outputs.FilmDetachSuctionVacOff };
            CamHead_CamPickerVac = new(_devices.Inputs.VtCamAssemblePnPVacOn) { Id = (int)EVaccum.CamHead_CamPickerVac, Name = EVaccum.CamHead_CamPickerVac.GetDescription(), VacOnOutput = _devices.Outputs.VtCamAssemblePnPVacOn, VacOffOutput = _devices.Outputs.VtCamAssemblePnPPurgeOn };
            FrontUnload_CvVac = new(_devices.Inputs.FrontUnloadCvVacOn) { Id = (int)EVaccum.FrontUnload_CvVac, Name = EVaccum.FrontUnload_CvVac.GetDescription(), VacOnOutput = _devices.Outputs.FrontUnloadCvVacOn, VacOffOutput = _devices.Outputs.FrontUnloadCvVacOff };
        }

        private Devices _devices;
    }
}
