using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EQX.Core.Units
{
    public class Tray<TECellStatus> : INotifyPropertyChanged, ITray<TECellStatus> where TECellStatus : Enum
    {
        #region Properties
        public TECellStatus this[uint index]
        {
            get
            {
                return Cells.First(c => c.Id == index).Status;
            }
            set
            {
                Cells.First(c => c.Id == index).Status = value;
            }
        }

        public string Name { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public int Rows
        {
            get => rows;
            set
            {
                rows = value;
                OnPropertyChanged(nameof(Rows));
            }
        }

        public int Columns
        {
            get => cols;
            set
            {
                cols = value;
                OnPropertyChanged(nameof(Columns));
            }
        }
        public ETrayOrientation Orientation { get; set; }
        public IList<ITrayCell<TECellStatus>> Cells { get; set; }
        #endregion

        #region Constructor(s)
        public Tray(string name)
        {
            Name = name;
        }
        #endregion

        #region Public methods
        public int GetFirstIndex(TECellStatus status)
        {
            if (Cells.Any(c => c.Status.Equals(status)) == false) return -1;

            int index = Cells.OrderBy(c => c.Id).First(c => c.Status.Equals(status)).Id;

            return index;
        }

        public int GetColumn(int index)
        {
            if (index == -1) return -1;

            int indexInList = Cells.ToList().FindIndex(c => c.Id == index);

            return indexInList % Columns + 1;
        }

        public int GetFirstColumn(TECellStatus status)
        {
            if (Cells.Any(c => c.Status.Equals(status)) == false) return -1;

            int index = Cells.OrderBy(c => c.Id).First(c => c.Status.Equals(status)).Id;

            return GetColumn(index);
        }

        public int GetRow(int index)
        {
            if (index == -1) return -1;

            int indexInList = Cells.ToList().FindIndex(c => c.Id == index);

            return indexInList / Columns + 1;
        }

        public int GetFirstRow(TECellStatus status)
        {
            if (Cells.Any(c => c.Status.Equals(status)) == false) return -1;

            int index = Cells.OrderBy(c => c.Id).First(c => c.Status.Equals(status)).Id;

            return GetRow(index);
        }

        public void GenerateCells()
        {
            Cells = new ObservableCollection<ITrayCell<TECellStatus>>();
            switch (Orientation)
            {
                case ETrayOrientation.TopLeft:
                    for (int r = 1; r <= Rows; r++)
                    {
                        for (int c = 1; c <= Columns; c++)
                        {
                            Cells.Add(new TrayCell<TECellStatus>((uint)(Columns * (r - 1) + c)));
                        }
                    }
                    break;
                case ETrayOrientation.TopRight:
                    for (int r = 1; r <= Rows; r++)
                    {
                        for (int c = Columns; c >= 1; c--)
                        {
                            Cells.Add(new TrayCell<TECellStatus>((uint)(Columns * (r - 1) + c)));
                        }
                    }
                    break;
                case ETrayOrientation.BottomLeft:
                    for (int r = Rows; r >= 1; r--)
                    {
                        for (int c = 1; c <= Columns; c++)
                        {
                            Cells.Add(new TrayCell<TECellStatus>((uint)(Columns * (r - 1) + c)));
                        }
                    }
                    break;
                case ETrayOrientation.BottomRight:
                    for (int r = Rows; r >= 1; r--)
                    {
                        for (int c = Columns; c >= 1; c--)
                        {
                            Cells.Add(new TrayCell<TECellStatus>((uint)(Columns * (r - 1) + c)));
                        }
                    }
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(Cells));
        }

        public void SetAllCell(TECellStatus status)
        {
            Cells.ToList().ForEach(c => c.Status = status);
        }
        #endregion

        #region Privates
        private int rows;
        private int cols;
        #endregion
    }
}
