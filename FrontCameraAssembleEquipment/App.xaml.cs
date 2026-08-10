using EQX.InOut.InputSimulation;
using EQX.UI.Converters;
using FrontCameraAssembleEquipment.Extensions;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.MVVM.Views;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace FrontCameraAssembleEquipment
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        private const string AppGuid = "E9768A42-35E0-4EBB-8B76-EDF0497CC896";
        private const string InputSimGuid = "0B996414-C1D0-4E6F-920D-2FEAC2CF89D5";
        private Mutex _mutex;

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
            bool isNewInstance = false;
            bool isInputSimInstance = false;

            if (e.Args.Length > 0 && e.Args[0] == "OpenInputSimWindow")
            {
                isInputSimInstance = true;
            }

            try
            {
                if (isInputSimInstance)
                {
                    _mutex = new Mutex(true, InputSimGuid, out isNewInstance);
                }
                else
                {
                    _mutex = new Mutex(true, AppGuid, out isNewInstance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mutex create error: {ex.Message}");
                Shutdown();
                return;
            }
            
            int processVisionCount = System.Diagnostics.Process.GetProcessesByName("SamSungFrontCamVision").Length;
            if (processVisionCount < 1)
            {
                //System.Diagnostics.Process.Start("C:\\FA\\BONDING_VTCAM_AUTO_LOADER\\VISION_PGM\\SamSungFrontCamVision.exe");
            }

            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            int processCount = System.Diagnostics.Process.GetProcessesByName(processName).Length;

            if (!isNewInstance)
            {
                MessageBox.Show("Ứng dụng đã được khởi chạy trước đó");

                Shutdown();
                return;
            }
        
            ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThread);
            ThreadPool.SetMinThreads(100, completionPortThread);
            ThreadPool.GetMinThreads(out int newWorker, out _);

#if SIMULATION
            if (isInputSimInstance)
            {
                Window inputSimWindow = new InputSimulationView();
                var inputSimulationVM = AppHost!.Services.GetRequiredService<IInputSimulationViewModel>();
                inputSimulationVM.SetOriginInputsCommand?.Execute(null);
                inputSimWindow.DataContext = inputSimulationVM;
                inputSimWindow.Show();
                return;
            }
            else
            {
                string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                System.Diagnostics.Process.Start(exePath, "OpenInputSimWindow");
            }
#endif

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
