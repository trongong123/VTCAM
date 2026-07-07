using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Communication.CIM.Custom;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using static EQX.Core.Communication.CIM.Custom.WordArea.PPIDListArea;

namespace EQX.UI.MVVM
{
    #region Enums for EQP Function options

    public enum OnOffNothing { ON, OFF, NOTHING }
    public enum TrackingControlMode { TKIN, TKOUT, BOTH, NOTHING }
    public enum UseUnuseNothing { USE, UNUSE, NOTHING }
    public enum AutoManualNothing { AUTO, MANU, NOTHING }
    public enum InterlockControlMode { TRANSFER, LOADING, STEP, OWN }
    #endregion

    /// <summary>
    /// DTO for JSON persist of CIM Function settings.
    /// </summary>
    internal class CIMFunctionData
    {
        public string CellTrackingState { get; set; } = nameof(OnOffNothing.ON);
        public string TrackingControlMode { get; set; } = nameof(EQX.UI.MVVM.TrackingControlMode.TKIN);
        public string MaterialTrackingState { get; set; } = nameof(OnOffNothing.NOTHING);
        public string CellMCRMode { get; set; } = nameof(UseUnuseNothing.USE);
        public string MaterialMCRMode { get; set; } = nameof(UseUnuseNothing.NOTHING);
        public string LotAssignMode => nameof(AutoManualNothing.NOTHING);
        public string AgvMode => nameof(AutoManualNothing.NOTHING);
        public string AreaSensorMode => nameof(UseUnuseNothing.NOTHING);
        public string SortMode => nameof(UseUnuseNothing.NOTHING);
        public string InterlockControlMode => nameof(EQX.UI.MVVM.InterlockControlMode.LOADING);
        public string LoaderLIMCRMode => nameof(OnOffNothing.NOTHING);
        public string LoaderLSMCRMode => nameof(OnOffNothing.NOTHING);
        public string UnloaderMCRMode => nameof(OnOffNothing.NOTHING);
        public string ApcMode { get; set; } = nameof(AutoManualNothing.NOTHING);
        public int MultiPassModeValue => 0;

        public void ToArea(ref EquipFunctionChangeEventArea area)
        {
            area.EFST[0] = CellTrackingState;          // 1. CELL TRACKING
            area.EFST[1] = TrackingControlMode;        // 2. TRACKING CONTROL
            area.EFST[2] = MaterialTrackingState;      // 3. MATERIAL TRACKING
            area.EFST[3] = CellMCRMode;                // 4. CELL MCR MODE 
            area.EFST[4] = MaterialMCRMode;            // 5. MATERIAL MCR MODE

            area.EFST[5] = LotAssignMode; // 6. LOT ASSIGN INFORMATION
            area.EFST[6] = AgvMode;             // 7. AGV ACCESS MODE

            area.EFST[7] = AreaSensorMode;             // 8. AREA SENSOR MODE
            area.EFST[8] = SortMode;                   // 9. SORT MODE
            area.EFST[9] = InterlockControlMode;       // 10. INTERLOCK CONTROL
            area.EFST[10] = LoaderLIMCRMode;           // 11. LOADER(LI01) LOAD PORT MCR MODE
            area.EFST[11] = LoaderLSMCRMode;           // 12. LOADER(LI01) USE PORT MCR MODE
            area.EFST[12] = UnloaderMCRMode;           // 13. UNLOADER USE PORT MCR MODE

            area.EFST[13] = ApcMode; // 14. APC MODE 
            area.EFST[14] = MultiPassModeValue.ToString();               // 15. MULTI PASS MODE (int to string)
        }

        public void FromArea(EquipFunctionChangeEventArea area)
        {
            CellTrackingState = area.EFST[0];          // 1. CELL TRACKING
            TrackingControlMode = area.EFST[1];        // 2. TRACKING CONTROL
            MaterialTrackingState = area.EFST[2];      // 3. MATERIAL TRACKING
            CellMCRMode = area.EFST[3];                // 4. CELL MCR MODE 
            MaterialMCRMode = area.EFST[4];            // 5. MATERIAL MCR MODE

            /*
            LotAssignMode = area.EFST[5]; // 6. LOT ASSIGN INFORMATION
            AgvMode = area.EFST[6];             // 7. AGV ACCESS MODE

            AreaSensorMode = area.EFST[7];             // 8. AREA SENSOR MODE
            SortMode = area.EFST[8];                   // 9. SORT MODE
            InterlockControlMode = area.EFST[9];       // 10. INTERLOCK CONTROL
            LoaderLIMCRMode = area.EFST[10];           // 11. LOADER(LI01) LOAD PORT MCR MODE
            LoaderLSMCRMode = area.EFST[11];           // 12. LOADER(LI01) USE PORT MCR MODE
            UnloaderMCRMode = area.EFST[12];           // 13. UNLOADER USE PORT MCR MODE

            ApcMode = area.EFST[13]; // 14. APC MODE 
            int.TryParse(area.EFST[14], out int value);               // 15. MULTI PASS MODE (int to string)
            MultiPassModeValue = value;
            */
        }
    }

    /// <summary>
    /// ViewModel for EQP Function Change (CIM Function) UI.
    /// Binds 15 equipment function/mode settings with option buttons or numeric input.
    /// Loads from file on init, saves when Save command runs.
    /// Function #1 (Cell Tracking), #2 (Tracking Control), #4 (Cell MCR Mode) are constrained
    /// to exactly 6 valid combinations; when one changes, the other two are auto-adjusted.
    /// </summary>
    public partial class CIMFunctionViewModel : ObservableObject
    {
        /// <summary>
        /// Six valid (Function#1 CellTracking, Function#2 TrackingControl, Function#4 CellMCR) combinations.
        /// </summary>
        /// <summary>
        /// Six valid combinations per image: 1–3 ON+TKIN/TKOUT/BOTH+USE; 4 OFF+NOTHING+USE; 5 NOTHING+NOTHING+UNUSE; 6 NOTHING+NOTHING+NOTHING.
        /// </summary>
        private static readonly IReadOnlyList<(OnOffNothing F1, TrackingControlMode F2, UseUnuseNothing F4)> ValidFunction1_2_4 = new[]
        {
            (OnOffNothing.ON, TrackingControlMode.TKIN, UseUnuseNothing.USE),     // 1
            (OnOffNothing.ON, TrackingControlMode.TKOUT, UseUnuseNothing.USE),    // 2
            (OnOffNothing.ON, TrackingControlMode.BOTH, UseUnuseNothing.USE),     // 3
            (OnOffNothing.OFF, TrackingControlMode.NOTHING, UseUnuseNothing.USE), // 4: OFF / NOTHING / USE
            (OnOffNothing.NOTHING, TrackingControlMode.NOTHING, UseUnuseNothing.UNUSE), // 5: NOTHING / NOTHING / UNUSE
            (OnOffNothing.NOTHING, TrackingControlMode.NOTHING, UseUnuseNothing.NOTHING), // 6: NOTHING / NOTHING / NOTHING
        };

        private readonly string _filePath;
        private bool _isCoercing;

        [ObservableProperty]
        private OnOffNothing _cellTrackingState = OnOffNothing.ON;

        [ObservableProperty]
        private TrackingControlMode _trackingControlMode = TrackingControlMode.TKIN;

        [ObservableProperty]
        private OnOffNothing _materialTrackingState = OnOffNothing.NOTHING;

        [ObservableProperty]
        private UseUnuseNothing _cellMCRMode = UseUnuseNothing.USE;

        [ObservableProperty]
        private UseUnuseNothing _materialMCRMode = UseUnuseNothing.NOTHING;

        [ObservableProperty]
        private AutoManualNothing _lotAssignMode = AutoManualNothing.NOTHING;

        [ObservableProperty]
        private AutoManualNothing _agvMode = AutoManualNothing.NOTHING;

        [ObservableProperty]
        private UseUnuseNothing _areaSensorMode = UseUnuseNothing.NOTHING;

        [ObservableProperty]
        private UseUnuseNothing _sortMode = UseUnuseNothing.NOTHING;

        [ObservableProperty]
        private InterlockControlMode _interlockControlMode = InterlockControlMode.LOADING;

        [ObservableProperty]
        private OnOffNothing _loaderLIMCRMode = OnOffNothing.NOTHING;

        [ObservableProperty]
        private OnOffNothing _loaderLSMCRMode = OnOffNothing.NOTHING;

        [ObservableProperty]
        private OnOffNothing _unloaderMCRMode = OnOffNothing.NOTHING;

        [ObservableProperty]
        private AutoManualNothing _apcMode = AutoManualNothing.NOTHING;

        /// <summary>
        /// When false, APC Mode AUTO/MANUAL buttons are disabled (NOTHING stays enabled).
        /// Set from application when creating the view model.
        /// </summary>
        [ObservableProperty]
        private bool _isUseAPC;

        [ObservableProperty]
        private int _multiPassModeValue = 0;

        public ICommand SaveCommand => new RelayCommand<string>(Save);

        /// <summary>
        /// Default path: %LocalApplicationData%\EQX.UI\CIMFunction.json.
        /// Use the other constructor to pass file path from Application.
        /// </summary>
        public CIMFunctionViewModel()
        {
            _filePath = GetDefaultFilePath();
            Load();
        }

        /// <summary>
        /// Pass file path from Application (e.g. from IConfiguration or appsettings).
        /// If null or empty, uses default path.
        /// </summary>
        public CIMFunctionViewModel(string? filePath)
        {
            _filePath = string.IsNullOrWhiteSpace(filePath) ? GetDefaultFilePath() : filePath;
            Load();
        }

        public void UpdateSingle(int index, string value)
        {
            try
            {
                if (index == 1) CellTrackingState = ParseEnum(value, CellTrackingState);
                if (index == 2) TrackingControlMode = ParseEnum(value, TrackingControlMode);
                if (index == 3) MaterialTrackingState = ParseEnum(value, MaterialTrackingState);
                if (index == 4) CellMCRMode = ParseEnum(value, CellMCRMode);
                if (index == 5) MaterialMCRMode = ParseEnum(value, MaterialMCRMode);
                if (index == 6) LotAssignMode = ParseEnum(value, LotAssignMode);
                if (index == 7) AgvMode = ParseEnum(value, AgvMode);
                if (index == 8) AreaSensorMode = ParseEnum(value, AreaSensorMode);
                if (index == 9) SortMode = ParseEnum(value, SortMode);
                if (index == 10) InterlockControlMode = ParseEnum(value, InterlockControlMode);
                if (index == 11) LoaderLIMCRMode = ParseEnum(value, LoaderLIMCRMode);
                if (index == 12) LoaderLSMCRMode = ParseEnum(value, LoaderLSMCRMode);
                if (index == 13) UnloaderMCRMode = ParseEnum(value, UnloaderMCRMode);
                if (index == 14) ApcMode = ParseEnum(value, ApcMode);
                if (index == 15)
                {
                    int.TryParse(value, out int strValue);
                    MultiPassModeValue = strValue;
                }
            }
            catch
            {
                // Keep defaults on load error
            }
        }

        private static string GetDefaultFilePath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EQX.UI");
            return Path.Combine(dir, "CIMFunction.json");
        }

        private void Load()
        {
            if (!File.Exists(_filePath))
                return;
            try
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonConvert.DeserializeObject<CIMFunctionData>(json);
                if (data == null) return;

                CellTrackingState = ParseEnum(data.CellTrackingState, CellTrackingState);
                TrackingControlMode = ParseEnum(data.TrackingControlMode, TrackingControlMode);
                MaterialTrackingState = ParseEnum(data.MaterialTrackingState, MaterialTrackingState);
                CellMCRMode = ParseEnum(data.CellMCRMode, CellMCRMode);
                MaterialMCRMode = ParseEnum(data.MaterialMCRMode, MaterialMCRMode);
                LotAssignMode = ParseEnum(data.LotAssignMode, LotAssignMode);
                AgvMode = ParseEnum(data.AgvMode, AgvMode);
                AreaSensorMode = ParseEnum(data.AreaSensorMode, AreaSensorMode);
                SortMode = ParseEnum(data.SortMode, SortMode);
                InterlockControlMode = ParseEnum(data.InterlockControlMode, InterlockControlMode);
                LoaderLIMCRMode = ParseEnum(data.LoaderLIMCRMode, LoaderLIMCRMode);
                LoaderLSMCRMode = ParseEnum(data.LoaderLSMCRMode, LoaderLSMCRMode);
                UnloaderMCRMode = ParseEnum(data.UnloaderMCRMode, UnloaderMCRMode);
                ApcMode = ParseEnum(data.ApcMode, ApcMode);
                MultiPassModeValue = data.MultiPassModeValue;
                EnsureValidFunction1_2_4();
            }
            catch
            {
                // Keep defaults on load error
            }
        }

        /// <summary>
        /// Ensures (CellTrackingState, TrackingControlMode, CellMCRMode) is one of the 6 valid tuples (e.g. after Load).
        /// </summary>
        private void EnsureValidFunction1_2_4()
        {
            if (_isCoercing) return;
            var f1 = CellTrackingState;
            var f2 = TrackingControlMode;
            var f4 = CellMCRMode;
            if (ValidFunction1_2_4.Any(t => t.F1 == f1 && t.F2 == f2 && t.F4 == f4))
                return;
            _isCoercing = true;
            try
            {
                var pick = ValidFunction1_2_4[0];
                CellTrackingState = pick.F1;
                TrackingControlMode = pick.F2;
                CellMCRMode = pick.F4;
            }
            finally
            {
                _isCoercing = false;
            }
        }

        private void Save(string byWho = "EQP")
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var data = new CIMFunctionData
                {
                    CellTrackingState = CellTrackingState.ToString(),
                    TrackingControlMode = TrackingControlMode.ToString(),
                    MaterialTrackingState = MaterialTrackingState.ToString(),
                    CellMCRMode = CellMCRMode.ToString(),
                    MaterialMCRMode = MaterialMCRMode.ToString(),
                    ApcMode = ApcMode.ToString(),
                    /*
                    LotAssignMode = LotAssignMode.ToString(),
                    AgvMode = AgvMode.ToString(),
                    AreaSensorMode = AreaSensorMode.ToString(),
                    SortMode = SortMode.ToString(),
                    InterlockControlMode = InterlockControlMode.ToString(),
                    LoaderLIMCRMode = LoaderLIMCRMode.ToString(),
                    LoaderLSMCRMode = LoaderLSMCRMode.ToString(),
                    UnloaderMCRMode = UnloaderMCRMode.ToString(),
                    ApcMode = ApcMode.ToString(),
                    MultiPassModeValue = MultiPassModeValue
                    */
                };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_filePath, json);

                var area = new EquipFunctionChangeEventArea
                {
                    ByWho = byWho,
                };
                data.ToArea(ref area);
                EquipEventHelpers.EquipFunctionChange(area);
            }
            catch
            {
                // Caller may show message if needed
            }
        }

        private static T ParseEnum<T>(string? value, T defaultValue) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        partial void OnCellTrackingStateChanged(OnOffNothing value) => CoerceToValidTuple(1);
        partial void OnTrackingControlModeChanged(TrackingControlMode value) => CoerceToValidTuple(2);
        partial void OnCellMCRModeChanged(UseUnuseNothing value) => CoerceToValidTuple(4);

        /// <summary>
        /// When one of Function#1, #2, #4 changes, adjust the other two so the triple stays in the 6 valid combinations.
        /// </summary>
        private void CoerceToValidTuple(int changedDimension)
        {
            if (_isCoercing) return;
            var f1 = CellTrackingState;
            var f2 = TrackingControlMode;
            var f4 = CellMCRMode;
            if (ValidFunction1_2_4.Any(t => t.F1 == f1 && t.F2 == f2 && t.F4 == f4))
                return; // already valid
            _isCoercing = true;
            try
            {
                (OnOffNothing F1, TrackingControlMode F2, UseUnuseNothing F4) pick;
                if (changedDimension == 1)
                {
                    var candidates = ValidFunction1_2_4.Where(t => t.F1 == f1).ToList();
                    if (candidates.Count == 0)
                        pick = ValidFunction1_2_4[0];
                    else
                    {
                        var preferred = candidates.FirstOrDefault(t => t.F2 == f2 && t.F4 == f4);
                        pick = candidates.Any(t => t.F2 == f2 && t.F4 == f4) ? preferred : candidates.First();
                    }
                    if (pick.F1 != f1) CellTrackingState = pick.F1;
                    if (pick.F2 != f2) TrackingControlMode = pick.F2;
                    if (pick.F4 != f4) CellMCRMode = pick.F4;
                }
                else if (changedDimension == 2)
                {
                    var candidates = ValidFunction1_2_4.Where(t => t.F2 == f2).ToList();
                    if (candidates.Count == 0)
                        pick = ValidFunction1_2_4[0];
                    else
                    {
                        var preferred = candidates.FirstOrDefault(t => t.F1 == f1 && t.F4 == f4);
                        pick = candidates.Any(t => t.F1 == f1 && t.F4 == f4) ? preferred : candidates.First();
                    }
                    if (pick.F1 != f1) CellTrackingState = pick.F1;
                    if (pick.F2 != f2) TrackingControlMode = pick.F2;
                    if (pick.F4 != f4) CellMCRMode = pick.F4;
                }
                else // 4
                {
                    var candidates = ValidFunction1_2_4.Where(t => t.F4 == f4).ToList();
                    if (candidates.Count == 0)
                        pick = ValidFunction1_2_4[0];
                    else
                    {
                        var preferred = candidates.FirstOrDefault(t => t.F1 == f1 && t.F2 == f2);
                        pick = candidates.Any(t => t.F1 == f1 && t.F2 == f2) ? preferred : candidates.First();
                    }
                    if (pick.F1 != f1) CellTrackingState = pick.F1;
                    if (pick.F2 != f2) TrackingControlMode = pick.F2;
                    if (pick.F4 != f4) CellMCRMode = pick.F4;
                }
            }
            finally
            {
                _isCoercing = false;
            }
        }
    }
}
