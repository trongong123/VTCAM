namespace EQX.Core.Common
{
    public interface INavigationService
    {
        event EventHandler? Navigating;

        void NavigateTo<T>() where T : ViewModelBase;
        void NavigateTo(Type viewModelType);
        void NavigateTo(ViewModelBase viewModel);
    }
}
