using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Common;

namespace EQX.UI.MVVM
{
    /// <summary>
    /// ViewModel for EQP STATE display. Wraps <see cref="EquipState"/> and exposes observable properties.
    /// Mapping (per spec): AVAILABILITY true=UP/false=DOWN; INTERLOCK true=OFF/false=ON; MOVE true=RUNNING/false=PAUSE; RUN true=RUN/false=IDLE.
    /// </summary>
    public partial class EquipStateViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isAvailable;

        [ObservableProperty]
        private bool _isInterlock;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private bool _isCellInEquip;

        /// <summary>
        /// Updates the view model from the given EquipState (e.g. MachineStatus.EquipState).
        /// Call this whenever the source state changes so the UI updates.
        /// </summary>
        public void UpdateFrom(EquipState? source)
        {
            if (source == null) return;
            IsAvailable = source.IsAvailable;
            IsInterlock = source.IsInterlock;
            IsRunning = source.IsRunning;
            IsCellInEquip = source.IsCellInEquip;
        }
    }
}
