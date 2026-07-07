using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Units;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class OriginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly AxisUnitList _axisUnitList;
        private readonly IWindowService _windowService;

        public OriginViewModel(Processes processes, MachineStatus machineStatus, Motions motions,
            INavigationService navigationService, AxisUnitList axisUnitList, IWindowService windowService)
        {
            Processes = processes;
            MachineStatus = machineStatus;
            Motions = motions;
            _navigationService = navigationService;
            _axisUnitList = axisUnitList;
            Log = LogManager.GetLogger("OriginVM");
            _windowService = windowService;
        }

        public Processes Processes { get; }
        public MachineStatus MachineStatus { get; }
        public Motions Motions { get; }


        public ICommand SelectAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //foreach(var motion in Motions.All)
                    //{
                    //    MotionSelection.IsSelected(motion);

                    //    Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = true);
                    //}
                });
            }
        }

        public ICommand UnSelectAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
                });
            }
        }

        public ICommand OriginCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_AreYouSureYouWantToSetMachineOrigin"], (string)Application.Current.Resources["str_Confirm"]) == false)
                    {
                        return;
                    }
                    MachineStatus.OPCommand = EOperationCommand.Origin;
                    //else
                    //{
                    //    foreach (var motion in Motions.All)
                    //    {
                    //        if (MotionSelection.IsSelected(motion))
                    //        {
                    //            motion.SearchOrigin();
                    //        }
                    //    }
                    //}

                    Log.Debug("Origin Button Click");
                });
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Debug("Stop Button Click");
                    MachineStatus.OPCommand = EOperationCommand.Stop;
                });
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsRunningProcessMode) return;

                    _windowService.Close<OriginViewModel>();
                });
            }
        }
    }
}
