using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FrontCameraAssembleEquipment.Process
{
    public class SetConveyerAssembleProcess : ProcessBase<ESequence>
    {
        private ECVLine line => Name == EProcess.FrontSetCVAssemble.ToString() ? ECVLine.Front : ECVLine.Rear;
        private bool IsOneConveyorFrontLine => _processConfig.MachineType == EMachineType.OneConveyor && line == ECVLine.Front;

        #region Inputs
        private IDInput In_CvStartDetect => line == ECVLine.Front ? _devices.Inputs.FrontAssembleCvStart
                                                                        : _devices.Inputs.RearAssembleCvStart;
        private IDInput In_CvEndDetect => line == ECVLine.Front ? _devices.Inputs.FrontAssembleCvEnd
                                                                      : _devices.Inputs.RearAssembleCvEnd;
        private IDInput In_StopperUp => line == ECVLine.Front ? _devices.Inputs.FrontAssembleCvStopperUp
                                                                           : _devices.Inputs.RearAssembleCvStopperUp;
        private IDInput In_AlignOnNG => line == ECVLine.Front ? _devices.Inputs.FrontAssembleCvCenteringNG
                                                                              : _devices.Inputs.RearAssembleCvCenteringNG;
        private IDInput In_AlignOff => line == ECVLine.Front ? _devices.Inputs.FrontAssembleCvCenteringOff
                                                                               : _devices.Inputs.RearAssembleCvCenteringOff;
        //Detach End Detect Sensor
        private IDInput In_DetachCVEndDetect => line == ECVLine.Front ? _devices.Inputs.FrontDetachCvEnd
                                                                    : _devices.Inputs.RearDetachCvEnd;
        //OutCv Start Detect Sensor
        private IDInput In_OutCVStartDetect => line == ECVLine.Front ? _devices.Inputs.FrontUnloadCvStart
                                                                    : _devices.Inputs.RearUnloadCvStart;
        #endregion

        #region Outputs
        private IDOutput Out_AssembleCvOn => line == ECVLine.Front ? _devices.Outputs.FrontAssembleCvOn
                                                                       : _devices.Outputs.RearAssembleCvOn;
        private IDOutput Out_AssembleCvStopperUp => line == ECVLine.Front ? _devices.Outputs.FrontAssembleCvStopperUp
                                                                              : _devices.Outputs.RearAssembleCvStopperUp;
        private IDOutput Out_AssembleCvCenteringOn => line == ECVLine.Front ? _devices.Outputs.FrontAssembleCvCenteringOn
                                                                                : _devices.Outputs.RearAssembleCvCenteringOn;
        #endregion

        #region Cylinders
        private ICylinder Cyl_Stopper => line == ECVLine.Front ? _devices.Cylinders.SetCV_FrontAssembleStopperUpDn
                                                                                    : _devices.Cylinders.SetCV_RearAssembleStopperUpDn;
        private ICylinder Cyl_Align => line == ECVLine.Front ? _devices.Cylinders.SetCV_FrontAssembleCentering
                                                                                  : _devices.Cylinders.SetCV_RearAssembleCentering;
        #endregion

        #region CVs
        private IConveyor Cv_SetCamAssemble => line == ECVLine.Front ? _devices.CVs.FrontSetWorkCV_SetCamAssemble
                                                        : _devices.CVs.RearSetWorkCV_SetCamAssemble;

        #endregion

        private EVisionCmd Cmd_Film_Inspection => line == ECVLine.Front ? EVisionCmd.CMD_FRONT_DETACH_SEARCH
                                                                            : EVisionCmd.CMD_REAR_DETACH_SEARCH;

        private EVisionCmd Cmd_Assemble_Inspection => line == ECVLine.Front ? EVisionCmd.CMD_FRONT_ASSEMBLE_SEARCH
                                                                                 : EVisionCmd.CMD_REAR_ASSEMBLE_SEARCH;

        #region Flags
        // Inputs
        private bool FlagIn_CamAssemblePlaceDone => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.CAM_ASSEMBLE_FRONT_WAIT_PUSH]
                                                                        : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.CAM_ASSEMBLE_REAR_WAIT_PUSH];
        private bool FlagIn_CamAssembleDone => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.CAM_ASSEMBLE_DONE]
                                                                        : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.CAM_ASSEMBLE_DONE];

        private bool FlagIn_SetOutRequest => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.FRONT_UNLOAD_REQUEST]
                                                                    : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.REAR_UNLOAD_REQUEST];
        private bool FlagIn_VisionInspectionRun => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.VISION_INSPECTION_RUN]
                                                                                : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.VISION_INSPECTION_RUN];
        private bool FlagIn_VisionFilmInspectionError => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.VISION_FRONT_FILM_INSPECTION_ERROR]
                                                                                      : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.VISION_REAR_FILM_INSPECTION_ERROR];
        private bool FlagIn_VisionAssembleInspectionError => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.VISION_FRONT_ASSEMBLE_INSPECTION_ERROR]
                                                                                           : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.VISION_REAR_ASSEMBLE_INSPECTION_ERROR];

        private bool FlagIn_CamHeadMoveAvoidToVision => line == ECVLine.Front ? _frontCvCamAssembleInput[(int)EFrontCvCamAssembleInput.CAM_ASSEMBLE_AVOID_TO_VISION]
                                                                                    : _rearCvCamAssembleInput[(int)ERearCvCamAssembleInput.CAM_ASSEMBLE_AVOID_TO_VISION];

        // Outputs
        private bool FlagOut_AssembleSetLoadRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvCamAssembleOutput[(int)EFrontCvCamAssembleOutput.FRONT_ASSEMBLE_LOAD_REQUEST] = value;
                else
                    _rearCvCamAssembleOutput[(int)ERearCvCamAssembleOutput.REAR_ASSEMBLE_LOAD_REQUEST] = value;
            }
        }

        private bool FlagOut_CamAssembleRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvCamAssembleOutput[(int)EFrontCvCamAssembleOutput.FRONT_CAM_ASSEMBLE_REQUEST] = value;
                else
                    _rearCvCamAssembleOutput[(int)ERearCvCamAssembleOutput.REAR_CAM_ASSEMBLE_REQUEST] = value;
            }
        }

        #endregion

        #region Material Status - Product Datas
        private MaterialStatus materialStatus => line == ECVLine.Front ? _materialStatusList.FrontSetAssembleCvMaterialStatus
                                                                            : _materialStatusList.RearSetAssembleCvMaterialStatus;

        //private StopWatch tackTimeWatch => line == ECVLine.Front ? _totalTackTime.FrontStopWatch
        //                                                                : _totalTackTime.RearStopWatch;

        private StopWatch processTackTime => line == ECVLine.Front ? _totalTackTime.FrontAssembleStopwatch
                                                                         : _totalTackTime.RearAssembleStopwatch;
        private StopWatch cycleTackTime => line == ECVLine.Front ? _totalTackTime.FrontCycleStopWatch
                                                                         : _totalTackTime.RearCycleStopWatch;
        private StopWatch totalTackTime => _totalTackTime.TotalStopWatch;

        private StopWatch TimeCheckConveyor => line == ECVLine.Front ? _totalTackTime.FrontAssembleStopwatch
                                                                         : _totalTackTime.RearAssembleStopwatch;

        private ProductCount countData => line == ECVLine.Front ? _workData.CountData.FrontCountData
                                                                      : _workData.CountData.RearCountData;

        #endregion

        #region Override Methods

        public override bool PreProcess()
        {
            // 1. UI material status update
            materialStatus.IsEditable = (_machineStatus.IsRunningProcessMode == false ? true : false);
            if (ProcessMode == EProcessMode.Run)
            {
                if (In_CvEndDetect.Value == true) materialStatus.Set();
                else materialStatus.Clear();
            }
            if (In_CvEndDetect.Value == false) materialStatus.Clear();

            // 2. Private process variable handle
            if (In_CvEndDetect.Value == false)
            {
                _isAssembleDone = false;
                _isPlaceDone = false;
            }

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
            switch ((ESetCVAssemble_ToRunStep)Step.ToRunStep)
            {
                case ESetCVAssemble_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    _startRunAgain = true; ;
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ESetCVAssemble_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ESetCVAssemble_ToRunStep.InternalInOutSignal_Reset:
                    if (line == ECVLine.Front)
                    {
                        ((MappableOutputDevice<EFrontCvCamAssembleOutput>)_frontCvCamAssembleOutput).ClearOutputs();
                    }
                    else
                    {
                        ((MappableOutputDevice<ERearCvCamAssembleOutput>)_rearCvCamAssembleOutput).ClearOutputs();
                    }
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ESetCVAssemble_ToRunStep.EndSensorSetDetect:
                    if (In_CvEndDetect.Value == true)
                    {
                        Log.Debug("Set End Sensor Detect");
                        Step.ToRunStep++;
                        break;
                    }
                    Step.ToRunStep = (int)ESetCVAssemble_ToRunStep.End;
                    break;
                case ESetCVAssemble_ToRunStep.EndSensorSetConfirm:
                    //if (_machineStatus.IsCVConditionConfirm == true)
                    //{
                    //    Cyl_StopperOn(true);
                    //    Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_Stopper.IsForward);
                    //    Step.ToRunStep++;
                    //    break;
                    //}
                    Step.ToRunStep++;
                    break;
                case ESetCVAssemble_ToRunStep.End:
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
                case ESequence.CVAssemble_Load:
                    Sequence_CoveyorAssemble_Load();
                    break;
                case ESequence.CamHead_Place:
                    Sequence_CamAssemble();
                    break;
                case ESequence.CVOut_Load:
                    Sequence_CoveyorAssemble_Unload();
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
            return line == ECVLine.Front ? EProcess.FrontSetCVAssemble.GetDescription()
                                             : EProcess.RearSetCVAssemble.GetDescription();
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
            _isOnAssembleProcess = false;
            _isAssembleDone = false;
            if (line == ECVLine.Front)
            {
                ((MappableOutputDevice<EFrontCvCamAssembleOutput>)_frontCvCamAssembleOutput).ClearOutputs();

            }
            else
            {
                ((MappableOutputDevice<ERearCvCamAssembleOutput>)_rearCvCamAssembleOutput).ClearOutputs();
            }
            materialStatus.CameraStatus = ECameraStatus.None;
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }
        private void Sequence_AutoRun()
        {
            switch ((ESetCVAssemble_AutoRunStep)Step.RunStep)
            {
                case ESetCVAssemble_AutoRunStep.Start:
                    Log.Debug("Set CV Assemble AutoRun start.");

                    cycleTackTime.RestartTime();

                    Cv_SetCamAssemble.Stop();
                    Cyl_Align.Backward();

                    cycleTackTime.Reset();
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AutoRunStep.ConditionCheck1:
                    if (ShouldMoveStopperDownBeforeOutLoad())
                    {
                        Sequence = ESequence.CamHead_Place;
                        break;
                    }
                    if (In_CvStartDetect.Value == false && In_CvEndDetect.Value == false)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AutoRunStep.StopperUp;
                        break;
                    }
                    if(In_CvEndDetect.Value == true && Cyl_Align.IsBackward == true && Cyl_Stopper.IsForward == true)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AutoRunStep.StopperUp;
                        break;
                    }
                    Step.RunStep = (int)ESetCVAssemble_AutoRunStep.ConditionCheck2;
                    break;
                case ESetCVAssemble_AutoRunStep.StopperUp:
                    Cyl_StopperOn(true);
                    Wait(10000, () => Cyl_Stopper.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AutoRunStep.CV_Run1:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_StopperUp_Fail
                                                                    : EWarning.RearAssembleCV_StopperUp_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Cv_SetCamAssemble.Run();
                    Wait(5000, () => In_CvEndDetect.Value || _machineStatus.IsDryRunMode);
                    Wait(1500); // Wait for stable sensor signal after CV move
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_CvEndDetect, true);
#endif
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AutoRunStep.CV_Stop1:
                    if (In_CvEndDetect.Value)
                    {
                        Log.Debug("Set CV Assemble set detect.");
                    }

                    Cv_SetCamAssemble.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AutoRunStep.ConditionCheck2:
                    if (In_CvEndDetect.Value)
                    {
                        if (_machineStatus.IsByPassMode)
                        {
                            Sequence = ShouldMoveStopperDownBeforeOutLoad()
                                ? ESequence.CamHead_Place
                                : ESequence.CVOut_Load;
                            break;
                        }

                        if (Cyl_Stopper.IsForward)
                        {
                            Sequence = ESequence.CamHead_Place;
                            break;
                        }

                        if (materialStatus.ProcessStatus == EMaterialProcessStatus.Done)
                        {
                            Sequence = ShouldMoveStopperDownBeforeOutLoad()
                                ? ESequence.CamHead_Place
                                : ESequence.CVOut_Load;
                            break;
                        }
                        
                        RaiseWarning(line == ECVLine.Front ? (int)EWarning.FrontAssembleCV_SetInStopperWarning :
                                                             (int)EWarning.RearAssembleCV_SetInStopperWarning);
                        break;
                    }

                    if (In_CvStartDetect.Value)
                    {
                        Sequence = ESequence.CVAssemble_Load;
                        break;
                    }

                    Sequence = ESequence.CVAssemble_Load;
                    break;
                case ESetCVAssemble_AutoRunStep.End:
                    break;
                default:
                    break;
            }
        }

        private bool requestLoadWithSetBetweenTwoCV = false;
        private bool ShouldMoveStopperDownBeforeOutLoad()
        {
            return _machineStatus.IsByPassMode
                && In_CvEndDetect.Value
                && Cyl_Stopper.IsBackward == false;
        }
        private void Sequence_CoveyorAssemble_Load()
        {
            switch ((ESetCVAssemble_LoadStep)Step.RunStep)
            {
                case ESetCVAssemble_LoadStep.Start:
                    Log.Debug("Set CV Cam Assemble Load start.");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.StopperUp:
                    Cyl_StopperOn(true);
                    Log.Debug($"{Cyl_Stopper} Up");
                    Wait(10000, () => Cyl_Stopper.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.StopperUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_StopperUp_Fail
                                                                    : EWarning.RearAssembleCV_StopperUp_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{Cyl_Stopper} Up Done");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.CV_ConditionCheck:
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_CvStartDetect, false);
#endif
                    if (In_CvStartDetect.Value == false)
                    {
                        Step.RunStep = (int)ESetCVAssemble_LoadStep.CVAssemble_LoadRequest_Set;
                        break;
                    }

                    // Set detect at current CV start & upstream CV end
                    if (In_DetachCVEndDetect.Value)
                    {
                        requestLoadWithSetBetweenTwoCV = true;
                        Step.RunStep = (int)ESetCVAssemble_LoadStep.CVAssemble_LoadRequest_Set;
                        break;
                    }

                    // Set detect at current CV start only
                    Step.RunStep = (int)ESetCVAssemble_LoadStep.CV_Run;
                    break;
                case ESetCVAssemble_LoadStep.CVAssemble_LoadRequest_Set:
                    FlagOut_AssembleSetLoadRequest = true;
                    if (requestLoadWithSetBetweenTwoCV)
                    {
                        Wait(200); // Make sure upstream CV receive the load request signal
                    }

                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.CV_StartDetect_Wait:
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_CvStartDetect, true);
#endif
                    if (In_CvStartDetect.Value || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Set CV assemble start detect.");
                        Cv_SetCamAssemble.Run();

                        FlagOut_AssembleSetLoadRequest = false;
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case ESetCVAssemble_LoadStep.CV_Run:
                    Log.Debug("Set CV assemble run.");
                    Cv_SetCamAssemble.Run();
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => In_CvEndDetect.Value || _machineStatus.IsDryRunMode);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_CvEndDetect, true);
#endif
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.CV_EndDetect_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Set CV assemble move timeout.");
                        Cv_SetCamAssemble.Stop();
                        //TODO: Raise Warning CV move timeout
                        //RaiseWarning();
                        break;
                    }

                    Wait(_setCVRecipe.EndAssembleCvStopWait); // Wait for stable sensor signal
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.CV_Stop:
                    Log.Debug("Set CV assemble stop.");
                    Cv_SetCamAssemble.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.Cylinder_AlignOn:
                    Log.Debug("Set CV assemble align moving forward.");
                    Cyl_AlignOn(true);
                    Wait(10000, () => Cyl_Align.IsForward || _machineStatus.IsDryRunMode == true);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.Cylinder_AlignOn_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Set CV assemble align fail.");
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AlignOn_Fail
                                            : EWarning.RearAssembleCV_AlignOn_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESetCVAssemble_LoadStep.End:
                    Log.Debug("Set CV assemble load done.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.CamHead_Place;
                    break;
                default:
                    break;

            }
        }

        private void Sequence_CamAssemble()
        {
            switch ((ESetCVAssemble_AssembleStep)Step.RunStep)
            {
                case ESetCVAssemble_AssembleStep.Start:
                    if(_machineStatus.IsByPassMode)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AssembleStep.CyAlignOff_StopperDown;
                        break;
                    }
                    Log.Debug("Set CV Cam Assemble start.");
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    materialStatus.CameraStatus = ECameraStatus.Processing;
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.Cyl_Align_On:
                    processTackTime.StartTiming();
                    Cyl_AlignOn(true);
                    Log.Debug($"{Cyl_Align} Align start.");
                    Wait(10000, () => !Cyl_Align.IsBackward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.Cyl_Align_On_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AlignOn_Fail
                                                                : EWarning.RearAssembleCV_AlignOn_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Wait(1000);
                    Log.Debug($"{Cyl_Align} Align done.");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.VisionConditionCheck:
                    if (_isOnAssembleProcess == true)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AssembleStep.CamAssemble_Request_Set;
                        break;
                    }

                    if (_isAssembleDone == true)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AssembleStep.WaitCamHeadOutSignalToCamInspection;
                        break;
                    }

                    if (_startRunAgain == true && _devRecipe.UseVIPRunMode == false)
                    {
                        Step.RunStep = (int)ESetCVAssemble_AssembleStep.WaitCamHeadOutSignalToCamInspection;
                        break;
                    }

                    Step.RunStep++;
                    break;
                // Film Inspection
                case ESetCVAssemble_AssembleStep.WaitCamHeadOutSignaltoFilmInspection:
                    if (!FlagIn_CamHeadMoveAvoidToVision)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Cam Avoid To vision signal Received");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.FilmInspect_Vision_Request_Set:
                    if (_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (FlagIn_VisionInspectionRun == false)
                    {
                        _visionProcess.Vision_job(Cmd_Film_Inspection);
                        Log.Debug($"Vision Film Inspection Request");
                        Wait(20);
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case ESetCVAssemble_AssembleStep.FilmInspect_Vision_Response_Wait:
                    if (FlagIn_VisionInspectionRun == true && !_machineStatus.IsDryRunMode)
                    {
                        Wait(10);
                        break;
                    }
                    Log.Debug("Vision Film Inspection Response Wait");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.FilmInspect_Vision_ResultHandle:
                    if (FlagIn_VisionFilmInspectionError == true && !_machineStatus.IsDryRunMode && !_devRecipe.UseVIPRunMode)
                    {
                        _productionData.WriteData(line, EPropertyProductionData.FilmDetachFail);

                        countData.FilmDetachFail++;
                        materialStatus.ProcessStatus = EMaterialProcessStatus.Fail;
                        Cyl_AlignOn(false);
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_FilmInspection_Fail
                                                                    : EWarning.RearAssembleCV_FilmInspection_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"Vision Film Inspection Request Result Check OK");
                    Step.RunStep++;
                    break;

                // Cam Assemble Request and wait to Done
                case ESetCVAssemble_AssembleStep.CamAssemble_Request_Set:
                    FlagOut_CamAssembleRequest = true;
                    materialStatus.CameraStatus = ECameraStatus.None;
                    _isOnAssembleProcess = true;
                    Log.Debug("Set Cam Assemble request.");
                    Step.RunStep++;
                    break;
                //case ESetCVAssemble_AssembleStep.CamAssemble_PlaceDone_Wait:
                //    if (FlagIn_CamAssemblePlaceDone)
                //    {
                //        Log.Debug("Set Cam Place Done signal received.");
                //        FlagOut_CamAssembleRequest = false;
                //        _isPlaceDone = true;
                //        Step.RunStep++;
                //        break;
                //    }
                //    Wait(10);
                //    break;

                case ESetCVAssemble_AssembleStep.CamAssemble_Done_Wait:
                    if (FlagIn_CamAssembleDone)
                    {
                        Log.Debug("Set Cam Assemble Done signal received.");
                        _isAssembleDone = true;
                        _isOnAssembleProcess = false;
                        materialStatus.CameraStatus = ECameraStatus.Exist;
                        Step.RunStep++;
                        break;
                    }
                    Wait(0.0001);
                    break;
                // Cam Assemble Inspection
                case ESetCVAssemble_AssembleStep.WaitCamHeadOutSignalToCamInspection:
                    if (FlagIn_CamHeadMoveAvoidToVision == false)
                    {
                        Wait(20);
                        break;
                    }

                    FlagOut_CamAssembleRequest = false;
                    Log.Debug("Cam Avoid To vision signal Received");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CamAssemble_Inspection_Request_Set:
                    if (_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (FlagIn_VisionInspectionRun == false)
                    {
                        _visionProcess.Vision_job(Cmd_Assemble_Inspection);
                        Log.Debug($"Vision Assemble Inspection Request");
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case ESetCVAssemble_AssembleStep.CamAssemble_Inspection_Response_Wait:
                    if (FlagIn_VisionInspectionRun == true && !_machineStatus.IsDryRunMode)
                    {
                        Wait(10);
                        break;
                    }
                    Log.Debug($"{line}:Vision Assemble Inspection Request Wait");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CamAssemble_Inspection_ResultHandle:
                    if (FlagIn_VisionAssembleInspectionError == true && !_machineStatus.IsDryRunMode && !_devRecipe.UseVIPRunMode)
                    {
                        if (_startRunAgain == true)
                        {
                            _startRunAgain = false;
                            Step.RunStep = (int)ESetCVAssemble_AssembleStep.WaitCamHeadOutSignaltoFilmInspection;
                            break;
                        }

                        if (_retryCount == 2)
                        {
                            _retryCount = 0;
                            _productionData.WriteData(line, EPropertyProductionData.AssembleFail);

                            countData.AssembleFail++;
                            materialStatus.ProcessStatus = EMaterialProcessStatus.Fail;
                            Cyl_AlignOn(false);
                            EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AssembleInspection_Fail
                                                                    : EWarning.RearAssembleCV_AssembleInspection_Fail;
                            RaiseWarning((int)eWarning);
                            break;
                        }
                        _retryCount++;
                        Step.RunStep = (int)ESetCVAssemble_AssembleStep.CylAlignOffToRetry;
                        break;
                    }
                    _startRunAgain = false;
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    Log.Debug($"Vision Assemble Inspection Request Result Check OK");
                    Step.RunStep = (int)ESetCVAssemble_AssembleStep.CyAlignOff_StopperDown;
                    break;

                // Retry Assy Cam by Cylinder Align
                case ESetCVAssemble_AssembleStep.CylAlignOffToRetry:
                    Cyl_AlignOn(false);
                    Log.Debug($"{Cyl_Align} Align Off to Retry");
                    Wait(10000, () => Cyl_Align.IsBackward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CylAlignOffToRetry_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AlignOff_Fail
                                                                : EWarning.RearAssembleCV_AlignOff_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{Cyl_Align} Align Off to Retry Done");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CylAlignOnToRetry:
                    Cyl_AlignOn(true);
                    Log.Debug($"{Cyl_Align} Align On to Retry");
                    Wait(10000, () => Cyl_Align.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CylAlignOnToRetry_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AlignOn_Fail
                                                                : EWarning.RearAssembleCV_AlignOn_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{Cyl_Align} Align On to Retry Done");
                    Step.RunStep = (int)ESetCVAssemble_AssembleStep.CamAssemble_Inspection_Request_Set;
                    break;
                // Retry Assy Cam by Cylinder Align - Done
                case ESetCVAssemble_AssembleStep.CyAlignOff_StopperDown:
                    Log.Debug($"Cylinder Align Off , Stopper Down");
                    Cyl_AlignOn(false);
                    Cyl_StopperOn(false);
                    Wait(10000, () => Cyl_Align.IsBackward && Cyl_Stopper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.CylAlignOff_StopperDown_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning;
                        if (Cyl_Align.IsBackward == false)
                        {
                            eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_AlignOff_Fail
                                                                : EWarning.RearAssembleCV_AlignOff_Fail;
                            RaiseWarning((int)eWarning);
                            break;
                        }
                        eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_StopperDown_Fail
                                        : EWarning.RearAssembleCV_StopperDown_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    processTackTime.StopTiming();
                    Log.Debug($"Cylinder Align Off , Stopper Down Done");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_AssembleStep.End:
                    _productionData.WriteData(line, EPropertyProductionData.Output);
                    countData.Output++;

                    if (cycleTackTime.ElapsedSecond != 0)
                    {
                        _workData.CycleTime = cycleTackTime.ElapsedSecond / 2.0;
                    }
                    cycleTackTime.RestartTime();

                    Log.Debug("Set CV assemble load done.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.CVOut_Load;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_CoveyorAssemble_Unload()
        {
            switch ((ESetCVAssemble_UnloadStep)Step.RunStep)
            {
                case ESetCVAssemble_UnloadStep.Start:
                    Log.Debug("Set CV Assemble Unload start.");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_UnloadStep.CVOut_Request_Wait:
                    if (FlagIn_SetOutRequest)
                    {
                        Log.Debug("Set CV assemble load request received.");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ESetCVAssemble_UnloadStep.StopperDown:
                    if (Cyl_Stopper.IsBackward)
                    {
                        Step.RunStep = (int)ESetCVAssemble_UnloadStep.CV_SetUnloadStart;
                        break;
                    }
                    Cyl_StopperOn(false);
                    Log.Debug($"{Cyl_Stopper} Down start.");
                    Wait(10000, () => Cyl_Stopper.IsBackward);
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_UnloadStep.StopperDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontAssembleCV_StopperDown_Fail
                                        : EWarning.RearAssembleCV_StopperDown_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{Cyl_Stopper} Down done.");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_UnloadStep.CV_SetUnloadStart:
                    Cv_SetCamAssemble.Run();
                    if (In_CvEndDetect.Value == false)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ESetCVAssemble_UnloadStep.ProductDataSet:
                    //countDataTotal++;
                    TimeCheckConveyor.RestartTime();
                    Log.Debug("Front Set Out");
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_UnloadStep.CVOut_SetReceive_Wait:

                    if (FlagIn_SetOutRequest == false || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Set CV assemble load request reset.");
                        Wait(_setCVRecipe.SetOutWorkAreaWait);
                        Step.RunStep++;
                        break;
                    }
                    else if (!IsOneConveyorFrontLine && (TimeCheckConveyor.ElapsedSecondTime) > 10 && (In_CvEndDetect.Value == false && In_CvStartDetect.Value == false && In_OutCVStartDetect.Value == false))
                    {
                        Step.RunStep++;
                        break;
                    }
                        break;
                case ESetCVAssemble_UnloadStep.CV_UnloadStop:
                    Cv_SetCamAssemble.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVAssemble_UnloadStep.End:
                    Log.Debug("Set CV assemble unload done.");

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
                Cyl_Align.Backward();
#if SIMULATION
            SimulationInputSetter.SetSimInput(In_AlignOff, true);
            SimulationInputSetter.SetSimInput(In_AlignOnNG, false);
#endif
        }
        private void StopRun()
        {
            Cv_SetCamAssemble.Stop();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
        }

        private void OnMaterialSetStatusChange(bool obj)
        {
            Cyl_AlignOn(obj);
        }
        #endregion

        #region Constructors
        public SetConveyerAssembleProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
             EDMLogger edmLogger,
            MaterialStatusList materialStatusList,
            VisionProcess visionProcess,
            MachineStatus machineStatus,
            TotalTackTime totalTackTime,
            CWorkData workData,
            [FromKeyedServices("FrontCvCamAssembleInput")] IDInputDevice<EFrontCvCamAssembleInput> frontCvCamAssembleInput,
            [FromKeyedServices("RearCvCamAssembleInput")] IDInputDevice<ERearCvCamAssembleInput> rearCvCamAssembleInput,
            [FromKeyedServices("FrontCvCamAssembleOutput")] IDOutputDevice<EFrontCvCamAssembleOutput> frontCvCamAssembleOutput,
            [FromKeyedServices("RearCvCamAssembleOutput")] IDOutputDevice<ERearCvCamAssembleOutput> rearCvCamAssembleOutput,
            ProductionData productionData,
            DevRecipe devRecipe,
            ProcessConfig processConfig)
        {
            _edmLogger = edmLogger;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _materialStatusList = materialStatusList;
            _visionProcess = visionProcess;
            _machineStatus = machineStatus;
            _totalTackTime = totalTackTime;
            _workData = workData;
            _frontCvCamAssembleInput = frontCvCamAssembleInput;
            _rearCvCamAssembleInput = rearCvCamAssembleInput;
            _frontCvCamAssembleOutput = frontCvCamAssembleOutput;
            _rearCvCamAssembleOutput = rearCvCamAssembleOutput;
            _productionData = productionData;
            _devRecipe = devRecipe;
            _processConfig = processConfig;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly DevRecipe _devRecipe;
        private VisionProcess _visionProcess { get; set; }
        private MachineStatus _machineStatus;
        private readonly IDInputDevice _frontCvCamAssembleInput;
        private readonly IDInputDevice _rearCvCamAssembleInput;
        private readonly IDOutputDevice _frontCvCamAssembleOutput;
        private readonly IDOutputDevice _rearCvCamAssembleOutput;
        private readonly ProductionData _productionData;
        private MaterialStatusList _materialStatusList;
        string[] strEDMPara = new string[4];
        private EDMLogger _edmLogger;
        private uint _retryCount = 0;
        private bool _isPlaceDone;
        private bool _isAssembleDone { get; set; }
        private bool _startRunAgain;

        private bool _isOnAssembleProcess;
        private SetConveyorRecipe _setCVRecipe => _recipeList.SetConveyorRecipe;
        private TotalTackTime _totalTackTime;
        private CWorkData _workData;
        private readonly ProcessConfig _processConfig;

        #endregion

    }
}
