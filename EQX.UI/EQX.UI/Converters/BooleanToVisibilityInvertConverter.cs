using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    internal class BooleanToVisibilityInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnable)
            {
                if (isEnable) return Visibility.Collapsed;
                else return Visibility.Visible;

            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
