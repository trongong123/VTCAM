using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public class CameraTypeSelectViewModel : ViewModelBase
    {
        private readonly RecipeSelector _recipeSelector;

        public CameraTypeSelectViewModel(RecipeSelector recipeSelector)
        {
            _recipeSelector = recipeSelector;
            _recipeSelector.PropertyChanged += _recipeSelector_PropertyChanged;
        }

        private void _recipeSelector_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CameraType)) return;
            throw new NotImplementedException();
        }

        public IRelayCommand CameraTypeSelectedCommand
        {
            get
            {
                return new RelayCommand<object>((cameraType) =>
                {
                    if (cameraType.GetType() != typeof(ECameraType)) throw new TypeLoadException();
                    _recipeSelector.CurrentRecipe.GlobalRecipe.CameraType = (ECameraType)cameraType;
                    OnPropertyChanged(nameof(CameraType));
                });
            }
        }

        public ECameraType CameraType => _recipeSelector.CurrentRecipe.GlobalRecipe.CameraType;

        public void UpdateCameraType()
        {
            OnPropertyChanged(nameof(CameraType));
        }
    }
}
