using Microsoft.Extensions.DependencyInjection;
using FrontCameraAssembleEquipment.MVVM.ViewModels;

namespace FrontCameraAssembleEquipment.Factories
{
    public class AbstractFactory<T> : IAbstractFactory<T>
    {
        private readonly Func<T> _factory;

        public AbstractFactory(Func<T> factory)
        {
            _factory = factory;
        }

        public T Create()
        {
            return _factory();
        }
    }
}
