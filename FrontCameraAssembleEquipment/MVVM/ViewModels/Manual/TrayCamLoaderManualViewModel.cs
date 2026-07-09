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
    public class TrayCamLoaderManualViewModel : AppManualViewModel
    {
        private readonly VaccumList _vaccumList;

        public TrayCamLoaderManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList , Devices devices , VaccumList vaccumList , RecipeList recipeList) 
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices)
        {
            _vaccumList = vaccumList;
        }

        public ICylinder PickerCylinder => Devices.Cylinders.TrayPicker;
        public Vaccum TrayPickerVacuum => _vaccumList.TrayHead_TrayPickerVac;
        public Vaccum CamPickerVacuum => _vaccumList.TrayHead_CamPickerVac;

        protected override void ActualInit()
        {
            var inputsTemp = PropertyHelpers.GetProperties<IDInput>(_processes.TransferHeadProcess);
            Inputs = new List<IDInput>(inputsTemp.ToHashSet());
            Outputs = PropertyHelpers.GetProperties<IDOutput>(_processes.TransferHeadProcess);

            TeachingPositions = GetPositionTeachingList(_processes.TransferHeadProcess);
        }
    }
}
