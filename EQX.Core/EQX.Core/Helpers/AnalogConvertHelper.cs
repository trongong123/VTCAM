using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Helpers
{
    public static class AnalogConvertHelper
    {
        public static double Convert(double value, double vMin, double vMax, double unitMin, double unitMax)
        {
            if (value < vMin) value = vMin;
            if (value > vMax) value = vMax;

            return (value - vMin) * (unitMax - unitMin) / (vMax - vMin) + unitMin;
        }
    }
}
