using EQX.Core.Common;
using EQX.InOut.InputSimulation;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.MVVM.Views;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                services.AddSingleton<IViewModelFactory, ViewModelFactory>();
                services.AddTransient<INavigationService, NavigationService>();
                services.AddSingleton<IWindowService, WindowService>();

                services.AddViewModel<CameraTypeSelectViewModel>();

                services.AddTransient<UnitManualControlViewModel>();
                services.AddSingleton<ConveyorManualControlViewModel>();

                services.AddSingleton<IInputSimulationViewModel>(services =>
                {
                    return services.GetRequiredService<ProcessConfig>().MachineType == EMachineType.OneConveyor
                        ? new MMFInputSimulationViewModel<EInput1CV>(
                            new List<string>
                            {
                                EInput1CV.FRONT_POWER_ON.ToString(),
                                EInput1CV.FRONT_DOOR.ToString(),
                                EInput1CV.RIGHT_DOOR.ToString(),
                                EInput1CV.REAR_DOOR.ToString(),
                                EInput1CV.TRAY_CENTERING1_OFF.ToString(),
                                EInput1CV.TRAY_CENTERING2_OFF.ToString(),
                                EInput1CV.FRONT_UNLOAD_CV_END.ToString(),
                                EInput1CV.FRONT_UNLOAD_CV_STOPPER_UP.ToString(),
                            },
                            new List<string>
                            {
                            })
                        : new MMFInputSimulationViewModel<EInput2CV>(
                            new List<string>
                            {
                                EInput2CV.FRONT_POWER_ON.ToString(),
                                EInput2CV.FRONT_DOOR.ToString(),
                                EInput2CV.RIGHT_DOOR.ToString(),
                                EInput2CV.REAR_DOOR.ToString(),
                                EInput2CV.TRAY_CENTERING1_OFF.ToString(),
                                EInput2CV.TRAY_CENTERING2_OFF.ToString(),
                                EInput2CV.FRONT_UNLOAD_CV_END.ToString(),
                            },
                            new List<string>
                            {
                            });
                });
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
