namespace EQX.Core.Common
{
    public interface IViewModelFactory
    {
        TViewModel Create<TViewModel>() where TViewModel : ViewModelBase;
        ViewModelBase Create(Type viewModelType);
    }
}
