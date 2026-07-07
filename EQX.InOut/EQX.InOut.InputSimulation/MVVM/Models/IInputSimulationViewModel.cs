using EQX.Core.InOut;
using System.Windows.Input;

namespace EQX.InOut.InputSimulation
{
    public interface IInputSimulationViewModel
    {
        List<IDInput> Inputs { get; }
        int SelectedBoardIndex { get; internal set; }

        ICommand BoardIndexDecrease { get; }
        ICommand BoardIndexIncrease { get; }

        ICommand SetOriginInputsCommand { get; }
        ICommand SetRunInputsCommand { get; }
        ICommand ResetInputsCommand { get; }

        ICommand InputClickCommand { get; }
    }
}
