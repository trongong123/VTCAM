using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    internal class TextToTextForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                string str = (string)value;

                SolidColorBrush color = new SolidColorBrush();

                if (str.Contains("ERROR") || str.Contains("FATAL"))
                {
                    color = new SolidColorBrush(Colors.Red);
                }
                else if (str.Contains("WARN"))
                {
                    color = new SolidColorBrush(Colors.Orange);
                }
                else if (str.Contains("INFO"))
                {
                    color = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    color = new SolidColorBrush(Colors.Black);
                }

                return color;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
