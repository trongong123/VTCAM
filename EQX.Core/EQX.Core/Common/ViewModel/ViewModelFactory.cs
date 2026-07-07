namespace EQX.Core.Common
{
    public class ViewModelFactory : IViewModelFactory
    {
        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TViewModel Create<TViewModel>() where TViewModel : ViewModelBase
        {
            if (_serviceProvider.GetService(typeof(TViewModel)) == null)
            {
                throw new Exception($"{typeof(TViewModel)} could not be solved");
            }

            return (TViewModel)_serviceProvider.GetService(typeof(TViewModel))!;
        }

        public ViewModelBase Create(Type viewModelType)
        {
            if (viewModelType.IsSubclassOf(typeof(ViewModelBase)) == false)
            {
                throw new Exception($"Type : {viewModelType.GetType()} not supported");
            }

            if (_serviceProvider.GetService(viewModelType) == null)
            {
                throw new Exception($"Type : {viewModelType.GetType()} could not be solved");
            }

            return (ViewModelBase)_serviceProvider.GetService(viewModelType)!;
        }

        #region Privates
        private readonly IServiceProvider _serviceProvider;
        #endregion
    }

}
