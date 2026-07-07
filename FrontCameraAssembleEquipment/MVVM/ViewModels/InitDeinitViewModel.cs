using EQX.Core.Common;
using EQX.Core.Communication.Modbus;
using EQX.Core.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    /// <summary>
    /// Step list for initialization / deinitialization machine
    /// </summary>
    public enum EHandleStep
    {
        Start,

        FileSystemHandle,

        MotionDeviceHandle,
        CommunicationHandle,
        IODeviceHandle,
        VisionDeviceHandle,

        RecipeHandle,
        TrayInit,
        WorkDataHandle,
        ProcessHandle,

        End,
        Error,

        Navigate,
    }

    public enum EHandleMode
    {
        None,
        Init,
        Deinit,
    }

    public class InitDeinitViewModel : ViewModelBase
    {
        #region Properties
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                // The property may be updated from another thread.
                // So that, call OnPropertyChanged() inside and UI Invoke to make sure UI update properly
                if (Application.Current == null) return;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OnPropertyChanged();
                }), DispatcherPriority.Render);
            }
        }

        private string LogFolder => _configuration["Folders:LogFolder"] ?? "";
        private string ErrorFolder => _configuration["Folders:ErrorFolder"] ?? "";

        public List<string> ErrorMessages { get; private set; }
        #endregion

        public InitDeinitViewModel(Devices devices,
            Processes processes,
            [FromKeyedServices("RollerModbusCommunication")] IModbusCommunication rollerModbusCommunication,
            INavigationService navigationService,
            RecipeSelector recipeSelector,
            VirtualIO virtualIO,
            TrayList trayList,
            TotalTackTime totalTackTime,
            CWorkData workData,
            DevRecipe devRecipe,
            ProductionData productionData,
            InterlockService interlockService,
            IConfiguration configuration)
        {
            _devices = devices;
            _processes = processes;
            _rollerModbusCommunication = rollerModbusCommunication;
            _navigationService = navigationService;
            _recipeSelector = recipeSelector;

            _task = new Task(() => { });
            ErrorMessages = new List<string>();
            _virtualIO = virtualIO;
            _trayList = trayList;
            _totalTackTime = totalTackTime;
            Log = LogManager.GetLogger("InitVM");
            _workData = workData;
            _devRecipe = devRecipe;
            _productionData = productionData;
            _interlockService = interlockService;
            _configuration = configuration;
        }

        public void Initialization()
        {
            ErrorMessages = new List<string>();

            _task = new Task(() =>
            {
                if (HandlePreCheck(EHandleMode.Init) == false) return;

                OnInitialization();
                mode = EHandleMode.None;
            });

            _task.Start();
        }

        public void Deinitialization()
        {
            _task = new Task(() =>
            {
                if (HandlePreCheck(EHandleMode.Deinit) == false) return;

                OnDeinitialization();
                mode = EHandleMode.None;
            });

            _task.Start();
        }

        private bool HandlePreCheck(EHandleMode _mode)
        {
            if (mode == _mode)
            {
                // Already handle the same mode
                return false;
            }

            while (mode != EHandleMode.None)
            {
                Thread.Sleep(10);
            }

            mode = _mode;
            isHandling = true;
            _step = EHandleStep.Start;

            return true;
        }

        private void OnInitialization()
        {
            while (isHandling)
            {
                switch (_step)
                {
                    case EHandleStep.Start:
                        MessageText = "Init Start";
                        Log.Debug("Machine Initialization Start");
                        _step++;
                        break;
                    case EHandleStep.FileSystemHandle:
                        _step++;
                        break;

                    case EHandleStep.CommunicationHandle:
                        Log.Debug("Connect Modbus Communication");
                        if (_rollerModbusCommunication.Connect() == false)
                        {
                            ErrorMessages.Add("Speed Controller Connect Fail");
                        }
                        Thread.Sleep(50);
                        _step++;
                        break;
                    case EHandleStep.MotionDeviceHandle:
                        Log.Debug("Connect Motion Devices");
                        MessageText = "Connect Motion Devices";
                        _interlockService.Config();
#if SIMULATION
                        _step++;
                        break;
#else
                        _devices.Motions.AjinMaster.Connect();
                        _devices.Motions.AjinMotions.All.ForEach(m => m.Connect());

                        if (_rollerModbusCommunication.IsConnected)
                        {
                            _devices.RollerList.SetDirection();
                            _devices.RollerList.RollerSet();
                        }

                        if (_devices.RollerList.All.Any(m => m.IsConnected == false))
                        {
                            //ErrorMessages.Add($"Speed Controller is not connected: " +
                            //    $"{string.Join(", ", _devices.RollerList.All.Where(m => m.IsConnected == false).Select(m => m.Name))}");
                        }

                        if (_devices.Motions.AjinMotions.All.Any(m => m.IsConnected == false))
                        {
                            ErrorMessages.Add($"Motion device is not connected: " +
                                $"{string.Join(", ", _devices.Motions.AjinMotions.All.Where(m => m.IsConnected == false).Select(m => m.Name))}");
                        }
                        _devices.Motions.AjinMotions.All.ForEach(m => m.Initialization());

                        _step++;
                        break;
#endif
                    case EHandleStep.IODeviceHandle:
                        MessageText = "Connect IO Devices";
                        Log.Debug("Connect IO Devices");
                        _isSuccess = true;

                        _isSuccess &= _devices.Inputs.Connect();
                        _isSuccess &= _devices.Outputs.Connect();

                        if (_isSuccess == false)
                        {
                            ErrorMessages.Add("IO Devices init failed.");
                        }

                        _step++;
                        break;
                    case EHandleStep.VisionDeviceHandle:
                        MessageText = "Connect Vision Devices";
                        Log.Debug("Connect Vision Devices");
                        _devices.Vision.Initialize("127.0.0.1", 8888);
                        _step++;
                        break;
                    case EHandleStep.RecipeHandle:
                        MessageText = "Recipe Init";
                        Log.Debug("Recipe Init");
                        bool recipeLoadResult = _recipeSelector.Load();
                        if (recipeLoadResult == false)
                        {
                            //initErrors.Add($"{_recipeSelector.RecipeSetting.CurrentRecipe} load failed.");
                        }

                        _devRecipe.Load();
                        _step++;
                        break;
                    case EHandleStep.TrayInit:
                        MessageText = "Tray Init";
                        Log.Debug("Tray Init");
                        if (!_trayList.Load())
                        {
                            _trayList.RecipeUpdatedHandle();
                        }
                        _trayList.SubscribeCellClickedEvent();
                        _step++;
                        break;
                    case EHandleStep.WorkDataHandle:
                        _workData.Load();
                        _productionData.Load();
                        StartLogCleanupScheduler();
                        Thread.Sleep(50);
                        _step++;
                        break;
                    case EHandleStep.ProcessHandle:
                        _processes.Initialize();

                        _virtualIO.Initialize();
                        _virtualIO.Mappings();

                        _totalTackTime.EDMLogStopWatch.StartTiming();
                        _step++;
                        break;

                    case EHandleStep.End:
                        _step++;
                        break;
                    case EHandleStep.Error:
                        if (ErrorMessages.Count > 0)
                        {
                            MessageText = "Error: " + string.Join(", ", ErrorMessages);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            MessageText = "Initialization completed successfully.";
                        }
                        _step++;
                        break;
                    case EHandleStep.Navigate:
                        MessageText = "Navigating...";
                        Thread.Sleep(500);
                        _navigationService.NavigateTo<AutoViewModel>();
                        _step++;
                        break;
                    default:
                        isHandling = false;
                        return;
                }
                Thread.Sleep(2);
            }
        }

        private void OnDeinitialization()
        {
            while (isHandling)
            {
                switch (_step)
                {
                    case EHandleStep.Start:
                        MessageText = "Deinit Start";
                        Log.Debug("Machine Deinitialization Start");
                        StopLogCleanupScheduler();
                        _step++;
                        break;
                    case EHandleStep.FileSystemHandle:
                        _step++;
                        break;
                    case EHandleStep.MotionDeviceHandle:
                        MessageText = "Disconnect Motion Devices";
                        Log.Debug("Disconnect Motion Devices");
                        _devices.Motions.AjinMotions.All.ForEach(m => m.Disconnect());
                        _step++;
                        break;
                    case EHandleStep.CommunicationHandle:
                        _step++;
                        break;
                    case EHandleStep.IODeviceHandle:
                        MessageText = "Disconnect IO Devices";
                        Log.Debug("Disconnect IO Devices");
                        _devices.Inputs.Disconnect();
                        _devices.Outputs.Disconnect();
                        _step++;
                        break;
                    case EHandleStep.VisionDeviceHandle:
                        _step++;
                        break;
                    case EHandleStep.RecipeHandle:
                        _devRecipe.Save();
                        _step++;
                        break;
                    case EHandleStep.TrayInit:
                        Thread.Sleep(500);
                        _trayList.Save();
                        MessageText = "Tray Deinit";
                        Log.Debug("Tray Deinit");
                        _step++;
                        break;
                    case EHandleStep.WorkDataHandle:
                        _workData.Save();
                        _productionData.Save();
                        Thread.Sleep(50);
                        _step++;
                        break;
                    case EHandleStep.ProcessHandle:
                        _step++;
                        break;
                    case EHandleStep.End:
                        _step++;
                        break;
                    case EHandleStep.Error:
                        _step++;
                        break;
                    case EHandleStep.Navigate:
                        _step++;
                        break;
                    default:
                        // Set isHandling to false to stop the loop

                        Thread.Sleep(1000);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Application.Current.Shutdown();
                        });

                        isHandling = false;
                        return;
                }
                Thread.Sleep(2);
            }
        }

        private void StartLogCleanupScheduler()
        {
            if (_logCleanupStarted)
                return;

            _logCleanupStarted = true;

            // Run the first cleanup after startup settles a bit, then periodically.
            TimeSpan dueTime = TimeSpan.FromMinutes(5);
            TimeSpan period = TimeSpan.FromHours(3);

            _logCleanupTimer = new Timer(_ =>
            {
                TriggerLogCleanup();
            }, state: null, dueTime, period);
        }

        private void StopLogCleanupScheduler()
        {
            _logCleanupStarted = false;
            try
            {
                _logCleanupTimer?.Dispose();
            }
            catch
            {
                // ignore
            }
            _logCleanupTimer = null;
        }

        private void TriggerLogCleanup()
        {
            if (Interlocked.Exchange(ref _logCleanupInProgress, 1) == 1)
                return;

            _ = Task.Run(() =>
            {
                try
                {
                    int keepDays = _recipeSelector.CurrentRecipe.GlobalRecipe.LogSaveDay;
                    CleanupOldLogs(LogFolder, keepDays);
                    CleanupOldLogs(ErrorFolder, keepDays);
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    Interlocked.Exchange(ref _logCleanupInProgress, 0);
                }
            });
        }

        private void CleanupOldLogs(string logRootPath, int keepDays = 15)
        {
            if (!Directory.Exists(logRootPath))
                return;

            var dayFolders = new List<(DateTime date, string path)>();

            foreach (var monthDir in Directory.GetDirectories(logRootPath))
            {
                foreach (var dayDir in Directory.GetDirectories(monthDir))
                {
                    var dayName = Path.GetFileName(dayDir);

                    if (DateTime.TryParseExact(
                        dayName,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date))
                    {
                        dayFolders.Add((date, dayDir));
                    }
                }
            }

            var ordered = dayFolders
                .OrderByDescending(x => x.date)
                .ToList();

            var cutoffDate = DateTime.Today.AddDays(-keepDays);

            var foldersToDelete = dayFolders
                .Where(x => x.date < cutoffDate)
                .Select(x => x.path)
                .ToList();

            foreach (var folder in foldersToDelete)
            {
                try
                {
                    Directory.Delete(folder, recursive: true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to delete log folder: {folder} - {ex.Message}");
                }
            }

            CleanupEmptyMonthFolders(logRootPath);
        }

        private void CleanupEmptyMonthFolders(string logRootPath)
        {
            foreach (var monthDir in Directory.GetDirectories(logRootPath))
            {
                if (!Directory.EnumerateFileSystemEntries(monthDir).Any())
                {
                    try
                    {
                        Directory.Delete(monthDir);
                    }
                    catch { }
                }
            }
        }

        #region Private fields
        private readonly INavigationService _navigationService;
        private readonly IModbusCommunication _rollerModbusCommunication;
        private readonly Devices _devices;
        private readonly Processes _processes;
        private readonly RecipeSelector _recipeSelector;
        private readonly VirtualIO _virtualIO;
        private readonly TrayList _trayList;
        private string _messageText = "";
        private Task _task;
        private TotalTackTime _totalTackTime;
        private CWorkData _workData;
        private readonly DevRecipe _devRecipe;
        private readonly ProductionData _productionData;
        private readonly InterlockService _interlockService;
        private readonly IConfiguration _configuration;
        EHandleMode mode = EHandleMode.None;
        bool isHandling = true;

        private EHandleStep _step;

        private Timer? _logCleanupTimer;
        private int _logCleanupInProgress;
        private bool _logCleanupStarted;

        /// <summary>
        /// Common use for all case (in switch statement)
        /// </summary>
        private bool _isSuccess = true;
        #endregion
    }
}
