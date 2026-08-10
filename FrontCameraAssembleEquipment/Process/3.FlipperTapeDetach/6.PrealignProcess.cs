using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Process
{
    public class PrealignProcess : ProcessBase<ESequence>
    {
        #region Inputs / Outputs / Cylinder
        private IDInput In_CameraVacuumOn => _devices.Inputs.VtCamPrealignVacOn;

        private IDOutput Out_CameraVacuumOn => _devices.Outputs.VtCamPrealignVacOn;
        private IDOutput Out_CameraVacuumOff => _devices.Outputs.VtCamPrealignVacOff;
        private IDOutput Out_FpcbVacuumOn => _devices.Outputs.VtCamPrealignFPCBVacON;

        private ICylinder Cyl_Centering => _devices.Cylinders.FlipperSpongeDetach_VtCamCentering;
        #endregion

        #region Flags
        // TrayHead -> PreAlign
        private bool FlagIn_TrayHeadCamInDone => _prealignInput[(int)EPrealignInput.TRAYHEAD_CAM_IN_DONE];
        private bool FlagIn_TrayHeadZUpDone => _prealignInput[(int)EPrealignInput.TRAYHEAD_Z_UP_DONE];

        // TapeDetach -> PreAlign
        private bool FlagIn_TapeRemoveDone => _prealignInput[(int)EPrealignInput.TAPE_REMOVE_DONE];
        private bool FlagIn_TapeRemoveSafeDone => _prealignInput[(int)EPrealignInput.TAPE_REMOVE_SAFE_DONE];

        // CameraRotator -> PreAlign
        private bool FlagIn_RotatorLoadDone => _prealignInput[(int)EPrealignInput.ROTATOR_LOAD_DONE];
        private bool FlagIn_RotatorRemoveSpongeDone => _prealignInput[(int)EPrealignInput.ROTATOR_REMOVE_SPONGE_DONE];
        private bool FlagIn_RotatorLeftPrealignDone => _prealignInput[(int)EPrealignInput.ROTATOR_LEFT_PREALIGN_DONE];
        private bool FlagIn_RotatorReadyForPrealign => _prealignInput[(int)EPrealignInput.ROTATOR_READY_FOR_PREALIGN];

        // PreAlign status
        private bool FlagOut_CameraDetected { set => _prealignOutput[(int)EPrealignOutput.CAMERA_DETECTED] = value; }
        private bool FlagOut_CenteringOpenDone { set => _prealignOutput[(int)EPrealignOutput.CENTERING_OPEN_DONE] = value; }
        private bool FlagOut_CenteringCloseDone { set => _prealignOutput[(int)EPrealignOutput.CENTERING_CLOSE_DONE] = value; }
        private bool FlagOut_VacuumOnDone { set => _prealignOutput[(int)EPrealignOutput.VACUUM_ON_DONE] = value; }
        private bool FlagOut_VacuumOffDone { set => _prealignOutput[(int)EPrealignOutput.VACUUM_OFF_DONE] = value; }
        private bool FlagOut_FpcbVacuumOnDone { set => _prealignOutput[(int)EPrealignOutput.FPCB_VACUUM_ON_DONE] = value; }
        private bool FlagOut_FpcbVacuumOffDone { set => _prealignOutput[(int)EPrealignOutput.FPCB_VACUUM_OFF_DONE] = value; }
        private bool FlagOut_MaterialClearDone { set => _prealignOutput[(int)EPrealignOutput.MATERIAL_CLEAR_DONE] = value; }
        private bool FlagOut_ReadyDone { set => _prealignOutput[(int)EPrealignOutput.READY_DONE] = value; }

        // PreAlign -> other units
        private bool FlagOut_TrayHeadCamInRequest { set => _prealignOutput[(int)EPrealignOutput.TRAYHEAD_CAM_IN_REQUEST] = value; }
        private bool FlagOut_TapeRemoveRequest { set => _prealignOutput[(int)EPrealignOutput.TAPE_REMOVE_REQUEST] = value; }
        private bool FlagOut_TapeReleaseRequest { set => _prealignOutput[(int)EPrealignOutput.TAPE_RELEASE_REQUEST] = value; }
        private bool FlagOut_RotatorLoadRequest { set => _prealignOutput[(int)EPrealignOutput.ROTATOR_LOAD_REQUEST] = value; }
        private bool FlagOut_RotatorRemoveSpongeRequest { set => _prealignOutput[(int)EPrealignOutput.ROTATOR_REMOVE_SPONGE_REQUEST] = value; }
        private bool FlagOut_RotatorUnloadRequest { set => _prealignOutput[(int)EPrealignOutput.ROTATOR_UNLOAD_REQUEST] = value; }
        #endregion

        private MaterialStatus MaterialStatus => _materialStatusList.PreAlignMaterialStatus;

        #region Process lifecycle
        public override bool PreProcess()
        {
            PublishPhysicalStatus();
            return base.PreProcess();
        }

        public override bool ProcessToStop()
        {
            SavePausedState();
            // Manual Stop intentionally keeps virtual handshake outputs frozen.
            // They are rebuilt deterministically during ToRun, when every process is in ToRun mode.
            return base.ProcessToStop();
        }

        public override bool ProcessToRun()
        {
            switch ((EPrealign_ToRunStep)Step.ToRunStep)
            {
                case EPrealign_ToRunStep.Start:
                    Log.Debug("PreAlign ToRun start");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)EPrealign_ToRunStep.End;
                        break;
                    }

                    RestorePausedState();
                    Step.ToRunStep++;
                    break;

                case EPrealign_ToRunStep.ResetAndRestoreHandshake:
                    ((MappableOutputDevice<EPrealignOutput>)_prealignOutput).ClearOutputs();
                    PublishPhysicalStatus();
                    RestoreHandshakeAfterStopStart();
                    Log.Debug($"PreAlign handshake restored: Sequence={Sequence}, RunStep={Step.RunStep}");
                    Step.ToRunStep++;
                    break;

                case EPrealign_ToRunStep.End:
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
            }

            return true;
        }

        public override bool ProcessToAlarm()
        {
            if (ProcessStatus == EProcessStatus.ToAlarmDone)
            {
                Thread.Sleep(50);
                return true;
            }

            StopRun(clearHandshake: true);
            ProcessStatus = EProcessStatus.ToAlarmDone;
            return base.ProcessToAlarm();
        }

        public override bool ProcessToWarning()
        {
            if (ProcessStatus == EProcessStatus.ToWarningDone)
            {
                Thread.Sleep(50);
                return true;
            }

            StopRun(clearHandshake: true);
            ProcessStatus = EProcessStatus.ToWarningDone;
            return base.ProcessToWarning();
        }

        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.AutoRun:
                    Sequence_AutoRun();
                    break;
                case ESequence.Prealign_Load:
                    Sequence_Load();
                    break;
                case ESequence.Prealign_RemoveSponge:
                    Sequence_RemoveSponge();
                    break;
                case ESequence.Prealign_Unload:
                    Sequence_Unload();
                    break;
                default:
                    Sequence = ESequence.AutoRun;
                    break;
            }

            return true;
        }
        #endregion

        #region Initialize / Ready
        private void Sequence_Ready()
        {
            switch ((EPrealign_ReadyStep)Step.RunStep)
            {
                case EPrealign_ReadyStep.Start:
                    if (!IsOriginOrInitSelected)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    _readyCompleted = false;
                    _cameraCycleActive = false;
                    _spongeRemoveCompleted = false;
                    ClearPausedState();
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.ClearHandshake:
                    ((MappableOutputDevice<EPrealignOutput>)_prealignOutput).ClearOutputs();
                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.CenteringOpen:
                    SetCentering(open: true);
                    Log.Debug("PreAlign centering open for initialize");
                    Wait(10000, () => Cyl_Centering.IsBackward);
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.CenteringOpenCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.VacuumCheckOn:
                    SetCameraVacuum(true);
                    Log.Debug("PreAlign vacuum ON to synchronize material during initialize");
                    Wait(_globalRecipe.VacCheckWaitTime);
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.VacuumCheck:
                    if (In_CameraVacuumOn.Value || _machineStatus.IsDryRunMode)
                    {
                        MaterialStatus.Set();
                        MaterialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                        Out_FpcbVacuumOn.Value = true;
                        _cameraCycleActive = true;
                        _spongeRemoveCompleted = false;
                        Log.Debug("Initialize: camera detected at PreAlign; keep vacuum and resume from remove-sponge phase on AutoRun.");
                    }
                    else
                    {
                        MaterialStatus.Clear();
                        SetCameraVacuum(false);
                        Out_FpcbVacuumOn.Value = false;
                        _cameraCycleActive = false;
                        _spongeRemoveCompleted = false;
                        Log.Debug("Initialize: PreAlign empty.");
                    }

                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EPrealign_ReadyStep.End:
                    _readyCompleted = true;
                    _machineStatus.IsResetErrorPreAlginVacOn = true;
                    PublishPhysicalStatus();
                    Log.Debug("PreAlign Ready End");
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region AutoRun coordinator
        private void Sequence_AutoRun()
        {
            switch ((EPrealign_AutoRunStep)Step.RunStep)
            {
                case EPrealign_AutoRunStep.Start:
                    if (_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_AutoRunStep.DecideNextSequence:
                    bool cameraDetected = In_CameraVacuumOn.Value ||
                                          MaterialStatus.Status == EMaterialStatus.Existing ||
                                          _machineStatus.IsDryRunMode && _cameraCycleActive;

                    if (!cameraDetected)
                    {
                        _cameraCycleActive = false;
                        _spongeRemoveCompleted = false;

                        if (!FlagIn_RotatorReadyForPrealign)
                        {
                            Wait(10);
                            break;
                        }

                        Sequence = ESequence.Prealign_Load;
                        break;
                    }

                    _cameraCycleActive = true;
                    MaterialStatus.Set();

                    Sequence = _spongeRemoveCompleted
                        ? ESequence.Prealign_Unload
                        : ESequence.Prealign_RemoveSponge;
                    break;
            }
        }
        #endregion

        #region PreAlign Load: TrayHead -> PreAlign
        private void Sequence_Load()
        {
            switch ((EPrealign_LoadStep)Step.RunStep)
            {
                case EPrealign_LoadStep.Start:
                    FlagOut_TrayHeadCamInRequest = false;
                    _cameraCycleActive = false;
                    _spongeRemoveCompleted = false;
                    Log.Debug("PreAlign Load Start");
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.WaitRotatorSafe:
                    if (!FlagIn_RotatorReadyForPrealign)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.CheckPrealignEmpty:
                    if (In_CameraVacuumOn.Value && !_machineStatus.IsDryRunMode)
                    {
                        MaterialStatus.Set();
                        _cameraCycleActive = true;
                        Sequence = ESequence.Prealign_RemoveSponge;
                        Log.Debug("PreAlign already has camera; skip load and continue remove-sponge.");
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.CenteringOpen:
                    SetCentering(open: true);
                    Wait(10000, () => Cyl_Centering.IsBackward);
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.CenteringOpenCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.VacuumOn:
                    SetCameraVacuum(true);
                    Out_FpcbVacuumOn.Value = true;
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.RequestCameraFromTrayHead:
                    FlagOut_TrayHeadCamInRequest = true;
                    Log.Debug("PreAlign requests camera from TrayHead");
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.WaitCameraFromTrayHead:
                    if (!FlagIn_TrayHeadCamInDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.VacuumCheck:
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_CameraVacuumOn.Value || _machineStatus.IsDryRunMode);
                    if (WaitTimeOutOccurred && !_machineStatus.IsDryRunMode)
                    {
                        _machineStatus.IsResetErrorPreAlginVacOn = false;
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }

                    MaterialStatus.Set();
                    MaterialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    _cameraCycleActive = true;
                    _spongeRemoveCompleted = false;
                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.WaitTrayHeadSafe:
                    if (!FlagIn_TrayHeadZUpDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.ClearRequest:
                    FlagOut_TrayHeadCamInRequest = false;
                    Step.RunStep++;
                    break;

                case EPrealign_LoadStep.End:
                    ClearPausedState();
                    Sequence = Parent?.Sequence == ESequence.AutoRun
                        ? ESequence.Prealign_RemoveSponge
                        : ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region PreAlign Remove Sponge coordinator
        private void Sequence_RemoveSponge()
        {
            switch ((EPrealign_SpongeRemoveStep)Step.RunStep)
            {
                case EPrealign_SpongeRemoveStep.Start:
                    Log.Debug("PreAlign RemoveSponge Start");
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.CheckCamera:
                    if (!In_CameraVacuumOn.Value && !_machineStatus.IsDryRunMode)
                    {
                        _machineStatus.IsResetErrorPreAlginVacOn = false;
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }
                    MaterialStatus.Set();
                    MaterialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.CenteringOpen:
                    SetCentering(open: true);
                    Wait(10000, () => Cyl_Centering.IsBackward);
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.CenteringOpenCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.RequestRotatorLoad:
                    FlagOut_RotatorLoadRequest = true;
                    Log.Debug("PreAlign -> Rotator LOAD request");
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.WaitRotatorLoadDone:
                    if (!FlagIn_RotatorLoadDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.ClearRotatorLoadRequest:
                    FlagOut_RotatorLoadRequest = false;
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.RequestTapeRemove:
                    FlagOut_TapeRemoveRequest = true;
                    Log.Debug("PreAlign -> TapeDetach REMOVE request");
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.WaitTapeRemoveDone:
                    if (!FlagIn_TapeRemoveDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.RequestRotatorRemoveSponge:
                    FlagOut_RotatorRemoveSpongeRequest = true;
                    Log.Debug("PreAlign -> Rotator REMOVE-SPONGE request");
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.WaitRotatorRemoveSpongeDone:
                    if (!FlagIn_RotatorRemoveSpongeDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.RequestTapeRelease:
                    FlagOut_TapeReleaseRequest = true;
                    Log.Debug("PreAlign -> TapeDetach RELEASE/SAFE-OUT request");
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.WaitTapeSafeDone:
                    if (!FlagIn_TapeRemoveSafeDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.ClearRemoveRequests:
                    FlagOut_RotatorRemoveSpongeRequest = false;
                    FlagOut_TapeReleaseRequest = false;
                    FlagOut_TapeRemoveRequest = false;
                    _spongeRemoveCompleted = true;
                    Step.RunStep++;
                    break;

                case EPrealign_SpongeRemoveStep.End:
                    ClearPausedState();
                    Sequence = Parent?.Sequence == ESequence.AutoRun
                        ? ESequence.Prealign_Unload
                        : ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region PreAlign Unload: release camera to Rotator and let Rotator leave
        private void Sequence_Unload()
        {
            switch ((EPrealign_UnloadStep)Step.RunStep)
            {
                case EPrealign_UnloadStep.Start:
                    Log.Debug("PreAlign Unload Start");
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.VacuumOff:
                    SetCameraVacuum(false);
                    Out_FpcbVacuumOn.Value = false;
                    Wait(_globalRecipe.VacCheckWaitTime, () => !In_CameraVacuumOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.VacuumOffCheck:
                    if (WaitTimeOutOccurred && !_machineStatus.IsDryRunMode)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PreAlignVacOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.ClearMaterial:
                    MaterialStatus.Clear();
                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.RequestRotatorUnload:
                    FlagOut_RotatorUnloadRequest = true;
                    Log.Debug("PreAlign -> Rotator UNLOAD request");
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.WaitRotatorLeftPrealign:
                    if (!FlagIn_RotatorLeftPrealignDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.ClearRotatorUnloadRequest:
                    FlagOut_RotatorUnloadRequest = false;
                    _cameraCycleActive = false;
                    _spongeRemoveCompleted = false;
                    Step.RunStep++;
                    break;

                case EPrealign_UnloadStep.End:
                    ClearPausedState();
                    Sequence = Parent?.Sequence == ESequence.AutoRun
                        ? ESequence.AutoRun
                        : ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region Stop/Start recovery
        private void SavePausedState()
        {
            if (Sequence == ESequence.Stop || Sequence == ESequence.Ready || Step.RunStep <= 0)
                return;

            _savedSequence = Sequence;
            _savedRunStep = Step.RunStep;
            _pausedFromRun = true;
            Log.Debug($"Save PreAlign pause state: Sequence={_savedSequence}, RunStep={_savedRunStep}");
        }

        private void RestorePausedState()
        {
            if (!_pausedFromRun || _savedSequence == null)
                return;

            Sequence = _savedSequence.Value;
            Step.RunStep = _savedRunStep;
            _pausedFromRun = false;
            Log.Debug($"Restore PreAlign pause state: Sequence={Sequence}, RunStep={Step.RunStep}");
        }

        private void RestoreHandshakeAfterStopStart()
        {
            switch (Sequence)
            {
                case ESequence.Prealign_Load:
                    if (Step.RunStep >= (int)EPrealign_LoadStep.RequestCameraFromTrayHead &&
                        Step.RunStep <= (int)EPrealign_LoadStep.WaitTrayHeadSafe)
                    {
                        FlagOut_TrayHeadCamInRequest = true;
                    }
                    break;

                case ESequence.Prealign_RemoveSponge:
                    if (Step.RunStep >= (int)EPrealign_SpongeRemoveStep.RequestRotatorLoad &&
                        Step.RunStep <= (int)EPrealign_SpongeRemoveStep.WaitRotatorLoadDone)
                    {
                        FlagOut_RotatorLoadRequest = true;
                    }

                    if (Step.RunStep >= (int)EPrealign_SpongeRemoveStep.RequestTapeRemove &&
                        Step.RunStep <= (int)EPrealign_SpongeRemoveStep.ClearRemoveRequests)
                    {
                        FlagOut_TapeRemoveRequest = true;
                    }

                    if (Step.RunStep >= (int)EPrealign_SpongeRemoveStep.RequestRotatorRemoveSponge &&
                        Step.RunStep <= (int)EPrealign_SpongeRemoveStep.ClearRemoveRequests)
                    {
                        FlagOut_RotatorRemoveSpongeRequest = true;
                    }

                    if (Step.RunStep >= (int)EPrealign_SpongeRemoveStep.RequestTapeRelease &&
                        Step.RunStep <= (int)EPrealign_SpongeRemoveStep.ClearRemoveRequests)
                    {
                        FlagOut_TapeReleaseRequest = true;
                    }
                    break;

                case ESequence.Prealign_Unload:
                    if (Step.RunStep >= (int)EPrealign_UnloadStep.RequestRotatorUnload &&
                        Step.RunStep <= (int)EPrealign_UnloadStep.WaitRotatorLeftPrealign)
                    {
                        FlagOut_RotatorUnloadRequest = true;
                    }
                    break;
            }
        }

        private void ClearPausedState()
        {
            _savedSequence = null;
            _savedRunStep = 0;
            _pausedFromRun = false;
        }
        #endregion

        #region Helpers
        private void PublishPhysicalStatus()
        {
            bool cameraDetected = In_CameraVacuumOn.Value ||
                                  (_machineStatus.IsDryRunMode && _cameraCycleActive);

            FlagOut_CameraDetected = cameraDetected;
            FlagOut_CenteringOpenDone = Cyl_Centering.IsBackward;
            FlagOut_CenteringCloseDone = Cyl_Centering.IsForward;
            FlagOut_VacuumOnDone = cameraDetected;
            FlagOut_VacuumOffDone = !In_CameraVacuumOn.Value;
            FlagOut_FpcbVacuumOnDone = Out_FpcbVacuumOn.Value;
            FlagOut_FpcbVacuumOffDone = !Out_FpcbVacuumOn.Value;
            FlagOut_MaterialClearDone = MaterialStatus.Status != EMaterialStatus.Existing;
            FlagOut_ReadyDone = _readyCompleted;
        }

        private void SetCentering(bool open)
        {
            if (open)
                Cyl_Centering.Backward();
            else
                Cyl_Centering.Forward();
        }

        private void SetCameraVacuum(bool on)
        {
            Out_CameraVacuumOn.Value = on;
            Out_CameraVacuumOff.Value = !on;

            if (!on)
            {
                Out_FpcbVacuumOn.Value = false;
                Task.Delay(500).ContinueWith(_ => Out_CameraVacuumOff.Value = false);
            }
        }

        private void StopRun(bool clearHandshake)
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            if (clearHandshake)
                ((MappableOutputDevice<EPrealignOutput>)_prealignOutput).ClearOutputs();
        }
        #endregion

        public override string ToString() => EProcess.Prealign.GetDescription();

        #region Constructor / fields
        public PrealignProcess(
            Devices devices,
            GlobalRecipe globalRecipe,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            [FromKeyedServices("PrealignInput")] IDInputDevice<EPrealignInput> prealignInput,
            [FromKeyedServices("PrealignOutput")] IDOutputDevice<EPrealignOutput> prealignOutput)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _machineStatus = machineStatus;
            _materialStatusList = materialStatusList;
            _prealignInput = prealignInput;
            _prealignOutput = prealignOutput;
        }

        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly MachineStatus _machineStatus;
        private readonly MaterialStatusList _materialStatusList;
        private readonly IDInputDevice _prealignInput;
        private readonly IDOutputDevice _prealignOutput;

        private bool _readyCompleted;
        private bool _cameraCycleActive;
        private bool _spongeRemoveCompleted;

        private ESequence? _savedSequence;
        private int _savedRunStep;
        private bool _pausedFromRun;
        #endregion
    }
}
