namespace FrontCameraAssembleEquipment.Services.WindowServices
{
    public interface IWindowService
    {
        void ShowWindow<TViewModel>() where TViewModel : class;
        bool? ShowDialog<TViewModel>() where TViewModel : class;
        void Close<TViewModel>() where TViewModel : class;
    }
}
