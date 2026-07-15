using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp.Internal.Vectors;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Media.Media3D;

namespace FrontCameraAssembleEquipment.Process
{
    public class SpongeDetachProcess : ProcessBase<ESequence>
    {
        #region Inputs
        private IDInput In_VtCamPreAlginVacOn => _devices.Inputs.VtCamPrealignVacOn;
        private IDInput In_SpongeHoldDetect => _devices.Inputs.SpongeHoldDetect;
        private IDInput In_SpongeHoldVacOn => _devices.Inputs.SpongeHoldVacOn;
        #endregion

        #region Outputs
        private IDOutput Out_VtCamPreAlignVacOn => _devices.Outputs.VtCamPrealignVacOn;
        private IDOutput Out_VtCamPrealignVacOff => _devices.Outputs.VtCamPrealignVacOff;
        private IDOutput Out_SpongeHoldVacOn => _devices.Outputs.SpongeHoldVacOn;
        private IDOutput Out_SpongeHoldVacOff => _devices.Outputs.SpongeHoldVacOff;
        private IDOutput Out_TrashSuctionOn => _devices.Outputs.TrashSuctionOn;
        #endregion

        #region Vaccum
        private Vaccum PreaAlignHoldVac => _vaccumList.Prealign_CamHoldVac;
        private Vaccum SpongeHoldVac => _vaccumList.SpongeDetach_SpongeHoldVac;

        #endregion

        #region Cylinders
        private ICylinder Cyl_VtCamCentering => _devices.Cylinders.FlipperSpongeDetach_VtCamCentering;
        private ICylinder Cyl_SpongePickupMoverFwBw => _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw;
        private ICylinder Cyl_SpongePickupMoverUpDn => _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn;
        private ICylinder Cyl_SpongeHoldGripper => _devices.Cylinders.FlipperSpongeDetach_SpongeHoldGripper;
        #endregion

        #region Motions
        #endregion

        #region Flags
        // Inputs
        private bool Flag_SpongeDetachCamInDone => _tapeDetachInput[(int)ESpongeDetachInput.TAPE_DETACH_CAM_IN_DONE];
        private bool FlagIn_CamOutDone => _tapeDetachInput[(int)ESpongeDetachInput.CAM_TAPE_DETACH_OUT_DONE];
        private bool FlagIn_FlipperGripOnDone => _tapeDetachInput[(int)ESpongeDetachInput.GRIP_ON_DONE];
        private bool FlagIn_TrayHeadZUpDone => _tapeDetachInput[(int)ESpongeDetachInput.TRAYHEAD_Z_UP_DONE];
        private bool FlagIn_FlipperGripperOffDone => _tapeDetachInput[(int)ESpongeDetachInput.FLIPPER_GRIPPER_OFF_DONE];

        // Outputs
        private bool FlagOut_SpongeDetachCamInRequest { set => _tapeDetachOutput[(int)ESpongeDetachOutput.TAPE_DETACH_CAM_IN_REQ] = value; }
        private bool FlagOut_FlipperInRequest { set => _tapeDetachOutput[(int)ESpongeDetachOutput.FLIPPER_IN_REQUEST] = value; }
        private bool FlagOut_SpongeRemoveDone { set => _tapeDetachOutput[(int)ESpongeDetachOutput.TAPE_REMOVE_DONE] = value; }
        private bool FlagOut_FlipperWorkRequest { set => _tapeDetachOutput[(int)ESpongeDetachOutput.FLIPPER_WORK_REQUEST] = value; }


        private MaterialStatus materialStatus => _materialStatusList.PreAlignMaterialStatus;
        #endregion

        #region Override Methods

        private int _gripperCount = 0;
        public override bool PreProcess()
        {
            switch ((ETapeDetachPreProcessStep)Step.PreProcessStep)
            {
                case ETapeDetachPreProcessStep.Start:
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.Material_Set:
                    if (ProcessMode == EProcessMode.Run)
                    {
                        if (In_VtCamPreAlginVacOn.Value == true)
                        {
                            materialStatus.Set();
                        }
                        else if (materialStatus.ProcessStatus != EMaterialProcessStatus.Processing)
                        {
                            materialStatus.Clear();
                        }
                    }
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.Gripper_OnOffEnable_Check:
                    //Skip auto retry 
                    //_isRetryGripOnOff = false;
                    //
                    if (_isRetryGripOnOff
                        && ProcessMode == EProcessMode.Run
                        && Sequence != ESequence.Ready
                        && Sequence != ESequence.Stop
                       )
                    {
                        if (_flipperSpongeDetachRecipe.SpongeRemoveGripRetryCount > 0)
                        {
                            Step.PreProcessStep++;
                            break;
                        }
                        Step.PreProcessStep = (int)ETapeDetachPreProcessStep.RemoveSponge_Up;
                        break;
                    }

                    Step.PreProcessStep = (int)ETapeDetachPreProcessStep.End;
                    break;
                case ETapeDetachPreProcessStep.Gripper_On:
                    Cyl_SpongeHoldGripper.Forward();
                    ProcessTimer.SpareTime = Environment.TickCount;
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.Gripper_On_Wait:
                    if (Environment.TickCount - ProcessTimer.SpareTime > 2000)
                    {
                        if (_isRetryGripOnOff == false)
                        {
                            Step.PreProcessStep = (int)ETapeDetachPreProcessStep.End;
                            break;
                        }
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOn_Fail);
                        break;
                    }
                    if (Cyl_SpongeHoldGripper.IsForward)
                    {
                        Step.PreProcessStep++;
                        break;
                    }
                    break;
                case ETapeDetachPreProcessStep.Gripper_Off:
                    Cyl_SpongeHoldGripper.Backward();
                    ProcessTimer.SpareTime = Environment.TickCount;
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.Gripper_Off_Wait:
                    if (Environment.TickCount - ProcessTimer.SpareTime > 2000)
                    {
                        if (_isRetryGripOnOff == false)
                        {
                            Step.PreProcessStep++;
                            break;
                        }
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    if (Cyl_SpongeHoldGripper.IsBackward)
                    {
                        _gripperCount++;
                        if (_gripperCount > _flipperSpongeDetachRecipe.SpongeRemoveGripRetryCount)
                        {
                            _gripperCount = 0;
                            _isRetryGripOnOff = false;
                            Step.PreProcessStep++;
                            break;
                        }
                        Step.PreProcessStep = (int)ETapeDetachPreProcessStep.Gripper_OnOffEnable_Check;
                        break;
                    }
                    break;
                case ETapeDetachPreProcessStep.RemoveSponge_Up:
                    Cyl_SpongePickupUpDn(true);
                    SpongeRemoverVacOn(false);
                    _isRetryGripOnOff = false;
                    Cyl_SpongeHoldGripper.Backward();
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.RemoveSponge_Up_Check:
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.PreProcessStep++;
                    break;
                case ETapeDetachPreProcessStep.End:
                    Step.PreProcessStep = (int)ETapeDetachPreProcessStep.Start;
                    break;
            }
            return base.PreProcess();
        }
        public override bool ProcessToStop()
        {
            _isRetryGripOnOff = false;
            ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
            Step.PreProcessStep = 0;

            return base.ProcessToStop();
        }
        //public override bool ProcessOrigin()
        //{
        //    switch ((ESpongeDetach_ReadyStep)Step.OriginStep)
        //    {
        //        case ESpongeDetach_ReadyStep.Start:
        //            Log.Debug("SpongeDetach Origin Start");
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachMoveUp:
        //            Cyl_SpongePickupUpDn(true);
        //            Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_SpongePickupMoverUpDn.IsForward);
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachMoveUp_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.SpongeRemover_MoveUp_Fail);
        //                break;
        //            }
        //            Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachMoveOut:
        //            Cyl_SpongePickupFwBw(false);
        //            Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_SpongePickupMoverFwBw.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachMoveOut_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.SpongeRemover_MoveBw_Fail);
        //                break;
        //            }
        //            Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachUngrip:
        //            Cyl_SpongeHoldGripper.Backward();
        //            Log.Debug($"{Cyl_SpongeHoldGripper} Grip Off");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_SpongeHoldGripper.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.SpongeDetachUngrip_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.SpongeRemover_GripOff_Fail);
        //                break;
        //            }
        //            Log.Debug($"{Cyl_SpongeHoldGripper} Grip Off Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.CamCenteringOff:
        //            Cyl_VtCamCentering.Backward();
        //            Log.Debug($"{Cyl_VtCamCentering} Off");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_VtCamCentering.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.CamCenteringOff_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.CamPrealign_CamCenteringOff_Fail);
        //                break;
        //            }

        //            Log.Debug($"{Cyl_VtCamCentering} Off Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESpongeDetach_ReadyStep.End:
        //            Log.Debug("SpongeDetach Origin End");
        //            ProcessStatus = EProcessStatus.OriginDone;
        //            Step.OriginStep++;
        //            break;
        //        default:
        //            Wait(10);
        //            break;
        //    }
        //    return true;
        //}

        public override bool ProcessToRun()
        {
            switch ((ESpongeDetach_ToRunStep)Step.ToRunStep)
            {
                case ESpongeDetach_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        FlagOut_SpongeRemoveDone = false;

                        Step.ToRunStep = (int)ESpongeDetach_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ESpongeDetach_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ESpongeDetach_ToRunStep.MaterialDataMatching_VacOn:
                    //PreaAlignVac(true);
                    //Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value);
                    Step.ToRunStep++;
                    break;
                case ESpongeDetach_ToRunStep.MaterialDataMatching_Check:
                    //Log.Debug("Material Data Matching Check");
                    //bool bCamExist = In_VtCamPreAlginVacOn.Value;
                    //bool bMaterialStatus = (materialStatus.Status == EMaterialStatus.Existing ? true : false);

                    //if (bCamExist == false)
                    //{
                    //    PreaAlignVac(false);
                    //}

                    //if (bCamExist != bMaterialStatus)
                    //{
                    //    RaiseWarning((int)EWarning.MaterialDataNotMatching);
                    //    break;
                    //}
                    Step.ToRunStep++;
                    break;
                case ESpongeDetach_ToRunStep.ErrorCheck:
                    if (_isResetErrorPreAlginVacOn == false)
                    {
                        if (MessageBoxEx.ShowDialog("WARNING: Check Camera In RemoveSponge and Initialze ! \r\n Cảnh báo: kiểm tra Camera ở cụm RemoveSponge và Initialze ! ") == true)
                        {
                            _isResetErrorPreAlginVacOn = true;
                        }
                        break;
                    }

                    Step.ToRunStep++;
                        break;
                case ESpongeDetach_ToRunStep.End:
                    Log.Debug("To Run End.");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
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
            Step.PreProcessStep = 0;

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
            Step.PreProcessStep = 0;
            StopRun();
            ProcessStatus = EProcessStatus.ToWarningDone;
            return base.ProcessToWarning();
        }
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
                case ESequence.TrayHead_Cam_Place:
                    Sequence_SpongeDetachLoad();
                    break;
                case ESequence.SpongeDetach_RemoveSponge:
                    if (_devRecipe.UseOriginalSpongeRemove)
                    {
                        Sequence_SpongeRemove_OriginalVer();
                        break;
                    }
                    Sequence_SpongeRemove();
                    break;
                case ESequence.Flipper_Pick:
                    Sequence_FlipperCamPick();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }

            return true;
        }

        public override string ToString()
        {
            return EProcess.SpongeDetach.GetDescription();
        }
        #endregion

        #region Privates Methods
        private void Sequence_Ready()
        {
            switch ((ESpongeDetach_ReadyStep)Step.RunStep)
            {
                case ESpongeDetach_ReadyStep.Start:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    PreaAlignVac(true);
                    Log.Debug("Prealign Vac On to sync material status during initialize");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.InternalInOutSignal_Reset:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        materialStatus.Clear();
                        PreaAlignVac(false);
                        Log.Debug("PreAlign vacuum check has no camera during initialize. Clear PreAlign material status.");
                    }
                    else
                    {
                        materialStatus.Set();
                        Log.Debug("PreAlign vacuum check has camera during initialize. Set PreAlign material status.");
                    }
                    Log.Debug("Clear Output Signal");
                    IsOnSpongeRemoveProcess = false;
                    _isSpongeRemoveDone = false;
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveUp:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveOut:
                    Cyl_SpongePickupFwBw(false);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveOut_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out Done");
                    FlagOut_SpongeRemoveDone = true;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachUngrip:
                    Cyl_SpongeHoldGripper.Backward();
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip Off");
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachUngrip_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip Off Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.CamCenteringOff:
                    Cyl_VtCamCentering.Backward();
                    Log.Debug($"{Cyl_VtCamCentering} Off");
                    Wait(10000, () => Cyl_VtCamCentering.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.CamCenteringOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }

                    Log.Debug($"{Cyl_VtCamCentering} Off Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveUpAgain:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.SpongeDetachMoveUpAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_ReadyStep.End:
                    isFirstCycleRemoveSponge = true;
                    _isResetErrorPreAlginVacOn = true;
                    Log.Debug("Ready End");
                    Sequence = ESequence.Stop;
                    Step.RunStep++;
                    break;
                default:
                    Wait(20);
                    break;
            }
        }

        private void Sequence_AutoRun()
        {
            switch ((ESpongeDetach_AutoRunStep)Step.RunStep)
            {
                case ESpongeDetach_AutoRunStep.Start:
                    if (_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }

                    Log.Debug("Sequence Sponge Detach AutoRun start");
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckPreAlginCamExist_VacOn:
                    PreaAlignVac(true);
                    Log.Debug("Prealign Vac On to Check Cam exist");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value);
                    if (_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep = (int)ESpongeDetach_AutoRunStep.CheckSpongeDetachDone;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckPreAlginCamExist:
                    if (In_VtCamPreAlginVacOn.Value == false && Cyl_SpongePickupMoverFwBw.IsBackward)
                    {
                        materialStatus.Clear();
                        isCameraExistOnPreAlignVac = false;
                        PreaAlignVac(false);
                        Log.Debug("PreAlign vacuum check has no camera. Clear PreAlign material status and sequence Tray Head Cam Place");
                        Sequence = ESequence.TrayHead_Cam_Place;
                        break;
                    }

                    materialStatus.Set();

                    isCameraExistOnPreAlignVac = true;

                    if (IsOnSpongeRemoveProcess && In_VtCamPreAlginVacOn.Value == true)
                    {
                        _isSpongeRemoveDone = false;
                        Sequence = ESequence.SpongeDetach_RemoveSponge;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckSpongeDetachDone:
                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.End:
                    Log.Debug("Sequence Sponge Remove");
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                default:
                    break;
            }

        }

        private uint retrySpongeGripCount { get; set; } = 1;
        private int _retrySpongGripper;
        private bool isFirstCycleRemoveSponge { get; set; }
        private bool isCameraExistOnPreAlignVac { get; set; }

        private void Sequence_SpongeDetachLoad()
        {
            switch ((ESpongeDetach_CamLoadStep)Step.RunStep)
            {
                case ESpongeDetach_CamLoadStep.Start:
                    Log.Debug("Sponge Detach Load Start");
                    IsOnSpongeRemoveProcess = false;
                    _isSpongeRemoveDone = false;
                    Step.RunStep++;
                    break;
                //case ESpongeDetach_CamLoadStep.SpongeGripper_Off:
                //    SpongeRemoverVacOn(false);
                //    Log.Debug("Sponge Gripper Off");
                //    Cyl_SpongeHoldGripper.Backward();
                //    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                //    Step.RunStep++;
                //    break;
                //case ESpongeDetach_CamLoadStep.SpongeGripper_Off_Wait:
                //    if (WaitTimeOutOccurred)
                //    {
                //        RaiseWarning(EWarning.CamSpongeDetach_GripOff_Fail);
                //        break;
                //    }

                //    Log.Debug("Sponge Gripper Done");
                //    Step.RunStep++;
                //    break;
                case ESpongeDetach_CamLoadStep.CamExistCheck_VacOn:
                    PreaAlignVac(true);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_VtCamPreAlginVacOn, false);
#endif
                    Wait(_globalRecipe.VacCheckWaitTime, () => (In_VtCamPreAlginVacOn.Value == false || _machineStatus.IsDryRunMode));
                    Log.Debug("Cam Exist Check");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.CamExistCheck_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CameraExist);
                        break;
                    }
                    Log.Debug("Camera Exist Check OK");
                    PreaAlignVac(false);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.CamCenteringOff:
                    Cyl_VtCamCenteringOn(false);
                    Wait(10000, () => Cyl_VtCamCentering.IsBackward);
                    Log.Debug($"{Cyl_VtCamCentering} Centering Off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.CamCenteringOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_VtCamCentering} Centering Off Done");
                    Step.RunStep++;
                    break;
                //case ESpongeDetach_CamLoadStep.SpongeDetach_Up:
                //    Cyl_SpongePickupUpDn(true);
                //    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                //    Step.RunStep++;
                //    break;
                //case ESpongeDetach_CamLoadStep.SpongeDetach_Up_Check:
                //    if (WaitTimeOutOccurred)
                //    {
                //        RaiseWarning(EWarning.CamSpongeDetach_MoveUp_Fail);
                //        break;
                //    }
                //    Step.RunStep++;
                //    break;
                case ESpongeDetach_CamLoadStep.SpongeDetach_Bw:
                    Cyl_SpongePickupFwBw(false);
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.SpongeDetach_Bw_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.RequestCamIn:
                    Log.Debug("Request Camera In");
                    FlagOut_SpongeDetachCamInRequest = true;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.CheckCamInComplete:
                    if (Flag_SpongeDetachCamInDone == false)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Camera In Done");
                    FlagOut_SpongeDetachCamInRequest = false;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamLoadStep.WaitTrayHeadZUpDone:
                    if (FlagIn_TrayHeadZUpDone)
                    {
                        Log.Debug("Tray Head Z Up Done Signal Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ESpongeDetach_CamLoadStep.End:
                    Log.Debug("Cam Centering End");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Log.Debug("Sequence Sponge Detach");
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                default:
                    break;
            }
        }

        private bool _isOnSpongeRemoveProcess;

        private bool IsOnSpongeRemoveProcess
        {
            get => _isOnSpongeRemoveProcess;
            set
            {
                _isOnSpongeRemoveProcess = value;
            }
        }
        private void Sequence_SpongeRemove()
        {
            switch ((ESpongeDetach_SpongeRemoveStep)Step.RunStep)
            {
                case ESpongeDetach_SpongeRemoveStep.Start:
                    Log.Debug("Sponge Remove Start");
                    retrySpongeGripCount = 1;
                    _retrySpongGripper = 0;
                    _countRetrySpongeRemove = 0;
                    FlagOut_SpongeRemoveDone = false;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.ConditionCheck:
                    if (_isSpongeRemoveDone)
                    {
                        Log.Debug("Sponge Remove Done -> Jump Step to Set_FlagOut_SpongeRemoveDone");
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.Set_FlagOut_SpongeRemoveDone;
                        break;
                    }
                    if (IsOnSpongeRemoveProcess == true)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.CenteringOff;
                        break;
                    }

                    if (isFirstCycleRemoveSponge && isCameraExistOnPreAlignVac)
                    {
                        Log.Debug("First Cycle -> Retry Centering");
                        isFirstCycleRemoveSponge = false;
                        isCameraExistOnPreAlignVac = false;
                        Step.RunStep++;
                        break;
                    }

                    if (_trayHeadRecipe.UsePreCentering == 1)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.Set_Status;
                        break;
                    }

                    Log.Debug("Wait Flipper in Safety Position");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.InterlockConditionCheck:
                    if ((_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw!.IsForward
                        && _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn!.IsBackward) == true)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Interlock with CamRotator check OK");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CenteringOn:
                    Cyl_VtCamCenteringOn(true);
                    Wait(10000, () => Cyl_VtCamCentering.IsForward);
                    Log.Debug($"{Cyl_VtCamCentering} Centering On");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CenteringOn_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_VtCamCenteringOn(false);

                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOn_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_VtCamCentering} Centering On Done");
                    Wait(300);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.PrealignVacOn:
                    Log.Debug("Prealign Vac On");
                    PreaAlignVac(true);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.Set_Status:
                    Log.Debug("Set Status Processing");
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CenteringOff:
                    FlagOut_FlipperInRequest = true;

                    if (Cyl_VtCamCentering.IsBackward)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.FlipperInRequest;
                        break;
                    }
                    Cyl_VtCamCenteringOn(false);
                    Wait(10000, () => Cyl_VtCamCentering.IsBackward);
                    Log.Debug($"{Cyl_VtCamCentering} Centering Off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CenteringOff_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }

                    Log.Debug($"{Cyl_VtCamCentering} Centering Off Done");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.PrealignVacOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }

                    Log.Debug($"PreAlign Vac On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.FlipperInRequest:
                    if (IsOnSpongeRemoveProcess || _trayHeadRecipe.UsePreCentering == 1)
                    {
                        FlagOut_FlipperInRequest = true;
                    }
                    FlagOut_FlipperWorkRequest = true;
                    IsOnSpongeRemoveProcess = true;
                    Log.Debug("Request Flipper In to Remove Sponge");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.WaitFlipperGripOnDoneSignal:
                    if (FlagIn_FlipperGripOnDone == true)
                    {
                        if (_materialStatusList.SpongeDetachEnable == false)
                        {
                            Wait(10);
                            break;
                        }
                        _isRetryGripOnOff = false;
                        FlagOut_FlipperWorkRequest = false;
                        FlagOut_FlipperInRequest = false;
                        Log.Debug("Flipper Grip On Done");
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;

                case ESpongeDetach_SpongeRemoveStep.SpongePosition_Check:
                    if (Cyl_SpongePickupMoverFwBw.IsForward && Cyl_SpongePickupMoverUpDn.IsBackward)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverVacOn;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverUnGrip_Blow:
                    Cyl_SpongeHoldGripper.Backward();
                    SpongeRemoverVacOn(false);
                    Log.Debug("Sponge Gipper Off and Blow ");
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverUnGrip_Blow_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Gripper Off Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveUp:
                    Cyl_SpongePickupUpDn(true);
                    Cyl_SpongeHoldGripper.Backward();
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SafetyConditionCheck:
                    if (!FlagIn_TrayHeadZUpDone)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Sponge Remover Safety condition check OK ");
                    Step.RunStep++;
                    break;

                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveIn:
                    Cyl_SpongePickupFwBw(true);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move In");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveIn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveFw_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move In Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOffBeforeDown:
                    if (Cyl_SpongeHoldGripper.IsBackward)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveDown;
                        break;
                    }

                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOffBeforeDown_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveDown:
                    Cyl_SpongePickupUpDn(false);
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverDownDone_Wait:
                    Wait(_flipperSpongeDetachRecipe.SpongeRemoverDownWait);
                    Log.Debug("Sponge pickup down done delay");
                    if (_flipperSpongeDetachRecipe.SpongeHeadFunction == 0) // Use Sponge Detect
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOn;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverVacOn:
                    SpongeRemoverVacOn(true);
                    Log.Debug("Sponge Hold Vac On");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_SpongeHoldVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverVacOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        _isSpongeNotExist = true;
                        //RaiseWarning((int)EWarning.SpongeRemover_VacOn_Fail);
                        //break;
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveUpAgain;
                        break;
                    }
                    Log.Debug("Sponge Hold Vac On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOn:
                    Cyl_SpongeHoldGripper.Forward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsForward);
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip On");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_SpongeHoldGripper.Backward();
                        _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper.Backward();
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOn_Fail);
                        break;
                    }

                    // TODO: May remove later
                    Wait(_recipeList.FlipperTapeDetachRecipe.SpongeGripperOnWait);
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOnDone_Wait:
                    //Wait(_flipperSpongeDetachRecipe.SpongeGripperOnWait);
                    if (retrySpongeGripCount >= (_flipperSpongeDetachRecipe.SpongeGripperGripCount))
                    {
                        retrySpongeGripCount = 1;
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveUpAgain;
                        Wait(_flipperSpongeDetachRecipe.SpongeRemoverUpWait);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOffToRetry:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Log.Debug("Sponge Hold grip off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOffToRetry_Check:
                    retrySpongeGripCount++;
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }

                    Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOn;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveUpAgain:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveUpAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.Set_FlagOut_SpongeRemoveDone:
                    if (In_SpongeHoldVacOn.Value == false && _isSpongeNotExist == false && _devRecipe.UseRetryRemoveSponge == true && _flipperSpongeDetachRecipe.SpongeHeadFunction == 1 && _machineStatus.IsDryRunMode == false)
                    {
                        if (_countRetrySpongeRemove > 1)
                        {
                            _countRetrySpongeRemove = 0;
                            Log.Debug($"Set FlagOut_SpongeRemoveDone");
                            _isSpongeRemoveDone = true;
                            IsOnSpongeRemoveProcess = false;
                            FlagOut_SpongeRemoveDone = true;
                            Step.RunStep++;
                            break;
                        }
                        _countRetrySpongeRemove++;
                        PreaAlignVac(true);
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOffBeforeDown;
                        break;
                    }
                    _countRetrySpongeRemove = 0;
                    Log.Debug($"Set FlagOut_SpongeRemoveDone");
                    _isSpongeRemoveDone = true;
                    IsOnSpongeRemoveProcess = false;
                    FlagOut_SpongeRemoveDone = true;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.WaitFlipperGripOffToCheckCamExist:
                    if (_devRecipe.UseCamPrealignCheckAfterRemoveSponge == false)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.PreAlignVac_Off;
                        break;
                    }
                    if (FlagIn_FlipperGripperOffDone == false)
                    {
                        Wait(10);
                        break;
                    }
                    Log.Debug("Flipper Grip Off Signal Received");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CheckCameraPrealignExist:
                    if (In_VtCamPreAlginVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        _isResetErrorPreAlginVacOn = false;
                        _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper.Backward();
                        Cyl_SpongeHoldGripper.Backward();
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.PreAlignVac_Off:
                    PreaAlignVac(false);
                    Wait(_recipeList.GlobalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value == false);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.PreAlignVac_Off_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PreAlignVacOff_Fail);
                        break;
                    }

                    materialStatus.Clear();
                    Log.Debug("PreAlign material status cleared after vacuum off");

                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveOut:
                    Cyl_SpongePickupFwBw(false);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverMoveOut_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                        break;
                    }

                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;
                    //Vac_TrashSuctionOn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverDoneSignal_Set:
                    Log.Debug("Sponge Remove Out Done");
                    PreaAlignVac(false);
                    _isSpongeNotExist = false;
                    if (FlagIn_CamOutDone == true)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep.SpongeRemoverDownAgain;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverDownAgain:
                    Cyl_SpongePickupUpDn(false);
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverDownAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                        break;
                    }
                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;

                    TrashSuctionOn(true);

                    Out_SpongeHoldVacOn.Value = false;
                    Out_SpongeHoldVacOff.Value = true;
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeHoldVacCheck:
                    if (_spongeVacCheckOK == false && _devRecipe.UseSpongeVacCheck)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOn_Fail);
                        break;
                    }
                    _spongeVacCheckOK = false;
                    Log.Debug("Sponge Hold Vac Check OK");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOff:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Log.Debug("Sponge Hold grip off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverGripOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Wait(200);
                    Log.Debug("Sponge Gripper Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverVacOff:
                    if (_flipperSpongeDetachRecipe.SpongeHeadFunction == 1) // Use Sponge Vaccum
                    {
                        //SpongeRemoverVacOn(false);
                        Log.Debug("Sponge Hold Vac Off");
                        Wait(_globalRecipe.VacCheckWaitTime, () => In_SpongeHoldVacOn.Value == false);
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.SpongeRemoverVacOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOff_Fail);
                        break;
                    }
                    //EDM
                    strEDMPara[0] = "3,";
                    strEDMPara[1] = "02,";
                    strEDMPara[2] = ",";
                    strEDMPara[3] = "TOPM38,";
                    //_edmLogger.AddEDMLog("9020", "00000002", strEDMPara);
                    //
                    _isRetryGripOnOff = true;
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    Log.Debug("Sponge Hold Vac Off Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.TrashSuctionDelay:
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.CheckCameraOutDone:
                    if (FlagIn_CamOutDone == false)
                    {
                        Wait(10);
                        break;
                    }

                    _isSpongeRemoveDone = false;
                    FlagOut_SpongeRemoveDone = false;
                    Log.Debug("Camera Out Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.Wait_GripperRemoveSpongeDone:
                    //if (_isRetryGripOnOff)
                    //{
                    //    Wait(20);
                    //    break;
                    //}

                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.TrayHead_Cam_Place;
                    break;
                default:
                    break;
            }
        }

        //=============================================================================================================================================
        private void Sequence_AutoRun_OriginalVer()
        {
            switch ((ESpongeDetach_AutoRunStep)Step.RunStep)
            {
                case ESpongeDetach_AutoRunStep.Start:
                    if (_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }

                    Log.Debug("Sequence Sponge Detach AutoRun start");
                    ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckPreAlginCamExist_VacOn:
                    PreaAlignVac(true);
                    Log.Debug("Prealign Vac On to Check Cam exist");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value);
                    if (_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep = (int)ESpongeDetach_AutoRunStep.CheckSpongeDetachDone;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckPreAlginCamExist:
                    if (IsOnSpongeRemoveProcess && In_VtCamPreAlginVacOn.Value == true
                        || Cyl_SpongePickupMoverFwBw.IsForward
                        || Cyl_VtCamCentering.IsForward)
                    {
                        Sequence = ESequence.SpongeDetach_RemoveSponge;
                        break;
                    }
                    if (In_VtCamPreAlginVacOn.Value == false && Cyl_SpongePickupMoverFwBw.IsBackward)
                    {
                        PreaAlignVac(false);
                        Log.Debug("Sequence Tray Head Cam Place");
                        Sequence = ESequence.TrayHead_Cam_Place;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESpongeDetach_AutoRunStep.CheckSpongeDetachDone:
                    //if (In_SpongeHoldVacOn.Value == true || Cyl_SpongePickupMoverFwBw.IsForward)
                    //{
                    //    Log.Info("Sequence Remove Sponge");
                    //    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    //    break;
                    //}

                    ////if (_isSpongeRemoveDone == true)
                    ////{
                    ////    Log.Info("Sequence Flipper Pick");
                    ////    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    ////    break;

                    ////}
                    Step.RunStep++;
                    break;

                case ESpongeDetach_AutoRunStep.End:
                    Log.Debug("Sequence Sponge Remove");
                    Sequence = ESequence.SpongeDetach_RemoveSponge;
                    break;
                default:
                    break;
            }

        }

        private void Sequence_SpongeRemove_OriginalVer()
        {
            switch ((ESpongeDetach_SpongeRemoveStep_OriginalVer)Step.RunStep)
            {
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.Start:
                    Log.Debug("Sponge Remove Start");
                    retrySpongeGripCount = 1;
                    _countRetrySpongeRemove = 0;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.ConditionCheck:
                    if (In_SpongeHoldVacOn.Value == true)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOn;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.PreaAlignCentering:
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    Cyl_VtCamCenteringOn(true);
                    Wait(4000, () => Cyl_VtCamCentering.IsForward);
                    Log.Debug($"{Cyl_VtCamCentering} Centering On");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.PreaAlignCentering_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_VtCamCenteringOn(false);

                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOn_Fail);
                        break;
                    }
                    Wait(200);
                    Log.Debug($"{Cyl_VtCamCentering} Centering On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveUp:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(3000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SafetyConditionCheck:
                    if (!FlagIn_TrayHeadZUpDone)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Sponge Remover Safety condition check OK ");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveIn:
                    //TODO: Check FrontCase Move On
                    //if (!FlagIn_ReadyFrontCaseLoadOn)
                    //{
                    //    break;
                    //}
                    Cyl_SpongePickupFwBw(true);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move In");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveIn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveFw_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move In Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOffBeforeDown:
                    if (Cyl_SpongeHoldGripper.IsBackward)
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveDown;
                        break;
                    }

                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOffBeforeDown_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveDown:
                    Cyl_SpongePickupUpDn(false);
                    Wait(3000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverDownDone_Wait:
                    Wait(_flipperSpongeDetachRecipe.SpongeRemoverDownWait);
                    Log.Debug("Sponge pickup down done delay");
                    if (_flipperSpongeDetachRecipe.SpongeHeadFunction == 0) // Use Sponge Detect
                    {
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOn;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverVacOn:
                    SpongeRemoverVacOn(true);
                    Log.Debug("Sponge Hold Vac On");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_SpongeHoldVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverVacOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        //RaiseWarning((int)EWarning.SpongeRemover_VacOn_Fail);
                        //break;
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveUpAgain;
                        break;
                    }
                    Log.Debug("Sponge Hold Vac On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOn:
                    Cyl_SpongeHoldGripper.Forward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsForward);
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip On");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_SpongeHoldGripper.Backward();
                        _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper.Backward();
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOn_Fail);
                        break;
                    }
                    Wait(200);
                    Log.Debug($"{Cyl_SpongeHoldGripper} Grip On Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOnDone_Wait:
                    //Wait(_flipperSpongeDetachRecipe.SpongeGripperOnWait);
                    if (retrySpongeGripCount >= (_flipperSpongeDetachRecipe.SpongeGripperGripCount))
                    {
                        retrySpongeGripCount = 1;
                        Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveUpAgain;
                        Wait(_flipperSpongeDetachRecipe.SpongeRemoverUpWait);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOffToRetry:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Log.Debug("Sponge Hold grip off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOffToRetry_Check:
                    retrySpongeGripCount++;
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }

                    Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOn;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveUpAgain:
                    Cyl_SpongePickupUpDn(true);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up");
                    Wait(10000, () => Cyl_SpongePickupMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveUpAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveUp_Fail);
                        break;
                    }
                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Up Done");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveOut:
                    Cyl_SpongePickupFwBw(false);
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out");
                    Wait(10000, () => Cyl_SpongePickupMoverFwBw.IsBackward);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverMoveOut_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveBw_Fail);
                        break;
                    }
                    //if (_isSpongeRemoveDone == true)
                    //{
                    //    Step.RunStep = (int)ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverDoneSignal_Set;
                    //    break;
                    //}
                    //FlagOut_CamOutRequest = true;
                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;
                    Log.Debug($"{Cyl_SpongePickupMoverFwBw} Move Out Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverDownAgain:
                    Cyl_SpongePickupUpDn(false);
                    Wait(3000, () => Cyl_SpongePickupMoverUpDn.IsBackward);
                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverDownAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_MoveDown_Fail);
                        break;
                    }
                    _spongeVacCheckOK |= In_SpongeHoldVacOn.Value;
                    FlagOut_FlipperInRequest = true; // FlagOut_CamOutRequest
                    SpongeRemoverVacOn(false);
                    Wait(200);

                    TrashSuctionOn(true);

                    Log.Debug($"{Cyl_SpongePickupMoverUpDn} Move Down Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeHoldVacCheck:
                    if (_spongeVacCheckOK == false && _devRecipe.UseSpongeVacCheck)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOn_Fail);
                        break;
                    }
                    _spongeVacCheckOK = false;
                    Log.Debug("Sponge Hold Vac Check OK");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOff:
                    Cyl_SpongeHoldGripper.Backward();
                    Wait(10000, () => Cyl_SpongeHoldGripper.IsBackward);
                    Log.Debug("Sponge Hold grip off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverGripOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_GripOff_Fail);
                        break;
                    }
                    Wait(200);
                    Log.Debug("Sponge Gripper Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverVacOff:
                    if (_flipperSpongeDetachRecipe.SpongeHeadFunction == 1) // Use Sponge Vaccum
                    {
                        SpongeRemoverVacOn(false);
                        Log.Debug("Sponge Hold Vac Off");
                        Wait(_globalRecipe.VacCheckWaitTime, () => In_SpongeHoldVacOn.Value == false);
                    }
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.SpongeRemoverVacOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_SpongeVacOff_Fail);
                        break;
                    }
                    //EDM
                    strEDMPara[0] = "3,";
                    strEDMPara[1] = "02,";
                    strEDMPara[2] = ",";
                    strEDMPara[3] = "TOPM38,";
                    //_edmLogger.AddEDMLog("9020", "00000002", strEDMPara);
                    //
                    _isRetryGripOnOff = true;
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    Log.Debug("Sponge Hold Vac Off Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.TrashSuctionDelay:
                    //Wait(500);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_SpongeRemoveStep_OriginalVer.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.Flipper_Pick;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_FlipperCamPick()
        {
            switch ((ESpongeDetach_CamUnloadStep)Step.RunStep)
            {
                case ESpongeDetach_CamUnloadStep.Start:
                    Log.Debug("Cam Unload From Prealign Block Start");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCenteringOff:
                    Cyl_VtCamCentering.Backward();
                    Wait(4000, () => Cyl_VtCamCentering.IsBackward);
                    Log.Debug("Cam Centering Off");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCenteringOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Log.Debug("Cam Centering Off Check Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCemteringRetryOn:
                    if (_isRetryCentering == false)
                    {
                        Step.RunStep = (int)ESpongeDetach_CamUnloadStep.PrealignVacCheck;
                        break;
                    }
                    Cyl_VtCamCenteringOn(true);
                    Wait(4000, () => Cyl_VtCamCentering.IsForward);
                    Log.Debug("Cam Centering On (Retry)");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCemteringRetryOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOn_Fail);
                        break;
                    }
                    Log.Debug("Cam Centering On (Retry) Check Done ");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCemteringRetryOff:
                    Cyl_VtCamCentering.Backward();
                    Wait(4000, () => Cyl_VtCamCentering.IsBackward);
                    Log.Debug("Cam Centering Off (Retry)");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamCemteringRetryOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Log.Debug("Cam Centering Off (Retry) Check Done");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.PrealignVacCheck:
                    if (In_VtCamPreAlginVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PrealignVacOn_Fail);
                        break;
                    }
                    Log.Debug("Prealign Vac Check OK");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.RequestCameraOut:
                    FlagOut_FlipperWorkRequest = true;
                    Log.Debug("Request Camera Out");
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamPrealignVacOff:
                    if (!FlagIn_FlipperGripOnDone)
                    {
                        break;
                    }
                    PreaAlignVac(false);
                    Log.Debug("Cam PreAlign Vac Off");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPreAlginVacOn.Value == false);
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CamPrealignVacOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PreAlignVacOff_Fail);
                        break;
                    }
                    materialStatus.Clear();
                    Log.Debug("Cam Prealign Vac Off Done; PreAlign material status cleared");
                    FlagOut_SpongeRemoveDone = true;
                    Step.RunStep++;
                    break;
                case ESpongeDetach_CamUnloadStep.CheckCameraOutDone:
                    if (FlagIn_CamOutDone)
                    {
                        _isSpongeRemoveDone = false;

                        FlagOut_SpongeRemoveDone = false;
                        FlagOut_FlipperInRequest = false; // FlagOut_CamOutRequest
                        FlagOut_FlipperWorkRequest = false;

                        Log.Debug("Camera Out Done");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case ESpongeDetach_CamUnloadStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.TrayHead_Cam_Place;
                    break;
            }
        }

        private void PreaAlignVac(bool bOnOff)
        {
            Out_VtCamPreAlignVacOn.Value = bOnOff;
            Out_VtCamPrealignVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(500).ContinueWith(t =>
                {
                    Out_VtCamPrealignVacOff.Value = false;
                });
            }
        }

        private void SpongeRemoverVacOn(bool bOnOff)
        {
            Out_SpongeHoldVacOn.Value = bOnOff;
            Out_SpongeHoldVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(700).ContinueWith(t =>
                {
                    Out_SpongeHoldVacOff.Value = false;
                });
            }
        }

        private void Cyl_VtCamCenteringOn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_VtCamCentering.Forward();
            }
            else
            {
                Cyl_VtCamCentering.Backward();
            }
        }

        private void Cyl_SpongePickupFwBw(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_SpongePickupMoverFwBw.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupFw, true);
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupBw, false);
#endif
            }
            else
            {
                Cyl_SpongePickupMoverFwBw.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupFw, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupBw, true);
#endif
            }
        }

        private void Cyl_SpongePickupUpDn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_SpongePickupMoverUpDn.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupDown, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupUp, true);
#endif
            }
            else
            {
                Cyl_SpongePickupMoverUpDn.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupUp, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.SpongePickupDown, true);
#endif
            }
        }

        private void TrashSuctionOn(bool bOnOff)
        {
            Out_TrashSuctionOn.Value = bOnOff;
            _machineStatus.Sponge_TrashSuctionOn = bOnOff;
            if (bOnOff)
            {
                Task.Delay((int)(_globalRecipe.TrashSuctionOnTime * 1000)).ContinueWith(t =>
                {
                    _machineStatus.Sponge_TrashSuctionOn = false;
                    if (_machineStatus.Vinyl_TrashSuctionOn) return;

                    TrashSuctionOn(false);
                });
            }
        }

        private void StopRun()
        {
            ((MappableOutputDevice<ESpongeDetachOutput>)_tapeDetachOutput).ClearOutputs();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
        }
        #endregion

        #region Constructors
        public SpongeDetachProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            EDMLogger edmLogger,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            VaccumList vaccumList,
            [FromKeyedServices("SpongeDetachInput")] IDInputDevice<ESpongeDetachInput> tapeDetachInput,
            [FromKeyedServices("SpongeDetachOutput")] IDOutputDevice<ESpongeDetachOutput> tapeDetachOutput,
            DevRecipe devRecipe,
            TrayHeadRecipe trayHeadRecipe)
        {
            _edmLogger = edmLogger;
            _machineStatus = machineStatus;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _materialStatusList = materialStatusList;
            _vaccumList = vaccumList;
            _tapeDetachInput = tapeDetachInput;
            _tapeDetachOutput = tapeDetachOutput;
            _devRecipe = devRecipe;
        }
        #endregion

        #region Privates
        private bool _isRetryGripOnOff { get; set; } = false;
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _tapeDetachInput;
        private readonly IDOutputDevice _tapeDetachOutput;
        private readonly DevRecipe _devRecipe;
        private TrayHeadRecipe _trayHeadRecipe => _recipeList.TrayHeadRecipe;
        private readonly VaccumList _vaccumList;
        private bool _isRetryCentering => _flipperSpongeDetachRecipe.RetryCentering == 1;

        private bool _isSpongeRemoveDone { get; set; }
        private bool _isSpongeNotExist;
        private int _countRetrySpongeRemove;
        private MaterialStatusList _materialStatusList;
        string[] strEDMPara = new string[4];
        private EDMLogger _edmLogger;
        private FlipperTapeDetachRecipe _flipperSpongeDetachRecipe => _recipeList.FlipperTapeDetachRecipe;

        private bool IsUseSpongeCheck => _flipperSpongeDetachRecipe.SpongeDetect == 1;
        private bool _isResetErrorPreAlginVacOn = false;
        private bool _spongeVacCheckOK = false;

        #endregion
    }
}
