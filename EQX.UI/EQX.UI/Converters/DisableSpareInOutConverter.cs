using System.Globalization;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class DisableSpareInOutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.ToUpper().StartsWith("SPARE") ? "true" : "false";
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
