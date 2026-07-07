namespace EQX.Core.Common
{
    //var timer = new NonOverlappingTimer(1000); // 1 giây
    //timer.Elapsed += (s, e) => { /* Xử lý khi timer tick */ };
    //timer.Start();

    //// Để tạm dừng:
    //timer.Pause();

    //// Để chạy lại:
    //timer.Resume();
    public class NonOverlappingTimer
    {
        private readonly System.Timers.Timer _timer;
        private readonly double _interval;

        public event System.Timers.ElapsedEventHandler Elapsed;

        public NonOverlappingTimer(double interval)
        {
            _interval = interval;
            _timer = new System.Timers.Timer(_interval);
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Elapsed?.Invoke(sender, e);
            }
            finally
            {
                _timer.Start();
            }
        }

        public void Start()
        {
            _timer.Interval = _interval;
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void Resume()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
