using EQX.Core.Sequence;

namespace EQX.Process
{
    public class ProcessTimer : IProcessTimer
    {
        public int WaitTime
        {
            get => waitTime;
            set
            {
                waitTime = value;
                WaitTimeSetMoment = Environment.TickCount;
            }
        }

        public int StepStartTime { get; internal set; }
        public int StepElapsedTime => Math.Max(Environment.TickCount - StepStartTime, 0);

        public int SpareTime { get; set; }

        public int TaktTimeCounter { get; set; }

        #region Privates
        private int waitTime;

        internal int WaitTimeSetMoment;
        #endregion
    }
}
