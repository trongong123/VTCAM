using FrontCameraAssembleEquipment.Defines;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontCameraAssembleEquipment.Converters
{
    public class MaterialTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is EMaterialType)
            {
                switch (value)
                {
                    case EMaterialType.Tray:
                        return (ImageSource)Application.Current.FindResource("image_material_tray");
                    case EMaterialType.Front:
                        return (ImageSource)Application.Current.FindResource("image_material_front");
                    case EMaterialType.Camera:
                        return (ImageSource)Application.Current.FindResource("image_material_vtcamflip");
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
