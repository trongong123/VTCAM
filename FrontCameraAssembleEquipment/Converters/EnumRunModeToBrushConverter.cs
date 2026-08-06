using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FrontCameraAssembleEquipment.Defines.Process;

namespace FrontCameraAssembleEquipment.Converters
{
    public class EnumRunModeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not EMachineRunMode runMode) return Binding.DoNothing;

            switch (runMode)
            {
                case EMachineRunMode.Dryrun:
                    return Brushes.Orange;
                case EMachineRunMode.Auto:
                    return Brushes.Lime;
                case EMachineRunMode.ByPass:
                    return Brushes.Gray;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
