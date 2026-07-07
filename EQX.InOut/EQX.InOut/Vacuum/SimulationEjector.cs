using EQX.Core.InOut;

namespace EQX.InOut
{
    public class SimulationEjector : EjectorBase
    {
        public SimulationEjector(IDOutput vacuumOutput, IDInput? vacuumInput = null, IDOutput? blowOutput = null)
            : base(vacuumOutput, vacuumInput, blowOutput)
        {
        }

        protected override void VacuumOnOffAction(bool isOn)
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
    }
}
