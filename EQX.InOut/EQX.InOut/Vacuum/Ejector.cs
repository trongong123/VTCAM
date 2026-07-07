using EQX.Core.InOut;

namespace EQX.InOut
{
    public class Ejector : EjectorBase
    {
        public Ejector(IDOutput vacuumOutput, IDInput? vacuumInput = null, IDOutput? blowOutput = null)
            : base(vacuumOutput, vacuumInput, blowOutput)
        {
        }
    }
}
