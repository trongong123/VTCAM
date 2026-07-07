using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return false;
            }

            // Xử lý trường hợp giá trị đầu tiên là null (trạng thái ban đầu)
            if (values[0] == null)
            {
                return false;
            }

            return object.Equals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Hỗ trợ two-way cho RadioButton + enum: khi IsChecked=true và parameter có giá trị (tên enum), gán lại cho binding đầu tiên
            if (value is bool isChecked && isChecked && parameter != null && targetTypes != null && targetTypes.Length > 0 && targetTypes[0].IsEnum)
            {
                var first = Enum.Parse(targetTypes[0], parameter.ToString()!);
                return targetTypes.Length > 1
                    ? new object[] { first, Binding.DoNothing }
                    : new object[] { first };
            }
            throw new NotImplementedException();
        }
    }
}