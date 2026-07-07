using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Helpers
{
    public static class EnumHelpers
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return value.ToString();

            var attr = field.GetCustomAttribute<DescriptionAttribute>();

            return attr?.Description ?? value.ToString();
        }
    }
}
