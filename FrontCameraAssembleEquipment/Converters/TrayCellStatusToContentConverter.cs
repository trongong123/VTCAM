using System.Globalization;
using System.Windows.Data;
using FrontCameraAssembleEquipment.Defines;

namespace FrontCameraAssembleEquipment.Converters
{
    public class TrayCellStatusToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ETrayCellStatus cellStatus)
            {
                if (cellStatus == ETrayCellStatus.PickFail) return "NG";
                if (cellStatus == ETrayCellStatus.Done) return "OK";

                return cellStatus.ToString();
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
