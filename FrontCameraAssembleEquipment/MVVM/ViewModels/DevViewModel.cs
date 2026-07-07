using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class DevViewModel : ViewModelBase
    {
        public DevViewModel(DevRecipe devRecipe,
            Processes processes,
            MachineStatus machineStatus)
        {
            DevRecipe = devRecipe;
            Processes = processes;
            MachineStatus = machineStatus;

            foreach (var process in Processes.RootProcess.Childs!)
            {
                process.Step.StepChangedHandler += Step_StepChangedHandler;
            }
        }

        private void Step_StepChangedHandler(object? sender, EventArgs e)
        {
            OnPropertyChanged("Step.RunStep");
        }

        public DevRecipe DevRecipe { get; }
        public Processes Processes { get; }
        public MachineStatus MachineStatus { get; }
    }
}
