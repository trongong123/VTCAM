using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.InOut;
using System.Linq.Expressions;
using System.Windows.Input;

namespace EQX.InOut.InputSimulation
{
    public class InputSimulationViewModelBase : ObservableObject, IInputSimulationViewModel
    {
        #region Properties
        public List<IDInput> Inputs { get; protected set; }
        public int SelectedBoardIndex
        {
            get { return _SelectedBoardIndex; }
            set { _SelectedBoardIndex = value; }
        }
        #endregion

        #region Commands
        public ICommand InputClickCommand => new RelayCommand<object>((o) =>
        {
            if (o is DInput dInput == false) return;
            ToggleInput(dInput);
        });

        public ICommand SetOriginInputsCommand { get; protected set; }
        public ICommand SetRunInputsCommand { get; protected set; }
        public ICommand ResetInputsCommand => new RelayCommand(() =>
        {
            if (Inputs == null) return;
            if (Inputs.Count <= 0) return;

            foreach (var item in Inputs)
            {
                ResetInput(item);
            }
        });

        public ICommand BoardIndexDecrease => new RelayCommand(() =>
        {
            if (SelectedBoardIndex > 0)
            {
                SelectedBoardIndex--;
                OnPropertyChanged(nameof(SelectedBoardIndex));
            }
        });

        public ICommand BoardIndexIncrease => new RelayCommand(() =>
        {
            if (Inputs == null) return;
            if (Inputs.Count <= 0) return;

            if (SelectedBoardIndex < Inputs.Count / 16 - (Inputs.Count % 16 == 0 ? 1 : 0))
            {
                SelectedBoardIndex++;
                OnPropertyChanged(nameof(SelectedBoardIndex));
            }
        });
        #endregion

        #region Constructor(s)
        public InputSimulationViewModelBase(IEnumerable<string> originInputs,
            IEnumerable<string> runInputs)
        {
            timerUpdateValue = new NonOverlappingTimer(100);
            timerUpdateValue.Elapsed += (s, e) =>
            {
                UpdateValue();
            };
            timerUpdateValue.Start();

            SetOriginInputsCommand = new RelayCommand(() =>
            {
                if (Inputs == null) return;
                if (Inputs.Count <= 0) return;

                foreach (var item in originInputs)
                {
                    if (Inputs.Any(i => i.Name == item))
                        SetInput(Inputs.First(i => i.Name == item));
                }
            });

            SetRunInputsCommand = new RelayCommand(() =>
            {
                if (Inputs == null) return;
                if (Inputs.Count <= 0) return;

                foreach (var item in runInputs)
                {
                    if (Inputs.Any(i => i.Name == item))
                        SetInput(Inputs.First(i => i.Name == item));
                }
            });
        }
        #endregion

        #region Public Methods
        public virtual void SetInput(IDInput input)
        {
        }
        public virtual void ResetInput(IDInput input)
        {
        }
        public virtual void ToggleInput(IDInput input)
        {
        }
        #endregion

        #region Private Methods
        private void UpdateValue()
        {
            if (Inputs == null) return;
            if (Inputs.Count <= 0) return;

            foreach (var input in Inputs)
            {
                input.RaiseValueUpdated();
            }
        }
        #endregion

        #region Privates
        private int _SelectedBoardIndex;
        private NonOverlappingTimer timerUpdateValue;
        #endregion
    }
}
