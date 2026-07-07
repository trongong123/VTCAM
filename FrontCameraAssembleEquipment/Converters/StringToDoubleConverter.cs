using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() ?? "";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                text = text.Replace(",", ".").Trim();
                if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                    return number;
            }
            return 0;
        }
    }
}
