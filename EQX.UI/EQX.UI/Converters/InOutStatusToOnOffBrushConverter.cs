using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class InOutStatusToOnOffBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool bValue && values[1] is string strName)
            {
                if (strName.ToUpper().StartsWith("SPARE")) return Brushes.DarkGray;
                return bValue ? Brushes.Lime : Brushes.White;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
