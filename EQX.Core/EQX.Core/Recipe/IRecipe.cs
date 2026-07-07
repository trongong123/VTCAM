using EQX.Core.Common;
using System.Reflection;

namespace EQX.Core.Recipe
{
    public delegate void RecipeChangeEventHandler(object? oldValue, object? newValue, string? propertyName = null);

    public interface IRecipe : IBackupable<IRecipe>, IIdentifier
    {
        event RecipeChangeEventHandler? RecipeChanged;
    }

    public static class RecipeHelpers
    {
        public static void Clone(this IRecipe targetRecipe, IRecipe sourceRecipe)
        {
            PropertyInfo[] properties = targetRecipe.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.SetMethod == null) continue;

                object sourceValue = property.GetValue(sourceRecipe, null);
                property.SetValue(targetRecipe, sourceValue);
            }
        }
    }
}
