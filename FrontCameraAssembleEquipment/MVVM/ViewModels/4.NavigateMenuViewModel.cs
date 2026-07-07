using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class NavigateMenuViewModel : ViewModelBase
    {
        public MachineStatus MachineStatus { get; set; }
        public NavigationStore NavigationStore;
        public event Action? TabMenuChanged;

        #region Command(s)

        public IRelayCommand AutoNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TabMenuChanged?.Invoke();
                    _navigationService.NavigateTo<AutoViewModel>();
                });
            }
        }

        public IRelayCommand ManualNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TabMenuChanged?.Invoke();
                    _navigationService.NavigateTo<ManualViewModel>();
                });
            }
        }

        public IRelayCommand DataNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TabMenuChanged?.Invoke();
                    if (NavigationStore.CurrentViewModel == _viewModelFactory.Create<DataViewModel>()) return;
                    if(_systemConfig.RestrictedMode == true)
                    {
                        VirtualKeyboard virtualKeyboard = new VirtualKeyboard();
                        virtualKeyboard.Width = 900;
                        virtualKeyboard.Height = 400;
                        virtualKeyboard.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        if (virtualKeyboard.ShowDialog() == true)
                        {
                            if(virtualKeyboard.InputText == _systemConfig.LoginPassword)
                            {
                                _navigationService.NavigateTo<DataViewModel>();
                            }
                            else
                            {
                                MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_WrongPassword"]);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    _navigationService.NavigateTo<DataViewModel>();
                });
            }
        }

        public IRelayCommand TeachNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TabMenuChanged?.Invoke();
                    if (NavigationStore.CurrentViewModel == _viewModelFactory.Create<TeachViewModel>()) return;
                    if (_systemConfig.RestrictedMode == true)
                    {
                        VirtualKeyboard virtualKeyboard = new VirtualKeyboard();
                        virtualKeyboard.Width = 900;
                        virtualKeyboard.Height = 400;
                        virtualKeyboard.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        if (virtualKeyboard.ShowDialog() == true)
                        {
                            if (virtualKeyboard.InputText == _systemConfig.LoginPassword)
                            {
                                _navigationService.NavigateTo<TeachViewModel>();
                            }
                            else
                            {
                                MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_WrongPassword"]);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    _navigationService.NavigateTo<TeachViewModel>();
                });
            }
        }

        public IRelayCommand LogNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TabMenuChanged?.Invoke();
                    _navigationService.NavigateTo<ErrorLogViewModel>();
                });
            }
        }

        public IRelayCommand HideCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        WindowStateHelper.ShowTaskbar();
                    });
                });
            }
        }

        public ICommand ApplicationCloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if(MachineStatus.CurrentProcessMode == EProcessMode.Run)
                    {
                        MessageBoxEx.ShowDialog((string)System.Windows.Application.Current.Resources["str_MachineStatusIsRuning"]);
                        return;
                    }
                    if (MessageBoxEx.ShowDialog((string)System.Windows.Application.Current.Resources["str_AreYouSureYouWantToCloseApplication"]) == false)
                    {
                        return;
                    }

                    _navigationService.NavigateTo<InitDeinitViewModel>();
                    _viewModelFactory.Create<InitDeinitViewModel>().Deinitialization();
                });
            }
        }
        #endregion

        public NavigateMenuViewModel(INavigationService navigationService, 
            IViewModelFactory viewModelFactory, 
            MachineStatus machineStatus,
            IWindowService windowService, 
            NavigationStore navigationStore,
            SystemConfig systemConfig)
        {
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;
            MachineStatus = machineStatus;
            _windowService = windowService;
            NavigationStore = navigationStore;
            _systemConfig = systemConfig;

            _navigationService.Navigating += OnNavigationService_Navigating;
        }

        private void OnNavigationService_Navigating(object? sender, EventArgs e)
        {
            _systemConfig.DevMode = false;
        }


        #region Privates
        private readonly INavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IWindowService _windowService;
        private readonly SystemConfig _systemConfig;
        #endregion
    }

}
