using EQX.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class FooterViewModel : ViewModelBase
    {
        public ViewModelBase NavigateVM { get; set; }

        /// <summary>
        /// Is InitDeinitView currently be displayed?
        /// </summary>
        public bool IsInitializing => _navigationStore?.CurrentViewModel?.GetType() != typeof(InitDeinitViewModel);

        public FooterViewModel(NavigationStore navigationStore,
            IViewModelFactory viewModelFactory)
        {
            _navigationStore = navigationStore;
            _viewModelFactory = viewModelFactory;

            _navigationStore.CurrentViewModelChanged += _viewModelNavigationStore_CurrentViewModelChanged;

            NavigateVM = _viewModelFactory.Create<NavigateMenuViewModel>();
        }

        private void _viewModelNavigationStore_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(IsInitializing));
        }

        private readonly NavigationStore _navigationStore;
        private readonly IViewModelFactory _viewModelFactory;

    }
}
