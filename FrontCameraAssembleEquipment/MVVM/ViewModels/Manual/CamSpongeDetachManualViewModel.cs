using EQX.Core.Common;
using EQX.Core.Helpers;
using EQX.Core.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class CamSpongeDetachManualViewModel : AppManualViewModel
    {
        private readonly VaccumList _vaccumList;

        public CamSpongeDetachManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList, Devices devices, ProcessConfig processConfig, VaccumList vaccumList , RecipeList recipeList) 
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices , processConfig)
        {
            _vaccumList = vaccumList;
        }

        //Sponge Detach
        public ICylinder PreAlignCylinder => Devices.Cylinders.FlipperSpongeDetach_VtCamCentering;
        public ICylinder SpongeFwBwCylinder => Devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw;
        public ICylinder SpongeUpDownCylinder => Devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn;
        public ICylinder SpongeGripperCylinder => Devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper;
        public Vaccum CamAlignVac => _vaccumList.Prealign_CamHoldVac;
        public Vaccum SpongeDetachVac => _vaccumList.SpongeDetach_SpongeHoldVac;

        //Rotator
        public ICylinder RotatorFwBwCylinder => Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw;
        public ICylinder RotatorUpDownCylinder => Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn;
        public ICylinder RotatorGripperCylinder => Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper;
        public ICylinder RotatorFlipperCylinder => Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper;
        
        protected override void ActualInit()
        {
            var inputsTemp = PropertyHelpers.GetProperties<IDInput>(_processes.SpongeDetachProcess);
            AddRange(PropertyHelpers.GetProperties<IDInput>(_processes.CameraFlipperProcess), inputsTemp);
            Inputs = new List<IDInput>(inputsTemp.ToHashSet());

            var outputsTemp = PropertyHelpers.GetProperties<IDOutput>(_processes.SpongeDetachProcess);
            AddRange(PropertyHelpers.GetProperties<IDOutput>(_processes.CameraFlipperProcess), outputsTemp);
            Outputs = new List<IDOutput>(outputsTemp.ToHashSet());
        }
    }
}
