using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Communication.Modbus;
using EQX.Core.InOut;
using EQX.Core.Recipe;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.MVVM.Views;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using FrontCameraAssembleEquipment.Vision;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class AutoViewModel : ViewModelBase
    {
        public IRelayCommand SelectRunModeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var selected = Resources.Controls.RunModeDialog.ShowRunModeDialog(Enum.GetValues<Defines.Process.EMachineRunMode>(), MachineStatus);

                    if (selected.HasValue)
                    {
                        var confirm = EQX.UI.Controls.MessageBoxEx.ShowDialog($"Do you want to change RunMode to {selected.Value}?");
                        if (confirm == false) return;

                        MachineStatus.MachineRunMode = selected.Value;
                    }
                });
            }
        }


        public IRelayCommand IoMonitoringCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IOMonitorWindowView iOMonitorWindowView = new IOMonitorWindowView();
                    iOMonitorWindowView.DataContext = _viewModelFactory.Create<IOMonitoringViewModel>();
                    iOMonitorWindowView.ShowDialog();
                });
            }
        }
        public ICommand OriginCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //_navigationService.NavigateTo<OriginViewModel>();
                    _windowService.ShowDialog<OriginViewModel>();
                });
            }
        }

        public ICommand DevViewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    VirtualKeyboard virtualKeyboard = new VirtualKeyboard();
                    virtualKeyboard.Width = 900;
                    virtualKeyboard.Height = 400;
                    virtualKeyboard.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    if (virtualKeyboard.ShowDialog() == true)
                    {
                        if (virtualKeyboard.InputText == DateTime.Today.ToString("MMdd"))
                        {
                            _navigationService.NavigateTo<DevViewModel>();
                        }
                    }
                });
            }
        }

        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    InitializeWindowView initializeWindowView = new InitializeWindowView();
                    initializeWindowView.DataContext = _viewModelFactory.Create<InitializeViewModel>();
                    initializeWindowView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    initializeWindowView.ShowDialog();
                });
            }
        }

        public ICommand ProductInforCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ProductionInfoView productionInfoView = new ProductionInfoView();
                    productionInfoView.DataContext = _viewModelFactory.Create<ProductionInfoViewModel>();
                    //_viewModelProvider.GetViewModel<ProductionInfoViewModel>().IsAllShift = true;
                    productionInfoView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    productionInfoView.ShowDialog();
                    //_windowService.ShowDialog<ProductionInfoViewModel>();
                });
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    MachineStatus.OPCommand = EOperationCommand.Start;
                    TotalTackTime.FrontStopWatch.RestartTime();
                    TotalTackTime.RearStopWatch.RestartTime();
                    TotalTackTime.TotalStopWatch.RestartTime();
                });
            }
        }

        public ICommand ResetWorkDataCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is string param)
                    {
                        switch (param)
                        {
                            case "Production":
                                if (MessageBoxEx.ShowDialog("Do you want to Reset Production Data?") == true)
                                {
                                    WorkData.Reset();
                                }
                                break;

                        }
                    }
                });
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    MachineStatus.OPCommand = EOperationCommand.Stop;
                });
            }
        }

        public ICommand VisionPGMChangeCommand
        {

            get
            {
                return new RelayCommand(() =>
                {
                    VisionProcess.Vision.SendRequestShowUI();
                    VisionProcess.Vision.CheckTurnOnVisionPGM();
                });
            }
        }

        public ICommand InputStopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsInputStop == false)
                    {
                        MachineStatus.IsInputStop = true;
                        Log.Debug("ENABLE STOP INTPUT!");
                    }
                    else
                    {
                        MachineStatus.IsInputStop = false;
                        Log.Debug("DISABLE STOP INTPUT!");
                    }
                });
            }
        }
        public ICommand OutputStopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsOutputStop == false)
                    {
                        MachineStatus.IsOutputStop = true;
                        Log.Debug("ENABLE STOP INTPUT!");
                    }
                    else
                    {
                        MachineStatus.IsOutputStop = false;
                        Log.Debug("DISABLE STOP INTPUT!");
                    }
                });
            }
        }

        public ICommand PickupStopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsPickUpStop == false)
                    {
                        MachineStatus.IsPickUpStop = true;
                        Log.Debug("ENABLE PICKUP STOP!");
                    }
                    else
                    {
                        MachineStatus.IsPickUpStop = false;
                        Log.Debug("DISABLE PICKUP STOP!");
                    }
                });
            }
        }

        public ICommand TrayOutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if(MachineStatus.IsStandByProcessMode)
                    {
                        if (MessageBoxEx.ShowDialog("Do you want Unload Tray Out ?", "Confirm") == false) return;

                        if (Devices.Inputs.TrayOutCv2DetectStart.Value == false &&
                           Devices.Inputs.TrayOutCv2DetectExist.Value == false &&
                           Devices.Inputs.TrayOutCv2DetectEnd.Value == false)
                        {
                            MessageBoxEx.ShowDialog("Tray Out not Exist");
                            return;
                        }

                        MachineStatus.SemiAutoSequence = ESemiSequence.TrayOutElevator_Unload;
                        MachineStatus.OPCommand = EOperationCommand.SemiAuto;

                        return;
                    }    
                    
                    if (MachineStatus.IsTrayOut == false)
                    {
                        MachineStatus.IsTrayOut = true;
                        Log.Debug("ENABLE TRAY OUT");
                    }
                    else
                    {
                        MachineStatus.IsTrayOut = false;
                        Log.Debug("DISABLE TRAY OUT");
                    }
                });
            }
        }

        #region Properties
        public string MachineRunModeDisplay => MachineStatus.MachineRunModeDisplay;

        public MachineStatus MachineStatus { get; }
        public TrayList TrayList { get; }
        public RecipeSelector RecipeSelector { get; }
        public MaterialStatusList MaterialStatusList { get; }
        public Devices Devices { get; }
        public TotalTackTime TotalTackTime { get; }
        public VisionProcess VisionProcess { get; }
        public CWorkData WorkData { get; }
        public CameraTypeSelectViewModel CameraTypeSelectViewModel { get; }
        public ObservableCollection<IDInput> InterfaceInputList
        {
            get
            {
                var inputList = new ObservableCollection<IDInput>();
                inputList.Add(Devices.Inputs.DownstreamRearLoadEnable);
                inputList.Add(Devices.Inputs.DownstreamFrontLoadEnable);

                return inputList;
            }
        }
        public ObservableCollection<IDOutput> InterfaceOutputList
        {
            get
            {
                var outputList = new ObservableCollection<IDOutput>();
                outputList.Add(Devices.Outputs.DownstreamRearLoadEnable);
                outputList.Add(Devices.Outputs.DownstreamFrontLoadEnable);

                return outputList;
            }
        }

        public ProductionDay TodayProduction
        {
            get
            {
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)
                {
                    return _productionData.ProductionDatas.FirstOrDefault(pd => pd.Date.ToString("ddMMyyyy") == DateTime.Now.AddDays(-1).ToString("ddMMyyyy"), new ProductionDay());
                }
                return _productionData.ProductionDatas.FirstOrDefault(pd => pd.Date.ToString("ddMMyyyy") == DateTime.Now.ToString("ddMMyyyy"), new ProductionDay());
            }
        }
        public bool SpeedControllersIsConnected => Devices.RollerList.All.All(sc => sc.IsConnected);
        #endregion
        private void MachineStatusOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MachineStatus.MachineRunMode))
            {
                OnPropertyChanged(nameof(MachineRunModeDisplay));
            }
        }

        private void TraySizeChanged_Handler(object? sender, EventArgs e)
        {
            if (sender is TraySuplierRecipe traySuplierRecipe == false) return;
            TrayList.TrayCamera.Rows = traySuplierRecipe.Rows;
            TrayList.TrayCamera.Columns = traySuplierRecipe.Columns;
            TrayList.TrayCamera.Orientation = RecipeSelector.CurrentRecipe.TraySuplierRecipe.JigOrientation;
            TrayList.TrayCamera.GenerateCells();
            TrayList.SubscribeCellClickedEvent();

            TrayList.Save();

        }

        #region Contructors
        public AutoViewModel(INavigationService navigationService,
            MachineStatus machineStatus,
            TrayList trayList,
            Devices devices,
            RecipeSelector recipeSelector,
            MaterialStatusList materialStatusList,
            TotalTackTime totalTackTime,
            VisionProcess visionProcess,
            RecipeList recipeList,
            CWorkData workData,
            ProductionData productionData,
            IWindowService windowService,
            IViewModelFactory viewModelFactory,
            NavigationStore navigationStore,
            CameraTypeSelectViewModel cameraTypeSelectViewModel)
        {
            _navigationService = navigationService;
            MachineStatus = machineStatus;
            TrayList = trayList;
            Devices = devices;
            RecipeSelector = recipeSelector;
            MaterialStatusList = materialStatusList;
            TotalTackTime = totalTackTime;
            VisionProcess = visionProcess;
            _recipeList = recipeList;
            WorkData = workData;
            _productionData = productionData;
            _windowService = windowService;
            _viewModelFactory = viewModelFactory;
            _navigationStore = navigationStore;
            CameraTypeSelectViewModel = cameraTypeSelectViewModel;
            recipeSelector.CurrentRecipe.TraySuplierRecipe.TraySizeChanged += TraySizeChanged_Handler;
            Log = LogManager.GetLogger("AutoVM");

            _productionData.TodayProductionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TodayProduction));
            };
        }
        #endregion

        #region Privates
        private readonly INavigationService _navigationService;
        private readonly IWindowService _windowService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly NavigationStore _navigationStore;
        private RecipeList _recipeList;
        private readonly ProductionData _productionData;
        #endregion
    }
}
