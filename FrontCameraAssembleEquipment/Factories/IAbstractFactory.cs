using EQX.Core.Motion;
using EQX.Motion;

namespace FrontCameraAssembleEquipment.Factories
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}
