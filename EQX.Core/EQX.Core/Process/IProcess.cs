using EQX.Core.Common;
using EQX.Core.Sequence;
using Newtonsoft.Json;

namespace EQX.Core.Process
{
    public delegate void AlarmWarningRaisedHandler(int alarmId, string alarmSource);

    public interface IProcess<TESequence> : ILogable, INameable, IProcessExecutor where TESequence : Enum
    {
        event AlarmWarningRaisedHandler? AlarmRaised;
        event AlarmWarningRaisedHandler? WarningRaised;
        event EventHandler? ProcessModeUpdated;

        /// <summary>
        /// Notification start index of the process. 
        /// <br/>It is used to notify the process status to the client (Alarm, warning, etc.)
        /// </summary>
        int NotifStartIndex { get; set; }

        /// <summary>
        /// Parent process of current process. It may be null if it's the Root Process
        /// </summary>
        [JsonIgnore]
        IProcess<TESequence>? Parent { get; }

        [JsonIgnore]
        IList<IProcess<TESequence>>? Childs { get; }

        /// <summary>
        /// Add Children process to handle
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        int AddChild(IProcess<TESequence> child);

        /// <summary>
        /// Process Mode of the process. None / Stop / Warning / Alarm / Run...
        /// </summary>
        EProcessMode ProcessMode { get; set; }
        /// <summary>
        /// Status of <see cref="ProcessMode"/>> execution. ToStopDone / ToOriginDone...
        /// </summary>
        EProcessStatus ProcessStatus { get; set; }

        /// <summary>
        /// Step of the process
        /// </summary>
        IProcessStep Step { get; }
        IProcessTimer ProcessTimer { get; }
        /// <summary>
        /// Client define RunMode (Sequence of the Automation Machine)<br/>
        /// For example: FirstInspect -> Pick -> SecondInspect -> Place...
        /// </summary>
        TESequence Sequence { get; set; }

        bool Start();
        bool Stop();

        void RaiseAlarm(Enum alarmCode);
        void RaiseAlarm(int alarmId);
        /// <summary>
        /// Raise alarm with name of the process that rasied the alarm
        /// </summary>
        /// <param name="alarmId">Alarm code</param>
        /// <param name="alarmSource">Name of process that raised the alarm</param>
        void RaiseAlarm(int alarmId, string alarmSource);

        void RaiseWarning(Enum warningCode);
        void RaiseWarning(int warningId);
        /// <summary>
        /// Raise warning with name of the process that rasied the alarm
        /// </summary>
        /// <param name="warningId">Warning code</param>
        /// <param name="warningSource">Name of the process that rasied the warning</param>
        void RaiseWarning(int warningId, string warningSource);

        bool WaitTimeOutOccurred { get; }
        /// <summary>
        /// Process wait for exactly timeout (in second)
        /// </summary>
        /// <param name="timeout">time to wait (second)</param>
        void Wait(double timeoutSecond);
        /// <summary>
        /// Process wait for exactly timeout (in millisecond)
        /// </summary>
        /// <param name="timeout">time to wait (ms)</param>
        void Wait(int timeout);
        /// <summary>
        /// Process wait for which come first, exactly timeout or action return true
        /// </summary>
        /// <param name="timeout">time to wait (ms)</param>
        /// <param name="waitUntil">Action return true will break waiting</param>
        void Wait(int timeout, Func<bool>? waitUntil);

        /// <summary>
        /// Process wait for which come first, exactly timeout or action return true
        /// </summary>
        /// <param name="timeout">time to wait (second)</param>
        /// <param name="waitUntil">Action return true will break waiting</param>
        void Wait(double timeout, Func<bool>? waitUntil);

        bool IsAlive { get; }
        bool IsAlarm { get; set; }
        bool IsCanStop { get; set; }

        bool IsOriginOrInitSelected { get; set; }
    }
}
