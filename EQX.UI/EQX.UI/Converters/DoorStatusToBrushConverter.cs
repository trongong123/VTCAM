using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class DoorStatusToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not bool lockValue || values[1] is not bool latchValue)
            {
                return Brushes.Gray;
            }

            if (lockValue && latchValue)
            {
                return Brushes.LimeGreen;
            }

            if (lockValue || latchValue)
            {
                return Brushes.Gold;
            }

            return Brushes.Red;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
