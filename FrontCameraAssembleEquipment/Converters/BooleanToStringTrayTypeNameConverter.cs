using System.Globalization;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    public class BooleanToStringTrayTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bValue)
            {
                return bValue ? "CAMMSYS" : "NAMUGA";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
