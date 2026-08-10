using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Process
{
    public class TapeDetachProcess : ProcessBase<ESequence>
    {
        #region Physical IO
        private IDInput In_SpongeHoldVacOn => _devices.Inputs.SpongeHoldVacOn;

        private IDOutput Out_SpongeHoldVacOn => _devices.Outputs.SpongeHoldVacOn;
        private IDOutput Out_SpongeHoldVacOff => _devices.Outputs.SpongeHoldVacOff;
        private IDOutput Out_TrashSuctionOn => _devices.Outputs.TrashSuctionOn;

        private ICylinder Cyl_SpongePickupMoverFwBw => _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw;
        private ICylinder Cyl_SpongePickupMoverUpDn => _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn;
        private ICylinder Cyl_SpongeHoldGripper => _devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper;
        #endregion

        #region Virtual handshake
        private bool FlagIn_RemoveRequest => _tapeDetachInput[(int)ESpongeDetachInput.PREALIGN_REMOVE_SPONGE_REQUEST];
        private bool FlagIn_ReleaseRequest => _tapeDetachInput[(int)ESpongeDetachInput.PREALIGN_RELEASE_SPONGE_REQUEST];

        private bool FlagOut_RemoveDone
        {
            set => _tapeDetachOutput[(int)ESpongeDetachOutput.TAPE_REMOVE_DONE] = value;
        }

        private bool FlagOut_SafeDone
        {
            set => _tapeDetachOutput[(int)ESpongeDetachOutput.TAPE_REMOVE_SAFE_DONE] = value;
        }

        private bool FlagOut_ReadyDone
        {
            set => _tapeDetachOutput[(int)ESpongeDetachOutput.READY_DONE] = value;
        }
        #endregion

        #region Lifecycle
        public override bool PreProcess()
        {
            FlagOut_ReadyDone = _readyCompleted;
            return base.PreProcess();
        }

        public override bool ProcessToStop()
        {
            SavePausedState();
            return base.ProcessToStop();
        }

        public override bool ProcessToRun()
        {
            switch ((ETapeDetach_ToRunStep)Step.ToRunStep)
            {
                case ETapeDetach_ToRunStep.Start:
                    Log.Debug("TapeDetach ToRun start");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETapeDetach_ToRunStep.End;
                        break;
                    }

                    RestorePausedState();
                    Step.ToRunStep++;
                    break;

                case ETapeDetach_ToRunStep.ResetAndRestoreHandshake:
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    RestoreHandshakeAfterStopStart();
                    FlagOut_ReadyDone = _readyCompleted;
                    Log.Debug($"TapeDetach handshake restored: Sequence={Sequence}, RunStep={Step.RunStep}");
                    Step.ToRunStep++;
                    break;

                case ETapeDetach_ToRunStep.End:
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

                case ESequence.SpongeRemove_RemoveSponge:
                    Sequence_RemoveSponge();
                    break;

                default:
                    // This process owns no PreAlign Load/Unload or Rotator sequence.
                    Sequence = ESequence.AutoRun;
                    break;
            }

            return true;
        }
        #endregion

        #region Initialize / Ready
        private void Sequence_Ready()
        {
            switch ((ETapeDetach_ReadyStep)Step.RunStep)
            {
                case ETapeDetach_ReadyStep.Start:
                    if (!IsOriginOrInitSelected)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    _readyCompleted = false;
                    _isSpongeNotExist = false;
                    _spongeVacCheckOk = false;
                    _retryGripCount = 1;
                    _retryRemoveCount = 0;
                    ClearPausedState();
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.ClearHandshake:
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.VacuumOff:
                    SpongeRemoverVacOn(false);
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.MoveUp:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug("Initialize TapeDetach: move sponge remover Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.MoveUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.MoveBackward:
                    Cyl_SpongePickupFwBw(false);
                    Log.Debug("Initialize TapeDetach: move sponge remover Backward");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.MoveBackwardCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.GripperOff:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.GripperOffCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_ReadyStep.End:
                    _readyCompleted = true;
                    FlagOut_ReadyDone = true;
                    Log.Debug("TapeDetach Ready End");
                    Sequence = ESequence.Stop;
                    break;
            }
        }
        #endregion

        #region AutoRun
        private void Sequence_AutoRun()
        {
            switch ((ETapeDetach_AutoRunStep)Step.RunStep)
            {
                case ETapeDetach_AutoRunStep.Start:
                    if (_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }

                    Step.RunStep++;
                    break;

                case ETapeDetach_AutoRunStep.WaitRemoveRequest:
                    if (!FlagIn_RemoveRequest)
                    {
                        Wait(10);
                        break;
                    }

                    Sequence = ESequence.SpongeRemove_RemoveSponge;
                    break;
            }
        }
        #endregion

        #region Remove Sponge - TapeDetach physical sequence only
        private void Sequence_RemoveSponge()
        {
            switch ((ETapeDetach_RemoveSpongeStep)Step.RunStep)
            {
                case ETapeDetach_RemoveSpongeStep.Start:
                    Log.Debug("TapeDetach RemoveSponge Start");
                    _retryGripCount = 1;
                    _retryRemoveCount = 0;
                    _isSpongeNotExist = false;
                    _spongeVacCheckOk = false;
                    FlagOut_RemoveDone = false;
                    FlagOut_SafeDone = false;
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.PrepareGripperAndBlow:
                    Cyl_SpongeHoldGripper.Backward();
                    SpongeRemoverVacOn(false);
                    Log.Debug("TapeDetach gripper OFF + blow before approach");
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.PrepareGripperAndBlowCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveUp:
                    Cyl_SpongePickupUpDn(true);
                    Cyl_SpongeHoldGripper.Backward();
                    Log.Debug("TapeDetach remover Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveUpCheck:
                    if (!Cyl_SpongePickupMoverUpDn.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUp;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveIn:
                    Cyl_SpongePickupFwBw(true);
                    Log.Debug("TapeDetach remover Move In");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsForward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveInCheck:
                    if (!Cyl_SpongePickupMoverFwBw.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveFw_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveIn;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripperOffBeforeDown:
                    if (Cyl_SpongeHoldGripper.IsBackward)
                    {
                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveDown;
                        break;
                    }

                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripperOffBeforeDownCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveDown:
                    Cyl_SpongePickupUpDn(false);
                    Log.Debug("TapeDetach remover Move Down");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveDownCheck:
                    if (!Cyl_SpongePickupMoverUpDn.IsBackward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveDown;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.DownDelay:
                    Wait(_tapeRecipe.SpongeRemoverDownWait);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.VacuumOn:
                    SpongeRemoverVacOn(true);
                    Log.Debug("TapeDetach sponge vacuum ON");
                    Wait(_globalRecipe.VacCheckWaitTime,
                        () => In_SpongeHoldVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.VacuumOnCheck:
                    if (!In_SpongeHoldVacOn.Value && !_machineStatus.IsDryRunMode)
                    {
                        _isSpongeNotExist = true;
                        Cyl_SpongeHoldGripper.Backward();
                        Log.Debug("Sponge vacuum not detected; skip clamp and continue retract path.");
                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach;
                        break;
                    }

                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripOn:
                    if (!In_SpongeHoldVacOn.Value && !_machineStatus.IsDryRunMode)
                    {
                        _isSpongeNotExist = true;
                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach;
                        break;
                    }

                    Cyl_SpongeHoldGripper.Forward();
                    Log.Debug("TapeDetach sponge gripper ON");
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsForward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripOnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_SpongeHoldGripper.Backward();
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOn_Fail);
                        break;
                    }

                    if (!In_SpongeHoldVacOn.Value && !_machineStatus.IsDryRunMode)
                    {
                        _isSpongeNotExist = true;
                        Cyl_SpongeHoldGripper.Backward();
                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach;
                        break;
                    }

                    Wait(_tapeRecipe.SpongeGripperOnWait);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripOnDelay:
                    if (_retryGripCount >= _tapeRecipe.SpongeGripperGripCount)
                    {
                        _retryGripCount = 1;
                        Wait(_tapeRecipe.SpongeRemoverUpWait);
                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach;
                        break;
                    }

                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripOffRetry:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripOffRetryCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }

                    _retryGripCount++;
                    Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.GripOn;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug("TapeDetach remover Move Up after detach");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveUpAfterDetachCheck:
                    if (!Cyl_SpongePickupMoverUpDn.IsForward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveUpAfterDetach;
                        break;
                    }

                    _spongeVacCheckOk |= In_SpongeHoldVacOn.Value;
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.ValidateDetach:
                    if (!In_SpongeHoldVacOn.Value
                        && !_isSpongeNotExist
                        && _devRecipe.UseRetryRemoveSponge
                        && _tapeRecipe.SpongeHeadFunction == 1
                        && !_machineStatus.IsDryRunMode)
                    {
                        if (_retryRemoveCount < 2)
                        {
                            _retryRemoveCount++;
                            Log.Debug($"Retry sponge remove after vacuum loss. Retry={_retryRemoveCount}");
                            Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.GripperOffBeforeDown;
                            break;
                        }
                    }

                    _retryRemoveCount = 0;
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.SetDetachDone:
                    FlagOut_RemoveDone = true;
                    Log.Debug("TapeDetach REMOVE_DONE = ON");
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.WaitReleaseRequest:
                    if (!FlagIn_ReleaseRequest)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveOut:
                    Cyl_SpongePickupFwBw(false);
                    Log.Debug("TapeDetach remover Move Out");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveOutCheck:
                    if (!Cyl_SpongePickupMoverFwBw.IsBackward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveOut;
                        break;
                    }

                    _spongeVacCheckOk |= In_SpongeHoldVacOn.Value;
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveDownForDisposal:
                    Cyl_SpongePickupUpDn(false);
                    Log.Debug("TapeDetach remover Move Down for disposal");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.MoveDownForDisposalCheck:
                    if (!Cyl_SpongePickupMoverUpDn.IsBackward)
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                            break;
                        }

                        Step.RunStep = (int)ETapeDetach_RemoveSpongeStep.MoveDownForDisposal;
                        break;
                    }

                    _spongeVacCheckOk |= In_SpongeHoldVacOn.Value;
                    TrashSuctionOn(true);
                    SpongeRemoverVacOn(false);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.VacuumResultCheck:
                    if (!_spongeVacCheckOk && _devRecipe.UseSpongeVacCheck)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOn_Fail);
                        break;
                    }

                    _spongeVacCheckOk = false;
                    Wait(1000);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripperOff:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.GripperOffCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }

                    Wait(200);
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.VacuumOff:
                    SpongeRemoverVacOn(false);
                    if (_tapeRecipe.SpongeHeadFunction == 1)
                    {
                        Wait(_globalRecipe.VacCheckWaitTime,
                            () => !In_SpongeHoldVacOn.Value || _machineStatus.IsDryRunMode);
                    }
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.VacuumOffCheck:
                    if (WaitTimeOutOccurred && !_machineStatus.IsDryRunMode)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOff_Fail);
                        break;
                    }

                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.SetSafeDone:
                    _isSpongeNotExist = false;
                    FlagOut_SafeDone = true;
                    Log.Debug("TapeDetach SAFE_DONE = ON");
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.WaitRequestClear:
                    if (FlagIn_RemoveRequest || FlagIn_ReleaseRequest)
                    {
                        Wait(10);
                        break;
                    }

                    FlagOut_RemoveDone = false;
                    FlagOut_SafeDone = false;
                    Step.RunStep++;
                    break;

                case ETapeDetach_RemoveSpongeStep.End:
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
            Log.Debug($"Save TapeDetach pause state: Sequence={_savedSequence}, RunStep={_savedRunStep}");
        }

        private void RestorePausedState()
        {
            if (!_pausedFromRun || _savedSequence == null)
                return;

            Sequence = _savedSequence.Value;
            Step.RunStep = _savedRunStep;
            _pausedFromRun = false;
            Log.Debug($"Restore TapeDetach pause state: Sequence={Sequence}, RunStep={Step.RunStep}");
        }

        private void RestoreHandshakeAfterStopStart()
        {
            if (Sequence != ESequence.SpongeRemove_RemoveSponge)
                return;

            // DONE levels belong to one remove cycle only. Re-assert them only while
            // the process is still inside the request/ack window; never after End.
            if (Step.RunStep >= (int)ETapeDetach_RemoveSpongeStep.SetDetachDone
                && Step.RunStep <= (int)ETapeDetach_RemoveSpongeStep.WaitRequestClear)
            {
                FlagOut_RemoveDone = true;
            }

            if (Step.RunStep >= (int)ETapeDetach_RemoveSpongeStep.SetSafeDone
                && Step.RunStep <= (int)ETapeDetach_RemoveSpongeStep.WaitRequestClear)
            {
                FlagOut_SafeDone = true;
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
        private void SpongeRemoverVacOn(bool on)
        {
            Out_SpongeHoldVacOn.Value = on;
            Out_SpongeHoldVacOff.Value = !on;

            if (!on)
            {
                Task.Delay(700).ContinueWith(_ => Out_SpongeHoldVacOff.Value = false);
            }
        }

        private void Cyl_SpongePickupFwBw(bool forward)
        {
            if (forward)
                Cyl_SpongePickupMoverFwBw.Forward();
            else
                Cyl_SpongePickupMoverFwBw.Backward();
        }

        private void Cyl_SpongePickupUpDn(bool up)
        {
            if (up)
                Cyl_SpongePickupMoverUpDn.Forward();
            else
                Cyl_SpongePickupMoverUpDn.Backward();
        }

        private void TrashSuctionOn(bool on)
        {
            Out_TrashSuctionOn.Value = on;
            _machineStatus.Sponge_TrashSuctionOn = on;

            if (!on)
                return;

            Task.Delay((int)(_globalRecipe.TrashSuctionOnTime * 1000)).ContinueWith(_ =>
            {
                _machineStatus.Sponge_TrashSuctionOn = false;
                if (_machineStatus.Vinyl_TrashSuctionOn)
                    return;

                TrashSuctionOn(false);
            });
        }

        private void StopRun(bool clearHandshake)
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            if (clearHandshake)
                ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
        }
        #endregion

        public override string ToString() => EProcess.SpongeDetach.GetDescription();

        #region Constructor / fields
        public TapeDetachProcess(
            Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            DevRecipe devRecipe,
            [FromKeyedServices("SpongeDetachInput")] IDInputDevice<ESpongeDetachInput> tapeDetachInput,
            [FromKeyedServices("SpongeDetachOutput")] IDOutputDevice<ESpongeDetachOutput> tapeDetachOutput)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _devRecipe = devRecipe;
            _tapeDetachInput = tapeDetachInput;
            _tapeDetachOutput = tapeDetachOutput;
        }

        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly DevRecipe _devRecipe;
        private readonly IDInputDevice _tapeDetachInput;
        private readonly IDOutputDevice _tapeDetachOutput;

        private FlipperTapeDetachRecipe _tapeRecipe => _recipeList.FlipperTapeDetachRecipe;

        private bool _readyCompleted;
        private bool _isSpongeNotExist;
        private bool _spongeVacCheckOk;
        private uint _retryGripCount = 1;
        private int _retryRemoveCount;

        private ESequence? _savedSequence;
        private int _savedRunStep;
        private bool _pausedFromRun;
        #endregion
    }
}
