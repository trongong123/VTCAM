namespace EQX.Core.Units
{
    public interface ICellFactory<TECellStatus> where TECellStatus : Enum
    {
        ITrayCell<TECellStatus> Create(int index);
    }
}
