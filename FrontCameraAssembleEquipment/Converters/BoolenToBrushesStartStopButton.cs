using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontCameraAssembleEquipment.Converters
{
    public class BoolenToBrushesStartStopButton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if(parameter is string paramStr && paramStr == "Start")
                {
                    return (bool)value ? Brushes.Lime : Brushes.Silver;
                }
                else if(parameter is string paramStrStop && paramStrStop == "Stop")
                {
                    return (bool)value ? Brushes.Red : Brushes.Silver;
                }
                else if(parameter is string paramStrReset && paramStrReset == "InOutStop")
                {
                    return (bool)value ? Brushes.Tomato : Brushes.White;
                }
                else if (parameter is string paramStrMotion && paramStrMotion == "Motion")
                {
                    return (bool)value ? Brushes.Yellow : Brushes.White;
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
