using EQX.Core.Helpers;
using EQX.Core.Motion;
using EQX.Core.Process;
using EQX.Core.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.UI.MVVM
{
    public class TeachPositionUnitViewModel<TESequence> : TeachPositionUnitViewModelBase where TESequence : Enum
    {
        private IProcess<TESequence> currentProcess;

        public IProcess<TESequence> CurrentProcess
        {
            get => currentProcess;
            set
            {
                currentProcess = value;
                OnPropertyChanged(nameof(Recipes));
                OnPropertyChanged(nameof(Motions));
                OnPropertyChanged(nameof(XAxis));
                OnPropertyChanged(nameof(YAxis));
                OnPropertyChanged(nameof(ZAxis));
                SelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override ObservableCollection<IRecipe> Recipes => PropertyHelpers.GetProperties<IRecipe>(CurrentProcess);
        public override ObservableCollection<IMotion> Motions => PropertyHelpers.GetProperties<IMotion>(CurrentProcess);
    }
}
