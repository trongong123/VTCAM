using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Helpers;
using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Extensions;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.Hosting;
using ScottPlot.Statistics;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class ConveyorManualViewModel : AppManualViewModel
    {
        public ConveyorManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList , Devices devices, RecipeList recipeList ,ProcessConfig processConfig, VaccumList vaccumList) 
            : base(navigationStore, machineStatus, processes, positionList, recipeList, devices, processConfig)
        {
            _vaccumList = vaccumList;
            _processesConfig = processConfig;

        }

        public ObservableCollection<ICylinder> CylindersInCv => PropertyHelpers.GetProperties<ICylinder>(_processes.FrontCVSetLoadProcess);
        public ObservableCollection<ICylinder> CylindersDetachCv => PropertyHelpers.GetProperties<ICylinder>(_processes.FrontCVSetFilmDetachProcess);
        public ObservableCollection<ICylinder> CylindersAssembleCv => PropertyHelpers.GetProperties<ICylinder>(_processes.FrontCVSetCamAssembleProcess);
        public ObservableCollection<ICylinder> CylindersOutCv => PropertyHelpers.GetProperties<ICylinder>(_processes.FrontCVSetUnloadProcess);

        public IConveyor CVsInCv => PropertyHelpers.GetProperties<IConveyor>(_processes.FrontCVSetLoadProcess).ToList().Last();
        public IConveyor ManualInCv => PropertyHelpers.GetProperties<IConveyor>(_processes.FrontCVSetLoadProcess).ToList().First();
        public ObservableCollection<IConveyor> CVsDetachCv => PropertyHelpers.GetProperties<IConveyor>(_processes.FrontCVSetFilmDetachProcess);
        public ObservableCollection<IConveyor> CVsAssembleCv => PropertyHelpers.GetProperties<IConveyor>(_processes.FrontCVSetCamAssembleProcess);
        public ObservableCollection<IConveyor> CVsOutCv => PropertyHelpers.GetProperties<IConveyor>(_processes.FrontCVSetUnloadProcess);

        public ObservableCollection<ICylinder> RearCylindersInCv => PropertyHelpers.GetProperties<ICylinder>(_processes.RearCVSetLoadProcess);
        public ObservableCollection<ICylinder> RearCylindersDetachCv => PropertyHelpers.GetProperties<ICylinder>(_processes.RearCVSetFilmDetachProcess);
        public ObservableCollection<ICylinder> RearCylindersAssembleCv => PropertyHelpers.GetProperties<ICylinder>(_processes.RearCVSetCamAssembleProcess);
        public ObservableCollection<ICylinder> RearCylindersOutCv => PropertyHelpers.GetProperties<ICylinder>(_processes.RearCVSetUnloadProcess);

        public IConveyor RearCVsInCv => PropertyHelpers.GetProperties<IConveyor>(_processes.RearCVSetLoadProcess).ToList().Last();
        public IConveyor RearManualInCv => PropertyHelpers.GetProperties<IConveyor>(_processes.RearCVSetLoadProcess).ToList().First();
        public ObservableCollection<IConveyor> RearCVsDetachCv => PropertyHelpers.GetProperties<IConveyor>(_processes.RearCVSetFilmDetachProcess);
        public ObservableCollection<IConveyor> RearCVsAssembleCv => PropertyHelpers.GetProperties<IConveyor>(_processes.RearCVSetCamAssembleProcess);
        public ObservableCollection<IConveyor> RearCVsOutCv => PropertyHelpers.GetProperties<IConveyor>(_processes.RearCVSetUnloadProcess);

        public static ProcessConfig _processesConfig;
        public static bool checkDeleteRear => _processesConfig.IsTwoConveyor;
        public Vaccum CvVac => _vaccumList.FrontUnload_CvVac;
        public ICommand TestOneConveyorFrontUnloadCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog("Test Front One-Conveyor Unload sequence?") == false) return;

                    MachineStatus.SemiAutoSequence = ESemiSequence.CVOut_Unload;
                    MachineStatus.OPCommand = EOperationCommand.SemiAuto;
                });
            }
        }


        protected override void ActualInit()
        {
            
        }
        private readonly VaccumList _vaccumList;
    }
}
