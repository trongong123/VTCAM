using EQX.Core.Interlock;
using EQX.Core.Motion;
using log4net;

namespace EQX.Motion
{
    public class MotionBase : IMotion
    {
        public Dictionary<string, Func<bool>>? PositionIncreaseInterlocks { get; set; }
        public Dictionary<string, Func<bool>>? PositionDecreaseInterlocks { get; set; }

        public IMotionStatus Status { get; protected set; }
        public IMotionParameter Parameter { get; protected set; }
        public int Id { get; init; }
        public string Name { get; init; }
        public virtual bool IsConnected { get; protected set; }

        public MotionBase(int id, string name, IMotionParameter parameter)
        {
            Id = id;
            Name = name;
            Parameter = parameter;

            Status = new MotionStatus();

            cts = new CancellationTokenSource();
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    UpdateAxisStatus();
                    await Task.Delay(50);
                }
            }, cts.Token);
        }

        private CancellationTokenSource cts;

        #region Virtual Methods
        protected virtual bool ActualConnect()
        {
            return true;
        }

        protected virtual bool ActualDisconnect()
        {
            return true;
        }

        protected virtual bool ActualSearchOrigin()
        {
            return true;
        }

        protected virtual bool ActualInitialization()
        {
            return true;
        }

        protected virtual bool ActualMotionOff()
        {
            return true;
        }

        protected virtual bool ActualMotionOn()
        {
            return true;
        }

        protected virtual bool ActualMoveAbs(double position, double speed)
        {
            return true;
        }

        protected virtual bool ActualMoveInc(double position, double speed)
        {
            return true;
        }

        protected virtual void ActualMoveJog(double speed, bool isForward) { }

        public virtual bool AlarmReset()
        {
            return true;
        }

        protected virtual void UpdateAxisStatus() { }
        #endregion
        public override string ToString() => Name;

        private void StatusUpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsConnected == false) return;

            //lock (searchOriginLock)
            {
                UpdateAxisStatus();
            }
        }

        public bool Connect()
        {
            return ActualConnect();
        }

        public bool Disconnect()
        {
            return ActualDisconnect();
        }

        public bool Initialization()
        {
            return ActualInitialization();
        }

        public bool MotionOff()
        {
            return ActualMotionOff();
        }

        public bool MotionOn()
        {
            return ActualMotionOn();
        }

        public bool MoveAbs(double position)
        {
            return MoveAbs(position, Parameter.Velocity);
        }

        public bool MoveAbs(double position, double speed)
        {
            if (position > Status.ActualPosition && PositionIncreaseInterlocks != null)
            {
                var blockedInterlock = PositionIncreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move INCREMENTAL", blockedInterlock?.Key!);
                    return false;
                }
            }

            if (position < Status.ActualPosition && PositionDecreaseInterlocks != null)
            {
                var blockedInterlock = PositionDecreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move DECREMENTAL", blockedInterlock?.Key!);
                    return false;
                }
            }

            return ActualMoveAbs(position, speed);
        }

        public bool MoveInc(double position)
        {
            return MoveInc(position, Parameter.Velocity);
        }

        public bool MoveInc(double position, double speed)
        {
            if (position > Status.ActualPosition && PositionIncreaseInterlocks != null)
            {
                var blockedInterlock = PositionIncreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move INCREMENTAL", blockedInterlock?.Key!);
                    return false;
                }
            }

            if (position < Status.ActualPosition && PositionDecreaseInterlocks != null)
            {
                var blockedInterlock = PositionDecreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move DECREMENTAL", blockedInterlock?.Key!);
                    return false;
                }
            }

            return ActualMoveInc(position, speed);
        }

        public void MoveJog(double speed, bool isForward)
        {
            if (speed > 0 && PositionIncreaseInterlocks != null)
            {
                var blockedInterlock = PositionIncreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move INCREMENTAL", blockedInterlock?.Key!);
                    return;
                }
            }

            if (speed < 0 && PositionDecreaseInterlocks != null)
            {
                var blockedInterlock = PositionDecreaseInterlocks?.FirstOrDefault(i => !i.Value());

                if (blockedInterlock?.Key != null)
                {
                    InterlockMonitor.InterlockBlocked(this, "Move DECREMENTAL", blockedInterlock?.Key!);
                    return;
                }
            }

            ActualMoveJog(speed, isForward);
        }

        public bool SearchOrigin()
        {
            // Lock to prevent IsHomeDone status Set / Clear by timer
            //lock (searchOriginLock)
            {
                ((MotionStatus)Status).IsHomeDone = false;

                return ActualSearchOrigin();
            }
        }

        public virtual bool Stop(bool forceStop = true)
        {
            return true;
        }

        protected double PulseToMM(int pulse)
        {
            return pulse * 1.0 * Parameter.Unit / Parameter.Pulse;
        }

        protected int MMtoPulse(double mm)
        {
            return (int)(mm * 1.0 * Parameter.Pulse / Parameter.Unit);
        }

        public bool IsOnPosition(double dPosition)
        {
            return (Math.Abs(Status.ActualPosition - dPosition) < allowPositionDiff) & (Status.IsMotioning == false);
        }

        public virtual bool ClearPosition()
        {
            return true;
        }

        private double allowPositionDiff = 0.02;
    }
}
