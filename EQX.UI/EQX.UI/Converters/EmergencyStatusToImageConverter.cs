using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EQX.UI.Converters
{
    public class EmergencyStatusToImageConverter : IMultiValueConverter, IValueConverter
    {
        private static readonly ImageSource NormalImage = CreateImage("pack://application:,,,/EQX.UI;component/Resources/Images/Components/Common/ems_normal.png");
        private static readonly ImageSource SelectedImage = CreateImage("pack://application:,,,/EQX.UI;component/Resources/Images/Components/Common/ems_selected.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsActive(value) ? SelectedImage : NormalImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
            {
                return NormalImage;
            }

            foreach (var value in values)
            {
                if (IsActive(value))
                {
                    return SelectedImage;
                }
            }

            return NormalImage;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool IsActive(object value)
        {
            return value is true;
        }

        private static ImageSource CreateImage(string uri)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
