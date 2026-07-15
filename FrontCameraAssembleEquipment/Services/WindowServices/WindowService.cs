using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Services.WindowServices
{
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowWindow<TViewModel>() where TViewModel : class
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();

            var viewTypeName = typeof(TViewModel).FullName!.Replace("ViewModel", "View");
            var viewType = Type.GetType(viewTypeName);
            if (viewType == null)
                throw new InvalidOperationException($"Cannot find View for {typeof(TViewModel).Name}");

            var view = (Window)_serviceProvider.GetRequiredService(viewType);
            view.DataContext = vm;
            view.Show();
        }

        public bool? ShowDialog<TViewModel>() where TViewModel : class
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();
            var viewTypeName = typeof(TViewModel).FullName!.Replace("ViewModel", "View");
            var viewType = Type.GetType(viewTypeName);
            var view = (Window)_serviceProvider.GetRequiredService(viewType);
            view.DataContext = vm;
            return view.ShowDialog();
        }

        public void Close<TViewModel>() where TViewModel : class
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext is TViewModel)
                {
                    window.Close();
                    break;
                }
            }
        }

    }
}
