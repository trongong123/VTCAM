using EQX.Core.Motion;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TopComponent.Forms;

namespace FrontCameraAssembleEquipment.Converters
{
    public class MotionsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is not ObservableCollection<IMotion>) return Binding.DoNothing;

            ObservableCollection<IMotion> motions = (ObservableCollection<IMotion>)value;
            List<string> motionNameList = new List<string>();
            foreach(var motion in motions)
            {
                motionNameList.Add(motion.Name.Split('_').Last());
            }
            int count = 0;
            foreach (var name in motionNameList)
            {
                if(name.Equals(parameter.ToString()))
                {
                    count++;
                }  
            }

            if (count == 0)
            {
                return System.Windows.Visibility.Hidden;
            }
            else
            {
                return System.Windows.Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
