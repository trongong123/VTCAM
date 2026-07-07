using System.Collections.ObjectModel;
using System.Reflection;

namespace EQX.Core.Helpers
{
    public static class PropertyHelpers
    {
        public static ObservableCollection<TProp> GetProperties<TProp>(object instance)
        {
            if (instance == null) return new ObservableCollection<TProp>();

            var props = instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => typeof(TProp).IsAssignableFrom(p.PropertyType));

            ObservableCollection<TProp> result = new ObservableCollection<TProp>();

            foreach (var prop in props)
            {
                var value = prop.GetValue(instance);
                if (value is TProp typedValue)
                {
                    result.Add(typedValue);
                }
            }

            return result;
        }
    }
}
