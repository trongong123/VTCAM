using EQX.Core.Process;
using EQX.Core.Sequence;

namespace EQX.Process
{
    public static class ProcessHelpers
    {
        /// <summary>
        /// Alarm or ToAlarm
        /// </summary>
        public static bool IsInAlarmMode<TESequence>(this IProcess<TESequence> process) where TESequence : Enum
        {
            return process.ProcessMode == EProcessMode.ToAlarm ||
                process.ProcessMode == EProcessMode.Alarm;
        }

        /// <summary>
        /// Warning or ToWarning
        /// </summary>
        public static bool IsInWarningMode<TESequence>(this IProcess<TESequence> process) where TESequence : Enum
        {
            return process.ProcessMode == EProcessMode.ToWarning ||
                process.ProcessMode == EProcessMode.Warning;
        }
    }
}
