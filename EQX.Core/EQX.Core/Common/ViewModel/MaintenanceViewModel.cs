using CommunityToolkit.Mvvm.Input;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.Core.Sequence;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Input;

namespace EQX.Core.Common
{
    public class JogSpeedLevel
    {
        public string Name { get; set; }
        public double Ratio { get; set; } // Ví dụ: 0.1 cho 10%
    }

    public class MaintenanceViewModel<TESemiSequence, TRecipeList> : ViewModelBase
        where TESemiSequence : Enum where TRecipeList : class
    {
        #region Properties
        public Action<string> RelatedViewModelNavigateEvent;
        public double MaxJogVelocity { get; set; }

        public IEnumerable<string> RelatedViewModel { get; protected set; }

        public IEnumerable<IDInput> Inputs { get; protected set; }
        public IEnumerable<IDOutput> Outputs { get; protected set; }
        public IEnumerable<ICylinder> Cylinders { get; protected set; }
        public IEnumerable<IMotion> Motions { get; protected set; }
        public IEnumerable<IEjector> Ejectors { get; protected set; }

        public ObservableCollection<MultiPointPosition> GroupedPositions => PositionManager?.GroupedPositions;

        public MultiPointPosition SelectedGroupedPosition { get; protected set; }

        public ObservableCollection<JogSpeedLevel> JogSpeedLevels { get; }

        public JogSpeedLevel SelectedJogSpeed { get; set; }

        public IEnumerable<TESemiSequence> Sequences { get; protected set; }

        public EMaintenanceView MaintenanceView { get; set; }

        public double IncStep { get; set; } = 0.1;
        #endregion

        #region Commands
        public IAsyncRelayCommand MoveToTarget
        {
            get
            {
                return new AsyncRelayCommand(async () =>
                {
                    if (ConfirmSemiSequence($"Do you want to Move {SelectedGroupedPosition.Name} ?") == false) return;

                    MachineStatus.MoveMultiPointPositionSequence(SelectedGroupedPosition);
                });
            }
        }

        public ICommand SaveCurrentToTarget
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedGroupedPosition == null || SelectedGroupedPosition.Points.Count <= 0) return;

                    foreach (var positionPoint in SelectedGroupedPosition.Points)
                    {
                        double actualPosition = positionPoint.Motion.Status.ActualPosition;
                        positionPoint.FixedPos = actualPosition - positionPoint.OffsetPos - positionPoint.ModelPos - positionPoint.VisionPos;
                    }
                });
            }
        }

        public ICommand SemiSequenceButtonCommand
        {
            get
            {
                return new RelayCommand<TESemiSequence>((seq) =>
                {
                    if (seq == null) return;
                    if (ConfirmSemiSequence($"Do you want to Run {GetEnumDescription(seq)} ?") == false) return;

                    MachineStatus.OPCommand = EOperationCommand.SemiAuto;
                    MachineStatus.SemiAutoSequence = seq;
                });
            }
        }

        public ICommand TeachingPositionButtonCommand
        {
            get
            {
                return new RelayCommand<MultiPointPosition>((mpp) =>
                {
                    if (mpp == null) return;
                    if (BeforeTeachingPositionSelectionChanged(SelectedGroupedPosition, mpp) == false) return;

                    SelectedGroupedPosition = mpp;
                    OnPropertyChanged(nameof(SelectedGroupedPosition));
                });
            }
        }

        public ICommand JogNegativeStartCommand
        {
            get
            {
                return new RelayCommand<IMotion>((motion) =>
                {
                    if (motion == null) return;
                    if (SelectedJogSpeed == null) SelectedJogSpeed = JogSpeedLevels.FirstOrDefault()!;

                    motion.MoveJog(Math.Min(motion.Parameter.Velocity, MaxJogVelocity) * SelectedJogSpeed.Ratio, false);
                });
            }
        }

        public ICommand JogPositiveStartCommand
        {
            get
            {
                return new RelayCommand<IMotion>((motion) =>
                {
                    if (motion == null) return;
                    if (SelectedJogSpeed == null) SelectedJogSpeed = JogSpeedLevels.FirstOrDefault()!;

                    motion.MoveJog(Math.Min(motion.Parameter.Velocity, MaxJogVelocity) * SelectedJogSpeed.Ratio, true);
                });
            }
        }

        public ICommand JogStopCommand
        {
            get
            {
                return new RelayCommand<IMotion>((motion) =>
                {
                    if (motion == null) return;

                    motion.Stop();
                });
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    MachineStatus.OPCommand = EOperationCommand.Stop;
                });
            }
        }

        public ICommand IncMinusCommand
        {
            get
            {
                return new RelayCommand<IMotion>((motion) =>
                {
                    if (motion == null) return;
                    motion.MoveInc(IncStep * -1.0);
                });
            }
        }

        public ICommand IncPlusCommand
        {
            get
            {
                return new RelayCommand<IMotion>((motion) =>
                {
                    if (motion == null) return;
                    motion.MoveInc(IncStep);
                });
            }
        }

        public ICommand RelatedViewModelNavigateCommand
        {
            get
            {
                return new RelayCommand<string>((viewModelName) =>
                {
                    RelatedViewModelNavigateEvent?.Invoke(viewModelName);
                });
            }
        }
        public MachineStatusBase<TESemiSequence> MachineStatus { get; }
        #endregion

        #region Constructor(s)
        public MaintenanceViewModel(NavigationStore navigationStore,
            MachineStatusBase<TESemiSequence> machineStatus)
        {
            _navigationStore = navigationStore;
            MachineStatus = machineStatus;
            _navigationStore.CurrentViewModelChanged += _navigationStore_CurrentViewModelChanged;

            MaxJogVelocity = 100;

            _timer = new NonOverlappingTimer(100);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            JogSpeedLevels = new ObservableCollection<JogSpeedLevel>
            {
                new JogSpeedLevel { Name = "Low", Ratio = 0.1 },
                new JogSpeedLevel { Name = "Mid", Ratio = 0.5 },
                new JogSpeedLevel { Name = "High", Ratio = 1.0 }
            };
        }

        protected virtual void ExternalTimerElapsedAction()
        {
        }

        protected virtual bool ConfirmSemiSequence(string message)
        {
            return true;
        }

        protected virtual bool BeforeTeachingPositionSelectionChanged(MultiPointPosition? currentSelection, MultiPointPosition nextSelection)
        {
            return true;
        }

        protected string GetEnumDescription(TESemiSequence seq)
        {
            MemberInfo[] memberInfo = typeof(TESemiSequence).GetMember(seq.ToString());
            if (memberInfo.Length == 0)
            {
                return seq.ToString();
            }

            DescriptionAttribute? descriptionAttribute = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? seq.ToString();
        }

        private void _navigationStore_CurrentViewModelChanged()
        {
            if (_navigationStore.CurrentViewModel == this)
            {
                _timer.Resume();
            }
            else
            {
                _timer.Pause();
            }
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_navigationStore.CurrentViewModel != this)
            {
                return;
            }

            Inputs?.ToList().ForEach(i => i.RaiseValueUpdated());
            Outputs?.ToList().ForEach(o => o.RaiseValueUpdated());
            Cylinders?.ToList().ForEach(c => c.UpdateIOStatus());

            ExternalTimerElapsedAction();
        }
        #endregion

        public void Init()
        {
            if (isInitiated) return;

            ActualInit();
            PositionManager = UpdatePositionManager();

            if (GroupedPositions != null && GroupedPositions.Count > 0)
            {
                SelectedGroupedPosition = GroupedPositions.FirstOrDefault()!;
            }

            CaptureTeachingSnapshot();
            WireTeachingPointEvents();
            isInitiated = true;
        }

        // Prevent Multiplicate Init
        protected virtual void ActualInit()
        {
        }

        protected virtual void CaptureTeachingSnapshot()
        {

        }

        protected virtual void WireTeachingPointEvents()
        {

        }
        protected virtual RecipePositionManagerBase<TRecipeList> UpdatePositionManager()
        {
            throw new NotImplementedException();
        }

        protected RecipePositionManagerBase<TRecipeList> PositionManager { get; private set; }

        #region Privates
        protected readonly NavigationStore _navigationStore;
        private readonly NonOverlappingTimer _timer;
        bool isInitiated;
        #endregion
    }
}
