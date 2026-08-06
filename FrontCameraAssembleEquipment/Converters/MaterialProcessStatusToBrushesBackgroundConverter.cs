using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FrontCameraAssembleEquipment.Defines;

namespace FrontCameraAssembleEquipment.Converters
{
    public class MaterialProcessStatusToBrushesBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is EMaterialProcessStatus)
            {
                switch (value)
                {
                    case EMaterialProcessStatus.None:
                        return Brushes.White;
                    case EMaterialProcessStatus.Processing:
                        return Brushes.Yellow;
                    case EMaterialProcessStatus.Fail:
                        return Brushes.Tomato;
                    case EMaterialProcessStatus.Done:
                        return Brushes.Lime;
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
