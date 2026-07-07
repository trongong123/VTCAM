using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.InOut
{
    public class EjectorBase : ObservableObject, IEjector
    {
        protected readonly IDOutput _vacuumOutput;
        protected readonly IDInput? _vacuumInput;
        protected readonly IDOutput? _blowOutput;
        #region Properties
        public int Id { get; internal set; }
        public string Name { get; init; }
        public int PurgeDelay { get; set; } = 300;

        public bool IsVacuumOn => _vacuumInput != null && _vacuumInput.Value;
        #endregion

        #region Constructor(s)
        public EjectorBase(IDOutput vacuumOutput, IDInput? vacuumInput = null, IDOutput? blowOutput = null)
        {
            if (vacuumOutput == null)
            {
                throw new ArgumentNullException(nameof(vacuumOutput), "OutVacuum cannot be null.");
            }

            this._vacuumOutput = vacuumOutput;
            this._vacuumInput = vacuumInput;
            this._blowOutput = blowOutput;

            if (_vacuumInput != null)
            {
                _vacuumInput.ValueChanged += VacuumInput_ValueChanged;
            }

        }
        #endregion

        #region Public Methods
        public void VacuumOn()
        {
            VacuumOnOffAction(true);
        }

        public void VacuumOff()
        {
            VacuumOnOffAction(false);
        }
        #endregion

        #region Private Methods
        protected virtual void VacuumOnOffAction(bool isOn)
        {
            _vacuumOutput.Value = isOn;
            SimulationInputSetter.SetSimInput(_vacuumInput, isOn);

            if (_blowOutput == null) return;

            _blowOutput.Value = !isOn;
            if (isOn == false)
            {
                Task.Delay(PurgeDelay).ContinueWith(t =>
                {
                    _blowOutput.Value = false;
                });
            }
        }

        private void VacuumInput_ValueChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsVacuumOn));
        }
        #endregion
    }
}
