using CommunityToolkit.Mvvm.ComponentModel;

namespace EQX.Core.Recipe
{
    public class RecipeSetting : ObservableObject
    {
        private string currentRecipe;

        public string CurrentRecipe
        {
            get => currentRecipe;
            set
            {
                currentRecipe = value;
                OnPropertyChanged(nameof(CurrentRecipe));
            }
        }

        public RecipeSetting()
        {
            CurrentRecipe = "";
        }
    }
}
