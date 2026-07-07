using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface IEjector : IIdentifier
    {
        /// <summary>
        /// Hold purge state for a certain time before turning off purge.
        /// </summary>
        int PurgeDelay { get; set; }
        bool IsVacuumOn { get; }
        void VacuumOn();
        void VacuumOff();
    }
}
