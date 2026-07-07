using System.Collections.ObjectModel;

namespace EQX.Core.Recipe
{
    /// <summary>
    /// Đại diện cho một vị trí logic đa điểm, ví dụ: "Vị trí Sẵn sàng", "Vị trí Tiêm".
    /// </summary>
    public class MultiPointPosition
    {
        public string Name { get; set; }
        public ObservableCollection<PositionPoint> Points { get; set; } = new ObservableCollection<PositionPoint>();
    }
}
