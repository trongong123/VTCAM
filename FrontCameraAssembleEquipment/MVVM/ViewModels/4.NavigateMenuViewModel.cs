using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;

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
                    if (NavigationStore.CurrentViewModel.GetType() == typeof(DataViewModel)) return;
                    if (_systemConfig.RestrictedMode == true)
                    {
                        LoginDialog loginDialog = new LoginDialog();
                        loginDialog.InputPasswordToCheck = _systemConfig.LoginPassword;
                        if (loginDialog.ShowDialog() == true)
                        {
                            _navigationService.NavigateTo<DataViewModel>();
                        }
                        else return;
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
                    if (NavigationStore.CurrentViewModel.GetType() == typeof(TeachViewModel)) return;
                    if (_systemConfig.RestrictedMode == true)
                    {
                        LoginDialog loginDialog = new LoginDialog();
                        loginDialog.InputPasswordToCheck = _systemConfig.LoginPassword;
                        if (loginDialog.ShowDialog() == true)
                        {
                            _navigationService.NavigateTo<TeachViewModel>();
                        }
                        else return;
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
