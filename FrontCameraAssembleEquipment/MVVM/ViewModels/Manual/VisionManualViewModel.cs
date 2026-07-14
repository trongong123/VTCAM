using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class VisionManualViewModel : AppManualViewModel
    {
        public VisionManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList, RecipeList recipeList, Devices devices, ProcessConfig processConfig, VisionProcess visionProcess) 
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices, processConfig)
        {
            VisionProcess = visionProcess;
        }

        public VisionProcess VisionProcess { get; }
    }
}
