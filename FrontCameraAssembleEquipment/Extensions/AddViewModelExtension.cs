using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Factories;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.MVVM.Views;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Ports;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddViewViewModelExtension
    {
        public static void AddViewModel<TViewModel>(this IServiceCollection services) where TViewModel : ViewModelBase
        {
            services.AddSingleton<TViewModel>();
        }

        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
            services.AddSingleton<MainWindowViewModel>();

            services.AddViewModel<NavigateMenuViewModel>();
            services.AddViewModel<HeaderViewModel>();
            services.AddViewModel<FooterViewModel>();

            services.AddViewModel<InitDeinitViewModel>();
            services.AddViewModel<AutoViewModel>();
            services.AddViewModel<IOMonitoringViewModel>();
            services.AddViewModel<ManualViewModel>();
            services.AddViewModel<DataViewModel>();
            services.AddViewModel<TeachViewModel>();
            services.AddViewModel<LogViewModel>();
            services.AddViewModel<OriginViewModel>();
            services.AddViewModel<InitializeViewModel>();
            services.AddViewModel<InterfaceViewModel>();
            services.AddViewModel<ProductionInfoViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddViewModel<ErrorLogViewModel>();
            services.AddViewModel<DevViewModel>();

            services.AddSingleton<NavigationStore>();
            services.AddSingleton<IViewModelFactory,ViewModelFactory>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddSingleton<IWindowService, WindowService>();

            services.AddViewModel<CameraTypeSelectViewModel>();

            services.AddTransient<UnitManualControlViewModel>();
            services.AddSingleton<ConveyorManualControlViewModel>();


            });

            return hostBuilder;
        }

        public static IHostBuilder AddManualViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, TraySupplierManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, TrayCamLoaderManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, CamSpongeDetachManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, VinylDetachManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, CamAssembleManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, ConveyorManualViewModel>();
                services.AddSingleton<MaintenanceViewModel<ESemiSequence, RecipeList>, VisionManualViewModel>();
            });

            return hostBuilder;
        }


        public static IHostBuilder AddViews(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindowView>();
                services.AddTransient<InterfaceView>();
                services.AddTransient<ProductionInfoView>();
                services.AddTransient<LoginView>();
                services.AddTransient<OriginView>();
                services.AddTransient<UnitManualControl>();
                services.AddTransient<ConveyorManualControlView>();

            });

            return hostBuilder;
        }
    }
}
