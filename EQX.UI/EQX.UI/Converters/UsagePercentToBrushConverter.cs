using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class UsagePercentToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || double.TryParse(value.ToString(), out var percent) == false)
            {
                return Brushes.Green;
            }

            if (percent > 85)
            {
                return Brushes.Red;
            }

            if (percent > 60)
            {
                return Brushes.Orange;
            }

            return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
