using EQX.Core.Common;
using System.Windows.Input;

namespace EQX.Core.Units
{
    public delegate void CellClickedEventHandler<TECellStatus>(uint id, TECellStatus beforeClickStatus) where TECellStatus : Enum;

    public interface ITrayCell<TECellStatus> : IIndexer where TECellStatus : Enum
    {
        event CellClickedEventHandler<TECellStatus> CellClicked;
        event CellClickedEventHandler<TECellStatus> CellDoubleClicked;
        ICommand CellClickedCommand { get; }
        ICommand CellDoubleClickedCommand { get; }

        TECellStatus Status { get; set; }
    }
}
