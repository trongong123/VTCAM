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
    public class InOutListPageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values[1] is uint) == false) return Binding.DoNothing;
            if (values[0] is List<IDInput> inputs)
            {
                if (inputs.Count < 9)
                {
                    return inputs.ToList();
                }

                var subList = inputs.GetRange((int)((uint)values[1] * 9), 9);

                return subList.ToList();
            }
            else if (values[0] is List<IDOutput> outputs)
            {
                if (outputs.Count < 9)
                {
                    return outputs.ToList();
                }
                var subList = outputs.GetRange((int)((uint)values[1] * 9), 9);

                return subList.ToList();
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
