using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class IOMonitoringViewModel : ViewModelBase
    {
        public uint SelectedInputDeviceIndex { get; set; }
        public uint SelectedOutputDeviceIndex { get; set; }

        public ICommand InputDeviceIndexFirst
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedInputDeviceIndex = 0;
                    OnPropertyChanged(nameof(SelectedInputDeviceIndex));
                });
            }
        }

        public ICommand InputDeviceIndexLast
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedInputDeviceIndex = (uint)(Inputs.All.Count / 16 - 1);
                    OnPropertyChanged(nameof(SelectedInputDeviceIndex));
                });
            }
        }

        public ICommand InputDeviceIndexDecrease
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedInputDeviceIndex > 0)
                    {
                        SelectedInputDeviceIndex--;
                        OnPropertyChanged(nameof(SelectedInputDeviceIndex));
                    }
                });
            }
        }

        public ICommand InputDeviceIndexHome
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedInputDeviceIndex > 0)
                    {
                        SelectedInputDeviceIndex = 0;
                        OnPropertyChanged(nameof(SelectedInputDeviceIndex));
                    }
                });
            }
        }

        public ICommand InputDeviceIndexIncrease
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedInputDeviceIndex < Inputs.All.Count / 16 - 1)
                    {
                        SelectedInputDeviceIndex++;
                        OnPropertyChanged(nameof(SelectedInputDeviceIndex));
                    }
                });
            }
        }

        public ICommand OutputDeviceIndexFirst
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedOutputDeviceIndex = 0;
                    OnPropertyChanged(nameof(SelectedOutputDeviceIndex));
                });
            }
        }

        public ICommand OutputDeviceIndexLast
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedOutputDeviceIndex = (uint)(Outputs.All.Count / 16 - 1);
                    OnPropertyChanged(nameof(SelectedOutputDeviceIndex));
                });
            }
        }

        public ICommand OutputDeviceIndexDecrease
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedOutputDeviceIndex > 0)
                    {
                        SelectedOutputDeviceIndex--;
                        OnPropertyChanged(nameof(SelectedOutputDeviceIndex));
                    }
                });
            }
        }

        public ICommand OutputDeviceIndexHome
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedOutputDeviceIndex > 0)
                    {
                        SelectedOutputDeviceIndex = 0;
                        OnPropertyChanged(nameof(SelectedOutputDeviceIndex));
                    }
                });
            }
        }

        public ICommand OutputDeviceIndexIncrease
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedOutputDeviceIndex < Outputs.All.Count / 16 - 1)
                    {
                        SelectedOutputDeviceIndex++;
                        OnPropertyChanged(nameof(SelectedOutputDeviceIndex));
                    }
                });
            }
        }

        public ICommand CloseIOViewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _navigationService.NavigateTo<AutoViewModel>();
                });
            }
        }

        public ICommand InterfaceViewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _windowService.ShowDialog<InterfaceViewModel>();
                });
            }
        }

        public Inputs Inputs { get { return _devices.Inputs; } }
        public Outputs Outputs { get { return _devices.Outputs; } }

        public MachineStatus MachineStatus { get; }

        public IOMonitoringViewModel(Devices devices, INavigationService navigationService, IWindowService windowService, MachineStatus machineStatus)
        {
            _devices = devices;
            _navigationService = navigationService;
            _windowService = windowService;
            MachineStatus = machineStatus;
        }

        private readonly Devices _devices;
        private readonly INavigationService _navigationService;
        private readonly IWindowService _windowService;
    }
}
