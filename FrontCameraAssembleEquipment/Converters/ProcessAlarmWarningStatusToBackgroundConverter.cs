using System.Globalization;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    public class ProcessAlarmWarningStatusToLabelContentConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
            {
                return Binding.DoNothing;
            }

            if (parameter is string param && param == "Alarm")
            {
                return (bool)value ? "Alarm, Need Origin" : "Origin Done";
            }
            else if (parameter is string param2 && param2 == "Warning")
            {
                return (bool)value ? "Warning,Need Initialize" : "Ready";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
