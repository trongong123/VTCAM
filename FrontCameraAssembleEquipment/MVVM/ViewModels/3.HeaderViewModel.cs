using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class HeaderViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;

        public Information Information { get; }
        public DateTime Now => DateTime.Now;
        public RecipeSelector RecipeSelector { get; }

        public ICommand ApplicationCloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)System.Windows.Application.Current.Resources["str_AreYouSureYouWantToCloseApplication"]) == false)
                    {
                        return;
                    }

                    _navigationService.NavigateTo<InitDeinitViewModel>();
                    _viewModelFactory.Create<InitDeinitViewModel>().Deinitialization();
                });
            }
        }

        public HeaderViewModel(Information information,
            INavigationService navigationService,
            IViewModelFactory viewModelFactory,
            RecipeSelector recipeSelector)
        {
            Information = information;
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;
            RecipeSelector = recipeSelector;
            System.Timers.Timer timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(Now));
        }
    }
}
