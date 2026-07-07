using EQX.Core.Common;

namespace EQX.Core.Units
{
    public interface ITray<TECellStatus> : INameable where TECellStatus : Enum
    {
        TECellStatus this[uint index] { get; set; }

        /// <summary>
        /// Total row count
        /// </summary>
        int Rows { get; set; }
        /// <summary>
        /// Total column count
        /// </summary>
        int Columns { get; set; }
        /// <summary>
        /// First cell start position
        /// </summary>
        ETrayOrientation Orientation { get; set; }

        /// <summary>
        /// Cell list
        /// </summary>
        IList<ITrayCell<TECellStatus>> Cells { get; set; }

        int GetFirstIndex(TECellStatus status);

        /// <summary>
        /// Get Row number of cell by its index
        /// </summary>
        /// <param name="index">Index of cell</param>
        /// <returns>Row of the cell</returns>
        int GetRow(int index);

        /// <summary>
        /// Get first Row number of cell by it's status
        /// <param>return "-1" if no cell found</param>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int GetFirstRow(TECellStatus status);

        /// <summary>
        /// Get Column number of cell by its index
        /// </summary>
        /// <param name="index">Index of cell</param>
        /// <returns>Column of the cell</returns>
        int GetColumn(int index);

        /// <summary>
        /// Get first Column number of cell by it's status
        /// <param>return "-1" if no cell found</param>
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int GetFirstColumn(TECellStatus status);

        void GenerateCells();
        void SetAllCell(TECellStatus status);
    }
}
