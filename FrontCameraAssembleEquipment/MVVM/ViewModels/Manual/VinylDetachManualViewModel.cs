using EQX.Core.Common;
using EQX.Core.Helpers;
using EQX.Core.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class VinylDetachManualViewModel : AppManualViewModel
    {
        private readonly VaccumList _vaccumList;

        public VinylDetachManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList, Devices devices, ProcessConfig processConfig, VaccumList vaccumList, RecipeList recipeList)
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices, processConfig)
        {
            _vaccumList = vaccumList;
        }

        public ICylinder VinylUpDownCylinder => Devices.Cylinders.FilmDetach_MoverUpDn;
        public ICylinder VinylGripperCylinder => Devices.Cylinders.FilmDetach_GripperOnOff;

        protected override void ActualInit()
        {
            var inputsTemp = PropertyHelpers.GetProperties<IDInput>(_processes.FilmDetachProcess);
            Inputs = new List<IDInput>(inputsTemp.ToHashSet());
            Outputs = PropertyHelpers.GetProperties<IDOutput>(_processes.FilmDetachProcess);

            TeachingPositions = GetPositionTeachingList(_processes.FilmDetachProcess);

        }
    }
}
