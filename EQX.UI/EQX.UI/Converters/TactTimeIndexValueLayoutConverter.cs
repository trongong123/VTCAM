using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class TactTimeIndexValueLayoutConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            int i = (int)values[0];
            int columns = (int)values[2];

            if (columns <= 0)
                return null;

            int row, col, rows;

            if (parameter?.ToString() == "Index")
            {
                int total = (int)values[1];

                rows = (int)Math.Ceiling((double)total / columns);
                row = i / columns;
                col = i % columns;

                return (col * rows + row).ToString();
            }
            else // Value
            {
                var list = values[1] as IList;
                if (list == null) return null;

                int total = list.Count;

                rows = (int)Math.Ceiling((double)total / columns);
                row = i / columns;
                col = i % columns;

                int idx = col * rows + row;

                return idx < total ? ((double)list[idx]).ToString("F3") + " s" : null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
