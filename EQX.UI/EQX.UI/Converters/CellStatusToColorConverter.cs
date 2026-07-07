using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public interface ICellColorRepository
    {
        SolidColorBrush GetColorByStatus(object status);
    }

    public class CellStatusToColorConverter : IValueConverter
    {
        private readonly ICellColorRepository _cellColorRepository;

        public CellStatusToColorConverter(ICellColorRepository cellColorRepository)
        {
            _cellColorRepository = cellColorRepository;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                SolidColorBrush color = _cellColorRepository.GetColorByStatus(value);

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
