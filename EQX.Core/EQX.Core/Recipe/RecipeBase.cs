using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Communication.CIM.Custom;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EQX.Core.Recipe
{
    public class RecipeBase : ObservableObject, IRecipe
    {
        public event RecipeChangeEventHandler? RecipeChanging;
        public event RecipeChangeEventHandler? RecipeChanged;

        public int Id { get; internal set; }

        public string Name { get; set; }

        public virtual IRecipe Load()
        {
            return new RecipeBase();
        }

        public virtual void Save(){}

        protected void OnRecipeChanged(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
        {
            RecipeChanged?.Invoke(oldValue, newValue, propertyName);
        }

        protected void OnRecipeChanging(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
        {
            RecipeChanging?.Invoke(oldValue, newValue, propertyName);
        }

        protected bool SetRecipe<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            // We duplicate the code here instead of calling the overload because we can't
            // guarantee that the invoked SetProperty<T> will be inlined, and we need the JIT
            // to be able to see the full EqualityComparer<T>.Default.Equals call, so that
            // it'll use the intrinsics version of it and just replace the whole invocation
            // with a direct comparison when possible (eg. for primitive numeric types).
            // This is the fastest SetProperty<T> overload so we particularly care about
            // the codegen quality here, and the code is small and simple enough so that
            // duplicating it still doesn't make the whole class harder to maintain.
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            OnRecipeChanging(field, newValue, propertyName);

            T backup = field;
            field = newValue;

            OnRecipeChanged(backup, newValue, propertyName);
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
