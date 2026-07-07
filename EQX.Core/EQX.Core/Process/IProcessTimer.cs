namespace EQX.Core.Sequence
{
    public interface IProcessTimer
    {
        /// <summary>
        /// In milisecond (ms)
        /// </summary>
        int StepElapsedTime { get; }
        /// <summary>
        /// In milisecond (ms)
        /// </summary>
        int StepStartTime { get; }
        /// <summary>
        /// Set wait time for waiting before running next step
        /// </summary>
        int WaitTime { get; }

        /// <summary>
        /// User free to use
        /// </summary>
        int SpareTime { get; set; }
        int TaktTimeCounter { get; set; }
    }
}