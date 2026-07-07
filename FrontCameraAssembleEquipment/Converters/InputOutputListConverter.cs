using EQX.Core.InOut;
using System.Globalization;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    internal class InputOutputListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values[1] is uint) == false) return Binding.DoNothing;

            if (values[0] is List<IDInput> inputs)
            {
                var subList = inputs.GetRange((int)((uint)values[1] * 16), 16);
                IDInput[] newList = new IDInput[subList.Count];
                for (int i = 0; i < subList.Count / 2; i++)
                {
                    newList[i * 2] = subList[i];
                    newList[i * 2 + 1] = subList[i + 8];
                }

                return newList.ToList();
            }
            else if (values[0] is List<IDOutput> outputs)
            {
                var subList = outputs.GetRange((int)((uint)values[1] * 16), 16);
                IDOutput[] newList = new IDOutput[subList.Count];
                for (int i = 0; i < subList.Count / 2; i++)
                {
                    newList[i * 2] = subList[i];
                    newList[i * 2 + 1] = subList[i + 8];
                }

                return newList.ToList();
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
