using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.Process
{
    public class SetConveyerDetachProcess : ProcessBase<ESequence>
    {
        private ECVLine line => Name == EProcess.FrontSetCVDetach.ToString() ? ECVLine.Front : ECVLine.Rear;

        #region Inputs
        private IDInput In_CvStartDetect => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvStart
                                                                      : _devices.Inputs.RearDetachCvStart;
        private IDInput In_CvEndDetect => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvEnd
                                                                    : _devices.Inputs.RearDetachCvEnd;
        private IDInput In_SetReverseDetect => _devices.Inputs.SetReverseDetect;

        private IDInput In_LoadCVEndDetect => line == ECVLine.Front ? _devices.Inputs.FrontLoadCvEnd
                                                                       : _devices.Inputs.RearLoadCvEnd;
        private IDInput In_StopperUp => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvStopperUp
                                                                 : _devices.Inputs.RearDetachCvStopperUp;
        private IDInput In_AlignOff => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvCenteringOff
                                                                : _devices.Inputs.RearDetachCvCenteringOff;
        private IDInput In_AlignOnNG => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvCenteringNG
                                                                : _devices.Inputs.RearDetachCvCenteringNG;
        #endregion

        #region Outputs
        private IDOutput Out_DetachCvOn => line == ECVLine.Front ? _devices.Outputs.FrontDetachCvOn
                                                                    : _devices.Outputs.RearDetachCvOn;
        private IDOutput Out_SetAlginOn => line == ECVLine.Front ? _devices.Outputs.FrontDetachCvCenteringOn
                                                                    : _devices.Outputs.RearDetachCvCenteringOn;
        private IDOutput Out_StopperUp => line == ECVLine.Front ? _devices.Outputs.FrontDetachCvStopperUp
                                                                    : _devices.Outputs.RearDetachCvStopperUp;
        #endregion

        #region Cylinders
        private ICylinder Cyl_Stopper => line == ECVLine.Front ? _devices.Cylinders.SetCV_FrontDetachStopperUpDn
                                                                             : _devices.Cylinders.SetCV_RearDetachStopperUpDn;
        private ICylinder Cyl_Align => line == ECVLine.Front ? _devices.Cylinders.SetCV_FrontDetachCentering
                                                                           : _devices.Cylinders.SetCV_RearDetachCentering;
        #endregion

        #region CVs
        private IConveyor Cv_SetFilmDetach => line == ECVLine.Front ? _devices.CVs.FrontSetWorkCV_SetFilmDetach
                                                                  : _devices.CVs.RearSetWorkCV_SetFilmDetach;
        #endregion

        #region Motions
        #endregion

        #region Flags
        // Inputs
        private bool FlagIn_FilmDetachDone => line == ECVLine.Front ? _frontCvSetFilmDetachInput[(int)EFrontCvFilmDetachInput.FILM_DETACH_DONE]
                                                                        : _rearCvSetFilmDetachInput[(int)ERearCvFilmDetachInput.FILM_DETACH_DONE];
        private bool FlagIn_AssembleLoadRequest => line == ECVLine.Front ? _frontCvSetFilmDetachInput[(int)EFrontCvFilmDetachInput.FRONT_ASSEMBLE_LOAD_REQUEST]
                                                                           : _rearCvSetFilmDetachInput[(int)ERearCvFilmDetachInput.REAR_ASSEMBLE_LOAD_REQUEST];

        //Outputs
        private bool FlagOut_DetachLoadRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvSetFilmDetachOutput[(int)EFrontCvFilmDetachOutput.FRONT_DETACH_LOAD_REQUEST] = value;
                else
                    _rearCvSetFilmDetachOutput[(int)ERearCvFilmDetachOutput.REAR_DETACH_LOAD_REQUEST] = value;
            }
        }
        private bool FlagOut_DetachLoadDone
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvSetFilmDetachOutput[(int)EFrontCvFilmDetachOutput.FRONT_DETACH_LOAD_DONE] = value;
                else
                    _rearCvSetFilmDetachOutput[(int)ERearCvFilmDetachOutput.REAR_DETACH_LOAD_DONE] = value;
            }
        }
        private bool FlagOut_FilmDetachRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvSetFilmDetachOutput[(int)EFrontCvFilmDetachOutput.FRONT_FILM_DETACH_REQUEST] = value;
                else
                    _rearCvSetFilmDetachOutput[(int)ERearCvFilmDetachOutput.REAR_FILM_DETACH_REQUEST] = value;
            }
        }

        private bool FlagOut_FilmDetachStartWorkRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvSetFilmDetachOutput[(int)EFrontCvFilmDetachOutput.FRONT_FILM_DETACH_START_WORK_REQUEST] = value;
                else
                    _rearCvSetFilmDetachOutput[(int)ERearCvFilmDetachOutput.REAR_FILM_DETACH_START_WORK_REQUEST] = value;
            }
        }
        private MaterialStatus materialStatus => line == ECVLine.Front ? _materialStatusList.FrontSetDetachCvMaterialStatus
                                                                            : _materialStatusList.RearSetDetachCvMaterialStatus;

        private StopWatch processTackTime => line == ECVLine.Front ? _totalTackTime.FrontDetachStopwatch
                                                                            : _totalTackTime.RearDetachStopWatch;

        private bool requestLoadWithSetBetweenTwoCV = false;
        #endregion

        #region Override Methods
        public override bool PreProcess()
        {
            materialStatus.IsEditable = (_machineStatus.IsRunningProcessMode == false ? true : false);
            if (ProcessMode == EProcessMode.Run)
            {
                if (In_CvEndDetect.Value == true) materialStatus.Set();
                else materialStatus.Clear();
            }
            if (_machineStatus.IsRunningProcessMode == false)
            {
                //bool bMaterialToAlign = (materialStatus.Status == EMaterialStatus.Existing);
                //Cyl_AlignOn(bMaterialToAlign);
            }
            if (In_CvEndDetect.Value == false) materialStatus.Clear();
            return base.PreProcess();
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
        //public override bool ProcessOrigin()
        //{
        //    switch ((ESetCVFilmDetach_OriginStep)Step.OriginStep)
        //    {
        //        case ESetCVFilmDetach_OriginStep.Start:
        //            Log.Debug("SetCVFilmDetachProcess Origin Start");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.CV_Stop:
        //            Cv_SetFilmDetach.Stop();
        //            Log.Debug($"{Cv_SetFilmDetach} Stopped");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.Cyl_Align_Off:
        //            Cyl_AlignOn(false);
        //            Log.Debug($"{Cyl_Align} Align Off");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_Align.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.Cyl_Align_Off_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.SetCVDetach_AlignOff_Fail);
        //                break;
        //            }
        //            Log.Debug($"{Cyl_Align} Align Off Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.Cyl_Stopper_Down:
        //            Cyl_StopperOn(false);
        //            Log.Debug($"{Cyl_Stopper} Stopper Down");
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_Stopper.IsBackward);
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.Cyl_Stopper_Down_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.SetCVOut_StopperDown_Fail);
        //                break;
        //            }
        //            Log.Debug($"{Cyl_Stopper} Stopper Down Done");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVFilmDetach_OriginStep.End:
        //            Log.Debug("SetCVFilmDetachProcess Origin End.");
        //            ProcessStatus = EProcessStatus.OriginDone;
        //            Step.OriginStep++;
        //            break;
        //        default:
        //            Wait(10);
        //            break;
        //    }
        //    return true;
        //}

        public override bool ProcessToStop()
        {
            if (ProcessStatus == EProcessStatus.ToStopDone)
            {
                Thread.Sleep(50);
                return true;
            }

            StopRun();
            return base.ProcessToStop();
        }

        public override bool ProcessToRun()
        {
            switch ((ESetCVFilmDetach_ToRunStep)Step.ToRunStep)
            {
                case ESetCVFilmDetach_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ESetCVFilmDetach_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ESetCVFilmDetach_ToRunStep.InternalInOutSignal_Reset:
                    if (line == ECVLine.Front)
                    {
                        ((MappableOutputDevice<EFrontCvFilmDetachOutput>)_frontCvSetFilmDetachOutput).ClearOutputs();
                    }
                    else
                    {
                        ((MappableOutputDevice<ERearCvFilmDetachOutput>)_rearCvSetFilmDetachOutput).ClearOutputs();
                    }
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ESetCVFilmDetach_ToRunStep.EndSensorSetDetect:
                    if (In_CvEndDetect.Value == true)
                    {
                        Log.Debug("Set End Sensor Detect");
                        Step.ToRunStep++;
                        break;
                    }
                    Step.ToRunStep = (int)ESetCVFilmDetach_ToRunStep.End;
                    break;
                case ESetCVFilmDetach_ToRunStep.EndSensorSetConfirm:
                    //if (_machineStatus.IsCVConditionConfirm == true)
                    //{
                    //    Cyl_StopperOn(true);
                    //    Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_Stopper.IsForward);
                    //    Step.ToRunStep++;
                    //    break;
                    //}
                    Step.ToRunStep++;
                    break;
                case ESetCVFilmDetach_ToRunStep.End:
                    Log.Debug("To Run End.");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }
            return true;
        }
        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    break;
                case ESequence.AutoRun:
                    Sequence_AutoRun();
                    break;
                case ESequence.CVDetach_Load:
                    Sequence_CVDetach_Load();
                    break;
                case ESequence.Detach_FilmDetach:
                    Sequence_Detach_FilmDetach();
                    break;
                case ESequence.CVAssemble_Load:
                    Sequence_CVAssemble_Load();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }

            return true;
        }

        public override string ToString()
        {
            return line == ECVLine.Front ? EProcess.FrontSetCVDetach.GetDescription()
                                             : EProcess.RearSetCVDetach.GetDescription();
        }
        #endregion

        #region Private Methods
        private void Sequence_Ready()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            if (line == ECVLine.Front)
            {
                ((MappableOutputDevice<EFrontCvFilmDetachOutput>)_frontCvSetFilmDetachOutput).ClearOutputs();

            }
            else
            {
                ((MappableOutputDevice<ERearCvFilmDetachOutput>)_rearCvSetFilmDetachOutput).ClearOutputs();
            }
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }
        private void Sequence_AutoRun()
        {

            switch ((ESetCVFilmDetach_AutoRunStep)Step.RunStep)
            {
                case ESetCVFilmDetach_AutoRunStep.Start:
                    Log.Debug("Set CV film detach AutoRun start.");

                    Cv_SetFilmDetach.Stop();
                    Cyl_Align.Backward();

                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_AutoRunStep.ConditionCheck1:
                    if (materialStatus.ProcessStatus == EMaterialProcessStatus.Done)
                    {
                        Sequence = ESequence.CVAssemble_Load;
                        break;
                    }

                    if (In_CvStartDetect.Value == false && In_CvEndDetect.Value == false)
                    {
                        Step.RunStep = (int)ESetCVFilmDetach_AutoRunStep.StopperUp;
                        break;
                    }

                    Step.RunStep = (int)ESetCVFilmDetach_AutoRunStep.ConditionCheck2;
                    break;
                case ESetCVFilmDetach_AutoRunStep.StopperUp:
                    Cyl_StopperOn(true);
                    Wait(10000, () => Cyl_Stopper.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_AutoRunStep.CV_Run1:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_StopperUp_Fail
                                                                : EWarning.RearDetachCV_StopperUp_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Cv_SetFilmDetach.Run();
                    Wait(10000, () => In_CvEndDetect.Value);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_CvEndDetect, true);
#endif
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_AutoRunStep.CV_Stop1:
                    if (In_CvEndDetect.Value)
                    {
                        Log.Debug("Set CV film detach set detect.");
                    }

                    Cv_SetFilmDetach.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_AutoRunStep.ConditionCheck2:
                    if (In_CvEndDetect.Value && In_CvStartDetect.Value)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_TwoOfSetExist
                                                                : EWarning.RearDetachCV_TwoOfSetExist;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    if (In_CvEndDetect.Value)
                    {
                        if(_machineStatus.IsByPassMode)
                        {
                            Sequence = ESequence.CVAssemble_Load;
                            break;
                        }

                        if (Cyl_Stopper.IsForward)
                        {
                            Sequence = ESequence.Detach_FilmDetach;
                            break;
                        }

                        if (materialStatus.ProcessStatus == EMaterialProcessStatus.Done)
                        {
                            Sequence = ESequence.CVAssemble_Load;
                            break;
                        }

                        // TODO: stopper is not UP Raise Warning - ask OP to remove set before continue
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_SetInStopperWarning
                                                                  : EWarning.RearDetachCV_SetInStopperWarning;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    if (In_CvStartDetect.Value)
                    {
                        Sequence = ESequence.CVDetach_Load;
                        break;
                    }

                    Sequence = ESequence.CVDetach_Load;
                    break;
                case ESetCVFilmDetach_AutoRunStep.End:
                    break;
                default:
                    break;
            }
        }

        private void Sequence_CVDetach_Load()
        {
            switch ((ESetCVFilmDetach_LoadStep)Step.RunStep)
            {
                case ESetCVFilmDetach_LoadStep.Start:
                    FlagOut_DetachLoadDone = false;
                    Log.Debug("Set CV Film Detach Load start.");
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.StopperUp:
                    Cyl_StopperOn(true);
                    Wait(10000, () => Cyl_Stopper.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.StopperUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_StopperUp_Fail
                                                                   : EWarning.RearDetachCV_StopperUp_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.CV_ConditionCheck:
                    if (In_CvStartDetect.Value == false)
                    {
                        Step.RunStep = (int)ESetCVFilmDetach_LoadStep.CVFilmDetach_LoadRequest_Set;
                        break;
                    }

                    // Set detect at current CV start & upstream CV end
                    if (In_LoadCVEndDetect.Value)
                    {
                        requestLoadWithSetBetweenTwoCV = true;
                        Step.RunStep = (int)ESetCVFilmDetach_LoadStep.CVFilmDetach_LoadRequest_Set;
                        break;
                    }

                    // Set detect at current CV start only
                    Step.RunStep = (int)ESetCVFilmDetach_LoadStep.CV_Run;
                    break;
                case ESetCVFilmDetach_LoadStep.CVFilmDetach_LoadRequest_Set:
                    FlagOut_DetachLoadRequest = true;
                    if (requestLoadWithSetBetweenTwoCV)
                    {
                        Wait(100); // Make sure upstream CV receive the load request signal
                    }

                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.CV_StartDetect_Wait:
                    if (In_CvStartDetect.Value || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Set CV film detach start detect.");
                        Cv_SetFilmDetach.Run();
                        Wait(400);

                        FlagOut_FilmDetachRequest = true;
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case ESetCVFilmDetach_LoadStep.CV_Run:
                    FlagOut_DetachLoadRequest = false;
                    Log.Debug("Set CV film detach run.");
                    Cv_SetFilmDetach.Run();
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => (In_CvEndDetect.Value || _machineStatus.IsDryRunMode));
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.CV_EndDetect_Wait:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        Log.Debug("Set CV film detach move timeout.");
                        Cv_SetFilmDetach.Stop();

                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_SetLoad_Timeout
                                                                  : EWarning.RearDetachCV_SetLoad_Timeout;

                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Wait(_setCVRecipe.EndDetachCvStopWait); // Wait for stable sensor signal
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.CV_Stop:
                    FlagOut_DetachLoadDone = true;
                    Log.Debug("Set CV film detach stop.");
                    Cv_SetFilmDetach.Stop();
                    Step.RunStep++;
                    break;

                case ESetCVFilmDetach_LoadStep.CV_SetDetectCondition_Check:
                    if (In_CvEndDetect.Value && In_CvStartDetect.Value)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_TwoOfSetExist
                                                                  : EWarning.RearDetachCV_TwoOfSetExist;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ESetCVFilmDetach_LoadStep.Cylinder_AlignOn:
                    Log.Debug("Set CV film detach align moving forward.");
                    Cyl_AlignOn(true);
                    Wait(10000, () => (!Cyl_Align.IsBackward || _machineStatus.IsDryRunMode));
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.Cylinder_AlignOn_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Set CV film detach align fail.");
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_AlignOn_Fail
                                                                  : EWarning.RearDetachCV_AlignOn_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_LoadStep.End:
                    countData.Input++;
                    Log.Debug("Set CV film detach load done.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.Detach_FilmDetach;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_Detach_FilmDetach()
        {
            switch ((ESetCVFilmDetach_DetachStep)Step.RunStep)
            {
                case ESetCVFilmDetach_DetachStep.Start:
                    if(_machineStatus.IsByPassMode)
                    {
                        Step.RunStep = (int)ESetCVFilmDetach_DetachStep.Cylinder_AlignOff_Stopper_Down;
                        break;
                    }

                    Log.Debug("Set CV film detach start.");
                    Step.RunStep++;
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    break;
                case ESetCVFilmDetach_DetachStep.Cylinder_AlignOn:
                    FlagOut_FilmDetachRequest = true;
                    Cyl_AlignOn(true);
                    Log.Debug($"{Cyl_Align} Align start.");
                    Wait(10000, () => (!Cyl_Align.IsBackward || _machineStatus.IsDryRunMode));
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_DetachStep.Cylinder_AlignOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_AlignOn_Fail
                                                                  : EWarning.RearDetachCV_AlignOn_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    processTackTime.StartTiming();
                    Wait(500);
                    Log.Debug($"{Cyl_Align} Align done.");
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_DetachStep.FilmDetach_Request_Set:
                    FlagOut_FilmDetachStartWorkRequest = true;
                    Log.Debug("Set film detach request on.");
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_DetachStep.FilmDetach_Done_Check:
                    if (FlagIn_FilmDetachDone)
                    {
                        Log.Debug("Set film detach done signal received.");
                        FlagOut_FilmDetachRequest = false;
                        FlagOut_FilmDetachStartWorkRequest = false;
                        materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case ESetCVFilmDetach_DetachStep.Cylinder_AlignOff_Stopper_Down:
                    Log.Debug("Cylinder UnAlign , Stopper Down");
                    Cyl_AlignOn(false);
                    Cyl_StopperOn(false);

                    Wait(10000, () => ((Cyl_Align.IsBackward && Cyl_Stopper.IsBackward) || _machineStatus.IsDryRunMode));
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_DetachStep.Cylinder_AlignOff_Stopper_Down_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning;
                        if (Cyl_Align.IsBackward == false)
                        {
                            eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_AlignOn_Fail
                                                                  : EWarning.RearDetachCV_AlignOn_Fail;
                            RaiseWarning((int)eWarning);
                            break;
                        }

                        eWarning = line == ECVLine.Front ? EWarning.FrontDetachCV_StopperDown_Fail
                                                                    : EWarning.RearDetachCV_StopperDown_Fail;
                        RaiseWarning((int)eWarning);
                        break;

                    }

                    Log.Debug("Cylinder UnAlign , Stopper Down Done");
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_DetachStep.End:
                    Log.Debug("Set CV Film detach.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.CVAssemble_Load;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_CVAssemble_Load()
        {
            switch ((ESetCVFilmDetach_UnloadStep)Step.RunStep)
            {
                case ESetCVFilmDetach_UnloadStep.Start:
                    Log.Debug("Set CV film detach Unload start.");
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_UnloadStep.CVAssemble_Request_Wait:
                    if (FlagIn_AssembleLoadRequest)
                    {
                        Log.Debug("Set CV assemble load request received.");
                        Step.RunStep++;
                        break;
                    }
                    break;
                case ESetCVFilmDetach_UnloadStep.CV_SetUnloadStart:
                    Log.Debug("Set CV film detach unload start.");
                    Cv_SetFilmDetach.Run();
                    Wait(5000, () => In_CvEndDetect.Value == false);
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_UnloadStep.CVAssemble_SetReceive_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        if (_machineStatus.IsByPassMode && In_CvEndDetect.Value)
                        {
                            Log.Debug("Set CV film detach unload timeout ignored in bypass mode because the set is still on detach CV end. Restart unload wait.");
                            Cv_SetFilmDetach.Run();
                            Wait(5000, () => In_CvEndDetect.Value == false);
                            break;
                        }
                        RaiseWarning(line == ECVLine.Front ? (int)EWarning.FrontDetachCV_SetUnload_Timeout : (int)EWarning.RearDetachCV_SetUnload_Timeout);
                        break;
                    }

                    Wait(10000, () => FlagIn_AssembleLoadRequest == false || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_UnloadStep.CVAssemble_SetReceive_Check:
                    if (WaitTimeOutOccurred)
                    {
                        if (_machineStatus.IsByPassMode && FlagIn_AssembleLoadRequest)
                        {
                            Log.Debug("Set CV assemble load request reset timeout ignored in bypass mode. Restart request reset wait.");
                            Wait(10000, () => FlagIn_AssembleLoadRequest == false || _machineStatus.IsDryRunMode);
                            break;
                        }
                        RaiseWarning(line == ECVLine.Front ? (int)EWarning.FrontDetachCV_SetUnload_Timeout : (int)EWarning.RearDetachCV_SetUnload_Timeout);
                        break;
                    }

                    Log.Debug("Set CV assemble load request reset.");
                    Wait(_setCVRecipe.SetOutWorkAreaWait);
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_UnloadStep.CV_UnloadStop:
                    Cv_SetFilmDetach.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVFilmDetach_UnloadStep.End:
                    Log.Debug("Set CV film detach unload done.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.CVDetach_Load;
                    break;
                default:
                    break;
            }
        }

        private void Cyl_StopperOn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_Stopper.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_StopperUp, true);
#endif
            }
            else
            {
                Cyl_Stopper.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_StopperUp, false);
#endif
            }
        }

        private void Cyl_AlignOn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_Align.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_AlignOff, false);
                SimulationInputSetter.SetSimInput(In_AlignOnNG, false);
#endif
            }
            else
            {
                Cyl_Align.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_AlignOff, true);
                SimulationInputSetter.SetSimInput(In_AlignOnNG, false);
#endif
            }
        }
        private void StopRun()
        {
            Cv_SetFilmDetach.Stop();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
        }
        #endregion

        #region Constructors
        public SetConveyerDetachProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            TotalTackTime totalTackTime,
            [FromKeyedServices("GripperTimer")] ActionAssignableTimer gripperTimer,
            [FromKeyedServices("FrontCvFilmDetachInput")] IDInputDevice<EFrontCvFilmDetachInput> frontCvSetFilmDetachInput,
            [FromKeyedServices("RearCvFilmDetachInput")] IDInputDevice<ERearCvFilmDetachInput> rearCvSetFilmDetachInput,
            [FromKeyedServices("FrontCvFilmDetachOutput")] IDOutputDevice<EFrontCvFilmDetachOutput> frontCvSetFilmDetachOutput,
            [FromKeyedServices("RearCvFilmDetachOutput")] IDOutputDevice<ERearCvFilmDetachOutput> rearCvSetFilmDetachOutput,
            CWorkData workData)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _materialStatusList = materialStatusList;
            _totalTackTime = totalTackTime;
            _machineStatus = machineStatus;
            _frontCvSetFilmDetachInput = frontCvSetFilmDetachInput;
            _rearCvSetFilmDetachInput = rearCvSetFilmDetachInput;
            _frontCvSetFilmDetachOutput = frontCvSetFilmDetachOutput;
            _rearCvSetFilmDetachOutput = rearCvSetFilmDetachOutput;
            _workData = workData;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _frontCvSetFilmDetachInput;
        private readonly IDInputDevice _rearCvSetFilmDetachInput;
        private readonly IDOutputDevice _frontCvSetFilmDetachOutput;
        private readonly IDOutputDevice _rearCvSetFilmDetachOutput;
        private readonly MaterialStatusList _materialStatusList;
        private TotalTackTime _totalTackTime;
        private SetConveyorRecipe _setCVRecipe => _recipeList.SetConveyorRecipe;

        private CWorkData _workData;
        private ProductCount countData => line == ECVLine.Front ? _workData.CountData.FrontCountData
                                                                      : _workData.CountData.RearCountData;
        #endregion

        // Minh
        #region Add_Timer_To_Detach_Assemble 
        private void Add_Timer_To_Detach_top()
        {

        }
        private void Add_Timer_To_Detach_bottom()
        {

        }
        private void Add_Timer_To_Assemle_top()
        {

        }
        private void Add_Timer_To_Assemle_bottom()
        {

        }
        #endregion
    }
}
