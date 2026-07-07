using EQX.Core.Sequence;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class ProcessModeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EProcessMode processMode)
            {
                switch (processMode)
                {
                    case EProcessMode.ToWarning:
                    case EProcessMode.Warning:
                        return new SolidColorBrush(Color.FromArgb(0x99, 0xff, 0xff, 0x00));
                    case EProcessMode.ToAlarm:
                    case EProcessMode.Alarm:
                        return new SolidColorBrush(Colors.Tomato);
                    case EProcessMode.ToOrigin:
                    case EProcessMode.Origin:
                    case EProcessMode.ToStop:
                    case EProcessMode.ToRun:
                    case EProcessMode.Run:
                        return new SolidColorBrush(Colors.Lime);
                    case EProcessMode.Stop:
                    case EProcessMode.None:
                    default:
                        if (Application.Current.Resources["PrimaryBackgroundColor"] is SolidColorBrush color) return color;
                        else return Binding.DoNothing;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
