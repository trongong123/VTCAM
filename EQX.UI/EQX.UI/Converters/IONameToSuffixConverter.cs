using System.Globalization;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    internal class IONameToSuffixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string ioName && string.IsNullOrWhiteSpace(ioName) == false)
            {
                return $" (IO: {ioName})";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
