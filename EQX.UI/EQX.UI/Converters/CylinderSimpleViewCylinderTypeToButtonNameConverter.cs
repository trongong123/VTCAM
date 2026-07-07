using EQX.Core.InOut;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EQX.UI.Converters
{
    public class CylinderSimpleViewCylinderTypeToButtonNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ECylinderType cylinderType && parameter is string strStatus)
            {
                bool status1 = strStatus == "true";

                switch (cylinderType)
                {
                    case ECylinderType.ForwardBackward:
                        return status1 ? (string)Application.Current.Resources["str_Forward"] : (string)Application.Current.Resources["str_Backward"];
                    case ECylinderType.ForwardBackwardReverse:
                        return status1 ? (string)Application.Current.Resources["str_Backward"] : (string)Application.Current.Resources["str_Forward"];
                    case ECylinderType.UpDown:
                        return status1 ? (string)Application.Current.Resources["str_Up"] : (string)Application.Current.Resources["str_Down"];
                    case ECylinderType.UpDownReverse:
                        return status1 ? (string)Application.Current.Resources["str_Down"] : (string)Application.Current.Resources["str_Up"];
                    case ECylinderType.OpenClose:
                        return status1 ? (string)Application.Current.Resources["str_Open"] : (string)Application.Current.Resources["str_Close"];
                    case ECylinderType.OpenCloseReverse:
                        return status1 ? (string)Application.Current.Resources["str_Close"] : (string)Application.Current.Resources["str_Open"];
                    case ECylinderType.RightLeft:
                        return status1 ? (string)Application.Current.Resources["str_Right"] : (string)Application.Current.Resources["str_Left"];
                    case ECylinderType.RightLeftReverse:
                        return status1 ? (string)Application.Current.Resources["str_Left"] : (string)Application.Current.Resources["str_Right"];
                    case ECylinderType.GripUngrip:
                        return status1 ? (string)Application.Current.Resources["str_Grip"] : (string)Application.Current.Resources["str_Ungrip"];
                    case ECylinderType.GripUngripReverse:
                        return status1 ? (string)Application.Current.Resources["str_Ungrip"] : (string)Application.Current.Resources["str_Grip"];
                    case ECylinderType.AlignUnalign:
                        return status1 ? (string)Application.Current.Resources["str_Align"] : (string)Application.Current.Resources["str_Unalign"];
                    case ECylinderType.AlignUnalignReverse:
                        return status1 ? (string)Application.Current.Resources["str_Unalign"] : (string)Application.Current.Resources["str_Align"];
                    case ECylinderType.LockUnlock:
                        return status1 ? (string)Application.Current.Resources["str_Lock"] : (string)Application.Current.Resources["str_Unlock"];
                    case ECylinderType.LockUnlockReverse:
                        return status1 ? (string)Application.Current.Resources["str_Unlock"] : (string)Application.Current.Resources["str_Lock"];
                    case ECylinderType.FlipUnflip:
                        return status1 ? (string)Application.Current.Resources["str_Flip"] : (string)Application.Current.Resources["str_Unflip"];
                    case ECylinderType.FlipUnflipReverse:
                        return status1 ? (string)Application.Current.Resources["str_Unflip"] : (string)Application.Current.Resources["str_Flip"];
                    case ECylinderType.ClampUnclamp:
                        return status1 ? (string)Application.Current.Resources["str_Clamp"] : (string)Application.Current.Resources["str_Unclamp"];
                    case ECylinderType.ClampUnclampReverse:
                        return status1 ? (string)Application.Current.Resources["str_Unclamp"] : (string)Application.Current.Resources["str_Clamp"];
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
