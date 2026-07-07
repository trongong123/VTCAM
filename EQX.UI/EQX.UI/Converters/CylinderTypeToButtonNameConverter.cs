using EQX.Core.InOut;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class CylinderTypeToButtonNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ECylinderType cylinderType && parameter is string strStatus)
            {
                bool status1 = strStatus == "true";

                switch (cylinderType)
                {
                    case ECylinderType.ForwardBackward:
                    case ECylinderType.ForwardBackwardReverse:
                        return status1 ? (string)Application.Current.Resources["str_Forward"] : (string)Application.Current.Resources["str_Backward"];
                    case ECylinderType.UpDown:
                    case ECylinderType.UpDownReverse:
                        return status1 ? (string)Application.Current.Resources["str_Up"] : (string)Application.Current.Resources["str_Down"];
                    case ECylinderType.OpenClose:
                    case ECylinderType.OpenCloseReverse:
                        return status1 ? (string)Application.Current.Resources["str_Open"] : (string)Application.Current.Resources["str_Close"];
                    case ECylinderType.RightLeft:
                    case ECylinderType.RightLeftReverse:
                        return status1 ? (string)Application.Current.Resources["str_Right"] : (string)Application.Current.Resources["str_Left"];
                    case ECylinderType.GripUngrip:
                    case ECylinderType.GripUngripReverse:
                        return status1 ? (string)Application.Current.Resources["str_Grip"] : (string)Application.Current.Resources["str_Ungrip"];
                    case ECylinderType.AlignUnalign:
                    case ECylinderType.AlignUnalignReverse:
                        return status1 ? (string)Application.Current.Resources["str_Align"] : (string)Application.Current.Resources["str_Unalign"];
                    case ECylinderType.LockUnlock:
                    case ECylinderType.LockUnlockReverse:
                        return status1 ? (string)Application.Current.Resources["str_Lock"] : (string)Application.Current.Resources["str_Unlock"];
                    case ECylinderType.FlipUnflip:
                    case ECylinderType.FlipUnflipReverse:
                        return status1 ? (string)Application.Current.Resources["str_Flip"] : (string)Application.Current.Resources["str_Unflip"];
                    case ECylinderType.ClampUnclamp:
                    case ECylinderType.ClampUnclampReverse:
                        return status1 ? (string)Application.Current.Resources["str_Clamp"] : (string)Application.Current.Resources["str_Unclamp"];
                    default:
                        return Binding.DoNothing;
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
