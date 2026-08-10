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
    public class CameraFlipperProcess : ProcessBase<ESequence>
    {
        #region Physical IO
        private IDInput In_CameraDetect => _devices.Inputs.VtCamRotatorDetect;
        private IDInput In_SpongeDetect => _devices.Inputs.VtCamRotatorSpongeDetect;

        private ICylinder Cyl_MoverFwBw => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw;
        private ICylinder Cyl_MoverUpDn => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn;
        private ICylinder Cyl_Gripper => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper;
        private ICylinder Cyl_Flipper => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper;
        #endregion

        #region Virtual inputs
        // CameraAssemble -> Rotator
        private bool FlagIn_CamOutDone => _rotatorInput[(int)ECameraFlipperInput.CAM_OUT_DONE];
        private bool FlagIn_CamAssembleVacOnOk => _rotatorInput[(int)ECameraFlipperInput.CAMHEAD_VAC_ON_OK];

        // PreAlign -> Rotator
        private bool FlagIn_PrealignCameraDetected => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_CAMERA_DETECTED];
        private bool FlagIn_PrealignCenteringOpenDone => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_CENTERING_OPEN_DONE];
        private bool FlagIn_LoadRequest => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_ROTATOR_LOAD_REQUEST];
        private bool FlagIn_RemoveSpongeRequest => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_ROTATOR_REMOVE_SPONGE_REQUEST];
        private bool FlagIn_UnloadRequest => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_ROTATOR_UNLOAD_REQUEST];
        private bool FlagIn_PrealignReadyDone => _rotatorInput[(int)ECameraFlipperInput.PREALIGN_READY_DONE];

        // TapeDetach -> Rotator
        private bool FlagIn_TapeReadyDone => _rotatorInput[(int)ECameraFlipperInput.TAPE_READY_DONE];
        #endregion

        #region Virtual outputs
        // CameraAssemble handshake
        private bool FlagOut_GripOffDone
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.GRIPER_OFF_DONE] = value;
        }

        private bool FlagOut_CamOutRequest
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.CAM_OUT_REQUEST] = value;
        }

        // PreAlign handshake
        private bool FlagOut_LoadDone
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.PREALIGN_ROTATOR_LOAD_DONE] = value;
        }

        private bool FlagOut_RemoveSpongeDone
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.PREALIGN_ROTATOR_REMOVE_SPONGE_DONE] = value;
        }

        private bool FlagOut_LeftPrealignDone
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.PREALIGN_ROTATOR_LEFT_DONE] = value;
        }

        private bool FlagOut_ReadyForPrealign
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.PREALIGN_ROTATOR_READY_FOR_PREALIGN] = value;
        }

        private bool FlagOut_ReadyDone
        {
            set => _rotatorOutput[(int)ECameraFlipperOutput.READY_DONE] = value;
        }
        #endregion

        private MaterialStatus MaterialStatus => _materialStatusList.RotatorMaterialStatus;

        #region Lifecycle
        public override bool PreProcess()
        {
            PublishPhysicalStatus();
            return base.PreProcess();
        }

        public override bool ProcessToStop()
        {
            SavePausedState();
            return base.ProcessToStop();
        }

        public override bool ProcessToRun()
        {
            switch ((EFlipperCam_ToRunStep)Step.ToRunStep)
            {
                case EFlipperCam_ToRunStep.Start:
                    Log.Debug("CameraRotator ToRun start");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)EFlipperCam_ToRunStep.End;
                        break;
                    }

                    RestorePausedState();
                    Step.ToRunStep++;
                    break;

                case EFlipperCam_ToRunStep.ResetAndRestoreHandshake:
                    ((MappableOutputDevice<ECameraFlipperOutput>)_rotatorOutput).ClearOutputs();
                    PublishPhysicalStatus();
                    RestoreHandshakeAfterStopStart();
                    Log.Debug($"CameraRotator handshake restored: Sequence={Sequence}, RunStep={Step.RunStep}");
                    Step.ToRunStep++;
                    break;

                case EFlipperCam_ToRunStep.End:
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

                case ESequence.CameraRotator_Load:
                    Sequence_Load();
                    break;

                case ESequence.CameraRotator_RemoveSponge:
                    Sequence_RemoveSponge();
                    break;

                case ESequence.CameraRotator_Unload:
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
            switch ((EFlipperCam_ReadyStep)Step.RunStep)
            {
                case EFlipperCam_ReadyStep.Start:
                    if (!IsOriginOrInitSelected)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    _readyCompleted = false;
                    _gripCycleCount = 0;
                    _unloadRequestHandshakeActive = false;
                    ClearPausedState();
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.ClearHandshake:
                    ((MappableOutputDevice<ECameraFlipperOutput>)_rotatorOutput).ClearOutputs();
                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.WaitPrealignAndTapeReady:
                    if (!FlagIn_PrealignReadyDone || !FlagIn_TapeReadyDone)
                    {
                        Wait(20);
                        break;
                    }

                    Log.Debug("CameraRotator initialize: PreAlign and TapeDetach are ready");
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.CheckInterruptedState:
                    if (Cyl_MoverFwBw.IsForward
                        && Cyl_Gripper.IsForward
                        && (FlagIn_PrealignCameraDetected || _machineStatus.IsDryRunMode))
                    {
                        MaterialStatus.Set();
                        Step.RunStep = (int)EFlipperCam_ReadyStep.NormalizeLoadedDown;
                        break;
                    }

                    if ((In_CameraDetect.Value || _machineStatus.IsDryRunMode)
                        && Cyl_Gripper.IsForward)
                    {
                        MaterialStatus.Set();
                        Step.RunStep = (int)EFlipperCam_ReadyStep.MoveUp;
                        break;
                    }

                    MaterialStatus.Clear();
                    Step.RunStep = (int)EFlipperCam_ReadyStep.GripperOffIfEmpty;
                    break;

                case EFlipperCam_ReadyStep.NormalizeLoadedDown:
                    if (Cyl_MoverUpDn.IsBackward)
                    {
                        Step.RunStep = (int)EFlipperCam_ReadyStep.SyncCameraStatus;
                        break;
                    }

                    Cyl_RotatorUpDn(up: false);
                    Log.Debug("CameraRotator initialize: normalize interrupted PreAlign camera to Down");
                    Wait(10000, () => Cyl_MoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.NormalizeLoadedDownCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveDown_Fail);
                        break;
                    }

                    Step.RunStep = (int)EFlipperCam_ReadyStep.SyncCameraStatus;
                    break;

                case EFlipperCam_ReadyStep.GripperOffIfEmpty:
                    Cyl_RotatorGrip(on: false);
                    Wait(10000, () => Cyl_Gripper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.GripperOffIfEmptyCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.MoveUp:
                    Cyl_RotatorUpDn(up: true);
                    Log.Debug("CameraRotator initialize: Move Up");
                    Wait(10000, () => Cyl_MoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.MoveUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.MoveBackwardAndTurn:
                    Cyl_RotatorFwBw(forward: false);
                    Cyl_RotatorFlip(pickOrientation: false);
                    Log.Debug("CameraRotator initialize: Backward + unload orientation");
                    Wait(10000, () => Cyl_MoverFwBw.IsBackward && Cyl_Flipper.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.MoveBackwardAndTurnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPosAndRotate_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.SyncCameraStatus:
                    if (In_CameraDetect.Value ||
                        (Cyl_MoverFwBw.IsForward && FlagIn_PrealignCameraDetected) ||
                        _machineStatus.IsDryRunMode && Cyl_Gripper.IsForward)
                    {
                        MaterialStatus.Set();
                    }
                    else
                    {
                        MaterialStatus.Clear();
                    }

                    PublishPhysicalStatus();
                    Step.RunStep++;
                    break;

                case EFlipperCam_ReadyStep.End:
                    _readyCompleted = true;
                    _machineStatus.IsResetErrorRotatorNotExist = true;
                    _machineStatus.IsResetErrorPreAlginVacOn = true;
                    PublishPhysicalStatus();
                    Log.Debug("CameraRotator Ready End");
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region AutoRun coordinator
        private void Sequence_AutoRun()
        {
            switch ((EFlipperCam_AutoRunStep)Step.RunStep)
            {
                case EFlipperCam_AutoRunStep.Start:
                    if (_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_AutoRunStep.DecideNextSequence:
                    if (FlagIn_LoadRequest)
                    {
                        Sequence = ESequence.CameraRotator_Load;
                        break;
                    }

                    if (FlagIn_RemoveSpongeRequest)
                    {
                        Sequence = ESequence.CameraRotator_RemoveSponge;
                        break;
                    }

                    if (FlagIn_UnloadRequest)
                    {
                        Sequence = ESequence.CameraRotator_Unload;
                        break;
                    }

                    if (IsAtUnloadPosition()
                        && Cyl_Gripper.IsForward
                        && (In_CameraDetect.Value ||
                            MaterialStatus.Status == EMaterialStatus.Existing ||
                            _machineStatus.IsDryRunMode))
                    {
                        _unloadRequestHandshakeActive = false;
                        Sequence = ESequence.CameraRotator_Unload;
                        break;
                    }

                    Wait(10);
                    break;
            }
        }
        #endregion

        #region Rotator Load: safe position -> PreAlign camera grip
        private void Sequence_Load()
        {
            switch ((EFlipperCam_PickStep)Step.RunStep)
            {
                case EFlipperCam_PickStep.Start:
                    FlagOut_LoadDone = false;
                    Log.Debug("CameraRotator Load Start");
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.CheckAlreadyLoaded:
                    if (IsLoadedAtPrealign())
                    {
                        MaterialStatus.Set();
                        Log.Debug("CameraRotator already Forward + Down + Grip after recovery; acknowledge LOAD directly.");
                        Step.RunStep = (int)EFlipperCam_PickStep.SetLoadDone;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveUp:
                    Cyl_RotatorUpDn(up: true);
                    Wait(10000, () => Cyl_MoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.GripperOff:
                    Cyl_RotatorGrip(on: false);
                    Wait(10000, () => Cyl_Gripper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.GripperOffCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.RotateToPick:
                    Cyl_RotatorFlip(pickOrientation: true);
                    Wait(10000, () => Cyl_Flipper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.RotateToPickCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOffAndRotateReady_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveForward:
                    Cyl_RotatorFwBw(forward: true);
                    Log.Debug("CameraRotator Move Forward to PreAlign");
                    Wait(3000, () => Cyl_MoverFwBw.IsForward && Cyl_Flipper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveForwardCheck:
                    if (!Cyl_MoverFwBw.IsForward || !Cyl_Flipper.IsBackward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CAMRotator_MovePick_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_PickStep.MoveForward;
                        break;
                    }

                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.WaitCenteringOpen:
                    if (!FlagIn_PrealignCenteringOpenDone)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveDown:
                    if (!FlagIn_PrealignCenteringOpenDone)
                    {
                        Step.RunStep = (int)EFlipperCam_PickStep.WaitCenteringOpen;
                        Wait(10);
                        break;
                    }

                    Cyl_RotatorUpDn(up: false);
                    Log.Debug("CameraRotator Move Down at PreAlign");
                    Wait(10000, () => Cyl_MoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.MoveDownCheck:
                    if (!Cyl_MoverUpDn.IsBackward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CAMRotator_MoveDown_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_PickStep.MoveDown;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.DownDelay:
                    Wait(_rotatorRecipe.SpongeRemoverDownWait);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.GripOn:
                    Cyl_RotatorGrip(on: true);
                    Log.Debug("CameraRotator Grip ON at PreAlign");
                    Wait(10000, () => Cyl_Gripper.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.GripOnCheck:
                    if (!Cyl_Gripper.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            Cyl_RotatorGrip(on: false);
                            RaiseWarning((int)EWarning.CAMRotator_GripOn_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_PickStep.GripOn;
                        break;
                    }

                    if (!FlagIn_PrealignCameraDetected && !_machineStatus.IsDryRunMode)
                    {
                        Cyl_RotatorGrip(on: false);
                        _machineStatus.IsResetErrorPreAlginVacOn = false;
                        Log.Debug("PreAlign camera/vacuum is not confirmed while Rotator grip is ON.");
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }

                    MaterialStatus.Set();
                    Wait(_rotatorRecipe.CamGripOnWait);
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.SetLoadDone:
                    MaterialStatus.Set();
                    FlagOut_LoadDone = true;
                    Log.Debug("CameraRotator LOAD_DONE = ON");
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.WaitLoadRequestClear:
                    if (FlagIn_LoadRequest)
                    {
                        Wait(10);
                        break;
                    }

                    FlagOut_LoadDone = false;
                    Step.RunStep++;
                    break;

                case EFlipperCam_PickStep.End:
                    ClearPausedState();
                    Sequence = Parent?.Sequence == ESequence.AutoRun
                        ? ESequence.AutoRun
                        : ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region Rotator Remove Sponge: gripper centering only
        private void Sequence_RemoveSponge()
        {
            switch ((EFlipperCam_RemoveSpongeStep)Step.RunStep)
            {
                case EFlipperCam_RemoveSpongeStep.Start:
                    _gripCycleCount = 0;
                    FlagOut_RemoveSpongeDone = false;
                    Log.Debug("CameraRotator RemoveSponge Start");
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.CheckLoadedState:
                    if (!IsLoadedAtPrealign())
                    {
                        Log.Debug("Wait Rotator loaded state before gripper centering cycle.");
                        Wait(20);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.GripOff:
                    Cyl_RotatorGrip(on: false);
                    Log.Debug("CameraRotator Grip OFF for sponge-removal centering");
                    Wait(10000, () => Cyl_Gripper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.GripOffCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.GripOffDelay:
                    Wait(500);
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.GripOn:
                    Cyl_RotatorGrip(on: true);
                    Log.Debug("CameraRotator Grip ON for sponge-removal centering");
                    Wait(10000, () => Cyl_Gripper.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.GripOnCheck:
                    if (!Cyl_Gripper.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CAMRotator_GripOn_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_RemoveSpongeStep.GripOn;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.RepeatDecision:
                    _gripCycleCount++;
                    if (_gripCycleCount >= Math.Max(1, _rotatorRecipe.FlipperGripperGripCount))
                    {
                        Step.RunStep = (int)EFlipperCam_RemoveSpongeStep.SetRemoveDone;
                        break;
                    }

                    Wait(400);
                    Step.RunStep = (int)EFlipperCam_RemoveSpongeStep.GripOff;
                    break;

                case EFlipperCam_RemoveSpongeStep.SetRemoveDone:
                    MaterialStatus.Set();
                    FlagOut_RemoveSpongeDone = true;
                    Log.Debug("CameraRotator REMOVE_SPONGE_DONE = ON");
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.WaitRemoveRequestClear:
                    if (FlagIn_RemoveSpongeRequest)
                    {
                        Wait(10);
                        break;
                    }

                    FlagOut_RemoveSpongeDone = false;
                    Step.RunStep++;
                    break;

                case EFlipperCam_RemoveSpongeStep.End:
                    ClearPausedState();
                    Sequence = Parent?.Sequence == ESequence.AutoRun
                        ? ESequence.AutoRun
                        : ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region Rotator Unload: leave PreAlign then CameraAssemble handshake
        private void Sequence_Unload()
        {
            switch ((EFlipperCam_UnloadStep)Step.RunStep)
            {
                case EFlipperCam_UnloadStep.Start:
                    _unloadRequestHandshakeActive = FlagIn_UnloadRequest;
                    FlagOut_LeftPrealignDone = false;
                    FlagOut_CamOutRequest = false;
                    FlagOut_GripOffDone = false;
                    Log.Debug($"CameraRotator Unload Start. PreAlignRequest={_unloadRequestHandshakeActive}");
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.CheckPosition:
                    if (IsAtUnloadPosition())
                    {
                        Step.RunStep = _unloadRequestHandshakeActive
                            ? (int)EFlipperCam_UnloadStep.SetLeftPrealignDone
                            : (int)EFlipperCam_UnloadStep.SpongeExistCheck;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.MoveUp:
                    Cyl_RotatorUpDn(up: true);
                    Log.Debug("CameraRotator Move Up before leaving PreAlign");
                    Wait(10000, () => Cyl_MoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.MoveUpCheck:
                    if (!Cyl_MoverUpDn.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_UnloadStep.MoveUp;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.MoveBackwardAndTurn:
                    Cyl_RotatorFwBw(forward: false);
                    Cyl_RotatorFlip(pickOrientation: false);
                    Log.Debug("CameraRotator Backward + turn to CameraAssemble side");
                    Wait(10000, () => Cyl_MoverFwBw.IsBackward && Cyl_Flipper.IsForward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.MoveBackwardAndTurnCheck:
                    if (!Cyl_MoverFwBw.IsBackward || !Cyl_Flipper.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPosAndRotate_Fail);
                            break;
                        }

                        Step.RunStep = (int)EFlipperCam_UnloadStep.MoveBackwardAndTurn;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.SetLeftPrealignDone:
                    if (_unloadRequestHandshakeActive)
                    {
                        FlagOut_LeftPrealignDone = true;
                        Log.Debug("CameraRotator LEFT_PREALIGN_DONE = ON");
                        Step.RunStep++;
                        break;
                    }

                    Step.RunStep = (int)EFlipperCam_UnloadStep.SpongeExistCheck;
                    break;

                case EFlipperCam_UnloadStep.WaitUnloadRequestClear:
                    if (_unloadRequestHandshakeActive && FlagIn_UnloadRequest)
                    {
                        Wait(10);
                        break;
                    }

                    FlagOut_LeftPrealignDone = false;
                    _unloadRequestHandshakeActive = false;
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.SpongeExistCheck:
                    if (_rotatorRecipe.SpongeDetect == 1
                        && In_SpongeDetect.Value
                        && !_machineStatus.IsDryRunMode)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_Sponge_Exist);
                        break;
                    }

                    Wait(500);
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.CameraExistCheck:
                    if (!In_CameraDetect.Value && !_machineStatus.IsDryRunMode)
                    {
                        MaterialStatus.Clear();
                        _machineStatus.IsResetErrorRotatorNotExist = false;
                        RaiseWarning((int)EWarning.CAMRotator_Camera_Not_Exist);
                        break;
                    }

                    MaterialStatus.Set();
                    MaterialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.RequestCamUnload:
                    FlagOut_CamOutRequest = true;
                    Log.Debug("CameraRotator -> CameraAssemble CAM_OUT_REQUEST = ON");
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.WaitCamAssembleVacuumOn:
                    if (!FlagIn_CamAssembleVacOnOk)
                    {
                        Wait(10);
                        break;
                    }

                    FlagOut_CamOutRequest = false;
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.GripperOff:
                    Cyl_RotatorGrip(on: false);
                    Wait(10000, () => Cyl_Gripper.IsBackward);
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.GripperOffCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.SetGripOffDone:
                    FlagOut_GripOffDone = true;
                    Log.Debug("CameraRotator -> CameraAssemble GRIP_OFF_DONE = ON");
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.WaitCamUnloadDone:
                    if (!FlagIn_CamOutDone)
                    {
                        if (!_machineStatus.IsDryRunMode
                            && !In_CameraDetect.Value
                            && Cyl_Gripper.IsBackward)
                        {
                            Log.Debug("[STOP/START] Camera already left Rotator. Accept physical unload completion.");
                            Step.RunStep++;
                            break;
                        }

                        Wait(20);
                        break;
                    }

                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.ClearHandshake:
                    FlagOut_GripOffDone = false;
                    FlagOut_CamOutRequest = false;
                    MaterialStatus.Clear();
                    Log.Debug("CameraRotator camera unload completed");
                    Step.RunStep++;
                    break;

                case EFlipperCam_UnloadStep.End:
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
            Log.Debug($"Save CameraRotator pause state: Sequence={_savedSequence}, RunStep={_savedRunStep}");
        }

        private void RestorePausedState()
        {
            if (!_pausedFromRun || _savedSequence == null)
                return;

            Sequence = _savedSequence.Value;
            Step.RunStep = _savedRunStep;
            _pausedFromRun = false;
            Log.Debug($"Restore CameraRotator pause state: Sequence={Sequence}, RunStep={Step.RunStep}");
        }

        private void RestoreHandshakeAfterStopStart()
        {
            switch (Sequence)
            {
                case ESequence.CameraRotator_Load:
                    if (Step.RunStep >= (int)EFlipperCam_PickStep.SetLoadDone
                        && Step.RunStep <= (int)EFlipperCam_PickStep.WaitLoadRequestClear)
                    {
                        FlagOut_LoadDone = true;
                    }
                    break;

                case ESequence.CameraRotator_RemoveSponge:
                    if (Step.RunStep >= (int)EFlipperCam_RemoveSpongeStep.SetRemoveDone
                        && Step.RunStep <= (int)EFlipperCam_RemoveSpongeStep.WaitRemoveRequestClear)
                    {
                        FlagOut_RemoveSpongeDone = true;
                    }
                    break;

                case ESequence.CameraRotator_Unload:
                    if (_unloadRequestHandshakeActive
                        && Step.RunStep >= (int)EFlipperCam_UnloadStep.SetLeftPrealignDone
                        && Step.RunStep <= (int)EFlipperCam_UnloadStep.WaitUnloadRequestClear)
                    {
                        FlagOut_LeftPrealignDone = true;
                    }

                    if (Step.RunStep >= (int)EFlipperCam_UnloadStep.RequestCamUnload
                        && Step.RunStep <= (int)EFlipperCam_UnloadStep.WaitCamAssembleVacuumOn)
                    {
                        FlagOut_CamOutRequest = true;
                    }

                    if (Step.RunStep >= (int)EFlipperCam_UnloadStep.SetGripOffDone
                        && Step.RunStep <= (int)EFlipperCam_UnloadStep.WaitCamUnloadDone)
                    {
                        FlagOut_GripOffDone = true;
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
        private bool IsLoadedAtPrealign()
        {
            return Cyl_MoverFwBw.IsForward
                && Cyl_MoverUpDn.IsBackward
                && Cyl_Gripper.IsForward
                && (FlagIn_PrealignCameraDetected || _machineStatus.IsDryRunMode);
        }

        private bool IsAtUnloadPosition()
        {
            return Cyl_MoverFwBw.IsBackward
                && Cyl_MoverUpDn.IsForward
                && Cyl_Flipper.IsForward;
        }

        private void PublishPhysicalStatus()
        {
            FlagOut_ReadyForPrealign = Cyl_MoverFwBw.IsBackward && Cyl_MoverUpDn.IsForward;
            FlagOut_ReadyDone = _readyCompleted;
        }

        private void Cyl_RotatorFwBw(bool forward)
        {
            if (forward)
                Cyl_MoverFwBw.Forward();
            else
                Cyl_MoverFwBw.Backward();
        }

        private void Cyl_RotatorUpDn(bool up)
        {
            if (up)
                Cyl_MoverUpDn.Forward();
            else
                Cyl_MoverUpDn.Backward();
        }

        private void Cyl_RotatorGrip(bool on)
        {
            if (on)
                Cyl_Gripper.Forward();
            else
                Cyl_Gripper.Backward();
        }

        private void Cyl_RotatorFlip(bool pickOrientation)
        {
            if (pickOrientation)
                Cyl_Flipper.Backward();
            else
                Cyl_Flipper.Forward();
        }

        private void StopRun(bool clearHandshake)
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            if (clearHandshake)
                ((MappableOutputDevice<ECameraFlipperOutput>)_rotatorOutput).ClearOutputs();
        }
        #endregion

        public override string ToString() => EProcess.CameraRotator.GetDescription();

        #region Constructor / fields
        public CameraFlipperProcess(
            Devices devices,
            RecipeList recipeList,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            [FromKeyedServices("CameraFlipperInput")] IDInputDevice<ECameraFlipperInput> rotatorInput,
            [FromKeyedServices("CameraFlipperOutput")] IDOutputDevice<ECameraFlipperOutput> rotatorOutput)
        {
            _devices = devices;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _materialStatusList = materialStatusList;
            _rotatorInput = rotatorInput;
            _rotatorOutput = rotatorOutput;
        }

        private readonly Devices _devices;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly MaterialStatusList _materialStatusList;
        private readonly IDInputDevice _rotatorInput;
        private readonly IDOutputDevice _rotatorOutput;

        private FlipperTapeDetachRecipe _rotatorRecipe => _recipeList.FlipperTapeDetachRecipe;

        private bool _readyCompleted;
        private int _gripCycleCount;
        private bool _unloadRequestHandshakeActive;

        private ESequence? _savedSequence;
        private int _savedRunStep;
        private bool _pausedFromRun;
        #endregion
    }
}
