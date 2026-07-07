using FrontCameraAssembleEquipment.Defines;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrontCameraAssembleEquipment.Converters
{
    public class CellStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource imageSource;
            if (value is ETrayCellStatus status)
            {
                if (status == ETrayCellStatus.Ready || status == ETrayCellStatus.Working)
                    imageSource = Application.Current.TryFindResource("image_material_vtcam") as ImageSource;
                else
                    imageSource = null;

                return imageSource;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
