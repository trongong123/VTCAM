using System;
using System.Globalization;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    /// <summary>
    /// Enum ↔ bool for RadioButton IsChecked. Single Binding + ConverterParameter avoids "Two-way binding requires Path" with MultiBinding.
    /// Convert: (enum value, parameter string) → true if value.ToString() equals parameter.
    /// ConvertBack: (true, parameter string) → enum parsed from parameter using target type.
    /// </summary>
    public class EnumEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b && parameter != null && targetType.IsEnum)
                return Enum.Parse(targetType, parameter.ToString()!);
            return Binding.DoNothing;
        }
    }
}
