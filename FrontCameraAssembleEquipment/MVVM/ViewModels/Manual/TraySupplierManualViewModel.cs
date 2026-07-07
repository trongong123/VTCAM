using EQX.Core.Common;
using EQX.Core.Helpers;
using EQX.Core.InOut;
using EQX.Device.SpeedController;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using System.Collections.ObjectModel;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class TraySupplierManualViewModel : AppManualViewModel
    {
        public TraySupplierManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList, Devices devices, RecipeList recipeList, DevRecipe devRecipe)
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices)
        {
            DevRecipe = devRecipe;
        }

        public IConveyor TrayInExternalConveyor => Devices.CVs.TrayIn_ExternalCv;
        public BD201SRollerController TrayInBufferConveyor => Devices.RollerList.TrayInCVRoller;
        public BD201SRollerController TrayInLiftConveyor => Devices.RollerList.TrayInElevatorRoller;

        public IConveyor TrayOutExternalConveyor => Devices.CVs.TrayOut_ExternalCv;
        public BD201SRollerController TrayOutBufferConveyor => Devices.RollerList.TrayOutCVRoller;
        public BD201SRollerController TrayOutLiftConveyor => Devices.RollerList.TrayOutElevatorRoller;

        public ICylinder StopperCylinder => Devices.Cylinders.TraySupplier_TrayInStopper;
        public ICylinder CenteringCylinder => Devices.Cylinders.TraySupplier_TrayCentering1;

        public DevRecipe DevRecipe { get; }

        protected override void ActualInit()
        {
            ObservableCollection<IDInput> inputsTemp;
            inputsTemp = PropertyHelpers.GetProperties<IDInput>(_processes.TrayInCVProcess);
            AddRange<IDInput>(PropertyHelpers.GetProperties<IDInput>(_processes.TrayInElevatorProcess), inputsTemp);
            AddRange<IDInput>(PropertyHelpers.GetProperties<IDInput>(_processes.TrayOutCVProcess), inputsTemp);
            AddRange<IDInput>(PropertyHelpers.GetProperties<IDInput>(_processes.TrayOutElevatorProcess), inputsTemp);
            Inputs = new List<IDInput>(inputsTemp.ToHashSet());

            ObservableCollection<IDOutput> outputsTemp;
            outputsTemp = PropertyHelpers.GetProperties<IDOutput>(_processes.TrayInCVProcess);
            AddRange<IDOutput>(PropertyHelpers.GetProperties<IDOutput>(_processes.TrayInElevatorProcess), outputsTemp);
            AddRange<IDOutput>(PropertyHelpers.GetProperties<IDOutput>(_processes.TrayOutCVProcess), outputsTemp);
            AddRange<IDOutput>(PropertyHelpers.GetProperties<IDOutput>(_processes.TrayOutElevatorProcess), outputsTemp);
            Outputs = new ObservableCollection<IDOutput>(outputsTemp.ToHashSet());

            TeachingPositions = GetPositionTeachingList(_processes.TrayInElevatorProcess);
            AddRange<PositionGroup>(GetPositionTeachingList(_processes.TrayOutElevatorProcess), TeachingPositions);
        }
    }
}
