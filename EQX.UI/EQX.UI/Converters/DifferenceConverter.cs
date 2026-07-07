using System;
using System.Globalization;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class DifferenceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is double target &&
                values[1] is double current)
            {
                return target - current;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}