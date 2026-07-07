using EQX.Core.InOut;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
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
                        return status1 ? "FWD" : "BWD";
                    case ECylinderType.UpDown:
                    case ECylinderType.UpDownReverse:
                        return status1 ? "UP" : "DOWN";
                    case ECylinderType.RightLeft:
                    case ECylinderType.RightLeftReverse:
                        return status1 ? "RIGHT" : "LEFT";
                    case ECylinderType.GripUngrip:
                    case ECylinderType.GripUngripReverse:
                        return status1 ? "GRIP" : "UNGRIP";
                    case ECylinderType.AlignUnalign:
                    case ECylinderType.AlignUnalignReverse:
                        return status1 ? "FWD" : "BWD";
                    case ECylinderType.LockUnlock:
                    case ECylinderType.LockUnlockReverse:
                        return status1 ? "LOCK" : "UNLOCK";
                    case ECylinderType.FlipUnflip:
                    case ECylinderType.FlipUnflipReverse:
                        return status1 ? "TURN" : "RETURN";
                    case ECylinderType.ClampUnclamp:
                    case ECylinderType.ClampUnclampReverse:
                        return status1 ? "CLAMP" : "UNCLAMP";
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
