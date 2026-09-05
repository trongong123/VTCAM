using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Motion;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        public RecipeSelector RecipeSelector { get; }
        public RecipeList CurrentRecipe => RecipeSelector.CurrentRecipe;
        private readonly Motions _motions;
        private readonly IConfiguration _configuration;
        private readonly INavigationService _navigationService;
        private readonly ILog _optionLog = LogManager.GetLogger("OPTION");
        private readonly Dictionary<string, string> _savedOptionValues = new();

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
            RecipeSelector.RecipeLoaded += RememberOptionValues;
            RecipeSelector.RecipeChanged += (_, _) => RememberOptionValues();
            RememberOptionValues();
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
                        var changes = GetOptionChanges();
                        RecipeSelector.Save();
                        WriteOptionChanges(changes);
                        RememberOptionValues();
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

                var changes = new List<(string Name, string OldValue, string NewValue)>();

                for (int i = 0; i < _motions.All.Count && i < existingAjinParams.Count; i++)
                {
                    AddMotionChange(changes, _motions.All[i].Name, "Velocity", existingAjinParams[i].Velocity, _motions.All[i].Parameter.Velocity);
                    AddMotionChange(changes, _motions.All[i].Name, "Acceleration", existingAjinParams[i].Acceleration, _motions.All[i].Parameter.Acceleration);
                    AddMotionChange(changes, _motions.All[i].Name, "Deceleration", existingAjinParams[i].Deceleration, _motions.All[i].Parameter.Deceleration);
                    existingAjinParams[i].Velocity = _motions.All[i].Parameter.Velocity;
                    existingAjinParams[i].Acceleration = _motions.All[i].Parameter.Acceleration;
                    existingAjinParams[i].Deceleration = _motions.All[i].Parameter.Deceleration;
                }

                var ajinJson = JsonConvert.SerializeObject(existingAjinParams, Formatting.Indented);
                File.WriteAllText(ajinConfigPath, ajinJson);
                WriteOptionChanges(changes);
            }
        }

        private Dictionary<string, string> ReadOptionValues()
        {
            var values = new Dictionary<string, string>();
            foreach (PropertyInfo recipeProperty in CurrentRecipe.GetType().GetProperties())
            {
                object? recipe = recipeProperty.GetValue(CurrentRecipe);
                if (recipe == null) continue;

                foreach (PropertyInfo optionProperty in recipe.GetType().GetProperties())
                {
                    if (!optionProperty.CanRead || optionProperty.GetIndexParameters().Length > 0
                        || optionProperty.Name.Contains("Position", StringComparison.OrdinalIgnoreCase)) continue;

                    Type valueType = Nullable.GetUnderlyingType(optionProperty.PropertyType) ?? optionProperty.PropertyType;
                    if (!valueType.IsPrimitive && !valueType.IsEnum && valueType != typeof(string) && valueType != typeof(decimal)) continue;

                    values[$"{recipeProperty.Name}.{optionProperty.Name}"] = FormatOptionValue(optionProperty.GetValue(recipe));
                }
            }

            values["SerialCommunication.COMPort"] = FormatOptionValue(SerialCOMConfig.COMPort);
            values["SerialCommunication.Baudrate"] = FormatOptionValue(SerialCOMConfig.Baudrate);
            return values;
        }

        private void RememberOptionValues()
        {
            _savedOptionValues.Clear();
            foreach (var option in ReadOptionValues()) _savedOptionValues[option.Key] = option.Value;
        }

        private List<(string Name, string OldValue, string NewValue)> GetOptionChanges()
        {
            return ReadOptionValues()
                .Where(option => _savedOptionValues.TryGetValue(option.Key, out string? oldValue) && oldValue != option.Value)
                .Select(option => (option.Key, _savedOptionValues[option.Key], option.Value))
                .ToList();
        }

        private void WriteOptionChanges(IEnumerable<(string Name, string OldValue, string NewValue)> changes)
        {
            string changedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            foreach (var change in changes)
            {
                _optionLog.Info($"Option changed | Option: {change.Name} | Old value: {change.OldValue} | " +
                    $"New value: {change.NewValue} | Changed at: {changedAt}");
            }
        }

        private static void AddMotionChange(
            ICollection<(string Name, string OldValue, string NewValue)> changes,
            string motionName,
            string optionName,
            double oldValue,
            double newValue)
        {
            if (oldValue == newValue) return;

            changes.Add(($"Motion.{motionName}.{optionName}", FormatOptionValue(oldValue), FormatOptionValue(newValue)));
        }

        private static string FormatOptionValue(object? value)
        {
            return value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value?.ToString() ?? string.Empty;
        }


        private void OnSystemModeChange(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsDevModeOn));
        }

        private IWindowService _windowService;
    }
}
