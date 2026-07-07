using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.LogHistory;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class ErrorLogViewModel : ViewModelBase
    {
        public string LogErrorFolder => _configuration["Folders:ErrorFolder"] ?? "";

        public List<string> LogFilePathList
        {
            get
            {
                List<string> filterFilePathList = new();

                int days = (SearchEndTime - SearchStartTime).Days;

                for (int i = 0; i <= days; i++)
                {
                    DateTime currentDate = SearchStartTime.AddDays(i);
                    string currentCountFolder = Path.Combine(LogErrorFolder, currentDate.ToString("yyyy-MM"), currentDate.ToString("yyyy-MM-dd"));
                    if (Directory.Exists(currentCountFolder) == false) continue;

                    var lastFile = new DirectoryInfo(currentCountFolder).GetFiles("*.txt");

                    foreach (var file in lastFile)
                    {
                        if (file != null)
                        {
                            filterFilePathList.Add(file.FullName);
                        }
                    }
                }

                return filterFilePathList;
            }
        }

        public List<string> LogFilePaths
        {
            get
            {
                try
                {
                    List<string> filterFilePathList = new();

                    string currentCountFolder = Path.Combine(LogErrorFolder, SearchTime.ToString("yyyy-MM"), SearchTime.ToString("yyyy-MM-dd"));

                    if (Directory.Exists(currentCountFolder))
                    {
                        var currentDayFiles = new DirectoryInfo(currentCountFolder).GetFiles("*.txt").Where(f => int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) >= 8 && int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) <= 23);

                        foreach (var file in currentDayFiles)
                        {
                            if (file != null)
                            {
                                filterFilePathList.Add(file.FullName);
                            }
                        }
                    }

                    string nextCountFolder = Path.Combine(LogErrorFolder, SearchTime.AddDays(1).ToString("yyyy-MM"), SearchTime.AddDays(1).ToString("yyyy-MM-dd"));

                    if (Directory.Exists(nextCountFolder))
                    {
                        var nextDayFiles = new DirectoryInfo(nextCountFolder).GetFiles("*.txt").Where(f => int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) >= 0 && int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) <= 7);

                        foreach (var file in nextDayFiles)
                        {
                            if (file != null)
                            {
                                filterFilePathList.Add(file.FullName);
                            }
                        }
                    }

                    return filterFilePathList;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }

        public List<string> LogFilePathsShiftDay
        {
            get
            {
                try
                {
                    List<string> filterFilePathList = new();

                    string currentCountFolder = Path.Combine(LogErrorFolder, SearchTime.ToString("yyyy-MM"), SearchTime.ToString("yyyy-MM-dd"));

                    if (Directory.Exists(currentCountFolder))
                    {
                        var currentDayFiles = new DirectoryInfo(currentCountFolder).GetFiles("*.txt").Where(f => int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) >= 8 && int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) < 20);

                        foreach (var file in currentDayFiles)
                        {
                            if (file != null)
                            {
                                filterFilePathList.Add(file.FullName);
                            }
                        }
                    }

                    return filterFilePathList;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }

        public List<string> LogFilePathsShiftNight
        {
            get
            {
                try
                {
                    List<string> filterFilePathList = new();

                    string currentCountFolder = Path.Combine(LogErrorFolder, SearchTime.ToString("yyyy-MM"), SearchTime.ToString("yyyy-MM-dd"));

                    if (Directory.Exists(currentCountFolder))
                    {
                        var currentDayFiles = new DirectoryInfo(currentCountFolder).GetFiles("*.txt").Where(f => int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) >= 20 && int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) <= 23);

                        foreach (var file in currentDayFiles)
                        {
                            if (file != null)
                            {
                                filterFilePathList.Add(file.FullName);
                            }
                        }
                    }

                    string nextCountFolder = Path.Combine(LogErrorFolder, SearchTime.AddDays(1).ToString("yyyy-MM"), SearchTime.AddDays(1).ToString("yyyy-MM-dd"));

                    if (Directory.Exists(nextCountFolder))
                    {
                        var nextDayFiles = new DirectoryInfo(nextCountFolder).GetFiles("*.txt").Where(f => int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) >= 0 && int.Parse(f.Name.Substring(f.Name.IndexOf("_") + 1, 2).ToString()) <= 7);

                        foreach (var file in nextDayFiles)
                        {
                            if (file != null)
                            {
                                filterFilePathList.Add(file.FullName);
                            }
                        }
                    }

                    return filterFilePathList;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }

        private bool isAllShift = true;
        private bool isDayShift;
        private bool isNightShift;

        public bool IsAllShift
        {
            get { return isAllShift; }
            set
            {
                isAllShift = value;
                OnPropertyChanged();
                if (value)
                {
                    LoadLogData(LogFilePaths);
                }
            }
        }

        public bool IsDayShift
        {
            get { return isDayShift; }
            set
            {
                isDayShift = value;
                OnPropertyChanged();
                if (value)
                {
                    LoadLogData(LogFilePathsShiftDay);
                }
            }
        }

        public bool IsNightShift
        {
            get { return isNightShift; }
            set
            {
                isNightShift = value;
                OnPropertyChanged();
                if (value)
                {
                    LoadLogData(LogFilePathsShiftNight);
                }
            }
        }

        public DateTime SearchTime
        {
            get { return searchTime; }
            set
            {
                searchTime = value;
                OnPropertyChanged();
                if (IsAllShift)
                {
                    LoadLogData(LogFilePaths);
                }
                else if (IsDayShift)
                {
                    LoadLogData(LogFilePathsShiftDay);
                }
                else
                {
                    LoadLogData(LogFilePathsShiftNight);
                }
            }
        }

        public string Today => DateTime.Today.ToString("dd-MM-yyyy");
        public DateTime SearchStartTime
        {
            get { return _searchStartTime; }
            set
            {
                _searchStartTime = value;
                ReloadLogData();
                OnPropertyChanged(nameof(SearchStartTime));
            }
        }

        public DateTime SearchEndTime
        {
            get { return _searchEndTime; }
            set
            {
                _searchEndTime = value;
                ReloadLogData();
                OnPropertyChanged(nameof(SearchEndTime));
            }
        }

        public ObservableCollection<ErrorLogEntry> ErrorListEntry
        {
            get { return _errorListEntry; }
            set
            {
                _errorListEntry = value;
                _errorListEntry = new ObservableCollection<ErrorLogEntry>(_errorListEntry.Reverse());
                OnPropertyChanged(nameof(ErrorListEntry));
            }
        }

        public ObservableCollection<ErrorLogCount> ErrorLogCount
        {
            get { return _errorLogCount; }
            set
            {
                _errorLogCount = value;
                OnPropertyChanged(nameof(ErrorLogCount));
            }
        }


        private ErrorLogCount errorLogCountSelected;

        public ErrorLogCount ErrorLogCountSelected
        {
            get { return errorLogCountSelected; }
            set
            {
                errorLogCountSelected = value;
                OnPropertyChanged();
                if (IsAllShift)
                {
                    ErrorListEntry = SafeLoadLogEntries(LogFilePaths);
                }
                else if (IsDayShift)
                {
                    ErrorListEntry = SafeLoadLogEntries(LogFilePathsShiftDay);
                }
                else
                {
                    ErrorListEntry = SafeLoadLogEntries(LogFilePathsShiftNight);
                }
            }
        }

        public ErrorLogViewModel(IConfiguration configuration, INavigationService navigationService, Processes processes)
        {
            _configuration = configuration;
            _navigationService = navigationService;
            _processes = processes;
            //ErrorListEntry = SafeLoadLogEntries(LogFilePathList);
            //ErrorLogCount = LoadLogCounts(ErrorListEntry);

            ((RootProcess<ESequence, ESemiSequence>)processes.RootProcess).RootWarningEvent += RootWarningEventHandler;
            ((RootProcess<ESequence, ESemiSequence>)processes.RootProcess).RootAlarmEvent += RootAlarmEventHandler;

        }

        private void RootWarningEventHandler(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var entry in IgnoreErrorLogList)
                {
                    if ((int)sender == entry)
                        return;
                }

                if (IsDayShift)
                {
                    if (DateTime.Now.Hour >= 20 && DateTime.Now.Hour <= 7 || (SearchTime.ToString("dd:MM:yyyy") != DateTime.Now.ToString("dd:MM:yyyy"))) return;
                }

                if (IsNightShift)
                {
                    if (DateTime.Now.Hour < 20 && DateTime.Now.Hour > 7 || (SearchTime.ToString("dd:MM:yyyy") != DateTime.Now.AddDays(-1).ToString("dd:MM:yyyy"))) return;
                }

                var newErrorLogEntry = new ErrorLogEntry();
                newErrorLogEntry.Timestamp = DateTime.Now.ToString("hh:mm:ss");
                newErrorLogEntry.ErrorCode = (int)sender;
                newErrorLogEntry.Message = $"{((EWarning)((int)sender)).GetDescription()}";
                ErrorListEntry.Insert(0, newErrorLogEntry);

                ErrorLogCount = LoadLogCounts(ErrorListEntry);
            });
        }

        private void RootAlarmEventHandler(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var entry in IgnoreErrorLogList)
                {
                    if ((int)sender == entry)
                        return;
                }

                if (IsDayShift)
                {
                    if (DateTime.Now.Hour >= 20 && DateTime.Now.Hour <= 7) return;
                }

                if (IsNightShift)
                {
                    if (DateTime.Now.Hour < 20 && DateTime.Now.Hour > 7) return;
                }

                var newErrorLogEntry = new ErrorLogEntry();
                newErrorLogEntry.Timestamp = DateTime.Now.ToString("hh:mm:ss");
                newErrorLogEntry.ErrorCode = (int)sender;
                newErrorLogEntry.Message = $"{((EAlarm)((int)sender)).GetDescription()}";
                ErrorListEntry.Insert(0, newErrorLogEntry);

                ErrorLogCount = LoadLogCounts(ErrorListEntry);
            });
        }

        public ICommand DevLogCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _navigationService.NavigateTo<LogViewModel>();
                });
            }
        }

        public ICommand SetSearchTimeTodayCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SearchTime = DateTime.Today;
                });
            }
        }

        private DateTime _setClearTime;

        public string ClearTime => _setClearTime.ToString("dd-MM-yyyy") + " : " + _setClearTime.ToString("HH:mm:ss");

        private bool cleaLogSet = false;

        public bool ClearLogSet
        {
            get { return cleaLogSet; }
            set { cleaLogSet = value; OnPropertyChanged(); }
        }

        public void LoadLogSetTime()
        {
            ErrorListEntry = SafeLoadLogEntries(LogFilePathList, _setClearTime);
            ErrorLogCount = LoadLogCounts(ErrorListEntry);
        }

        public ICommand ClearLogCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _setClearTime = DateTime.Now;
                    ClearLogSet = true;
                    OnPropertyChanged(nameof(ClearTime));
                    if (IsAllShift)
                    {
                        LoadLogData(LogFilePaths);
                    }
                    else if (IsDayShift)
                    {
                        LoadLogData(LogFilePathsShiftDay);
                    }
                    else
                    {
                        LoadLogData(LogFilePathsShiftNight);
                    }
                });
            }
        }

        public ICommand ReadLogCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ClearLogSet = false;

                    if (IsAllShift)
                    {
                        LoadLogData(LogFilePaths);
                    }
                    else if (IsDayShift)
                    {
                        LoadLogData(LogFilePathsShiftDay);
                    }
                    else
                    {
                        LoadLogData(LogFilePathsShiftNight);
                    }
                });
            }
        }

        public ICommand ReloadLogDataCommand => new AsyncRelayCommand(ReloadLogData);

        public async Task ReloadLogData()
        {
            await Task.Run(() =>
            {
                ErrorListEntry = SafeLoadLogEntries(LogFilePathList);
                ErrorLogCount = LoadLogCounts(ErrorListEntry);
            });

            return;
        }

        public async Task LoadLogData(List<string> filePaths)
        {
            await Task.Run(() =>
            {
                ErrorListEntry = SafeLoadLogEntries(filePaths);
                ErrorLogCount = LoadLogCounts(ErrorListEntry);
            });

            return;
        }

        private List<ErrorLogEntry> LoadLogEntries(string filePath)
        {
            var logEntries = new List<ErrorLogEntry>();
            if (File.Exists(filePath) == false) return logEntries;
            var lines = File.ReadAllLines(filePath);

            var regex = new Regex(@"\[(?<time>[0-9:\.]+)\],(?<type>\w+)\s*,(?<source>.{0,180}),\[(?<errorcode>\d+)\]\s*(?<message>.+)");

            foreach (var line in lines)
            {
                var match = regex.Match(line);

                if (Int32.TryParse(match.Groups["errorcode"].Value.Trim(), out int errCode) == false)
                {
                    continue;
                }

                if (ClearLogSet && DateTime.Parse(match.Groups["time"].Value) < _setClearTime) continue;

                bool ignoreMatching = false;
                foreach (var warning in IgnoreErrorLogList)
                {
                    if (errCode == warning)
                    {
                        ignoreMatching = true;
                        break;
                    }
                }

                if (ignoreMatching) continue;

                if (match.Success && (match.Groups["type"].Value.Trim() == "ERROR" || match.Groups["type"].Value.Trim() == "WARN"))
                {
                    var logEntry = new ErrorLogEntry()
                    {
                        Timestamp = DateTime.Parse(match.Groups["time"].Value.Trim()).ToString("HH:mm:ss"),
                        ErrorCode = int.Parse(match.Groups["errorcode"].Value.Trim()),
                        Message = match.Groups["message"].Value.Trim(),
                    };

                    if (errorLogCountSelected != null)
                    {
                        logEntry.IsHightlight = logEntry.ErrorCode == errorLogCountSelected.ErrorCode;
                    }

                    logEntries.Add(logEntry);
                }
            }
            return logEntries;
        }

        private List<ErrorLogEntry> LoadLogEntries(string filePath, DateTime filterStartTime)
        {
            var logEntries = new List<ErrorLogEntry>();
            if (File.Exists(filePath) == false) return logEntries;
            var lines = File.ReadAllLines(filePath);

            var regex = new Regex(@"\[(?<time>[0-9:\.]+)\],(?<type>\w+)\s*,(?<source>.{0,180}),\[(?<errorcode>\d+)\]\s*(?<message>.+)");

            foreach (var line in lines)
            {
                var match = regex.Match(line);

                if (Int32.TryParse(match.Groups["errorcode"].Value.Trim(), out int errCode) == false)
                {
                    continue;
                }

                bool ignoreMatching = false;
                foreach (var warning in IgnoreErrorLogList)
                {
                    if (errCode == warning)
                    {
                        ignoreMatching = true;
                        break;
                    }
                }

                if (ignoreMatching) continue;

                if (match.Success
                    && (match.Groups["type"].Value.Trim() == "ERROR" || match.Groups["type"].Value.Trim() == "WARN")
                    && (DateTime.Parse(match.Groups["time"].Value.Trim()).CompareTo(filterStartTime) >= 0))
                {
                    logEntries.Add(new ErrorLogEntry
                    {
                        Timestamp = DateTime.Parse(match.Groups["time"].Value.Trim()).ToString("HH:mm:ss"),
                        ErrorCode = int.Parse(match.Groups["errorcode"].Value.Trim()),
                        Message = match.Groups["message"].Value.Trim()
                    });
                }
            }
            return logEntries;
        }

        private ObservableCollection<ErrorLogCount> LoadLogCounts(ObservableCollection<ErrorLogEntry> errorLogEntries)
        {
            var logCounts = new ObservableCollection<ErrorLogCount>();
            var result = errorLogEntries
                .GroupBy(e => new { e.ErrorCode, e.Message })
                .Select(g => new ErrorLogCount
                {
                    ErrorCode = g.Key.ErrorCode,
                    Message = g.Key.Message,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count);
            foreach (var logEntry in result)
            {
                logCounts.Add(logEntry);
            }
            return logCounts;
        }

        private ObservableCollection<ErrorLogEntry> SafeLoadLogEntries(List<string> path)
        {
            int retryCount = 0;
            const int maxRetry = 50;
            const int delayMs = 100;

            while (true)
            {
                try
                {
                    ObservableCollection<ErrorLogEntry> errorLogEntries = new ObservableCollection<ErrorLogEntry>();
                    List<ErrorLogEntry> errorList = new List<ErrorLogEntry>();
                    foreach (var file in path)
                    {
                        errorList.AddRange(LoadLogEntries(file));
                    }

                    foreach (var item in errorList)
                    {

                        errorLogEntries.Add(item);
                    }

                    return errorLogEntries;
                }
                catch (System.Xml.XmlException ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetry)
                        MessageBox.Show(ex.Message);

                    System.Threading.Thread.Sleep(delayMs);
                }
            }
        }

        private ObservableCollection<ErrorLogEntry> SafeLoadLogEntries(List<string> path, DateTime filterStartTime)
        {
            int retryCount = 0;
            const int maxRetry = 50;
            const int delayMs = 100;

            while (true)
            {
                try
                {
                    ObservableCollection<ErrorLogEntry> errorLogEntries = new ObservableCollection<ErrorLogEntry>();
                    List<ErrorLogEntry> errorList = new List<ErrorLogEntry>();
                    foreach (var file in path)
                    {
                        errorList.AddRange(LoadLogEntries(file, filterStartTime));
                    }

                    foreach (var item in errorList)
                    {

                        errorLogEntries.Add(item);
                    }

                    return errorLogEntries;
                }
                catch (System.Xml.XmlException ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetry)
                        MessageBox.Show(ex.Message);

                    System.Threading.Thread.Sleep(delayMs);
                }
            }
        }

        private ObservableCollection<ErrorLogEntry> LoadLogEntries(List<string> path)
        {
            try
            {
                ObservableCollection<ErrorLogEntry> errorLogEntries = new ObservableCollection<ErrorLogEntry>();
                List<ErrorLogEntry> errorList = new List<ErrorLogEntry>();
                foreach (var file in path)
                {
                    errorList.AddRange(LoadLogEntries(file));
                }

                foreach (var item in errorList)
                {
                    errorLogEntries.Add(item);
                }

                return errorLogEntries;
            }
            catch (Exception ex)
            {
                return new ObservableCollection<ErrorLogEntry>();
            }
        }


        private ObservableCollection<ErrorLogEntry> _errorListEntry = new ObservableCollection<ErrorLogEntry>();
        private ObservableCollection<ErrorLogCount> _errorLogCount = new ObservableCollection<ErrorLogCount>();
        private IConfiguration _configuration;
        private readonly INavigationService _navigationService;
        private readonly Processes _processes;
        private DateTime searchTime = DateTime.Today;
        private DateTime _searchStartTime = DateTime.Today;
        private DateTime _searchEndTime = DateTime.Today;

        private List<int> IgnoreErrorLogList = new List<int>()
        {
            ((int)EWarning.DoorOpen),
            ((int)EWarning.LightCurtainDetected),
            ((int)EAlarm.PowerMcOff),
            ((int) EAlarm.EmergencyStopPressed),
            //((int) EWarning.TrayINBuffer_StopperUp_Timeout),
            //((int) EWarning.TrayINBuffer_StopperDown_Timeout),
            //((int) EWarning.TrayCAMLoader_TrayPicker_Down_Fail),
            //((int) EWarning.TrayCAMLoader_TrayPicker_Up_Fail),
            //((int) EWarning.CamSpongeDetach_CenteringOn_Fail),
            //((int) EWarning.CamSpongeDetach_CenteringOff_Fail),
            //((int) EWarning.CamSpongeDetach_MoveUp_Fail),
            //((int) EWarning.CamSpongeDetach_MoveDown_Fail),
            //((int) EWarning.CamSpongeDetach_MoveFw_Fail),
            //((int) EWarning.CamSpongeDetach_MoveBw_Fail),
            //((int) EWarning.CamSpongeDetach_GripOn_Fail),
            //((int) EWarning.CamSpongeDetach_GripOff_Fail),
            //((int) EWarning.CAMRotator_MoveUnloadPos_Fail),
            //((int) EWarning.CAMRotator_MoveUnloadPosAndRotate_Fail),
            //((int) EWarning.CAMRotator_MoveAndFlipReady_Fail),
            //((int) EWarning.CAMRotator_MovePick_Fail),
            //((int) EWarning.CAMRotator_MoveDown_Fail),
            //((int) EWarning.CAMRotator_MoveUp_Fail),
            //((int) EWarning.CAMRotator_GripOn_Fail),
            //((int) EWarning.CAMRotator_GripOff_Fail),
            //((int) EWarning.CAMRotator_GripOffAndRotateReady_Fail),
            //((int) EWarning.CAMRotator_RotateReady_Fail),
            //((int) EWarning.CAMRotator_Rotate_Fail),
            //((int) EWarning.VinylDetach_MoveUp_Fail),
            //((int) EWarning.VinylDetach_MoveDown_Fail),
            //((int) EWarning.VinylDetach_GripOn_Fail),
            //((int) EWarning.VinylDetach_GripOff_Fail),
            //((int) EWarning.VinylDetach_CylinderSuctionUp_Fail),
            //((int) EWarning.VinylDetach_CylinderSuctionDown_Fail),
            //((int) EWarning.CAMAssemble_CylinderUp_Fail),
            //((int) EWarning.CAMAssemble_CylinderDown_Fail   ),
            //((int) EWarning.FrontDetachCV_StopperUp_Fail  ),
            //((int) EWarning.FrontDetachCV_StopperDown_Fail),
            //((int) EWarning.FrontDetachCV_AlignOn_Fail),
            //((int) EWarning.FrontDetachCV_AlignOff_Fail),
            //((int) EWarning.RearDetachCV_StopperUp_Fail),
            //((int) EWarning.RearDetachCV_StopperDown_Fail),
            //((int) EWarning.RearDetachCV_AlignOn_Fail),
            //((int) EWarning.RearDetachCV_AlignOff_Fail),
            //((int) EWarning.FrontAssembleCV_StopperUp_Fail),
            //((int) EWarning.FrontAssembleCV_StopperDown_Fail),
            //((int) EWarning.FrontAssembleCV_AlignOn_Fail),
            //((int) EWarning.FrontAssembleCV_AlignOff_Fail),
            //((int) EWarning.RearAssembleCV_StopperUp_Fail),
            //((int) EWarning.RearAssembleCV_StopperDown_Fail),
            //((int) EWarning.RearAssembleCV_AlignOn_Fail),
            //((int) EWarning.RearAssembleCV_AlignOff_Fail),
            //((int) EWarning.FrontOUTCV_StopperUp_Fail),
            //((int) EWarning.FrontOUTCV_StopperDown_Fail),
            //((int) EWarning.RearOUTCV_StopperUp_Fail),
            //((int)EWarning.RearOUTCV_StopperDown_Fail),
        };

    }
}
