using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Process;
using EQX.Core.Sequence;
using log4net;

namespace EQX.Process
{
    public class ProcessBase<TESequence> : ObservableObject, IProcess<TESequence> where TESequence : Enum
    {
        #region Properties
        public event AlarmWarningRaisedHandler? AlarmRaised;
        public event AlarmWarningRaisedHandler? WarningRaised;
        public event EventHandler? ProcessModeUpdated;

        public bool IsAlive => isAlive;

        public bool IsCanStop
        {
            get
            {
                // If Parent Process
                if (Childs != null && Childs.Count > 0) return true;

                return _isCanStop;
            }
            set
            {
                _isCanStop= value;
            }
        }

        public int NotifStartIndex { get; set; }

        public IProcess<TESequence>? Parent
        {
            get => _parent == null ? this : _parent;
            private set => _parent = value!;
        }
        public IList<IProcess<TESequence>>? Childs { get; }

        public EProcessMode ProcessMode
        {
            get => _processMode;
            set
            {
                if (_processMode == value) return;
                if ((value == EProcessMode.ToStop || value == EProcessMode.ToWarning || value == EProcessMode.ToAlarm) && IsCanStop == false) return;

                _processMode = value;

                Step.OriginStep = 0;
                Step.ToRunStep = 0;

                ProcessModeUpdated?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public EProcessStatus ProcessStatus
        {
            get
            {
                return _processStatus;
            }
            set
            {
                if (value == EProcessStatus.OriginDone && IsOriginOrInitSelected)
                {
                    IsAlarm = false;
                }
                _processStatus = value;
            }
        }

        public TESequence Sequence
        {
            get => sequence;
            set
            {
                sequence = value;
                Step.RunStep = 0;
                Step.ToRunStep = 0;
                OnPropertyChanged();
            }
        }

        public string Name { get; set; }

        public IProcessStep Step { get; }

        public IProcessTimer ProcessTimer { get; }

        public ILog Log => LogManager.GetLogger(Name);

        public bool WaitTimeOutOccurred { get; private set; }

        public bool IsAlarm
        {
            get
            {
                if (Childs != null && Childs.Count > 0)
                {
                    return Childs.Any(x => x.IsAlarm);
                }

                return isAlarm;
            }
            set
            {
                isAlarm = value;
                OnPropertyChanged();
            }
        }

        private bool isOriginOrInitSelected = true;
        public bool IsOriginOrInitSelected
        {
            get
            {
                return isOriginOrInitSelected;
            }
            set
            {
                isOriginOrInitSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor(s)
        public ProcessBase()
        {
            Name ??= GetType().Name;

            isAlive = false;

            Step = new ProcessStep();
            ProcessTimer = new ProcessTimer();
            Childs = new List<IProcess<TESequence>>();
            cts = new CancellationTokenSource();

            Step.StepChangedHandler += ((s, e) => { ((ProcessTimer)ProcessTimer).StepStartTime = Environment.TickCount; });
        }
        #endregion

        #region Public Method(s)
        public bool Start()
        {
            if (isAlive)
            {
                // Thread is already started
                Stop();
            }

            cts?.Dispose();
            cts = new CancellationTokenSource();

            //ThreadPool.QueueUserWorkItem(new WaitCallback(OnThreadStart), cts.Token);
            isAlive = true;

            Task mainThread = OnThreadStart(cts.Token);

            return true;
        }

        public bool Stop()
        {
            isAlive = false;
            cts.Cancel();

            return true;
        }

        public int AddChild(IProcess<TESequence> child)
        {
            if (Childs!.Contains(child) == false)
            {
                Childs.Add(child);
                ((ProcessBase<TESequence>)child).Parent = this;
            }

            return Childs.Count;
        }

        public void RaiseAlarm(Enum alarmCode)
        {
            RaiseAlarm(Convert.ToInt32(alarmCode), Name);
        }

        public void RaiseAlarm(int alarmId)
        {
            RaiseAlarm(alarmId, Name);
        }

        public void RaiseAlarm(int alarmId, string alarmSource)
        {
            IsCanStop = true;
            IsAlarm = true;
            AlarmRaised?.Invoke(alarmId, alarmSource);
        }

        public void RaiseWarning(Enum warningCode)
        {
            RaiseWarning(Convert.ToInt32(warningCode), Name);
        }

        public void RaiseWarning(int warningId)
        {
            RaiseWarning(warningId, Name);
        }

        public void RaiseWarning(int warningId, string warningSource)
        {
            IsCanStop = true;

            WarningRaised?.Invoke(warningId, warningSource);
        }

        /// <summary>
        /// Wait in seconds <br/>
        /// DO NOT USE THIS FUNCION ON PreProcess function
        /// </summary>
        /// <param name="timeout"></param>
        public void Wait(double timeout)
        {
            Wait((int)(timeout * 1000), null);
        }

        /// <summary>
        /// Wait in milliseconds <br/>
        /// DO NOT USE THIS FUNCION ON PreProcess function
        /// </summary>
        /// <param name="timeout"></param>
        public void Wait(int timeout)
        {
            Wait(timeout, null);
        }

        /// <summary>
        /// DO NOT USE THIS FUNCION ON PreProcess function
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="waitUntil"></param>
        public void Wait(double timeout, Func<bool>? waitUntil)
        {
            Wait((int)(timeout * 1000), waitUntil);
        }

        /// <summary>
        /// DO NOT USE THIS FUNCION ON PreProcess function
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="waitUntil"></param>
        public void Wait(int timeout, Func<bool>? waitUntil)
        {
            WaitTimeOutOccurred = false;

            ((ProcessTimer)ProcessTimer).WaitTime = timeout;
            WaitBrakeAction = waitUntil;
        }
        #endregion

        #region Public Virtual Method(s)
        public virtual bool PostProcess()
        {
            return true;
        }

        public virtual bool PreProcess()
        {
            return true;
        }

        public virtual bool ProcessAlarm()
        {
            return true;
        }

        public virtual bool ProcessNone()
        {
            return true;
        }

        public virtual bool ProcessOrigin()
        {
            ProcessStatus = EProcessStatus.OriginDone;

            return true;
        }

        public virtual bool ProcessRun()
        {
            return true;
        }

        public virtual bool ProcessStop()
        {
            return true;
        }

        public virtual bool ProcessToAlarm()
        {
            if (ProcessStatus == EProcessStatus.ToAlarmDone)
            {
                Thread.Sleep(50);
                return true;
            }

            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            WaitBrakeAction = null;
            ProcessStatus = EProcessStatus.ToAlarmDone;

            return true;
        }

        public virtual bool ProcessToOrigin()
        {
            if (ProcessStatus == EProcessStatus.ToOriginDone)
            {
                Thread.Sleep(50);
                return true;
            }

            Step.OriginStep = 0;
            ProcessStatus = EProcessStatus.ToOriginDone;

            return true;
        }

        public virtual bool ProcessToRun()
        {
            if (ProcessStatus == EProcessStatus.ToRunDone)
            {
                Thread.Sleep(50);
                return true;
            }

            Step.RunStep = 0;
            ProcessStatus = EProcessStatus.ToRunDone;

            return true;
        }

        public virtual bool ProcessToStop()
        {
            if (ProcessStatus == EProcessStatus.ToStopDone)
            {
                Thread.Sleep(50);
                return true;
            }

            Log.Debug("ToStopDone");
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            WaitBrakeAction = null;
            ProcessStatus = EProcessStatus.ToStopDone;

            return true;
        }

        public virtual bool ProcessToWarning()
        {
            if (ProcessStatus == EProcessStatus.ToWarningDone)
            {
                Thread.Sleep(50);
                return true;
            }

            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            WaitBrakeAction = null;
            ProcessStatus = EProcessStatus.ToWarningDone;

            return true;
        }

        public virtual bool ProcessWarning()
        {
            return true;
        }
        #endregion

        #region Private Method(s)
        private async Task OnThreadStart(CancellationToken token)
        {
            while (token.IsCancellationRequested == false && isAlive)
            {
                try
                {
                    if (Parent != null)
                    {
                        ProcessMode = Parent.ProcessMode;
                    }

                    PreProcess();

                    if (ProcessMode != EProcessMode.ToWarning && ProcessMode != EProcessMode.ToAlarm && ProcessMode != EProcessMode.ToStop)
                    {
                        if (ProcessTimer.WaitTime > 0 || WaitBrakeAction != null)
                        {
                            if (WaitBrakeAction != null)
                            {
                                if (WaitBrakeAction())
                                {
                                    // Wait break action occur first (before timeout), clear the waiting condition
                                    ((ProcessTimer)ProcessTimer).WaitTime = 0;
                                    WaitBrakeAction = null;
                                    continue;
                                }
                            }

                            if (Environment.TickCount - ((ProcessTimer)ProcessTimer).WaitTimeSetMoment < ProcessTimer.WaitTime)
                            {
                                await Task.Delay(10, token);
                                continue;
                            }
                            else
                            {
                                // Timeout occur, clear the waiting condition
                                WaitTimeOutOccurred = true;

                                ((ProcessTimer)ProcessTimer).WaitTime = 0;
                                WaitBrakeAction = null;
                                continue;
                            }
                        }
                    }

                    switch (ProcessMode)
                    {
                        case EProcessMode.None:
                            ProcessNone();
                            break;
                        case EProcessMode.ToAlarm:
                            ProcessToAlarm();
                            break;
                        case EProcessMode.Alarm:
                            ProcessAlarm();
                            break;
                        case EProcessMode.ToOrigin:
                            if (IsOriginOrInitSelected || Childs!.Count > 0)
                            {
                                ProcessToOrigin();
                            }
                            else
                            {
                                Step.OriginStep = 0;
                                ProcessStatus = EProcessStatus.ToOriginDone;
                            }
                            break;
                        case EProcessMode.Origin:
                            if (IsOriginOrInitSelected || Childs!.Count > 0)
                            {
                                ProcessOrigin();
                            }
                            else
                            {
                                ProcessStatus = EProcessStatus.OriginDone;
                            }
                            break;
                        case EProcessMode.ToStop:
                            ProcessToStop();
                            break;
                        case EProcessMode.Stop:
                            ProcessStop();
                            break;
                        case EProcessMode.ToWarning:
                            ProcessToWarning();
                            break;
                        case EProcessMode.Warning:
                            ProcessWarning();
                            break;
                        case EProcessMode.ToRun:
                            ProcessToRun();
                            break;
                        case EProcessMode.Run:
                            ProcessRun();
                            break;
                        default:
                            break;
                    }

                    PostProcess();

                    await Task.Delay(2, token);
                }
                catch (Exception ex)
                {
                    if(ex is OperationCanceledException == false)
                    {
                        Log.Error($"Process {Name} exception: {ex.Message}");
                        // TODO: Handle exception
                        throw new Exception(ex.Message);
                    }    
                }
            }

            Log.Fatal($"{Name} has END");
        }
        #endregion

        #region Privates
        private IProcess<TESequence> _parent;
        private bool isAlarm = true;
        bool isAlive;
        CancellationTokenSource cts;
        private EProcessMode _processMode;
        private EProcessStatus _processStatus;
        Func<bool>? WaitBrakeAction;
        private TESequence sequence = default!;
        public bool Wait_move = true;
        public TESequence CurrentSequence;
        public TESequence? _savedSequence;
        public TESequence LastSequence;
        public int _savedRunStep = 0;
        public bool _isPausedFromRun;
        public int CurrentStep;
        private bool _isCanStop = true;
        #endregion
    }
}
