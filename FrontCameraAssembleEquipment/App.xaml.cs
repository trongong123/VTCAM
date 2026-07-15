using EQX.UI.Converters;
using FrontCameraAssembleEquipment.Extensions;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.MVVM.Views;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace FrontCameraAssembleEquipment
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .AddConfigs()
                .AddViews()
                .AddViewModels()
                .AddManualViewModels()
                .AddStores()
                .AddMachineDescriptions()
                .AddMachineType()
                .AddDevices()
                .AddVaccum()
                .AddRecipes()
                .AddProcesses()
                .AddProcessIO()
                .AddVisionServices()
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            int processVisionCount = System.Diagnostics.Process.GetProcessesByName("SamSungFrontCamVision").Length;
            if (processVisionCount < 1)
            {
                //System.Diagnostics.Process.Start("C:\\FA\\BONDING_VTCAM_AUTO_LOADER\\VISION_PGM\\SamSungFrontCamVision.exe");
            }

            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            int processCount = System.Diagnostics.Process.GetProcessesByName(processName).Length;

            if (processCount > 1)
            {
                MessageBox.Show("This program is already running.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                await AppHost!.StopAsync();
                Application.Current.Shutdown();
                return;
            }
            ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThread);
            ThreadPool.SetMinThreads(100, completionPortThread);
            ThreadPool.GetMinThreads(out int newWorker, out _);

            if (MachineStatus.IsNoteBookMode == false) WindowStateHelper.HideTaskbar();

            await AppHost!.StartAsync();

            var converter = AppHost.Services.GetRequiredService<CellStatusToColorConverter>();
            Application.Current.Resources.Add(nameof(CellStatusToColorConverter), converter);

            Window window = AppHost.Services.GetRequiredService<MainWindowView>();
            window.DataContext = AppHost.Services.GetRequiredService<MainWindowViewModel>();
            window.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            WindowStateHelper.ShowTaskbar();

            base.OnExit(e);
        }
    }
}
