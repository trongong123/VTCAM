using FrontCameraAssembleEquipment.Defines.Process;
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
