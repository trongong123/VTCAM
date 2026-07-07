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
    public class CamAssembleManualViewModel : AppManualViewModel
    {
        private readonly VaccumList _vaccumList;

        public CamAssembleManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList, Devices devices, VaccumList vaccumList, RecipeList recipeList)
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices)
        {
            _vaccumList = vaccumList;
        }

        public Vaccum CamPickerVac => _vaccumList.CamHead_CamPickerVac;

        protected override void ActualInit()
        {
            var inputsTemp = PropertyHelpers.GetProperties<IDInput>(_processes.CameraAssembleProcess);
            Inputs = new List<IDInput>(inputsTemp.ToHashSet());
            Outputs = PropertyHelpers.GetProperties<IDOutput>(_processes.CameraAssembleProcess);
            TeachingPositions = GetPositionTeachingList(_processes.CameraAssembleProcess);
        }
    }
}
