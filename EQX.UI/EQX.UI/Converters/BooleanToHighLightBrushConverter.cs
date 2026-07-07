using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class BooleanToHighLightBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bValue)
            {
                if (parameter != null && bool.Parse(parameter.ToString()))
                {
                    return bValue ? Brushes.LightGray : Brushes.LightGreen;
                }
                return bValue ? Brushes.LightGreen : Brushes.LightGray;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
