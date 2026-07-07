using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.Motion;
using EQX.Motion.ByVendor.Inovance;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        public RecipeSelector RecipeSelector { get; }
        public RecipeList CurrentRecipe => RecipeSelector.CurrentRecipe;
        private readonly Motions _motions;
        private readonly IConfiguration _configuration;
        private readonly INavigationService _navigationService;

        public ObservableCollection<IMotion> AllMotions => new ObservableCollection<IMotion>(_motions.All);
        public DataViewModel(RecipeSelector recipeSelector,
            IWindowService windowService,
            SystemConfig systemConfig,
            Motions motions,
            IConfiguration configuration,
            INavigationService navigationService,
            CameraTypeSelectViewModel cameraTypeSelectViewModel,
            SerialCOMConfig serialCOMConfig)
        {
            RecipeSelector = recipeSelector;
            _windowService = windowService;
            SystemConfig = systemConfig;

            // Nếu Recipe có lưu thì load, còn không thì đặt default
            //SelectedComPort = RecipeSelector.CurrentRecipe.GlobalRecipe.comPort;

            SystemConfig.DevModeStateChange += OnSystemModeChange;
            _motions = motions;
            _configuration = configuration;
            _navigationService = navigationService;
            CameraTypeSelectViewModel = cameraTypeSelectViewModel;
            SerialCOMConfig = serialCOMConfig;
            RecipeSelector.RecipeSaved += RecipeSelector_RecipeSaved;
        }

        private void RecipeSelector_RecipeSaved()
        {
            var comConfigPath = _configuration["Files:SerialCommunicationConfig"] ?? "";
            if (File.Exists(comConfigPath))
            {
                var newComConfig = new SerialCOMConfig()
                {
                    COMPort = SerialCOMConfig.COMPort,
                    Baudrate = SerialCOMConfig.Baudrate
                };

                File.WriteAllText(comConfigPath, JsonConvert.SerializeObject(newComConfig));
            }
        }

        public string ComPortName { get; set; }
        public SystemConfig SystemConfig {get; set;}
        public CameraTypeSelectViewModel CameraTypeSelectViewModel { get; }
        public string SelectedRecipe { get; set; }
        public SerialCOMConfig SerialCOMConfig { get; }
        public event Action LoadRecipeEvent;
        public ObservableCollection<string> AvailableComPorts => new ObservableCollection<string>(SerialPort.GetPortNames());
        public ObservableCollection<int> BaudrateList => new ObservableCollection<int>()
        {
            4800,
            7200,
            9600,
            14400,
            19200,
            38400,
            57600,
            115200,
            128000
        };
        public bool IsDevModeOn => SystemConfig.DevMode;
        public ICommand DevModeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_windowService.ShowDialog<LoginViewModel>() == true)
                    {
                        SystemConfig.DevMode = true;
                    }
                });
            }
        }

        public ICommand SaveRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_SaveAllData"]) == true)
                    {
                        RecipeSelector.Save();
                    }
                });
            }
        }

        public ICommand RefreshRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RecipeSelector.ValidRecipes = RecipeSelector.UpdateValidRecipes();
                    LoadRecipeEvent?.Invoke();
                });
            }
        }
        public ICommand CopyRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string CopyRecipe = (string)Application.Current.Resources["str_CopyRecipe"];
                    if (MessageBoxEx.ShowDialog($"{CopyRecipe} {SelectedRecipe} ? ") == true)
                    {
                        RecipeSelector.Copy(SelectedRecipe);
                        RecipeSelector.ValidRecipes = RecipeSelector.UpdateValidRecipes();
                        LoadRecipeEvent?.Invoke();
                    }
                });
            }
        }

        public ICommand SaveMotionConfigCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        var result = MessageBoxEx.ShowDialog("Do you want to save the motion configurations?", true, "Confirm Save");

                        if (result == true)
                        {
                            SaveMotionConfigurations();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.ShowDialog($"Error saving motion configurations: {ex.Message}");
                    }
                });
            }
        }

        private void SaveMotionConfigurations()
        {
            var ajinConfigPath = _configuration["Files:MotionParaConfigFile"];
            if (!string.IsNullOrEmpty(ajinConfigPath))
            {
                var existingAjinParams = JsonConvert.DeserializeObject<List<MotionAjinParameter>>(
                    File.ReadAllText(ajinConfigPath)) ?? new List<MotionAjinParameter>();

                for (int i = 0; i < _motions.All.Count && i < existingAjinParams.Count; i++)
                {
                    existingAjinParams[i].Velocity = _motions.All[i].Parameter.Velocity;
                    existingAjinParams[i].Acceleration = _motions.All[i].Parameter.Acceleration;
                    existingAjinParams[i].Deceleration = _motions.All[i].Parameter.Deceleration;
                }

                var ajinJson = JsonConvert.SerializeObject(existingAjinParams, Formatting.Indented);
                File.WriteAllText(ajinConfigPath, ajinJson);
            }
        }


        private void OnSystemModeChange(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsDevModeOn));
        }

        private IWindowService _windowService;
    }
}
