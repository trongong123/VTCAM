using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Vision;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp.XImgProc;
using System.Net.Http.Headers;
using System.Windows.Annotations;
using System.Windows.Markup.Localizer;

namespace FrontCameraAssembleEquipment.Process
{
    public class CameraFlipperProcess : ProcessBase<ESequence>
    {
        #region Inputs
        private IDInput In_VtCamRotatorDetectExist => _devices.Inputs.VtCamRotatorDetect;
        private IDInput In_VtCamRotatorSpongeDetect => _devices.Inputs.VtCamRotatorSpongeDetect;
        #endregion

        #region Outputs
        #endregion

        #region Cylinders
        private ICylinder Cyl_VtCamRotatorMoverFwBw => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw;
        private ICylinder Cyl_VtCamRotatorMoverUpDn => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn;
        private ICylinder Cyl_VtCamRotatorGripper => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper;
        private ICylinder Cyl_VtCamRotatorFlipper => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper;
        #endregion

        #region Motions
        #endregion

        #region Flags
        //Inputs
        private bool FlagIn_CamOutDone => _cameraFlipperInput[(int)ECameraFlipperInput.CAM_OUT_DONE];
        private bool FlagIn_FlipperSpongeDetachRequest => _cameraFlipperInput[(int)ECameraFlipperInput.FLIPPER_IN_REQUEST];
        private bool FlagIn_CamAssembleHeadVacOnOK => _cameraFlipperInput[(int)ECameraFlipperInput.CAMHEAD_VAC_ON_OK];
        private bool FlagIn_SpongeRemoveDone => _cameraFlipperInput[(int)ECameraFlipperInput.TAPE_REMOVE_DONE];
        private bool FlagIn_CamHeadAssembleReadyDone => _cameraFlipperInput[(int)ECameraFlipperInput.CAM_ASSEMBLE_HEAD_READY_DONE];
        private bool FlagIn_FlipperWorkRequest => _cameraFlipperInput[(int)ECameraFlipperInput.FLIPPER_WORK_REQUEST];

        private bool FlagIn_TrayHeadOutOfPlaceArea => _cameraFlipperInput[(int)ECameraFlipperInput.TRAYHEAD_OUT_OF_PLACE_AREA];

        //Output
        private bool FlagOut_GripOffDone { set => _cameraFlipperOutput[(int)ECameraFlipperOutput.GRIPER_OFF_DONE] = value; }
        private bool FlagOut_GripOnDone { set => _cameraFlipperOutput[(int)ECameraFlipperOutput.GRIPER_ON_DONE] = value; }
        private bool FlagOut_CamOutRequest { set => _cameraFlipperOutput[(int)ECameraFlipperOutput.CAM_OUT_REQUEST] = value; }
        private bool FlagOut_CamPickDone { set => _cameraFlipperOutput[(int)ECameraFlipperOutput.CAM_PICKUP_DONE] = value; }
        private bool FlagOut_FlipperGripperOffToSpongeRemoveDone { set => _cameraFlipperOutput[(int)ECameraFlipperOutput.FLIPPER_GRIPPER_OFF_DONE] = value; }


        private MaterialStatus materialStatus => _materialStatusList.RotatorMaterialStatus;
        #endregion

        #region Override Methods
        public override bool PreProcess()
        {
            //materialStatus.IsEditable = (_machineStatus.IsRunningProcessMode == false ? true : false);
            return base.PreProcess();
        }
        public override bool ProcessToRun()
        {
            switch ((EFlipperCam_ToRunStep)Step.ToRunStep)
            {
                case EFlipperCam_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)EFlipperCam_ToRunStep.End;
                        break;
                    }
                    RestorePausedRunStep();
                    Step.ToRunStep++;
                    break;
                case EFlipperCam_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ECameraFlipperOutput>)_cameraFlipperOutput).ClearOutputs();
                    if (Cyl_VtCamRotatorGripper.IsForward)
                    {
                        FlagOut_GripOnDone = true;
                        Log.Debug("Restore rotator grip-on done signal from physical gripper state after stop/start.");
                    }
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case EFlipperCam_ToRunStep.End:
                    Log.Debug("To Run End.");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }
            return true;
        }

        //public override bool ProcessOrigin()
        //{
        //    switch ((EFlipperCam_OriginStep)Step.OriginStep)
        //    {
        //        case EFlipperCam_OriginStep.Start:
        //            Log.Debug("CameraRotator Origin Start");
        //            Step.OriginStep++;
        //            Log.Debug("Wait Tray Head Z Axis home done");
        //            break;
        //        case EFlipperCam_OriginStep.WaitTrayHeadHomeZAxisDoneFlag:
        //            if (!FlagIn_TrayHeadZAxisHomeDone)
        //            {
        //                Wait(20);
        //                break;
        //            }

        //            Step.OriginStep++;
        //            Log.Debug("Set Flag Tray Head Z Axis home done received");
        //            break;
        //        case EFlipperCam_OriginStep.Set_TrayHeadHomeZAxisDoneReceived:
        //            Log.Debug("Wait Cam Assemble Z Axis home done");
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.WaitCamAssembleHeadHomeZAxisDoneFlag:
        //            if (!FlagIn_CamAssembleHeadZAxisHomeDone)
        //            {
        //                Wait(20);
        //                break;
        //            }

        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperCurrentStateCheck:
        //            if (Cyl_VtCamRotatorMoverFwBw.IsBackward)
        //            {
        //                Step.OriginStep = (int)EFlipperCam_OriginStep.CamFLipperRotateReady;
        //                break;
        //            }
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveUp:
        //            Cyl_VtCamRotatorUpDn(true);
        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Up");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_VtCamRotatorMoverUpDn.IsForward);
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveUp_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.CamFlipper_MoveUp_Fail);
        //                break;
        //            }

        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Up Done");
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveReady:
        //            Cyl_VtCamRotatorFwBw(false);
        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverFwBw} to Ready Pos");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_VtCamRotatorMoverFwBw.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveReady_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.CamFlipper_MoveUnloadPos_Fail);
        //                break;
        //            }

        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverFwBw} to Ready Pos Done");
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFLipperRotateReady:
        //            Cyl_VtCamRotatorFlip(false);
        //            Log.Debug($"Move {Cyl_VtCamRotatorFlipper} to Ready Pos");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_VtCamRotatorFlipper.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFLipperRotateReady_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.CamFlipper_MoveAndFlipReady_Fail);
        //                break;
        //            }

        //            Log.Debug($"Move {Cyl_VtCamRotatorFlipper} to Ready Pos Done");
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveDown:
        //            Cyl_VtCamRotatorUpDn(false);
        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Down");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_VtCamRotatorMoverUpDn.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.CamFlipperMoverMoveDown_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.CamFlipper_MoveDown_Fail);
        //                break;
        //            }

        //            Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Down Done");
        //            Step.OriginStep++;
        //            break;
        //        case EFlipperCam_OriginStep.End:
        //            Log.Debug("CameraRotator Origin End");
        //            ProcessStatus = EProcessStatus.OriginDone;
        //            Step.OriginStep++;
        //            break;
        //        default:
        //            Wait(10);
        //            break;
        //    }
        //    return true;
        //}

        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    break;
                case ESequence.AutoRun:
                    if (_devRecipe.UseOriginalSpongeRemove)
                    {
                        Sequence_AutoRun_OriginalVer();
                        break;
                    }
                    Sequence_AutoRun();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.SpongeDetach_RemoveSponge:
                    Sequence_SpongeRemove();
                    break;
                case ESequence.CamHead_Pick:
                    Sequence_CamUnload();
                    break;
                case ESequence.Flipper_Pick:
                    Sequence_CamPick();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }
            return true;
        }

        public override bool ProcessToStop()
        {
            StopRun();
            ProcessStatus = EProcessStatus.ToStopDone;

            return base.ProcessToStop();
        }

        public override bool ProcessToAlarm()
        {
            if (ProcessStatus == EProcessStatus.ToAlarmDone)
            {
                Thread.Sleep(50);
                return true;
            }
            StopRun();
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
            StopRun();
            ProcessStatus = EProcessStatus.ToWarningDone;
            return base.ProcessToWarning();
        }
        public override string ToString()
        {
            return EProcess.CameraRotator.GetDescription();
        }
        #endregion

        #region Private Methods
        private void Sequence_Ready()
        {
            switch ((EFlipperCam_ReadyStep)Step.RunStep)
            {
                case EFlipperCam_ReadyStep.Start:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Log.Debug("Sequence Ready Start");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ECameraFlipperOutput>)_cameraFlipperOutput).ClearOutputs();
                    Log.Debug("Clear Output Signal");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.WaitSpongeRemoveOut:
                    if (_devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn.IsForward == false
                       || _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.IsBackward == false)
                    {
                        Log.Debug("Wait sponge remover move up/back before releasing rotator gripper during initialize.");
                        Wait(10);
                        break;
                    }

                    Log.Debug("Sponge remover is up/back. Do not wait sponge remove done before returning rotator to ready.");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.Check_Status_Gripper:
                    if (Cyl_VtCamRotatorGripper.IsForward)
                    {
                        Log.Debug("Rotator gripper is clamped. Release gripper before moving up/back to ready.");
                        Step.RunStep = (int)EFlipperCam_ReadyStep.GripperOff;
                        break;
                    }
                    Step.RunStep = (int)EFlipperCam_ReadyStep.FlipperUp;
                    break;
                case EFlipperCam_ReadyStep.GripperOff:
                    Cyl_VtCamRotatorGrip(false);
                    Log.Debug("Rotator Gripper Off");
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.GripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Log.Debug("Rotator Gripper Off Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperUp:
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug("Move Flipper Up Ready");
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper Up Ready Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperMoveToReady:
                    Cyl_VtCamRotatorFwBw(false);
                    Log.Debug("Move Flipper to Ready Pos (Backward)");
                    Wait(3000, () => Cyl_VtCamRotatorMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperMoveToReady_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPos_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper To Ready Pos (Backward) Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperTurn:
                    Cyl_VtCamRotatorFlipper.Forward();
                    Wait(10000, () => Cyl_VtCamRotatorFlipper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.FlipperTurn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_RotateReady_Fail);
                        break;
                    }
                    Log.Debug("Flipper move ready fail");
                    Step.RunStep++;
                    break;
                case EFlipperCam_ReadyStep.End:
                    Log.Debug("Sequence Flipper Ready End");
                    Sequence = ESequence.Stop;
                    break;
            }
        }

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

                    Log.Debug("Auto Run Start");
                    Step.RunStep++;
                    break;

                case EFlipperCam_AutoRunStep.FlipperConditionCheck:
                    if (Cyl_VtCamRotatorMoverFwBw.IsForward && Cyl_VtCamRotatorMoverUpDn.IsBackward)
                    {
                        Sequence = ESequence.SpongeDetach_RemoveSponge;
                        break;
                    }

                    if (Cyl_VtCamRotatorGripper.IsForward
                        && Cyl_VtCamRotatorMoverFwBw.IsBackward
                        && Cyl_VtCamRotatorFlipper.IsForward)
                    {
                        Step.RunStep = (int)EFlipperCam_AutoRunStep.DelayToCheckCamExist;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.FlipperMoveUp:
                    Cyl_VtCamRotatorUpDn(true);
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.FlipperMoveUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Flipper Move Up Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.MoveToReadyPos:
                    Cyl_VtCamRotatorFwBw(false);
                    Cyl_VtCamRotatorFlip(true);
                    Log.Debug("Move Flipper to Ready Pos");
                    Wait(3000, () => Cyl_VtCamRotatorMoverFwBw.IsBackward
                                    && Cyl_VtCamRotatorFlipper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.MoveToReadyPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPos_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper to Ready Pos Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.DelayToCheckCamExist:
                    Wait(400);
                    Log.Debug("Delay to Check Cam Exist");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.CheckCamExist:
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_VtCamRotatorDetectExist, false);
#endif
                    if (In_VtCamRotatorDetectExist.Value == true)
                    {
                        Log.Debug("Sequence Cam Head Pick");
                        materialStatus.Set();
                        Step.RunStep++;
                        break;
                    }
                    _isSpongeRemoveDone = false;
                    materialStatus.Clear();
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                case EFlipperCam_AutoRunStep.CheckSpongeExist:
                    Sequence = ESequence.CamHead_Pick;
                    break;
                case EFlipperCam_AutoRunStep.End:
                    Log.Debug("Sequence Remove Sponge");
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                default:
                    break;

            }
        }
        private void Sequence_SpongeRemove()
        {
            switch ((EFlipperCam_PickStep)Step.RunStep)
            {
                case EFlipperCam_PickStep.Start:
                    Log.Debug("Flipper Sponge Remove Start");
                    retryCenteringCount = 0;
                    FlagOut_CamPickDone = false;
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.ConditionCheck:
                    if (_isSpongeRemoveDone == true)
                    {
                        Log.Debug("Remove Sponge Done -> Jump Step to CamGripperOff");
                        Step.RunStep = (int)EFlipperCam_PickStep.CamGripperOff;
                        break;
                    }

                    if (Cyl_VtCamRotatorMoverFwBw.IsForward && Cyl_VtCamRotatorMoverUpDn.IsBackward)
                    {
                        Step.RunStep = (int)EFlipperCam_PickStep.CamGripperOn;
                        break;
                    }

                    Step.RunStep = (int)EFlipperCam_PickStep.MoveFlipperUp;
                    break;
                case EFlipperCam_PickStep.MoveFlipperUp:
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug("Move Flipper Up");
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MoveFlipperUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper Up Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.FlipperUngripAndRotateToPick:
                    Cyl_VtCamRotatorGrip(false);
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsBackward);
                    Log.Debug($"Flipper Grip Off and Rotate");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.FlipperUngripAndRotateToPick_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOffAndRotateReady_Fail);
                        break;
                    }
                    Log.Debug($"Flipper Grip Off and Rotate Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.WaitSpongeDetachRequest:
                    if (FlagIn_FlipperSpongeDetachRequest)
                    {
                        Log.Debug("PreAlign Sponge Detach Request Signal Received");
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case EFlipperCam_PickStep.CheckTrayHeadOutOfPlaceArea:
                    if (FlagIn_TrayHeadOutOfPlaceArea)
                    {
                        Log.Debug("Tray Head Out Of Place Area Signal Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep.MoveFlipMoverToPickPos:
                    Cyl_VtCamRotatorFlip(false);
                    Cyl_VtCamRotatorFwBw(true);
                    Log.Debug($"{Cyl_VtCamRotatorMoverFwBw} Move to Pick Pos");
                    Wait(3000, () => Cyl_VtCamRotatorMoverFwBw.IsForward
                                                                             && Cyl_VtCamRotatorFlipper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MoveFlipMoverToPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MovePick_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_VtCamRotatorMoverFwBw} Move to Pick Pos Done");
                    Wait(300);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.WaitFlipperWorkRequestSignal:
                    if (FlagIn_FlipperWorkRequest == true)
                    {
                        Log.Debug("Flipper Work Request Signal received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep.MovePickupDown:
                    Cyl_VtCamRotatorUpDn(false);
                    Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Down");
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MovePickupDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveDown_Fail);
                        break;
                    }
                    //FlagOut_GripOnDone = true;
                    Log.Debug($"Cylinder {Cyl_VtCamRotatorMoverUpDn} Move Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MovePickupDownDone_Wait:
                    Wait(_flipperSpongeDetachRecipe.SpongeRemoverDownWait);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.CamGripperOn:
                    Cyl_VtCamRotatorGrip(true);
                    Log.Debug($"{Cyl_VtCamRotatorGripper} Grip On");
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.CamGripperOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_VtCamRotatorGrip(false);
                        _devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper.Backward();
                        RaiseWarning((int)EWarning.CAMRotator_GripOn_Fail);
                        break;
                    }

                    if (_devices.Inputs.VtCamPrealignVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        Cyl_VtCamRotatorGrip(false);
                        _devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper.Backward();
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }

                    FlagOut_GripOnDone = true;
                    Log.Debug("Flipper Grip On");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.WaitSpongeDetachDoneSignal:
                    if (FlagIn_SpongeRemoveDone)
                    {
                        _isSpongeRemoveDone = true;
                        Log.Debug("Sponge Detach done signal Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep.CamGripperOff:
                    Cyl_VtCamRotatorGrip(false);
                    Log.Debug($"{Cyl_VtCamRotatorGripper} Grip Off");
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.CamGripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Wait(500);
                    Log.Debug("Flipper Grip Off");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.CamGripperOnAgain:
                    FlagOut_FlipperGripperOffToSpongeRemoveDone = true;
                    retryCenteringCount++;
                    Cyl_VtCamRotatorGrip(true);
                    Log.Debug($"{Cyl_VtCamRotatorGripper} Grip On");
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.CamGripperOnAgain_Check:
                    Log.Debug("Flipper Grip On");
                    if (retryCenteringCount > _recipeList.FlipperTapeDetachRecipe.FlipperGripperGripCount - 1)
                    {
                        _devices.Outputs.VtCamPrealignVacOn.Value = false;
                        _devices.Outputs.VtCamPrealignFPCBVacON.Value = false;
                        Wait(200);
                        retryCenteringCount = 0;
                        Step.RunStep++;
                        break;
                    }
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_VtCamRotatorGrip(false);
                        _devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper.Backward();
                        RaiseWarning((int)EWarning.CAMRotator_GripOn_Fail);
                        break;
                    }

                    Step.RunStep = (int)EFlipperCam_PickStep.CamGripperOff;
                    Wait(400);
                    break;
                case EFlipperCam_PickStep.Wait_Cylinder_SpongeRemoveBackward:
                    if (_devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.IsBackward == false)
                    {
                        Wait(20);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MovePickupUp:
                    FlagOut_FlipperGripperOffToSpongeRemoveDone = false;
                    materialStatus.Set();
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Up");
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.MovePickupUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    _isSpongeRemoveDone = false;
                    FlagOut_GripOnDone = false;
                    Log.Debug($"Cylinder {Cyl_VtCamRotatorFlipper} Move Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.CamHead_Pick;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_CamUnload()
        {
            switch ((EFlipperCam_UnloadStep)Step.RunStep)
            {
                case EFlipperCam_UnloadStep.Start:
                    Log.Debug("FLipper Cam Unload Start");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.FlipperConditionCheck:
                    if (Cyl_VtCamRotatorMoverFwBw.IsBackward && Cyl_VtCamRotatorMoverUpDn.IsForward && Cyl_VtCamRotatorFlipper.IsForward)
                    {
                        Step.RunStep = (int)EFlipperCam_UnloadStep.SpongeExisCheck;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.MoveFlipperUp:
                    if (_devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn.IsForward == false)
                    {
                        _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn.Forward();
                        Log.Debug("Command and wait sponge remover up before unload rotator up.");
                        Wait(20);
                        break;
                    }

                    if (_devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.IsBackward == false)
                    {
                        _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.Backward();
                        Log.Debug("Command and wait sponge remover backward before unload rotator up.");
                        Wait(20);
                        break;
                    }
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug("Move Flipper Up");
                    Wait(10000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.MoveFlipperUp_Check:
                    if (Cyl_VtCamRotatorMoverUpDn.IsForward == false && WaitTimeOutOccurred == false)
                    {
                        Step.RunStep = (int)EFlipperCam_UnloadStep.MoveFlipperUp;
                        break;
                    }
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper Up Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.MoveFlipperToUnloadAndPosRotate:
                    Cyl_VtCamRotatorFwBw(false);
                    Cyl_VtCamRotatorFlip(true);
                    Log.Debug($"Move Flipper to Unload Pos ");
                    Wait(3000, () => Cyl_VtCamRotatorMoverFwBw.IsBackward && Cyl_VtCamRotatorFlipper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.MoveFlipperToUnloadPosAndRotate_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPosAndRotate_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper to Unload Pos done");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.SpongeExisCheck:
                    //if (_flipperSpongeDetachRecipe.SpongeDetect == 1 && _machineStatus.IsDryRunMode == false)
                    //{
                    //    if(In_VtCamRotatorSpongeDetect.Value == true)
                    //    {
                    //        Log.Debug("Sponge Exist Check Fail");
                    //        RaiseWarning((int)EWarning.CAMRotator_Sponge_Exist);
                    //        break;
                    //    }
                    //}
                    Log.Debug("Sponge Exist Check OK");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.CamExistCheck:
                    if (In_VtCamRotatorDetectExist.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_Camera_Not_Exist);
                        break;
                    }
                    FlagOut_CamPickDone = true;
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    Wait(500);
                    Log.Debug("Cam Exist Check OK");
                    Log.Debug("Wait Remove Sponge Done Clear");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.Wait_RemoveSpongeDoneClear:
                    if (FlagIn_SpongeRemoveDone)
                    {
                        Wait(10);
                        break;
                    }
                    _isSpongeRemoveDone = false;
                    FlagOut_CamPickDone = false;
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.RequestCamUnload:
                    FlagOut_CamOutRequest = true;
                    Log.Debug("Request Cam pick On");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.CheckCamUnload:
                    if (!FlagIn_CamAssembleHeadVacOnOK)
                    {
                        break;
                    }
                    FlagOut_CamOutRequest = false;
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.CamGripperOff:
                    Cyl_VtCamRotatorGrip(false);
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.CamGripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning(((int)EWarning.CAMRotator_GripOff_Fail));
                        break;
                    }
                    materialStatus.Clear();
                    FlagOut_GripOffDone = true;
                    Log.Debug($"{Cyl_VtCamRotatorGripper} Grip Off Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.CheckCamUnloadComplete:
                    if (FlagIn_CamOutDone == false)
                    {
                        Wait(20);
                        break;
                    }
                    FlagOut_GripOffDone = false;
                    Log.Debug("Camera Unload Complete");
                    Step.RunStep++;
                    break;
                case EFlipperCam_UnloadStep.End:
                    ClearPausedRunStep();
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    if (_devRecipe.UseOriginalSpongeRemove)
                    {
                        Sequence = ESequence.Flipper_Pick;
                        break;
                    }
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                default:
                    break;
            }
        }

        private void SavePausedRunStep()
        {
            if (Sequence == ESequence.Stop || Sequence == ESequence.Ready || Step.RunStep <= 0)
            {
                return;
            }

            _savedSequence = Sequence;
            _savedRunStep = Step.RunStep;
            _isPausedFromRun = true;
            Log.Debug($"Save paused run step: Sequence={_savedSequence}, RunStep={_savedRunStep}");
        }

        private void RestorePausedRunStep()
        {
            if (_isPausedFromRun == false || _savedSequence == null)
            {
                return;
            }

            Sequence = _savedSequence;
            Step.RunStep = _savedRunStep;
            _isPausedFromRun = false;
            Log.Debug($"Restore paused run step: Sequence={Sequence}, RunStep={Step.RunStep}");
        }

        private void ClearPausedRunStep()
        {
            _savedSequence = default;
            _savedRunStep = 0;
            _isPausedFromRun = false;
        }

        #endregion

        //======================================================================================================================================
        private void Sequence_AutoRun_OriginalVer()
        {
            switch ((EFlipperCam_AutoRunStep)Step.RunStep)
            {
                case EFlipperCam_AutoRunStep.Start:
                    Log.Debug("Sequence Camera Rotation");
                    Step.RunStep++;
                    break;

                case EFlipperCam_AutoRunStep.FlipperConditionCheck:
                    if (_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (Cyl_VtCamRotatorGripper.IsBackward && Cyl_VtCamRotatorMoverFwBw.IsBackward)
                    {
                        Sequence = ESequence.Flipper_Pick;
                    }

                    if (Cyl_VtCamRotatorGripper.IsForward
                        && Cyl_VtCamRotatorMoverFwBw.IsBackward
                        && Cyl_VtCamRotatorFlipper.IsForward)
                    {
                        Step.RunStep = (int)EFlipperCam_AutoRunStep.DelayToCheckCamExist;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.FlipperMoveUp:
                    Cyl_VtCamRotatorUpDn(true);
                    Wait(3000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.FlipperMoveUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Flipper Move Up Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.MoveToReadyPos:
                    Cyl_VtCamRotatorFwBw(false);
                    Cyl_VtCamRotatorFlip(true);
                    Log.Debug("Move Flipper to Ready Pos");
                    Wait(3000, () => Cyl_VtCamRotatorMoverFwBw.IsBackward && Cyl_VtCamRotatorFlipper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.MoveToReadyPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUnloadPos_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper to Ready Pos Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.DelayToCheckCamExist:
                    Wait(400);
                    Log.Debug("Delay to Check Cam Exist");
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.CheckCamExist:
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_VtCamRotatorDetectExist, false);
#endif
                    if (In_VtCamRotatorDetectExist.Value == true || In_VtCamRotatorSpongeDetect.Value == true)
                    {
                        Log.Debug("Sequence Cam Head Pick");
                        Sequence = ESequence.CamHead_Pick;
                        materialStatus.Set();
                        break;
                    }
                    materialStatus.Clear();
                    Sequence = ESequence.Flipper_Pick;
                    Step.RunStep++;
                    break;
                case EFlipperCam_AutoRunStep.End:
                    Log.Debug("Sequence Flipper Pick");
                    Sequence = ESequence.Flipper_Pick;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_CamPick()
        {
            switch ((EFlipperCam_PickStep_OriginalVer)Step.RunStep)
            {
                case EFlipperCam_PickStep_OriginalVer.Start:
                    Log.Debug("Flipper Cam Pick Start");
                    FlagOut_CamPickDone = false;
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.FlipperConditionCheck:
                    if (Cyl_VtCamRotatorMoverUpDn.IsBackward)
                    {
                        Step.RunStep = (int)EFlipperCam_PickStep_OriginalVer.FlipperUngripAndRotateToPick;
                        break;
                    }

                    Step.RunStep = (int)EFlipperCam_PickStep_OriginalVer.MoveFlipperUp;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MoveFlipperUp:
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug("Move Flipper Up");
                    Wait(3000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MoveFlipperUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Move Flipper Up Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.FlipperUngripAndRotateToPick:
                    Cyl_VtCamRotatorGrip(false);
                    Wait(10000, () => Cyl_VtCamRotatorGripper.IsBackward);
                    Log.Debug($"Flipper Grip Off and Rotate");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.FlipperUngripAndRotateToPick_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOff_Fail);
                        break;
                    }
                    Log.Debug($"Flipper Grip Off and Rotate Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.CheckPrealignCamOutRequest:
                    if (FlagIn_FlipperSpongeDetachRequest)
                    {
                        Log.Debug("PreAlign Cam Out Request Signal Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep_OriginalVer.CheckTrayHeadOutOfPlaceArea:
                    if (FlagIn_TrayHeadOutOfPlaceArea)
                    {
                        Log.Debug("Tray Head Out Of Place Area Signal Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep_OriginalVer.MoveFlipMoverToPickPos:
                    Cyl_VtCamRotatorFwBw(true);
                    Cyl_VtCamRotatorFlip(false);
                    Log.Debug($"{Cyl_VtCamRotatorMoverFwBw} Move to Pick Pos");
                    Wait(4000, () => Cyl_VtCamRotatorMoverFwBw.IsForward && Cyl_VtCamRotatorFlipper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MoveFlipMoverToPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MovePick_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_VtCamRotatorMoverFwBw} Move to Pick Pos Done");
                    Wait(200);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.WaitPickRequestSignal:
                    if (FlagIn_FlipperWorkRequest)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case EFlipperCam_PickStep_OriginalVer.MovePickupDown:
                    Cyl_VtCamRotatorUpDn(false);
                    Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Down");
                    Wait(3000, () => Cyl_VtCamRotatorMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MovePickupDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveDown_Fail);
                        break;
                    }
                    Log.Debug($"Cylinder {Cyl_VtCamRotatorMoverUpDn} Move Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MovePickupDownDone_Wait:
                    Wait(_flipperSpongeDetachRecipe.FlipperDownWait);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.CamGripperOn:
                    Cyl_VtCamRotatorGrip(true);
                    Log.Debug($"{Cyl_VtCamRotatorGripper} Grip On");
                    Wait(2500, () => Cyl_VtCamRotatorGripper.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.CamGripperOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_GripOn_Fail);
                        break;
                    }
                    Log.Debug("Flipper Grip On");
                    FlagOut_GripOnDone = true;
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.CamGripperOnDone_Wait:
                    Wait(_flipperSpongeDetachRecipe.CamGripOnWait);
                    Log.Debug("Delay after grip on");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MovePickupUp:
                    if (!FlagIn_SpongeRemoveDone) //FlagIn_PreaglignVacOffDone
                    {
                        break;
                    }
                    materialStatus.Set();
                    Cyl_VtCamRotatorUpDn(true);
                    Log.Debug($"Move {Cyl_VtCamRotatorMoverUpDn} Up");
                    Wait(4000, () => Cyl_VtCamRotatorMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.MovePickupUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMRotator_MoveUp_Fail);
                        break;
                    }
                    FlagOut_GripOnDone = false;
                    Log.Debug($"Cylinder {Cyl_VtCamRotatorFlipper} Move Done");
                    Step.RunStep++;
                    break;
                case EFlipperCam_PickStep_OriginalVer.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.CamHead_Pick;
                    break;
                default:
                    break;
            }
        }


        #region Private Method(s)
        private void Cyl_VtCamRotatorFwBw(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_VtCamRotatorMoverFwBw.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorBw, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorFw, true);
#endif
            }
            else
            {
                Cyl_VtCamRotatorMoverFwBw.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorFw, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorBw, true);
#endif
            }
        }

        private void Cyl_VtCamRotatorUpDn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_VtCamRotatorMoverUpDn.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorDown, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorUp, true);
#endif
            }
            else
            {
                Cyl_VtCamRotatorMoverUpDn.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorUp, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorDown, true);
#endif
            }
        }

        private void Cyl_VtCamRotatorGrip(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_VtCamRotatorGripper.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorGrip, true);
#endif
            }
            else
            {
                Cyl_VtCamRotatorGripper.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotatorGrip, false);
#endif
            }
        }

        private void Cyl_VtCamRotatorFlip(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_VtCamRotatorFlipper.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotator0, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotator180, true);
#endif
            }
            else
            {
                Cyl_VtCamRotatorFlipper.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotator180, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamRotator0, true);
#endif
            }
        }
        private void StopRun()
        {
            SavePausedRunStep();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            ((MappableOutputDevice<ECameraFlipperOutput>)_cameraFlipperOutput).ClearOutputs();
            _isSpongeRemoveDone = false;
        }

        #endregion

        #region Constructors
        public CameraFlipperProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            DevRecipe devRecipe,
            [FromKeyedServices("CameraFlipperInput")] IDInputDevice<ECameraFlipperInput> cameraFlipperInput,
            [FromKeyedServices("CameraFlipperOutput")] IDOutputDevice<ECameraFlipperOutput> cameraFlipperOutput)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _materialStatusList = materialStatusList;
            _devRecipe = devRecipe;
            _cameraFlipperInput = cameraFlipperInput;
            _cameraFlipperOutput = cameraFlipperOutput;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _cameraFlipperInput;
        private readonly IDOutputDevice _cameraFlipperOutput;
        private readonly MaterialStatusList _materialStatusList;
        private readonly DevRecipe _devRecipe;

        private FlipperTapeDetachRecipe _flipperSpongeDetachRecipe => _recipeList.FlipperTapeDetachRecipe;
        private uint retryCenteringCount = 0;
        private bool _isSpongeRemoveDone { get; set; }
        #endregion
    }
}
