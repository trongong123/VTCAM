using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Windows.Input;

namespace EQX.Core.Units
{
    public class TrayCell<TECellStatus> : ObservableObject, ITrayCell<TECellStatus> where TECellStatus : Enum
    {
        public event CellClickedEventHandler<TECellStatus> CellClicked;
        public event CellClickedEventHandler<TECellStatus> CellDoubleClicked;
        [JsonIgnore]
        public ICommand CellClickedCommand { get; }
        [JsonIgnore]
        public ICommand CellDoubleClickedCommand { get; }

        public void CellDoubleClick()
        {
            CellDoubleClicked?.Invoke((uint)Id, Status);
        }
        public TECellStatus Status 
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public int Id { get; }

        public TrayCell(uint id)
        {
            Id = (int)id;

            CellClickedCommand = new RelayCommand(() =>
            {
                CellClicked?.Invoke((uint)Id, Status);
                OnPropertyChanged(nameof(Status));
            });

            CellDoubleClickedCommand = new RelayCommand<object>((o) =>
            {
                CellDoubleClicked?.Invoke((uint)Id, Status);
            });
        }

        private TECellStatus _status;
    }
}
