using EQX.Core.InOut;
using EQX.InOut;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EQX.UI.Converters
{
    public class CylinderButtonBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4)
                return Brushes.Gray;

            bool isForward = values[0] is bool bForward && bForward;
            bool isBackward = values[1] is bool bBackward && bBackward;

            if (values[2] is ECylinderType cylinderType == false) return Binding.DoNothing;
            if (values[3] is string property == false) return Binding.DoNothing;

            bool isReverse = cylinderType == ECylinderType.ForwardBackwardReverse ||
                             cylinderType == ECylinderType.UpDownReverse ||
                             cylinderType == ECylinderType.OpenCloseReverse ||
                             cylinderType == ECylinderType.RightLeftReverse ||
                             cylinderType == ECylinderType.GripUngripReverse ||
                             cylinderType == ECylinderType.AlignUnalignReverse ||
                             cylinderType == ECylinderType.LockUnlockReverse ||
                             cylinderType == ECylinderType.FlipUnflipReverse ||
                             cylinderType == ECylinderType.ClampUnclampReverse;

            if (property == "Forward")
            {
                if (!isReverse)
                    return isForward ? Brushes.LightGreen : Brushes.LightGray;
                else
                    return isBackward ? Brushes.LightGreen : Brushes.LightGray;
            }
            else if (property == "Backward")
            {
                if (isReverse)
                    return isForward ? Brushes.LightGreen : Brushes.LightGray;
                else
                    return isBackward ? Brushes.LightGreen : Brushes.LightGray;
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
