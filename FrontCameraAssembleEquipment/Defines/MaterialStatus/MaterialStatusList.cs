using FrontCameraAssembleEquipment.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class MaterialStatusList
    {
        private readonly Devices _devices;

        public MaterialStatus TrayInMaterialStatus { get; set; }
        public MaterialStatus TrayOutMaterialStatus { get; set; }
        public MaterialStatus TrayHeadMaterialStatus { get; set; }
        public MaterialStatus PreAlignMaterialStatus { get; set; }
        public MaterialStatus RotatorMaterialStatus { get; set; }
        public MaterialStatus CamHeadMaterialStatus { get; set; }

        public MaterialStatus RearSetInCvMaterialStatus { get; set; }
        public MaterialStatus RearSetDetachCvMaterialStatus { get; set; }
        public MaterialStatus RearSetAssembleCvMaterialStatus { get; set; }
        public MaterialStatus RearSetOut1CvMaterialStatus { get; set; }
        public MaterialStatus RearSetOut2CvMaterialStatus { get; set; }
        public MaterialStatus RearSetOut3CvMaterialStatus { get; set; }

        public MaterialStatus FrontSetInCvMaterialStatus { get; set; }
        public MaterialStatus FrontSetDetachCvMaterialStatus { get; set; }
        public MaterialStatus FrontSetAssembleCvMaterialStatus { get; set; }
        public MaterialStatus FrontSetOut1CvMaterialStatus { get; set; }
        public MaterialStatus FrontSetOut2CvMaterialStatus { get; set; }
        public MaterialStatus FrontSetOut3CvMaterialStatus { get; set; }
        public MaterialStatusList(Devices devices)
        {
            TrayInMaterialStatus = new MaterialStatus() { Name = "Tray IN", Status = EMaterialStatus.NotExist, Type = EMaterialType.Tray, IsEditable = false };
            TrayOutMaterialStatus = new MaterialStatus() { Name = "Tray OUT", Status = EMaterialStatus.NotExist, Type = EMaterialType.Tray, IsEditable = true };
            TrayHeadMaterialStatus = new MaterialStatus() { Name = "CAM Loader", Status = EMaterialStatus.NotExist, Type = EMaterialType.Camera, IsEditable = true };
            PreAlignMaterialStatus = new MaterialStatus() { Name = "CAM Detach", Status = EMaterialStatus.NotExist, Type = EMaterialType.Camera, IsEditable = true };
            RotatorMaterialStatus = new MaterialStatus() { Name = "Rotator", Status = EMaterialStatus.NotExist, Type = EMaterialType.Camera, IsEditable = true };
            CamHeadMaterialStatus = new MaterialStatus() { Name = "CAM Assy", Status = EMaterialStatus.NotExist, Type = EMaterialType.Camera, IsEditable = true };
            
            RearSetInCvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };
            RearSetDetachCvMaterialStatus = new MaterialStatus() { Name = "Detach", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };
            RearSetAssembleCvMaterialStatus = new MaterialStatus() { Name = "Assemble", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };
            RearSetOut1CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };
            RearSetOut2CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };
            RearSetOut3CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Rear };

            FrontSetInCvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            FrontSetDetachCvMaterialStatus = new MaterialStatus() { Name = "Detach", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            FrontSetAssembleCvMaterialStatus = new MaterialStatus() { Name = "Assemble", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            FrontSetOut1CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            FrontSetOut2CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            FrontSetOut3CvMaterialStatus = new MaterialStatus() { Name = "", Status = EMaterialStatus.NotExist, Type = EMaterialType.Front, IsEditable = true, CVLine = Process.ECVLine.Front };
            _devices = devices;
        }

        public bool SpongeDetachEnable
        {
            get
            {
                return (CountCamNeedSpongeDetach > 0);
                //return (_devices.Inputs.FrontDetachCvStart.Value || _devices.Inputs.FrontDetachCvEnd.Value) ||
                //       (_devices.Inputs.RearDetachCvStart.Value || _devices.Inputs.RearDetachCvEnd.Value) ||
                //       (_devices.Inputs.FrontAssembleCvStart.Value || (_devices.Inputs.FrontAssembleCvEnd.Value && FrontSetAssembleCvMaterialStatus.ProcessStatus != EMaterialProcessStatus.Done)) ||
                //       (_devices.Inputs.RearAssembleCvStart.Value || (_devices.Inputs.RearAssembleCvEnd.Value && RearSetAssembleCvMaterialStatus.ProcessStatus != EMaterialProcessStatus.Done));
            }
        }
        private int CountCamNeedSpongeDetach
        {

            get
            {
                int _count = 0;
                if (_devices.Inputs.FrontDetachCvStart.Value || _devices.Inputs.FrontDetachCvEnd.Value) _count++;
                if (_devices.Inputs.RearDetachCvStart.Value || _devices.Inputs.RearDetachCvEnd.Value) _count++;
                if (_devices.Inputs.FrontAssembleCvEnd.Value && FrontSetAssembleCvMaterialStatus.CameraStatus == ECameraStatus.None) _count++;
                if (_devices.Inputs.RearAssembleCvEnd.Value && RearSetAssembleCvMaterialStatus.CameraStatus ==  ECameraStatus.None) _count++;
                if (_devices.Inputs.VtCamRotatorDetect.Value) _count--;
                if (_devices.Inputs.VtCamAssemblePnPVacOn.Value ||
                    ((_devices.Inputs.VtCamAssemblePnPVacOn.Value == false)
                    && ((_devices.Inputs.FrontAssembleCvEnd.Value && FrontSetAssembleCvMaterialStatus.CameraStatus ==  ECameraStatus.Exist) 
                    || (_devices.Inputs.RearAssembleCvEnd.Value && RearSetAssembleCvMaterialStatus.CameraStatus == ECameraStatus.Exist)) )) _count--;
                return _count;
            }
        }
    }
}
 