using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace EQX.UI.MVVM
{
    public class DiskInfoModel
    {
        public string DriveName { get; set; }
        public double FreeGB { get; set; }
        public double TotalGB { get; set; }
        public double UsedPercent { get; set; }
    }

    public class PCInformationsystemViewModel : INotifyPropertyChanged
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MemoryStatusEx
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

        private readonly DispatcherTimer timer;
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter[] gpuCounters;

        public ObservableCollection<DiskInfoModel> Disks { get; set; }
            = new ObservableCollection<DiskInfoModel>();

        private double cpuPercent;
        public double CpuPercent
        {
            get => cpuPercent;
            set
            {
                cpuPercent = value;
                OnPropertyChanged();
                CpuText = $"{cpuPercent:F1}%";
            }
        }

        private string cpuText;
        public string CpuText
        {
            get => cpuText;
            set
            {
                cpuText = value;
                OnPropertyChanged();
            }
        }

        private double ramPercent;
        public double RamPercent
        {
            get => ramPercent;
            set
            {
                ramPercent = value;
                OnPropertyChanged();
            }
        }

        private string ramText;
        public string RamText
        {
            get => ramText;
            set
            {
                ramText = value;
                OnPropertyChanged();
            }
        }

        private double gpuPercent;
        public double GpuPercent
        {
            get => gpuPercent;
            set
            {
                gpuPercent = value;
                OnPropertyChanged();
                GpuText = $"{gpuPercent:F1}%";
            }
        }

        private string gpuText = "0.0%";
        public string GpuText
        {
            get => gpuText;
            set
            {
                gpuText = value;
                OnPropertyChanged();
            }
        }

        public PCInformationsystemViewModel()
        {
            cpuCounter = new PerformanceCounter(
                "Processor",
                "% Processor Time",
                "_Total");
            gpuCounters = CreateGpuCounters();

            var uiDispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            timer = new DispatcherTimer(DispatcherPriority.Background, uiDispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };

            timer.Tick += Timer_Tick;

            UpdateAll();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateCPU();
            UpdateRAM();
            UpdateGPU();
            UpdateDisk();
        }

        private void UpdateCPU()
        {
            CpuPercent = cpuCounter.NextValue();
        }

        private void UpdateRAM()
        {
            var memoryStatus = new MemoryStatusEx
            {
                dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>()
            };

            if (GlobalMemoryStatusEx(ref memoryStatus) == false || memoryStatus.ullTotalPhys == 0)
            {
                return;
            }

            double total =
                memoryStatus.ullTotalPhys / 1024.0 / 1024 / 1024;

            double available =
                memoryStatus.ullAvailPhys / 1024.0 / 1024 / 1024;

            double used = total - available;

            RamPercent = used / total * 100;

            RamText =
                $"{used:F1} / {total:F1} GB ({RamPercent:F1}%)";
        }

        private void UpdateGPU()
        {
            if (gpuCounters.Length == 0)
            {
                GpuPercent = 0;
                return;
            }

            float total = 0;
            foreach (var counter in gpuCounters)
            {
                try
                {
                    total += counter.NextValue();
                }
                catch
                {
                    // Ignore invalid counter instances and keep remaining values.
                }
            }

            GpuPercent = Math.Max(0, Math.Min(100, total));
        }

        private static PerformanceCounter[] CreateGpuCounters()
        {
            try
            {
                var category = new PerformanceCounterCategory("GPU Engine");
                return category
                    .GetInstanceNames()
                    .Where(name => name.EndsWith("engtype_3D", StringComparison.OrdinalIgnoreCase))
                    .Select(name => new PerformanceCounter("GPU Engine", "Utilization Percentage", name))
                    .ToArray();
            }
            catch
            {
                return Array.Empty<PerformanceCounter>();
            }
        }

        private void UpdateDisk()
        {
            Disks.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady)
                    continue;

                double total =
                    drive.TotalSize / 1024.0 / 1024 / 1024;

                double free =
                    drive.TotalFreeSpace / 1024.0 / 1024 / 1024;

                double usedPercent =
                    ((total - free) / total) * 100;

                Disks.Add(new DiskInfoModel
                {
                    DriveName = drive.Name,
                    FreeGB = free,
                    TotalGB = total,
                    UsedPercent = usedPercent
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
