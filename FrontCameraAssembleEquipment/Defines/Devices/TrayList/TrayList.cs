using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Units;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Converters;
using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.Defines
{
    public class TrayList : ObservableObject
    {
        #region Privates
        private ObservableCollection<ButtonOnOffTrayStatus> onOffCellButtons = new ObservableCollection<ButtonOnOffTrayStatus>();
        private ObservableCollection<ButtonOnOffTrayStatus> onOffCellColumnButtons = new ObservableCollection<ButtonOnOffTrayStatus>();

        private ITray<ETrayCellStatus> tray;
        private readonly RecipeSelector _recipeSelector;
        private readonly IConfiguration _configuration;
        private string BackupFolder => _configuration["Folders:BackupFolder"] ?? "";
        #endregion

        #region Contructors
        public TrayList(RecipeSelector recipeSelector,
            IConfiguration configuration)
        {
            _recipeSelector = recipeSelector;
            _configuration = configuration;
            TrayCamera = new Tray<ETrayCellStatus>("TrayCamera");

            ListStatus = new ObservableCollection<ETrayCellStatus>(Enum.GetValues(typeof(ETrayCellStatus)).Cast<ETrayCellStatus>());
        }
        #endregion

        public ObservableCollection<ButtonOnOffTrayStatus> OnOffCellButtons
        {
            get { return onOffCellButtons; }
            set
            {
                onOffCellButtons = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ButtonOnOffTrayStatus> OnOffCellColumnButtons
        {
            get { return onOffCellColumnButtons; }
            set 
            {
                onOffCellColumnButtons = value;
                OnPropertyChanged();
            }
        }



        public ITray<ETrayCellStatus> TrayCamera
        {
            get => tray;
            set
            {
                tray = value;
                OnPropertyChanged(nameof(TrayCamera));
                if (tray != null)
                {
                    OnOffCellButtons.Clear();
                    for (int i = 1; i <= tray.Rows; ++i) OnOffCellButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });

                    OnOffCellColumnButtons.Clear();
                    for (int i = 1; i <= tray.Columns; ++i) OnOffCellColumnButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });
                }
                OnPropertyChanged(nameof(OnOffCellButtons));
            }
        }

        [JsonIgnore]
        public ObservableCollection<ETrayCellStatus> ListStatus { get; set; }



        #region Public Methods
        public void RecipeUpdatedHandle()
        {
            TrayCamera.Rows = _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows;
            TrayCamera.Columns = _recipeSelector.CurrentRecipe.TraySuplierRecipe.Columns;

            TrayCamera.GenerateCells();
        }

        public void SubscribeCellClickedEvent()
        {
            TrayCamera.Cells?.ToList().ForEach(cell =>
            {
                cell.CellClicked += (id, status) =>
                {
                    if (status == ETrayCellStatus.Skip) cell.Status = ETrayCellStatus.Ready;
                    else cell.Status = ETrayCellStatus.Skip;
                };
            });

            TrayCamera.Cells?.ToList().ForEach(cell =>
            {
                cell.CellDoubleClicked += (id, status) =>
                {
                    if (status == ETrayCellStatus.Skip)
                    {
                        
                        TrayCamera.Cells?.ToList().Where(c => c.Id < id).ToList().ForEach(c =>
                        {
                            c.Status = ETrayCellStatus.Skip;
                        });
                        TrayCamera.Cells?.ToList().Where(c => c.Id > id).ToList().ForEach(c =>
                        {
                            c.Status = ETrayCellStatus.Ready;
                        });
                        TrayCamera.Cells?.ToList().Where(c => c.Id == id).ToList().ForEach(c =>
                        {
                            c.Status = ETrayCellStatus.Working;
                        });
                    }
                };
            });

            OnOffCellButtons.Clear();
            for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows; ++i) OnOffCellButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });

            OnOffCellColumnButtons.Clear();
            for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Columns; ++i) OnOffCellColumnButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });
        }

        public void Save()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            settings.Converters.Add(new TrayConverter());
            string backupTrayAndCassette = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
            string backupTrayAndCassetteFile = Path.Combine(BackupFolder, "TrayList.json");

            File.WriteAllText(backupTrayAndCassetteFile, backupTrayAndCassette);

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    OnOffCellButtons.Clear();

            //    for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows; ++i)
            //        OnOffCellButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });
            //});
        }

        public bool Load()
        {
            string backupTrayFile = Path.Combine(BackupFolder, "TrayList.json");

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            settings.Converters.Add(new TrayConverter());
            try
            {
                if (File.Exists(backupTrayFile) == false)
                {
                    File.WriteAllText(backupTrayFile, JsonConvert.SerializeObject(this, settings));
                }
                string backupTrayAndCassetteFileContent = File.ReadAllText(backupTrayFile);
                TrayList trayList = JsonConvert.DeserializeObject<TrayList>(backupTrayAndCassetteFileContent, settings);
                OnOffCellButtons.Clear();
                for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows; ++i) OnOffCellButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });

                OnOffCellColumnButtons.Clear();
                for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Columns; ++i) OnOffCellColumnButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });
                //
                TrayCamera.Rows = _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows;
                TrayCamera.Columns = _recipeSelector.CurrentRecipe.TraySuplierRecipe.Columns;
                TrayCamera.Orientation = _recipeSelector.CurrentRecipe.TraySuplierRecipe.JigOrientation;

                TrayCamera.GenerateCells();

                foreach (var cell in trayList.TrayCamera.Cells)
                {
                    TrayCamera.Cells.FirstOrDefault(c => c.Id == cell.Id, new TrayCell<ETrayCellStatus>(0)).Status = cell.Status;
                }

                if (TrayCamera.Cells == null) TrayCamera.GenerateCells();
                OnOffCellButtons.Clear();
                for (int i = 1; i <= _recipeSelector.CurrentRecipe.TraySuplierRecipe.Rows; ++i) OnOffCellButtons.Add(new ButtonOnOffTrayStatus() { Index = i, Name = "On" });
            }
            catch (Exception ex)
            {
                MessageBoxEx.ShowDialog(ex.Message);
                return false;
            }

            return true;
        }

        private void RowStatus(ITray<ETrayCellStatus> tray, object rowIndexObj, ETrayCellStatus readyStatus, ETrayCellStatus skipStatus)
        {
            if (tray?.Cells == null || tray.Columns <= 0) return;

            // I change to this because I Flip the Tray View
            int rowIndex = (tray.Rows + 1) - int.Parse(rowIndexObj.ToString()) - 1;
            int cnt = tray.Cells.Where((cell, index) => index / tray.Columns == rowIndex && cell.Status == readyStatus).Count();

            var newStatus = cnt == tray.Columns ? skipStatus : readyStatus;

            foreach (var cell in tray.Cells.Where((cell, index) => index / tray.Columns == rowIndex))
            {
                cell.Status = newStatus;
            }
        }

        private void ColumnStatus(ITray<ETrayCellStatus> tray, object columnIndexObj, ETrayCellStatus readyStatus, ETrayCellStatus skipStatus)
        {
            if (tray?.Cells == null || tray.Rows <= 0) return;

            // I change to this because I Flip the Tray View
            int columnIndex = int.Parse(columnIndexObj.ToString());

            ETrayCellStatus newStatus = ETrayCellStatus.Ready;
            if(tray.Cells.Where(cell => tray.GetColumn(cell.Id) == columnIndex).All(c => c.Status == skipStatus))
            {
                newStatus = readyStatus;
            }
            else
            {
                newStatus = skipStatus;
            }
            foreach (var cell in tray.Cells.Where(cell => tray.GetColumn(cell.Id) == columnIndex))
            {
                cell.Status = newStatus;
            }
        }

        private void TrayStatus(ITray<ETrayCellStatus> tray)
        {
            if (tray.Cells.Any(c => c.Status != ETrayCellStatus.Ready))
            {
                tray.SetAllCell(ETrayCellStatus.Ready);
            }
            else
            {
                tray.SetAllCell(ETrayCellStatus.Skip);
            }
        }

        [JsonIgnore]
        public RelayCommand<object> RowOnOffCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    RowStatus(TrayCamera, o, ETrayCellStatus.Ready, ETrayCellStatus.Skip);
                });
            }
        }

        [JsonIgnore]
        public RelayCommand<object> ColumnOnOffCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    ColumnStatus(TrayCamera, o, ETrayCellStatus.Ready, ETrayCellStatus.Skip);
                });
            }
        }

        [JsonIgnore]
        public RelayCommand ResetAllCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    TrayStatus(TrayCamera);
                });
            }
        }

        #endregion
    }
}
