using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Services.WindowServices
{
    public interface IWindowService
    {
        void ShowWindow<TViewModel>() where TViewModel : class;
        bool? ShowDialog<TViewModel>() where TViewModel : class;
        void Close<TViewModel>() where TViewModel : class;
    }
}
