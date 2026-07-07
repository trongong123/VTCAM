using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Common;
using EQX.Core.Sequence;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.MVVM.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Process
{
    public class MachineStatus : MachineStatusBase<ESemiSequence>
    {
        private Defines.Process.EMachineRunMode _machineRunMode;
        private EProcessMode currentProcessMode;
        private int _SemiAutoSequence;
        private int _OPCommand;

        private bool _originDone;
        private readonly Outputs _outputs;

        private bool isInputStop;
        private bool isOutputStop;
        private bool isPickupStop;
        private bool isCVConditionConfirm;
        private bool isTrayEmptyConfirm;
        private bool isOnMuting = false;
        private bool isTrayOut;

        public MachineStatus(Outputs outputs)
        {
            _outputs = outputs;
        }
        public Defines.Process.EMachineRunMode MachineRunMode
        {
            get
            {
                return _machineRunMode;
            }
            set
            {
                _machineRunMode = value;
                OnPropertyChanged(nameof(MachineRunModeDisplay));
                OnPropertyChanged(nameof(MachineRunMode));
            }
        }

        public string MachineRunModeDisplay
        {
            get
            {
                return _machineRunMode.ToString();
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
                OnPropertyChanged(nameof(IsEnableSelectMode));
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

        public bool IsEnableSelectMode => IsStandByProcessMode || IsInterfaceOnlyMode;

        public bool IsRunningProcessMode => !IsStandByProcessMode;
        public bool IsDryRunMode => _machineRunMode == Defines.Process.EMachineRunMode.Dryrun;
        public bool IsByPassMode => _machineRunMode == Defines.Process.EMachineRunMode.ByPass;
        public bool IsInterfaceOnlyMode => _machineRunMode == Defines.Process.EMachineRunMode.ManualInterface;

        public bool IsExistWindowPopup
        {
            get
            {
                return Application.Current.Dispatcher.Invoke<bool>(() =>
                {
                    foreach (var item in Application.Current.Windows)
                    {
                        if (item is IOMonitorWindowView || item is OriginView || item is InitializeWindowView) return true;
                    }

                    return false;

                });
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

        public bool IsOnMuting
        {
            get { return isOnMuting; }
            set 
            {
                isOnMuting = value;
                _outputs.MutingLamp.Value = value;
            }
        }

        public bool IsTrayOut
        {
            get { return isTrayOut; }
            set 
            {
                isTrayOut = value;
                OnPropertyChanged();
            }
        }

        public bool IsTrayHeadTrayPlacing { get; set; }
        public bool MachineReadyDone { get; set; }

        public bool OriginDone
        {
            get { return _originDone; }
            set { _originDone = value; }
        }

        public bool IsInputStop
        {
            get { return isInputStop; }
            set { isInputStop = value; OnPropertyChanged(); }
        }

        public bool IsOutputStop
        {
            get { return isOutputStop; }
            set
            {
                isOutputStop = value;
                OnPropertyChanged();
            }
        }

        public bool IsPickUpStop
        {
            get { return isPickupStop; }
            set { isPickupStop = value; OnPropertyChanged(); }
        }

        public bool IsCVConditionConfirm
        {
            get { return isCVConditionConfirm; }
            set { isCVConditionConfirm = value; OnPropertyChanged(); }
        }

        public bool IsTrayEmptyConfirm
        {
            get { return isTrayEmptyConfirm; }
            set { isTrayEmptyConfirm = value; OnPropertyChanged(); }
        }

        public ESemiSequence SemiAutoSequence
        {
            get => (ESemiSequence)_SemiAutoSequence;
            set
            {
                MultiThreadingHelpers.SafeSetValue(ref _SemiAutoSequence, value);
            }
        }

        public static bool IsNoteBookMode => File.Exists(@"D:\NoteBookMode.txt");

        public bool Vinyl_TrashSuctionOn { get; set; }
        public bool Sponge_TrashSuctionOn { get; set; }
    }
}

