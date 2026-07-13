using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontCameraAssembleEquipment.Process
{
    public class SetConveyerOutProcess : ProcessBase<ESequence>
    {
        private ECVLine line => Name == EProcess.FrontSetCVOut.ToString() ? ECVLine.Front : ECVLine.Rear;
        private bool IsOneConveyorFrontLine => _processConfig.MachineType == EMachineType.OneConveyor && line == ECVLine.Front;

        #region Inputs
        private IDInput In_UnloadCvStart => line == ECVLine.Front ? _devices.Inputs.FrontUnloadCvStart
                                                                      : _devices.Inputs.RearUnloadCvStart;
        private IDInput In_UnloadCvMid1 => line == ECVLine.Front ? _devices.Inputs.FrontUnloadCvMid1
                                                                    : _devices.Inputs.RearUnloadCvMid1;
        private IDInput In_UnloadCvMid2 => line == ECVLine.Front ? _devices.Inputs.FrontUnloadCvMid2
                                                                    : _devices.Inputs.RearUnloadCvMid2;
        private IDInput In_UnloadCvEnd => line == ECVLine.Front ? _devices.Inputs.FrontUnloadCvEnd
                                                                    : _devices.Inputs.RearUnloadCvEnd;
        private IDInput In_UnloadCvStopperUp => line == ECVLine.Front ? _devices.Inputs.FrontUnloadPosUp
                                                                        : _devices.Inputs.RearUnloadPosUp;
        private IDInput In_UnloadCvStopperDown => line == ECVLine.Front ? _devices.Inputs.FrontUnloadPosDown
                                                                        : _devices.Inputs.RearUnloadPosDown;
        private IDInput In_OutCvSetUpDetect => line == ECVLine.Front ? _devices.Inputs.FrontOutCvSetUpDetect
                                                                        : _devices.Inputs.RearOutCvSetUpDetect;

        private IDInput In_DownStreamLoadEnable => line == ECVLine.Front ? _devices.Inputs.DownstreamFrontLoadEnable
                                                                             : _devices.Inputs.DownstreamRearLoadEnable;
        #endregion

        #region Outputs
        private IDOutput Out_UnloadCvOn => line == ECVLine.Front ? _devices.Outputs.FrontUnloadCvOn
                                                                     : _devices.Outputs.RearUnloadCvOn;
        private IDOutput Out_DownStreamLoadEnable => line == ECVLine.Front ? _devices.Outputs.DownstreamFrontLoadEnable
                                                                               : _devices.Outputs.DownstreamRearLoadEnable;
        private IDOutput Out_UnloadCvStopperUp => line == ECVLine.Front ? _devices.Outputs.FrontUnloadPosUp
                                                                        : _devices.Outputs.RearUnloadPosUp;
        private IDOutput Out_UnloadCvSlow => line == ECVLine.Front ? _devices.Outputs.FrontUnloadCvSlow
                                                                        : _devices.Outputs.RearUnloadCvSlow;
        private IDOutput Front_CvOutVac => _devices.Outputs.FrontUnloadCvVacOn;
        #endregion

        #region Cylinders
        private ICylinder Cyl_UnloadCvMoverUpDn => line == ECVLine.Front ? _devices.Cylinders.SetCV_FrontUnloadMoverUpDn
                                                                           : _devices.Cylinders.SetCV_RearUnloadMoverUpDn;
        private ICylinder Cyl_FrontUnloadTurnReturn => _devices.Cylinders.SetCV_FrontUnloadTurnReturn;
        private ICylinder Cyl_FrontUnloadStopperUpDn => _devices.Cylinders.SetCV_FrontUnloadStopperUpDn;
        #endregion

        #region Vacuum 
        private Vaccum Front_OUTCvVac => _vacuumList.FrontUnload_CvVac;
        #endregion

        #region CVs
        private IConveyor Cv_SetOutput => line == ECVLine.Front ? _devices.CVs.FrontSetWorkCV_SetUnloadOutput
                                                                : _devices.CVs.RearSetWorkCV_SetUnloadOutput;
        #endregion

        #region Flags
        private bool FlagOut_UnloadRequest
        {
            set
            {
                if (line == ECVLine.Front)
                    _frontCvSetUnloadOutput[(int)EFrontCvSetUnloadOutput.FRONT_UNLOAD_REQUEST] = value;
                else
                    _rearCvSetUnloadOutput[(int)ERearCvSetUnloadOutput.REAR_UNLOAD_REQUEST] = value;
            }
        }

        #endregion

        private MaterialStatus materialStatusStart => line == ECVLine.Front ? _materialStatusList.FrontSetOut1CvMaterialStatus
                                                                            : _materialStatusList.RearSetOut1CvMaterialStatus;
        private MaterialStatus materialStatusMid => line == ECVLine.Front ? _materialStatusList.FrontSetOut2CvMaterialStatus
                                                                            : _materialStatusList.RearSetOut2CvMaterialStatus;
        private MaterialStatus materialStatusEnd => line == ECVLine.Front ? _materialStatusList.FrontSetOut3CvMaterialStatus
                                                                            : _materialStatusList.RearSetOut3CvMaterialStatus;

        #region Override Methods
        public override bool PreProcess()
        {
            // 1. UI Update
            if (In_UnloadCvStart.Value) materialStatusStart.Set();
            else materialStatusStart.Clear();

            if (In_UnloadCvMid2.Value || In_UnloadCvMid1.Value) materialStatusMid.Set();
            else materialStatusMid.Clear();

            if (IsOneConveyorFrontLine)
            {
                if (In_UnloadCvEnd.Value || Front_OUTCvVac.IsVaccumOn == true) materialStatusEnd.Set();
                else materialStatusEnd.Clear();
            }
            else
            {
                if (In_UnloadCvEnd.Value || In_OutCvSetUpDetect.Value) materialStatusEnd.Set();
                else materialStatusEnd.Clear();
            }

            // 2. Slow down CV
            if (In_UnloadCvMid2.Value) { Out_UnloadCvSlow.Value = true; }
            else { Out_UnloadCvSlow.Value = false; }

            //if (In_OutCvSetUpDetect.Value == false && Cyl_UnloadCvMoverUpDn.IsForward && ProcessMode == EProcessMode.Run && In_DownStreamLoadEnable.Value == false)
            //{
            //    Cyl_UnloadCvMoverUpDn.Backward();
            //}

            if ((ProcessMode == EProcessMode.ToWarning || ProcessMode == EProcessMode.ToAlarm) && _machineStatus.IsInterfaceOnlyMode)
            {
                _machineStatus.MachineRunMode = EMachineRunMode.Auto;
            }

            // 3. Interface Only Mode
            if (_machineStatus.IsInterfaceOnlyMode == true)
            {
                if (Sequence != ESequence.AutoRun
                     && Sequence != ESequence.Ready
                     && Sequence != ESequence.CVOut_Load
                     && Sequence != ESequence.CVOut_Unload)
                {
                    Sequence = ESequence.AutoRun;
                }
                ProcessMode = EProcessMode.Run;
            }

            // 4. Check CV Condition
            if (ProcessMode == EProcessMode.Run && Sequence != ESequence.Ready && Sequence != ESequence.Stop)
            {
                if (IsOneConveyorFrontLine)
                {
                    // OneConveyor Front không còn giao tiếp với máy sau.
                    Out_DownStreamLoadEnable.Value = false;
                }
                else
                {
                    Out_DownStreamLoadEnable.Value = In_OutCvSetUpDetect.Value && (_machineStatus.IsOutputStop == false) && (Cyl_UnloadCvMoverUpDn.IsForward == true);
                }

                if (IsOneConveyorFrontLine && Sequence == ESequence.CVOut_Unload)
                {
                    // Sequence unload mới tự quản lý Run/Stop conveyor.
                    // Không cho PreProcess ghi đè lệnh conveyor tại đây.
                }
                else
                {
                    switch (SensorConditionStatus)
                    {
                        case 1:
                        case 2:
                        case 4:
                            if (Sequence != ESequence.CVOut_Unload)
                            {
                                Cv_SetOutput.Run();
                                break;
                            }

                            if (Cyl_UnloadCvMoverUpDn.IsForward)
                                Cv_SetOutput.Run();
                            else
                                Cv_SetOutput.Stop();
                            break;

                        case 0:
                        default:
                            Cv_SetOutput.Stop();
                            break;

                        case 3:
                            if (In_UnloadCvEnd.Value == false)
                            {
                                Cv_SetOutput.Stop();
                                break;
                            }

                            if (isDelayingCVStop) break;

                            Task.Run(async () =>
                            {
                                isDelayingCVStop = true;
                                await Task.Delay(_recipeList.SetConveyorRecipe.OutSetConveyorStopWait);
                                Cv_SetOutput.Stop();
                                isDelayingCVStop = false;
                            });
                            break;
                    }
                }
            }
            return base.PreProcess();
        }

        private bool isDelayingCVStop = false;
        private bool _isOneConveyorFrontResumePrepared = false;

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
        //public override bool ProcessOrigin()
        //{
        //    switch ((ESetCVOut_OriginStep)Step.OriginStep)
        //    {
        //        case ESetCVOut_OriginStep.Start:
        //            Log.Debug("SetCVInProcess Origin Start");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVOut_OriginStep.CV_Stop:
        //            Cv_SetOutput.Stop();
        //            Log.Debug($"{Cv_SetOutput.Name} Stopped");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVOut_OriginStep.End:
        //            Log.Debug("SetCVInProcess Origin End");
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
            if (IsOneConveyorFrontLine)
                return ProcessToRun_OneConveyorFront();

            return ProcessToRun_Original();
        }
        public bool ProcessToRun_Original()
        {
            //Log.Debug("To Run End");
            switch ((ESetCVOut_ToRunStep)Step.ToRunStep)
            {
                case ESetCVOut_ToRunStep.Start:
                    Step.ToRunStep++;
                    break;
                case ESetCVOut_ToRunStep.Check_UpDownCylStatus:
                    if (In_OutCvSetUpDetect.Value == false && Cyl_UnloadCvMoverUpDn.IsForward)
                    {
                        Log.Debug("Cyl_UnloadCv Up but Set Not Exist");
                        Step.ToRunStep = (int)ESetCVOut_ToRunStep.Cyl_DownMove;
                        break;
                    }
                    Step.ToRunStep = (int)ESetCVOut_ToRunStep.End;
                    break;
                case ESetCVOut_ToRunStep.Cyl_DownMove:
                    Cyl_UnloadCvMover(false);
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsBackward);
                    Log.Debug("Cylinder Up Pos Move Down");
                    Step.ToRunStep++;
                    break;
                case ESetCVOut_ToRunStep.Cyl_DownWait:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontOUTCV_StopperDown_Fail
                                                                   : EWarning.RearOUTCV_StopperDown_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug("Cylinder Up Pos Move Down Done");
                    Step.ToRunStep++;
                    break;
                case ESetCVOut_ToRunStep.End:
                    Log.Debug("To Run End");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool ProcessToRun_OneConveyorFront()
        {
            switch ((EOneConveyorToRunStep)Step.ToRunStep)
            {
                case EOneConveyorToRunStep.Start:
                    Cv_SetOutput.Stop();
                    Out_DownStreamLoadEnable.Value = false;
                    _isOneConveyorFrontResumePrepared = false;

                    // Chờ input khí/sensor ổn định sau khi Start lại.
                    Wait(300);
                    Step.ToRunStep++;
                    break;

                case EOneConveyorToRunStep.CheckPhysicalState:
                    if (Sequence == ESequence.AutoRun)
                    {
                        if (PrepareOneConveyorFrontResume() == false)
                        {
                            break;
                        }

                        _isOneConveyorFrontResumePrepared = true;
                    }

                    Step.ToRunStep++;
                    break;

                case EOneConveyorToRunStep.End:
                    Log.Debug("To Run End");
                    Thread.Sleep(20);
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
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.CVOut_Load:
                    Sequence_CVOut_Load();
                    break;
                case ESequence.CVOut_Unload:
                    Sequence_CVOut_Unload();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }

            return true;
        }

        public override string ToString()
        {
            return line == ECVLine.Front ? EProcess.FrontSetCVOut.GetDescription()
                                             : EProcess.RearSetCVOut.GetDescription();
        }

        #endregion

        #region Private Methods
        private void Sequence_Ready()
        {
            if (IsOneConveyorFrontLine)
            {
                Sequence_Ready_OneConveyorFront();
                return;
            }

            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            if (line == ECVLine.Front)
            {
                _frontCvSetUnloadOutput.ClearOutputs();

            }
            else
            {
                _rearCvSetUnloadOutput.ClearOutputs();
            }
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }

        private void Sequence_Ready_OneConveyorFront()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }

            Cv_SetOutput.Stop();
            Out_DownStreamLoadEnable.Value = false;
            _frontCvSetUnloadOutput.ClearOutputs();

            if (Cyl_UnloadCvMoverUpDn.IsForward)
            {
                ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.CheckVacuumWhileMoverUp);
                return;
            }

            if (In_UnloadCvEnd.Value)
            {
                Log.Debug("Ready End - Product Hold");
                Sequence = ESequence.Stop;
                return;
            }

            Front_OUTCvVac.VaccumOff();

            if (Cyl_FrontUnloadTurnReturn.IsForward)
            {
                ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.TurnReturnBeforeReady);
                return;
            }

            if (Cyl_FrontUnloadTurnReturn.IsBackward)
            {
                if (Cyl_FrontUnloadStopperUpDn.IsBackward)
                {
                    Log.Debug("Ready End");
                    Sequence = ESequence.Stop;
                }
                else
                {
                    ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.StopperDownForReady);
                }

                return;
            }
        }

        private void Sequence_AutoRun()
        {
#if SIMULATION
            Random random = new Random();
            bool randomBool = (random.Next(2) == 1); // True if 1, False if 0
#endif
            switch ((ESetCVOut_AutoRunStep)Step.RunStep)
            {
                case ESetCVOut_AutoRunStep.Start:
                    //Log.Info("Sequence Case Output Unload");
                    if (IsOneConveyorFrontLine && _isOneConveyorFrontResumePrepared == false)
                    {
                        if (PrepareOneConveyorFrontResume() == false)
                            break;

                        _isOneConveyorFrontResumePrepared = true;

                        if (Sequence != ESequence.AutoRun)
                            break;
                    }
                    Step.RunStep++;
                    break;
                case ESetCVOut_AutoRunStep.CheckUpSetExistToDownCylinder:
                    if (IsOneConveyorFrontLine)
                    {
                        Step.RunStep = (int)ESetCVOut_AutoRunStep.CV_Run;
                        break;
                    }

                    if (In_OutCvSetUpDetect.Value == false && Cyl_UnloadCvMoverUpDn.IsForward)
                    {
                        Log.Debug("Cyl_UnloadCv Up but Set Not Exist");
                        Step.RunStep = (int)ESetCVOut_AutoRunStep.StopperUnloadDown;
                        break;
                    }
                    Step.RunStep = (int)ESetCVOut_AutoRunStep.CV_Run;
                    break;
                case ESetCVOut_AutoRunStep.StopperUnloadDown:
                    Cyl_UnloadCvMover(false);
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsBackward);
                    Log.Debug("Cylinder Up Pos Move Down");
                    Step.RunStep++;
                    break;
                case ESetCVOut_AutoRunStep.StopperUnloadDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontOUTCV_StopperDown_Fail
                                                                   : EWarning.RearDetachCV_StopperDown_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug("Cylinder Up Pos Move Down Done");
                    Step.RunStep++;
                    break;
                case ESetCVOut_AutoRunStep.CV_Run:
                    Cv_SetOutput.Run();
                    Step.RunStep++;
                    Wait(2000);
                    break;
                case ESetCVOut_AutoRunStep.CV_Stop:
                    Cv_SetOutput.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVOut_AutoRunStep.CheckConditionToRun:
                    if (IsOneConveyorFrontLine)
                    {
                        Sequence = In_UnloadCvEnd.Value
                            ? ESequence.CVOut_Unload
                            : ESequence.CVOut_Load;
                        break;
                    }

                    switch (SensorConditionStatus)
                    {
                        case 0:
                        case 2:
                            Sequence = ESequence.CVOut_Load;
                            break;
                        case 3:
                        case 4:
                            Sequence = ESequence.CVOut_Unload;
                            break;
                        case 1:
                            break;
                    }
                    break;
                case ESetCVOut_AutoRunStep.End:
                    break;
                default:
                    break;
            }
        }


        private void Sequence_CVOut_Load()
        {
            switch ((ESetCVOut_LoadStep)Step.RunStep)
            {
                case ESetCVOut_LoadStep.Start:
                    //Log.Debug("Set Out CV Load Start");
                    if (!IsOneConveyorFrontLine)
                    {
                        Step.RunStep = (int)ESetCVOut_LoadStep.CVUnload_RequestLoad_Set;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.Stopper_Up:
                    Log.Debug("Set Stopper Up");
                    Cyl_FrontUnloadStopperUpDn.Forward();
                    Wait(3000, () => Cyl_FrontUnloadStopperUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.Stopper_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_StopperUp_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CVUnload_RequestLoad_Set:
                    Log.Debug("Set Load Request signal");
                    FlagOut_UnloadRequest = true;
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CV_StartDetect_Wait:
                    Wait(2000, () => In_UnloadCvStart.Value == true);
                    Log.Debug("Set CV out start sensor wait");
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CV_StartDetect_Check:
                    if (WaitTimeOutOccurred)
                    {
                        FlagOut_UnloadRequest = false;
                        Sequence = ESequence.AutoRun;
                        break;
                    }
                    Log.Debug("Set CV out start detect.");
                    Cv_SetOutput.Run();
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CVUnload_RequestLoad_Clear:
                    Log.Debug("Clear Load Request signal");
                    FlagOut_UnloadRequest = false;
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CV_StartNotDetect_Wait:
                    if (IsOneConveyorFrontLine)
                    {
                        Cv_SetOutput.Run();
                        if (In_UnloadCvStart.Value)
                        {
                            Wait(10);
                            break;
                        }
                        Step.RunStep = (int)ESetCVOut_LoadStep.CV_StartNotDetect_Check;
                        break;
                    }

                    if (In_UnloadCvStart.Value == true)
                    {
                        if (In_UnloadCvEnd.Value == true)
                        {
                            Sequence = ESequence.CVOut_Unload;
                            break;
                        }
                        Wait(10);
                        break;
                    }
                    //Wait((int)(_setCVRecipe.SetCVDetectTimeout), () => In_UnloadCvStart.Value == false);
                    Log.Debug("Set CV out start sensor not detect wait");
                    //if (IsOneConveyorFrontLine)
                    //{
                    //    Step.RunStep = (int)ESetCVOut_LoadStep.End;
                    //}
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.CV_StartNotDetect_Check:
                    //if (WaitTimeOutOccurred)
                    //{
                    //    Sequence = ESequence.AutoRun;
                    //    break;
                    //}
                    //Log.Debug("Set CV out start not detect.");
                    if (IsOneConveyorFrontLine)
                    {
                        if (!In_UnloadCvEnd.Value)
                        {
                            Cv_SetOutput.Run();
                            Wait(10);
                            break;
                        }

                        Cv_SetOutput.Stop();
                        FlagOut_UnloadRequest = false;
                        Step.RunStep++;
                        break;
                    }
                    Cv_SetOutput.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVOut_LoadStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.AutoRun;
                    break;
            }
        }

        private void Sequence_CVOut_Unload()
        {
            if (IsOneConveyorFrontLine)
            {
                Sequence_CVOut_Unload_OneConveyorFront();
                return;
            }

            Sequence_CVOut_Unload_Original();
        }

        private void Sequence_CVOut_Unload_OneConveyorFront()
        {
            switch ((EOneConveyorFrontUnloadStep)Step.RunStep)
            {
                case EOneConveyorFrontUnloadStep.Start:
                    Log.Debug("Unload Start");
                    //Cv_SetOutput.Run();
                    Out_DownStreamLoadEnable.Value = false;
                    Step.RunStep++;
                    break;
                case EOneConveyorFrontUnloadStep.StopperUp:
                    Log.Debug("Stopper Up");
                    Cyl_FrontUnloadStopperUpDn.Forward();
                    Wait(3000, () => Cyl_FrontUnloadStopperUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EOneConveyorFrontUnloadStep.StopperUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_StopperUp_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.WaitEndSensor:
                    if (In_UnloadCvEnd.Value == false)
                    {
                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.Start;
                        break;
                    }
                    Log.Debug("Sensor detected");
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.StopConveyor:
                    Log.Debug("Conveyor Stop");
                    Cv_SetOutput.Stop();
                    Wait(500);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.MoverUpAndStopperDown:
                    Log.Debug("Mover Up and Stopper Down");
                    Front_OUTCvVac.VaccumOff();
                    Cyl_UnloadCvMover(true);
                    Cyl_FrontUnloadStopperUpDn.Backward();
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsForward && Cyl_FrontUnloadStopperUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.MoverUpAndStopperDownCheck:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning warning = !Cyl_UnloadCvMoverUpDn.IsForward
                            ? EWarning.FrontOUTCV_MoverCylDown_Fail
                            : EWarning.FrontOUTCV_MoverCylUp_Fail;
                        RaiseWarning((int)warning);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.VacuumOn:
                    Log.Debug("Vacuum On");
                    Front_OUTCvVac.VaccumOn();
                    Wait(2000, () => Front_OUTCvVac.IsVaccumOn || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.VacuumOnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_VacOn_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.Turn:
                    Log.Debug("Cylinder Turn");
                    if (!Front_OUTCvVac.IsVaccumOn)
                    {
                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.RecoverVacuumWhileMoverUp;
                        break;
                    }

                    Cyl_FrontUnloadTurnReturn.Forward();
                    Wait(3000, () => Cyl_FrontUnloadTurnReturn.IsForward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.TurnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_Turn_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.MoverDown:
                    Log.Debug("Mover Down");

                    Cyl_UnloadCvMover(false);
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.MoverDownCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_StopperDown_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.VacuumOff:
                    Log.Debug("VacuumOff");

                    Front_OUTCvVac.VaccumOff();
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.VacuumOffCheck:
                    Step.RunStep = In_UnloadCvEnd.Value
                        ? (int)EOneConveyorFrontUnloadStep.ConveyorRun
                        : (int)EOneConveyorFrontUnloadStep.TurnReturn;
                    break;

                case EOneConveyorFrontUnloadStep.ConveyorRun:
                    if (_machineStatus.IsOutputStop)
                    {
                        Cv_SetOutput.Stop();

                        Log.Debug("Conveyor Stop");

                        Wait(100);
                        break;
                    }

                    Log.Debug("Conveyor Run");

                    Cv_SetOutput.Run();

                    Wait(3000, () => In_UnloadCvEnd.Value == false || _machineStatus.IsOutputStop);

                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.WaitEndSensorOff:
                    if (_machineStatus.IsOutputStop && In_UnloadCvEnd.Value)
                    {
                        Cv_SetOutput.Stop();

                        Log.Debug("Conveyor Stop");

                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.ConveyorRun;

                        break;
                    }

                    Cv_SetOutput.Stop();

                    Log.Debug("Conveyor Stop");

                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.TurnReturn:
                    Log.Debug("Cylinder Return");

                    Cyl_FrontUnloadTurnReturn.Backward();
                    Wait(3000, () => Cyl_FrontUnloadTurnReturn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.TurnReturnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_Return_Fail);
                        break;
                    }
                    Step.RunStep = (int)EOneConveyorFrontUnloadStep.End;
                    break;

                case EOneConveyorFrontUnloadStep.RecoverVacuumWhileMoverUp:
                    Log.Debug("Vacuum On Again When MoverUp");
                    Cv_SetOutput.Stop();
                    Front_OUTCvVac.VaccumOn();
                    Wait(3000, () => Front_OUTCvVac.IsVaccumOn);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.RecoverVacuumWhileMoverUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_VacuumLostWhileMoverUp);
                        break;
                    }

                    if (Cyl_FrontUnloadTurnReturn.IsBackward)
                    {
                        Step.RunStep = Cyl_FrontUnloadStopperUpDn.IsBackward
                            ? (int)EOneConveyorFrontUnloadStep.Turn
                            : (int)EOneConveyorFrontUnloadStep.StopperDownBeforeTurn;
                        break;
                    }

                    if (Cyl_FrontUnloadTurnReturn.IsForward)
                    {
                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.MoverDown;
                        break;
                    }

                    break;

                case EOneConveyorFrontUnloadStep.StopperDownBeforeTurn:
                    Log.Debug("Stopper Down");
                    Cyl_FrontUnloadStopperUpDn.Backward();
                    Wait(3000, () => Cyl_FrontUnloadStopperUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.StopperDownBeforeTurnCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_StopperDown_Fail);
                        break;
                    }
                    Step.RunStep = (int)EOneConveyorFrontUnloadStep.VacuumOn;
                    break;

                case EOneConveyorFrontUnloadStep.StopperDownForReady:
                    Log.Debug("Stopper Down");
                    Cyl_FrontUnloadStopperUpDn.Backward();
                    Wait(3000, () => Cyl_FrontUnloadStopperUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.StopperDownForReadyCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_StopperDown_Fail);
                        break;
                    }
                    Step.RunStep = (int)EOneConveyorFrontUnloadStep.End;
                    break;

                case EOneConveyorFrontUnloadStep.MoverDownForReady:
                    Log.Debug("Mover Down For Ready");
                    Front_OUTCvVac.VaccumOff();

                    Cyl_UnloadCvMover(false);
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.MoverDownForReadyCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_MoverCylDown_Fail);
                        break;
                    }

                    Step.RunStep = Cyl_FrontUnloadStopperUpDn.IsBackward
                        ? (int)EOneConveyorFrontUnloadStep.End
                        : (int)EOneConveyorFrontUnloadStep.StopperDownForReady;
                    break;

                case EOneConveyorFrontUnloadStep.CheckVacuumWhileMoverUp:
                    Log.Debug("Check Vacuum While Mover Up");
                    Cv_SetOutput.Stop();
                    Front_OUTCvVac.VaccumOn();
                    Wait(1500, () => Front_OUTCvVac.IsVaccumOn);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.CheckVacuumWhileMoverUpCheck:
                    if (WaitTimeOutOccurred)
                    {
                        Front_OUTCvVac.VaccumOff();
                        Step.RunStep = Cyl_FrontUnloadTurnReturn.IsForward
                            ? (int)EOneConveyorFrontUnloadStep.TurnReturnBeforeReady
                            : (int)EOneConveyorFrontUnloadStep.MoverDownForReady;
                        break;
                    }

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.End;
                        break;
                    }

                    if (Cyl_FrontUnloadTurnReturn.IsBackward)
                    {
                        Step.RunStep = Cyl_FrontUnloadStopperUpDn.IsBackward
                            ? (int)EOneConveyorFrontUnloadStep.Turn
                            : (int)EOneConveyorFrontUnloadStep.StopperDownBeforeTurn;
                        break;
                    }

                    if (Cyl_FrontUnloadTurnReturn.IsForward)
                    {
                        Step.RunStep = (int)EOneConveyorFrontUnloadStep.VacuumOffBeforeMoverDown;
                        break;
                    }

                    break;

                case EOneConveyorFrontUnloadStep.TurnReturnBeforeReady:
                    Log.Debug("Cylinder Return Before Ready");
                    Front_OUTCvVac.VaccumOff();
                    Cyl_FrontUnloadTurnReturn.Backward();
                    Wait(3000, () => Cyl_FrontUnloadTurnReturn.IsBackward);
                    Step.RunStep++;
                    break;

                case EOneConveyorFrontUnloadStep.TurnReturnBeforeReadyCheck:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.FrontOUTCV_Return_Fail);
                        break;
                    }

                    Step.RunStep = (int)EOneConveyorFrontUnloadStep.MoverDownForReady;
                    break;

                case EOneConveyorFrontUnloadStep.VacuumOffBeforeMoverDown:
                    Log.Debug("Vacuum Off Before Mover Down");
                    Front_OUTCvVac.VaccumOff();
                    Step.RunStep = (int)EOneConveyorFrontUnloadStep.MoverDown;
                    break;

                case EOneConveyorFrontUnloadStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.AutoRun;
                    break;
            }
        }

        private bool PrepareOneConveyorFrontResume()
        {
            Cv_SetOutput.Stop();
            Out_DownStreamLoadEnable.Value = false;

            if (Cyl_UnloadCvMoverUpDn.IsForward)
            {
                ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.CheckVacuumWhileMoverUp);
                return true;
            }

            if (Cyl_UnloadCvMoverUpDn.IsBackward && Cyl_FrontUnloadTurnReturn.IsBackward)
            {
                Front_OUTCvVac.VaccumOff();

                if (In_UnloadCvEnd.Value)
                {
                    ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.StopperUp);
                    return true;
                }

                if (Cyl_FrontUnloadStopperUpDn.IsBackward)
                {
                    ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.End);
                }
                else
                {
                    ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.StopperDownForReady);
                }

                return true;
            }

            if (Cyl_UnloadCvMoverUpDn.IsBackward && Cyl_FrontUnloadTurnReturn.IsForward)
            {
                ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep.VacuumOff);
                return true;
            }

            return false;
        }

        private void ResumeOneConveyorFrontAt(EOneConveyorFrontUnloadStep step)
        {
            Sequence = ESequence.CVOut_Unload;
            Step.RunStep = (int)step;
        }

        private void Sequence_CVOut_Unload_Original()
        {
            switch ((ESetCVOut_UnLoadStep)Step.RunStep)
            {
                case ESetCVOut_UnLoadStep.Start:
                    //Log.Debug("Set Out CV Unload Start");
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.WaitEndCvDetect:
                    if (In_UnloadCvEnd.Value == false)
                    {
                        Sequence = ESequence.AutoRun;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.WaitLoadEnableSignal:
                    if (In_DownStreamLoadEnable.Value == false)
                    {
                        if (In_UnloadCvEnd.Value == false)
                        {
                            Sequence = ESequence.AutoRun;
                            break;
                        }
                        Wait(10);
                        break;
                    }

                    if (_machineStatus.IsOutputStop)
                    {
                        Wait(100);
                        break;
                    }

                    Log.Debug("Downstream machine Enable Load signal");
                    Wait(_recipeList.SetConveyorRecipe.OutSetConveyorStopWait);
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.Cyl_UnloadUp:
                    Cyl_UnloadCvMover(true);
                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsForward);
                    Log.Debug("Cylinder Unload move Up");
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.Cyl_UnloadUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontOUTCV_StopperUp_Fail
                                                                   : EWarning.RearDetachCV_StopperUp_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Wait(500);
                    Log.Debug("Cylinder Unload move Done");
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.In_OutCvSetUpDetectCheck:
                    if (In_OutCvSetUpDetect.Value == false)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontOUTCV_SensorUnloadPosUpDetect_Timeout
                                                                   : EWarning.RearOUTCV_SensorUnloadPosUpDetect_Timeout;
                        RaiseWarning((int)eWarning);
                        break;

                    }
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.DownstreamSetUnload_Set:
                    if (_machineStatus.IsOutputStop)
                    {
                        Wait(100);
                        break;
                    }

                    Out_DownStreamLoadEnable.Value = true;
                    Log.Debug("Wait Downstream machine Enable Load signal");
                    Step.RunStep++;
                    break;
                //Step.RunStep++;
                //break;
                case ESetCVOut_UnLoadStep.DownStream_WorkEnd_Wait:
                    if (In_OutCvSetUpDetect.Value || In_DownStreamLoadEnable.Value == true)
                    {
                        Wait(10);
                        break;
                    }
                    Out_DownStreamLoadEnable.Value = false;

                    Log.Debug("Set Up UnloadCv Not Exist");
                    Step.RunStep++;
                    break;
                case ESetCVOut_UnLoadStep.Cyl_UnloadDown:
                    Log.Debug("Cylinder MoverDown");

                    Cyl_UnloadCvMover(false);

                    Wait(3000, () => Cyl_UnloadCvMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;

                case ESetCVOut_UnLoadStep.Cyl_UnloadDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = line == ECVLine.Front
                                ? EWarning.FrontOUTCV_StopperDown_Fail
                                : EWarning.RearOUTCV_StopperDown_Fail;

                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Step.RunStep++;
                    break;

                case ESetCVOut_UnLoadStep.End:
                    Out_DownStreamLoadEnable.Value = false;
                    Log.Debug($"[{line}] Set conveyor unload sequence completed");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.AutoRun;
                    break;
            }
        }

        private int SensorConditionStatus
        {
            get
            {
                if (IsOneConveyorFrontLine)
                {
                    if (In_UnloadCvEnd.Value) return 4;
                    if (In_UnloadCvStart.Value) return 1;
                    return 0;
                }
                bool[] signalStatusArr = { In_UnloadCvStopperDown.Value == false, In_UnloadCvEnd.Value, In_UnloadCvMid2.Value, In_UnloadCvMid1.Value, In_UnloadCvStart.Value };
                int result = 0;
                foreach (bool b in signalStatusArr)
                {
                    result = (result << 1) | (b ? 1 : 0);
                }

                switch (result)
                {
                    case 0:
                    case 16:
                        result = 0;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                    case 7:
                        result = 1;
                        break;
                    case 4:
                        result = 2;
                        break;
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                        result = 3;
                        break;
                    case 17:
                    case 18:
                    case 19:
                        result = 4;
                        break;
                }

                return result;
            }
        }

        private void Cyl_UnloadCvMover(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_UnloadCvMoverUpDn.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_UnloadCvStopperUp, true);
                SimulationInputSetter.SetSimInput(In_UnloadCvStopperDown, false);
#endif
            }
            else
            {
                Cyl_UnloadCvMoverUpDn.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(In_UnloadCvStopperUp, false);
                SimulationInputSetter.SetSimInput(In_UnloadCvStopperDown, true);
#endif
            }
        }
        private void StopRun()
        {
            Out_DownStreamLoadEnable.Value = false;
            Cv_SetOutput.Stop();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
        }
        #endregion

        #region Constructors
        public SetConveyerOutProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MaterialStatusList materialStatusList,
            MachineStatus machineStatus,
            [FromKeyedServices("FrontCvSetUnloadOutput")] IDOutputDevice<EFrontCvSetUnloadOutput> frontCvSetUnloadOutput,
            [FromKeyedServices("RearCvSetUnloadOutput")] IDOutputDevice<ERearCvSetUnloadOutput> rearCvSetUnloadOutput,
            ProcessConfig processConfig,
            VaccumList vacuumList)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _materialStatusList = materialStatusList;
            _machineStatus = machineStatus;
            _frontCvSetUnloadOutput = frontCvSetUnloadOutput;
            _rearCvSetUnloadOutput = rearCvSetUnloadOutput;
            _processConfig = processConfig;
            _vacuumList = vacuumList;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly IDOutputDevice _frontCvSetUnloadOutput;
        private readonly IDOutputDevice _rearCvSetUnloadOutput;
        private readonly MaterialStatusList _materialStatusList;
        private readonly MachineStatus _machineStatus;
        private readonly ProcessConfig _processConfig;
        private readonly VaccumList _vacuumList;
        private SetConveyorRecipe _setCVRecipe => _recipeList.SetConveyorRecipe;
        #endregion

    }
}
