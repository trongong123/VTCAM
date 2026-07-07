using CommunityToolkit.Mvvm.ComponentModel;
using FrontCameraAssembleEquipment.Defines.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class StopWatch : ObservableObject
    {
        public event EventHandler? WatchStop;
        public StopWatch() 
        {
            _stopwatch = new Stopwatch();

            System.Timers.Timer statusUpdateTimer = new System.Timers.Timer(100);
            statusUpdateTimer.Elapsed += TackTimeTimer_Elapsed;
            statusUpdateTimer.AutoReset = true;
            statusUpdateTimer.Enabled = true;
        }

        private void TackTimeTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (IsRunning)
            {
                OnPropertyChanged(nameof(ElapsedSecond));
            }
        }

        public bool IsRunning => _stopwatch.IsRunning;
        public double ElapsedSecondTime
        {
            get => _elapsedSecondTime;
            set
            {
                _elapsedSecondTime = value;
                OnPropertyChanged(nameof(ElapsedSecondTime));
            }
        }

        public void StartTiming()
        {
            RestartTime();
        }

        public void StopTiming()
        {
            _stopwatch?.Stop();
        }

        public void TimingSet()
        {
            ElapsedSecondTime = (double)(_stopwatch.ElapsedMilliseconds / 1000.0);
            
            WatchStop?.Invoke(this, EventArgs.Empty);
            RestartTime();
        }

        public void RestartTime()
        {
            _stopwatch.Restart();
        }

        public void Reset()
        {
            ElapsedSecondTime = 0;
            _stopwatch.Reset();
        }

        public double ElapsedSecond => (_stopwatch.ElapsedMilliseconds / 1000.0);

        private Stopwatch _stopwatch;
        private double _elapsedSecondTime;
    }
}
