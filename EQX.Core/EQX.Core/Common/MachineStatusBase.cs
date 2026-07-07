using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Recipe;
using EQX.Core.Sequence;
using TOPENG_Device;

namespace EQX.Core.Common
{
    public class EquipState : ObservableObject
    {
        private bool isAvailable;
        private bool isInterlock;
        private bool isRunning;
        private bool isCellInEquip;

        /// <summary>
        /// AVAILABILITY
        /// </summary>
        public bool IsAvailable 
        {
            get => isAvailable;
            set
            {
                isAvailable = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// INTERLOCK
        /// </summary>
        public bool IsInterlock 
        {
            get => isInterlock;
            set
            {
                isInterlock = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// MOVE
        /// </summary>
        public bool IsRunning 
        {
            get => isRunning;
            set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// RUN
        /// </summary>
        public bool IsCellInEquip 
        {
            get => isCellInEquip;
            set
            {
                isCellInEquip = value;
                OnPropertyChanged();
            }
        }
    }

    public class MachineStatusBase<TESemiSequence> : ObservableObject where TESemiSequence : Enum
    {
        #region Properties
        public EMachineRunMode MachineRunMode
        {
            get
            {
                return _machineRunMode;
            }
            set
            {
                if (_machineRunMode == value)
                {
                    return;
                }

                _machineRunMode = value;
                OnPropertyChanged(nameof(MachineRunMode));
                OnPropertyChanged(nameof(MachineRunModeDisplay));
                OnPropertyChanged(nameof(IsDryRunMode));
            }
        }

        public string MachineRunModeDisplay
        {
            get
            {
                return _machineRunMode.ToString();
            }
        }

        public bool IsDryRunMode => _machineRunMode == EMachineRunMode.DryRun;
        public bool IsAutoRunMode => _machineRunMode == EMachineRunMode.Auto;

        public TESemiSequence SemiAutoSequence
        {
            get => (TESemiSequence)(object)_SemiAutoSequence;
            set
            {
                MultiThreadingHelpers.SafeSetValue(ref _SemiAutoSequence, value);
            }
        }

        public EOperationCommand OPCommand
        {
            get => (EOperationCommand)_OPCommand;
            set
            {
                MultiThreadingHelpers.SafeSetValue(ref _OPCommand, value);
            }
        }

        public EProcessMode CurrentProcessMode
        {
            get => currentProcessMode;
            set
            {
                currentProcessMode = value;
                OnPropertyChanged(nameof(IsRunningProcessMode));
                OnPropertyChanged(nameof(IsStandByProcessMode));
                OnPropertyChanged(nameof(IsReadyToRunProcessMode));
                OnPropertyChanged();
            }
        }

        public bool IsReadyToRunProcessMode
        {
            get
            {
                return
                    currentProcessMode == EProcessMode.Warning ||
                    currentProcessMode == EProcessMode.Stop;
            }
        }

        public bool IsStandByProcessMode
        {
            get
            {
                return
                    currentProcessMode == EProcessMode.None ||
                    currentProcessMode == EProcessMode.Alarm ||
                    currentProcessMode == EProcessMode.Warning ||
                    currentProcessMode == EProcessMode.Stop;
            }
        }

        public EquipState EquipState { get; set; }

        public bool IsError => currentProcessMode == EProcessMode.Warning || currentProcessMode == EProcessMode.Alarm;
        public bool IsOrigin => currentProcessMode == EProcessMode.ToOrigin || currentProcessMode == EProcessMode.Origin;
        public bool IsRunningProcessMode => !IsStandByProcessMode;

        public virtual void MoveMultiPointPositionSequence(MultiPointPosition multiPointPosition)
        {

        }
        #endregion

        public MachineStatusBase()
        {
            EquipState = new EquipState() { IsRunning = false };
        }

        #region Privates
        private EProcessMode currentProcessMode;
        private EMachineRunMode _machineRunMode;
        private int _SemiAutoSequence;
        private int _OPCommand;
        #endregion
    }
}
