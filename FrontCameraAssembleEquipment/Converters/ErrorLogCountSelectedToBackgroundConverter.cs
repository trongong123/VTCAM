using System.Globalization;
using System.Windows.Data;
using FrontCameraAssembleEquipment.Defines.LogHistory;

namespace FrontCameraAssembleEquipment.Converters
{
    public class ErrorLogCountSelectedToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ErrorLogEntry && values[1] is ErrorLogCount)
            {

            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
