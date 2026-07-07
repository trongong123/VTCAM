using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Helpers;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Process;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;

namespace EQX.UI.MVVM
{
    public class ManualUnitViewModel<TESequence> : ViewModelBase where TESequence : Enum
    {
        public IProcess<TESequence> CurrentProcess
        {
            get { return _currentProcess; }
            set
            {
                if (_currentProcess == value) return;

                _currentProcess = value;

                Inputs = PropertyHelpers.GetProperties<IDInput>(_currentProcess);
                Outputs = PropertyHelpers.GetProperties<IDOutput>(_currentProcess);
                Cylinders = PropertyHelpers.GetProperties<ICylinder>(_currentProcess);
                Motions = PropertyHelpers.GetProperties<IMotion>(_currentProcess);
                Ejectors = PropertyHelpers.GetProperties<IEjector>(_currentProcess);

                OnPropertyChanged(nameof(Inputs));
                OnPropertyChanged(nameof(Outputs));
                OnPropertyChanged(nameof(Cylinders));
                OnPropertyChanged(nameof(Motions));
                OnPropertyChanged(nameof(Ejectors));

                OnPropertyChanged(nameof(CurrentProcess));
            }
        }

        public List<IProcess<TESequence>> DependenceProcessList { get; set; }

        public ObservableCollection<IDInput> Inputs { get; private set; }
        public ObservableCollection<IDOutput> Outputs { get; private set; }
        public ObservableCollection<ICylinder> Cylinders { get; private set; }
        public ObservableCollection<IMotion> Motions { get; private set; }
        public ObservableCollection<IEjector> Ejectors { get; private set; }

        public ICommand DependenceProcessSelectCommand
        {
            get
            {
                return new RelayCommand<string>((name) =>
                {
                    CurrentProcess = _processes.ToList().First(p => p.Name == name)!;
                });
            }
        }

        #region Constructor(s)
        public ManualUnitViewModel(IEnumerable<IProcess<TESequence>> processes, NavigationStore navigationStore)
        {
            _processes = processes;
            _navigationStore = navigationStore;

            _timer = new NonOverlappingTimer(100);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            Inputs = new ObservableCollection<IDInput>();
            Outputs = new ObservableCollection<IDOutput>();
            Cylinders = new ObservableCollection<ICylinder>();
            Motions = new ObservableCollection<IMotion>();
            Ejectors = new ObservableCollection<IEjector>();
        }
        #endregion

        #region Privates
        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_navigationStore.CurrentViewModel != this) return;

            if (Inputs != null)
            {
                foreach (var input in Inputs)
                {
                    input.RaiseValueUpdated();
                }
            }

            if (Cylinders != null)
            {
                foreach (var cylinder in Cylinders)
                {
                    cylinder.UpdateIOStatus();
                }
            }
        }
        #endregion

        #region Privates
        private Random rand = new Random();
        private readonly NonOverlappingTimer _timer;
        private readonly IEnumerable<IProcess<TESequence>> _processes;
        private readonly NavigationStore _navigationStore;
        private IProcess<TESequence> _currentProcess;
        #endregion
    }
}
