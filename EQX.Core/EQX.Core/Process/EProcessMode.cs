namespace EQX.Core.Sequence
{
    public enum EProcessMode
    {
        /// <summary>
        /// The default mode, before origin occurs
        /// </summary>
        None,       // 0
        ToWarning,
        /// <summary>
        /// Machine need to be check before continue working. No Origin action required.
        /// </summary>
        Warning,
        ToAlarm,
        /// <summary>
        /// Machine need to be check before continue working. Origin action required.
        /// </summary>
        Alarm,
        ToOrigin,
        /// <summary>
        /// The Origin action.
        /// </summary>
        Origin,
        ToStop,
        /// <summary>
        /// Machine 
        /// </summary>
        Stop,
        ToRun,
        Run,        // 12
    }
}
