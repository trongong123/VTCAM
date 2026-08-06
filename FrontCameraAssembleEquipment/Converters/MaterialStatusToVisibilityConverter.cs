using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FrontCameraAssembleEquipment.Defines;

namespace FrontCameraAssembleEquipment.Converters
{
    public class MaterialStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is EMaterialStatus)
            {
                switch (value)
                {
                    case EMaterialStatus.NotExist:
                        return Visibility.Hidden;
                    case EMaterialStatus.Existing:
                        return Visibility.Visible;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
