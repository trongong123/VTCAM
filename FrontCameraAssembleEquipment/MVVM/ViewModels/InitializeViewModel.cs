using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Process;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class InitializeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ProcessConfig _processConfig;

        public InitializeViewModel(Devices devices,
            INavigationService navigationService,
            MachineStatus machineStatus,
            Processes processes,
            ProcessConfig processConfig)
        {
            Devices = devices;
            _navigationService = navigationService;
            MachineStatus = machineStatus;
            Processes = processes;
            _processConfig = processConfig;
            Log = LogManager.GetLogger("InitializeVM");
        }
        public bool CheckTwoConveyor => _processConfig.IsTwoConveyor;
        public Devices Devices { get; }
        public MachineStatus MachineStatus { get; }
        public Processes Processes { get; }

        public ICommand SelectAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = true);
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

        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog("Are you sure you want Initialize Machine") == false)
                    {
                        return;
                    }
                    if (Devices.Motions.AjinMotions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], false, (string)Application.Current.Resources["str_Confirm"]);
                        return;
                    }

                    if (MachineStatus.OriginDone == false)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                        return;
                    }
                       
                    MachineStatus.OPCommand = EOperationCommand.Ready;
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
                    _navigationService.NavigateTo<AutoViewModel>();
                });
            }
        }
    }
}
