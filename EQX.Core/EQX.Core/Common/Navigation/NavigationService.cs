using CommunityToolkit.Mvvm.ComponentModel;

namespace EQX.Core.Common
{
    public class NavigationService : ObservableObject, INavigationService
    {
        public event EventHandler? Navigating;

        public NavigationService(NavigationStore viewModelavigationStore,
            IViewModelFactory viewModelFactory)
        {
            _viewModelavigationStore = viewModelavigationStore;
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            Navigating?.Invoke(this, EventArgs.Empty);

            var viewModel = _viewModelFactory.Create<TViewModel>();
            _viewModelavigationStore.CurrentViewModel = viewModel;
        }

        public void NavigateTo(Type viewModelType)
        {
            if (viewModelType.IsSubclassOf(typeof(ViewModelBase)) == false)
            {
                throw new Exception($"Type : {viewModelType.GetType()} not supported");
            }

            Navigating?.Invoke(this, EventArgs.Empty);

            var viewModel = _viewModelFactory.Create(viewModelType);
            _viewModelavigationStore.CurrentViewModel = viewModel;
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            _viewModelavigationStore.CurrentViewModel = viewModel;
        }

        #region Privates
        private readonly IViewModelFactory _viewModelFactory;
        private readonly NavigationStore _viewModelavigationStore;
        #endregion
    }
}
