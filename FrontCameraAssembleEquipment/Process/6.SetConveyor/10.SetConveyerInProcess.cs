using EQX.Core.InOut;
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
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading;

namespace FrontCameraAssembleEquipment.Process
{
    public class SetConveyerInProcess : ProcessBase<ESequence>
    {
        private ECVLine line => Name == EProcess.FrontSetCVIn.ToString() ? ECVLine.Front : ECVLine.Rear;

        #region Inputs

        private IDInput In_LoadCvStart => line == ECVLine.Front ? _devices.Inputs.FrontLoadCvStart
                                                                  : _devices.Inputs.RearLoadCvStart;
        private IDInput In_LoadCvEnd => line == ECVLine.Front ? _devices.Inputs.FrontLoadCvEnd
                                                                : _devices.Inputs.RearLoadCvEnd;
        private IDInput In_PreCvDetect => line == ECVLine.Front ? _devices.Inputs.PreFrontCvDetect
                                                                : _devices.Inputs.PreRearCvDetect;
        private IDInput In_PreCvSwitch => line == ECVLine.Front ? _devices.Inputs.PreFrontCvSwitch
                                                                : _devices.Inputs.PreRearCvSwitch;
        private IDInput In_UpStreamSignal => line == ECVLine.Front ? _devices.Inputs.UpStreamFrontLoadEnable
                                                                : _devices.Inputs.UpStreamRearLoadEnable;
        #endregion

        #region Outputs
        private IDOutput Out_LoadCvOn => line == ECVLine.Front ? _devices.Outputs.FrontLoadCvOn
                                                                   : _devices.Outputs.RearLoadCvOn;
        private IDOutput Out_UpStreamLoadEnable => line == ECVLine.Front ? _devices.Outputs.UpstreamFrontLoadEnable
                                                                             : _devices.Outputs.UpstreamRearLoadEnable;
        private IDOutput Out_PreCVRun => line == ECVLine.Front ? _devices.Outputs.PreFrontCVRun
                                                                            : _devices.Outputs.PreRearCVRun;
        #endregion

        #region Cylinders
        #endregion

        #region CVs
        private IConveyor Cv_PreSetLoad => line == ECVLine.Front ? _devices.CVs.FrontSetWorkCV_PreLoadCV
                                                                   : _devices.CVs.RearSetWorkCV_PreLoadCV;
        private IConveyor Cv_SetInput => line == ECVLine.Front ? _devices.CVs.FrontSetWorkCV_SetLoadInput
                                                                   : _devices.CVs.RearSetWorkCV_SetLoadInput;
        #endregion

        #region Motions
        #endregion

        #region Flags
        private bool FlagIn_DetachLoadRequest => line == ECVLine.Front ? _frontInCvSetLoadInput[(int)EFrontInCvSetLoadInput.FRONT_DETACH_LOAD_REQUEST]
                                                                           : _rearInCvSetLoadInput[(int)ERearInCvSetLoadInput.REAR_DETACH_LOAD_REQUEST];
        private bool FlagIn_DetachLoadDone => line == ECVLine.Front ? _frontInCvSetLoadInput[(int)EFrontInCvSetLoadInput.FRONT_DETACH_LOAD_DONE]
                                                                           : _rearInCvSetLoadInput[(int)ERearInCvSetLoadInput.REAR_DETACH_LOAD_DONE];
        #endregion

        private MaterialStatus materialStatus => line == ECVLine.Front ? _materialStatusList.FrontSetInCvMaterialStatus
                                                                        : _materialStatusList.RearSetInCvMaterialStatus;
        /// <summary>
        /// Check Time Sensor In CV Start No detect 
        /// </summary>
        /// <returns></returns>
        private StopWatch TimeCheckSensorExist => line == ECVLine.Front ? _totalTackTime.FrontAssembleStopwatch
                                                                         : _totalTackTime.RearAssembleStopwatch;

        private bool IsLoadCvOccupied => In_LoadCvStart.Value || In_LoadCvEnd.Value;

        private bool CanRequestUpStreamLoad => ProcessMode == EProcessMode.Run
                                               && _machineStatus.IsInputStop == false
                                               && IsLoadCvOccupied == false;

        #region Override Methods
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
        //    switch ((ESetCVIn_OriginStep)Step.OriginStep)
        //    {
        //        case ESetCVIn_OriginStep.Start:
        //            Log.Debug("SetCVInProcess Origin Start");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVIn_OriginStep.CV_Stop:
        //            Cv_SetInput.Stop();
        //            Log.Debug($"{Cv_SetInput} Stopped");
        //            Step.OriginStep++;
        //            break;
        //        case ESetCVIn_OriginStep.End:
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
       
        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    break;
                case ESequence.AutoRun:
                    Sequence_AutoRun();
                    break;
                case ESequence.CVIn_Load:
                    if(_globalRecipe.UseInputAuto == true)
                    {
                        Sequence_CVIn_Load_Auto();
                    }
                    else
                        Sequence_CVIn_Load();
                    break;
                case ESequence.CVDetach_Load:
                    Sequence_CVIn_Unload();
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

        public override bool PreProcess()
        {
            // 1. UI: Update Material Status
            if (IsLoadCvOccupied) materialStatus.Set();
            else materialStatus.Clear();

            // 2. PreConveyor Run Condition
            if (In_PreCvSwitch.Value == false)
            {
                if (In_PreCvDetect.Value == false)
                {
                    Cv_PreSetLoad.Stop();
                }
                else
                {
                    Cv_PreSetLoad.Run();
                }
            }
            else
            {
                //Cv_PreSetLoad.Run();
            }

            // 3. Run 
            
            // Run Only Process in AutoRun Process mode
            if (In_PreCvSwitch.Value == true)
            {
                if ((In_LoadCvStart.Value == true && In_LoadCvEnd.Value == true) || (In_LoadCvStart.Value == true && ProcessMode != EProcessMode.Run) || (In_LoadCvStart.Value == true && Out_LoadCvOn.Value == false))
                {
                    Cv_PreSetLoad.Stop();
                }
                else
                {
                    Cv_PreSetLoad.Run();
                }
            }
            // TODO: Add option InputStop
            //if ((In_LoadCvStart.Value || _machineStatus.IsInputStop) && !_machineStatus.IsDryRunMode) Out_UpStreamLoadEnable.Value = false;
            //else Out_UpStreamLoadEnable.Value = true;
            //if (_globalRecipe.UsePreCV == false && In_UpStreamSignal.Value == true)
            //{
            //    Cv_SetInput.Stop();
            //}

            // IF With Pre MC
            switch ((ESetConveyorIn_PreProcessStep)Step.PreProcessStep)
            {
                case ESetConveyorIn_PreProcessStep.Start:
                    Step.PreProcessStep++;
                    break;
                case ESetConveyorIn_PreProcessStep.Check_Sensor_Detect:
                    if(_globalRecipe.UseInputAuto == true)
                    {
                        Step.PreProcessStep = (int)ESetConveyorIn_PreProcessStep.End;
                        break;
                    }
                    if (CanRequestUpStreamLoad == false)
                    {
                        Step.PreProcessStep++;
                        break;
                    }
                    TimeCheckSensorExist.RestartTime();
                    Step.PreProcessStep = (int)ESetConveyorIn_PreProcessStep.Wait_Sensor_No_Detect_Enough;
                    break;
                case ESetConveyorIn_PreProcessStep.UpStreamSignal_Off:
                    Log.Debug("UpStream Signal Off");
                    Out_UpStreamLoadEnable.Value = false;
                    Step.PreProcessStep = (int)ESetConveyorIn_PreProcessStep.Start;
                    break;
                case ESetConveyorIn_PreProcessStep.Wait_Sensor_No_Detect_Enough:
                    if (CanRequestUpStreamLoad == false)
                    {
                        Step.PreProcessStep = (int)ESetConveyorIn_PreProcessStep.Start;
                        break;
                    }
                    if (TimeCheckSensorExist.ElapsedSecond > 0.5)
                    {
                        Log.Debug("Sensor Off > 0.5s");
                        Step.PreProcessStep++;
                        break;
                    }
                    break;
                case ESetConveyorIn_PreProcessStep.UpStreamSignal_On:
                    Log.Debug("UpStream Signal On");
                    Out_UpStreamLoadEnable.Value = true;
                    Step.PreProcessStep++;
                    break;
                case ESetConveyorIn_PreProcessStep.End:
                    Step.PreProcessStep = (int)ESetConveyorIn_PreProcessStep.Start;
                    break;
            }
            // Check IF of Pre MC to Stop CV

            //switch ((ESetConveyorIn_IFStep)Step.PreProcessStep)
            //{
            //    case ESetConveyorIn_IFStep.Start:
            //        Step.PreProcessStep++;
            //        break;
            //    case ESetConveyorIn_IFStep.Wait_IF_On:
            //        if (In_UpStreamSignal.Value == true)
            //        {
            //            Step.PreProcessStep++;
            //            break;
            //        }
            //        break;
            //    case ESetConveyorIn_IFStep.CV_Stop:
            //        Cv_SetInput.Stop();
            //        Step.PreProcessStep++;
            //        break;
            //    case ESetConveyorIn_IFStep.Wait_IF_Off:
            //        if (In_UpStreamSignal.Value == false)
            //        {
            //            Step.PreProcessStep++;
            //            break;
            //        }
            //        break;
            //    case ESetConveyorIn_IFStep.CV_Run:
            //        //Cv_SetInput.Run();
            //        Step.PreProcessStep++;
            //        break;
            //    case ESetConveyorIn_IFStep.End:
            //        Step.PreProcessStep = (int)ESetConveyorIn_IFStep.Start;
            //        break;
            //}
            return base.PreProcess();
        }

        public override string ToString()
        {
            return line == ECVLine.Front ? EProcess.FrontSetCVIn.GetDescription()
                                             : EProcess.RearSetCVIn.GetDescription();
        }
        #endregion

        #region Private Methods
        private void Sequence_AutoRun()
        {
            if (In_LoadCvEnd.Value || _machineStatus.IsDryRunMode)
            {
                Sequence = ESequence.CVDetach_Load;
            }

            else
            {
                Sequence = ESequence.CVIn_Load;
            }
        }

        private void Sequence_Ready()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }

        private void Sequence_CVIn_Load()
        {
            switch ((ESetCVIn_LoadStep)Step.RunStep)
            {
                case ESetCVIn_LoadStep.Start:
                    if (_machineStatus.IsInputStop == true)
                    {
                        Wait(100);
                        break;
                    }

                    Log.Debug($"Set CV in load start");
                    Step.RunStep++;
                    break;
                case ESetCVIn_LoadStep.CV_Stop1:
                    Cv_SetInput.Stop();
                    Log.Debug($"Stop CV");
                    Step.RunStep++;
                    break;
                case ESetCVIn_LoadStep.CV_ConditionCheck:
                    if (In_LoadCvEnd.Value)
                    {
                        Cv_SetInput.Stop();
                        Sequence = ESequence.CVDetach_Load;
                        break;
                    }

                    if (In_LoadCvStart.Value)
                    {
                        Step.RunStep = (int)ESetCVIn_LoadStep.CV_TransferSet_ToEnd;
                        break;
                    }

                    //if (_globalRecipe.UsePreCV == false && In_UpStreamSignal.Value == true) break;
                    Cv_SetInput.Run();
                    Wait(10);
                    break;
                case ESetCVIn_LoadStep.CV_TransferSet_ToEnd:
                    Log.Debug($"Run CV to end");
                    Cv_SetInput.Run();
                    Wait(30000, () => In_LoadCvEnd.Value);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_LoadCvEnd, true);
#endif
                    Step.RunStep++;
                    break;
                case ESetCVIn_LoadStep.CV_EndDetect_Wait:
                    //if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    //{
                    //    Log.Error($"Set CV in load timeout");
                    //    RaiseWarning((int)EWarning.FrontINCV_SetLoad_Timeout);
                    //    break;
                    //}
                    Step.RunStep++;
                    break;
                case ESetCVIn_LoadStep.CV_Stop2:
                    Cv_SetInput.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVIn_LoadStep.End:
                    Log.Debug("Set CV in load End");

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
        private void Sequence_CVIn_Load_Auto()
        {
            switch ((ESetConveyerIn_LoadAutoStep)Step.RunStep)
            {
                case ESetConveyerIn_LoadAutoStep.Start:
                    if (_machineStatus.IsInputStop == true)
                    {
                        Wait(100);
                        break;
                    }

                    Log.Debug($"Set CV in load start");
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_Stop1:
                    Cv_SetInput.Stop();
                    Log.Debug($"Stop CV");
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_ConditionCheck:
                    if (In_LoadCvEnd.Value)
                    {
                        Cv_SetInput.Stop();
                        Sequence = ESequence.CVDetach_Load;
                        break;
                    }

                    if (In_LoadCvStart.Value)
                    {
                        Step.RunStep = (int)ESetConveyerIn_LoadAutoStep.CV_TransferSet_ToEnd;
                        break;
                    }

                    //if (_globalRecipe.UseInputAuto == false && In_UpStreamSignal.Value == true) break;
                    Cv_SetInput.Stop();
                    Step.RunStep = (int)ESetConveyerIn_LoadAutoStep.Send_IF_PreMC_On;
                    Wait(10);
                    break;
                case ESetConveyerIn_LoadAutoStep.Send_IF_PreMC_On:
                    Out_UpStreamLoadEnable.Value =true;
                    Log.Debug("Send IF PreMC On");
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_Wait_IF_PreMC_On:
                    if (In_UpStreamSignal.Value == true)
                    {
                        Log.Debug("IF PreMC On");
                        Step.RunStep++;
                        break;
                    }
                    //if (In_LoadCvEnd.Value)
                    //{
                    //    Cv_SetInput.Stop();
                    //    Step.RunStep = (int)ESetConveyerIn_LoadAutoStep.Send_IF_PreMC_Off;
                    //    break;
                    //}

                    //if (In_LoadCvStart.Value)
                    //{
                    //    Log.Debug("Set already detected on load CV start while waiting IF PreMC On");
                    //    Step.RunStep = (int)ESetConveyerIn_LoadAutoStep.Send_IF_PreMC_Off;
                    //    break;
                    //}                 
                    Wait(10);
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_Stop2:
                    Cv_SetInput.Stop();
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_Wait_IF_PreMC_Off:
                    if (In_UpStreamSignal.Value == false)
                    {
                        Cv_SetInput.Stop();
                        Log.Debug("IF PreMC Off");
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case ESetConveyerIn_LoadAutoStep.Send_IF_PreMC_Off:
                    Out_UpStreamLoadEnable.Value = false;
                    Log.Debug("Send IF PreMC Off");
                    Step.RunStep++;
                    break;

                case ESetConveyerIn_LoadAutoStep.CV_Wait_LoadCvStart:
                    if (In_LoadCvEnd.Value)
                    {
                        Cv_SetInput.Stop();
                        Sequence = ESequence.CVDetach_Load;
                        break;
                    }
                    if (In_LoadCvStart.Value)
                    {
                        Log.Debug("Set detected on load CV start");
                        Step.RunStep++;
                        break;
                    }

                    Cv_SetInput.Stop();
                    Step.RunStep = (int)ESetConveyerIn_LoadAutoStep.CV_ConditionCheck;
                    Wait(10);
                    break;

                case ESetConveyerIn_LoadAutoStep.CV_TransferSet_ToEnd:
                    Log.Debug($"Run CV to end");
                    Cv_SetInput.Run();
                    Wait(30000, () => In_LoadCvEnd.Value);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_LoadCvEnd, true);
#endif
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_EndDetect_Wait:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        EWarning eWarning = line == ECVLine.Front ? EWarning.FrontINCV_SetLoad_Timeout
                                                                   : EWarning.RearINCV_SetLoad_Timeout;
                        RaiseWarning((int)eWarning);
                        Log.Error($"Set CV in load timeout");
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.CV_Stop3:
                    Cv_SetInput.Stop();
                    Step.RunStep++;
                    break;
                case ESetConveyerIn_LoadAutoStep.End:
                    Log.Debug("Set CV in load End");

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

        private void Sequence_CVIn_Unload()
        {
            switch ((ESetCVIn_UnloadStep)Step.RunStep)
            {
                case ESetCVIn_UnloadStep.Start:
                    Log.Debug($"Set CV in detach unload start");
                    Step.RunStep++;
                    break;
                case ESetCVIn_UnloadStep.CVDetach_Request_Wait:
                    if (FlagIn_DetachLoadRequest && (_machineStatus.IsInputStop == false))
                    {
                        Log.Debug($"Receive detach load request");
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case ESetCVIn_UnloadStep.CVIn_SetUnloadStart:
                    Cv_SetInput.Run();
                    Step.RunStep++;
                    break;
                case ESetCVIn_UnloadStep.CVDetach_SetReceive_Wait:
                    if (FlagIn_DetachLoadRequest == false)
                    {
                        Log.Debug($"Set CV detach new set detected.");

                        //Wait(100);
                        Step.RunStep++;
                        break;
                    }
                    Wait(10);
                    break;
                case ESetCVIn_UnloadStep.CVIn_Stop_Delay:
                    Log.Debug("Set Work Out CV Delay");
                    //Wait(_setCVRecipe.SetOutWorkAreaWait);
                    Step.RunStep++;
                    break;
                case ESetCVIn_UnloadStep.CVIn_Stop:
                    Log.Debug($"Set CV in stop.");
                    Cv_SetInput.Stop();
                    Step.RunStep++;
                    break;
                case ESetCVIn_UnloadStep.Wait_Set_OutDone:
                    if (FlagIn_DetachLoadDone == false)
                    {
                        Wait(10);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ESetCVIn_UnloadStep.End:
                    Log.Debug($"Set CV in unload done.");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.CVIn_Load;
                    break;
                default:
                    break;
            }
        }
        private void StopRun()
        {
            Cv_SetInput.Stop();
            Cv_PreSetLoad.Stop();
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
        }

        #endregion

        #region Constructors
        public SetConveyerInProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            EDMLogger edmLogger,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            TotalTackTime totalTackTime,
            [FromKeyedServices("FrontCvSetLoadInput")] IDInputDevice<EFrontInCvSetLoadInput> frontInCvSetLoadInput,
            [FromKeyedServices("RearCvSetLoadInput")] IDInputDevice<ERearInCvSetLoadInput> rearInCvSetLoadInput)
        {
            _totalTackTime = totalTackTime;
            _edmLogger = edmLogger;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _materialStatusList = materialStatusList;
            _frontInCvSetLoadInput = frontInCvSetLoadInput;
            _rearInCvSetLoadInput = rearInCvSetLoadInput;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _frontInCvSetLoadInput;
        private readonly IDInputDevice _rearInCvSetLoadInput;
        private readonly MaterialStatusList _materialStatusList;
        string[] strEDMPara = new string[4];
        private int m_nTowerLampCurrentData = 0;
        private int m_nTowerLampPreviousData = 0;
        public Stopwatch m_swSaveEDMLog = new();
        private TotalTackTime _totalTackTime;

        private EDMLogger _edmLogger;
        private SetConveyorRecipe _setCVRecipe => _recipeList.SetConveyorRecipe;
        #endregion

    }
}
