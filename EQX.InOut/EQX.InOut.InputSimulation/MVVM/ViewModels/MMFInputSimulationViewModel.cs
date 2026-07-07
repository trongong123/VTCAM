using CommunityToolkit.Mvvm.Input;
using EQX.Core.InOut;
using EQX.InOut.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EQX.InOut.InputSimulation
{
    public class MMFInputSimulationViewModel<TEInputs> : InputSimulationViewModelBase where TEInputs : Enum
    {
        #region Commands
        private ICommand resetInputs
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var values = Enum.GetValues(typeof(TEInputs));
                    foreach (var value in values)
                    {
                        inputServer.SetValue((int)value, false);
                    }
                });
            }
        }
        #endregion

        #region Constructor(s)
        public MMFInputSimulationViewModel(IEnumerable<string> originInputs,
            IEnumerable<string> runInputs) : base(originInputs, runInputs)
        {
            inputServer = new SimulationInputDevice_ServerMMF<TEInputs>()
            { 
                MaxPin = 1024
            };
            inputServer.Initialize();

            Inputs = inputServer.Inputs;
        }
        #endregion

        #region Public Methods
        public override void ToggleInput(IDInput input)
        {
            inputServer.ToggleInput(input.Id);
        }
        public override void SetInput(IDInput input)
        {
            inputServer.SetValue(input.Id, true);
        }
        public override void ResetInput(IDInput input)
        {
            inputServer.SetValue(input.Id, false);
        }
        #endregion

        #region Privates
        readonly SimulationInputDevice_ServerMMF<TEInputs> inputServer;
        #endregion
    }
}
