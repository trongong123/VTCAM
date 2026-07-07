namespace EQX.Core.Sequence
{
    public interface IProcessExecutor
    {
        /// <summary>
        /// Execute before main loop finish
        /// </summary>
        /// <returns>Return code</returns>
        bool PreProcess();

        /// <summary>
        /// Execute after main loop start
        /// </summary>
        /// <returns></returns>
        bool PostProcess();

        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.None"/>
        /// <br/>The None state is when the process not origin
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessNone();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.ToOrigin"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessToOrigin();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.Origin"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessOrigin();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.ToStop"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessToStop();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.Stop"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessStop();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.ToWarning"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessToWarning();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.Warning"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessWarning();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.ToAlarm"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessToAlarm();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.Alarm"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessAlarm();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.ToRun"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessToRun();
        /// <summary>
        /// Execute when <see cref="ProcessMode"/> is <see cref="EProcessMode.Run"/>
        /// </summary>
        /// <returns>Return code</returns>
        bool ProcessRun();
    }
}
