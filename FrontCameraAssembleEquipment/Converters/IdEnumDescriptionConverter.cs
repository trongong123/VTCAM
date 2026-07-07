using FrontCameraAssembleEquipment.Defines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    public class IdEnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = (string)parameter;
            if (param == null) return Binding.DoNothing;

            switch (param)
            {
                case "Cylinder":
                    ECylinder eCylinderValue = (ECylinder)value;
                    var field = typeof(ECylinder).GetField(eCylinderValue.ToString());
                    var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                    return attr?.Description ?? value.ToString();
                case "Roller":
                    ESpeedController eRollerValue = (ESpeedController)value;
                    var field1 = typeof(ESpeedController).GetField(eRollerValue.ToString());
                    var attr1 = (DescriptionAttribute)Attribute.GetCustomAttribute(field1, typeof(DescriptionAttribute));
                    return attr1?.Description ?? value.ToString();
                case "Conveyor":
                    ECV eCVValue = (ECV)value;
                    var field2 = typeof(ECV).GetField(eCVValue.ToString());
                    var attr2 = (DescriptionAttribute)Attribute.GetCustomAttribute(field2, typeof(DescriptionAttribute));
                    return attr2?.Description ?? value.ToString();
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
