using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using EQX.Core.Interlock;
using log4net;
using System.Diagnostics;

namespace EQX.InOut
{
    public class CylinderBase : ObservableObject, ICylinder
    {
        #region Properties
        public int Id { get; internal set; }

        public string Name { get; internal set; }

        public ECylinderType CylinderType { get; set; }

        public event EventHandler? StateChanged;

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
            Output_ValueUpdated();
        }

        public Dictionary<string, Func<bool>>? ForwardInterlocks { get; set; }
        public Dictionary<string, Func<bool>>? BackwardInterlocks { get; set; }

        public bool IsForward
        {
            get
            {
                if (InForward != null & InBackward != null)
                {
                    return InForward!.Value & !InBackward!.Value;
                }
                else if (InForward == null & InBackward == null)
                {
                    // Both input is null
                    return false;
                }
                else if (InBackward != null)
                {
                    // Only backward is not null
                    return !InBackward!.Value;
                }
                else
                {
                    // Only forward is not null
                    return InForward!.Value;
                }
            }
        }

        public bool IsBackward
        {
            get
            {
                if (InForward != null & InBackward != null)
                {
                    // Both input not null
                    return !InForward!.Value & InBackward!.Value;
                }
                else if (InForward == null & InBackward == null)
                {
                    // Both input is null
                    return false;
                }
                else if (InBackward != null)
                {
                    // Only backward is not null
                    return InBackward!.Value;
                }
                else
                {
                    // Only forward is not null
                    return !InForward!.Value;
                }
            }
        }

        public bool IsOutForward
        {
            get
            {
                if (OutForward != null)
                {
                    return OutForward.Value;
                }
                else if (OutBackward != null)
                {
                    return !OutBackward.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsOutBackward
        {
            get
            {
                if (OutBackward != null)
                {
                    return OutBackward.Value;
                }
                else if (OutForward != null)
                {
                    return !OutForward.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsBackwardInterlock
        {
            get
            {
                return BackwardInterlocks?.Any(i => !i.Value()) == true;
            }
        }

        public bool IsForwardInterlock
        {
            get
            {
                return ForwardInterlocks?.Any(i => !i.Value()) == true;
            }
        }
        #endregion

        #region Constructors
        public CylinderBase(IDInput? inForwards, IDInput? inBackwards, IDOutput? outForward, IDOutput? outBackward)
        {
            InForward = inForwards;
            InBackward = inBackwards;
            OutForward = outForward;
            OutBackward = outBackward;


            if (InForward != null)
            {
                InForward.ValueUpdated += InForward_ValueUpdated;

            }
            if (InBackward != null)
            {
                InBackward.ValueUpdated += InForward_ValueUpdated;
            }
        }

        private void InForward_ValueUpdated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsForward));
            OnPropertyChanged(nameof(IsBackward));
            OnStateChanged();
        }

        private void Output_ValueUpdated()
        {
            OnPropertyChanged(nameof(IsOutForward));
            OnPropertyChanged(nameof(IsOutBackward));
        }
        #endregion

        #region Public methods
        public void Forward()
        {
            var blockedInterlock = ForwardInterlocks?.FirstOrDefault(i => !i.Value());

            if (blockedInterlock?.Key != null)
            {
                InterlockMonitor.InterlockBlocked(this, "Forward", blockedInterlock?.Key!);
                return;
            }

            //LogManager.GetLogger($"{Name}").Debug("Forward");
            ForwardAction();
            OnStateChanged();
        }

        public void Backward()
        {
            var blockedInterlock = BackwardInterlocks?.FirstOrDefault(i => !i.Value());

            if (blockedInterlock?.Key != null)
            {
                InterlockMonitor.InterlockBlocked(this, "Backward", blockedInterlock?.Key!);
                return;
            }

            //LogManager.GetLogger($"{Name}").Debug("Backward");
            BackwardAction();
            OnStateChanged();
        }

        public void UpdateIOStatus()
        {
            OnPropertyChanged(nameof(IsForward));
            OnPropertyChanged(nameof(IsBackward));

            OnPropertyChanged(nameof(IsForwardInterlock));
            OnPropertyChanged(nameof(IsBackwardInterlock));

            OnPropertyChanged(nameof(IsOutForward));
            OnPropertyChanged(nameof(IsOutBackward));
        }

        protected virtual void ForwardAction() { }
        protected virtual void BackwardAction() { }
        #endregion

        #region Protected
        protected IDOutput? OutForward { get; }
        protected IDOutput? OutBackward { get; }
        protected IDInput? InForward { get; }
        protected IDInput? InBackward { get; }

        public override string ToString() => Name;
        #endregion
    }
}
