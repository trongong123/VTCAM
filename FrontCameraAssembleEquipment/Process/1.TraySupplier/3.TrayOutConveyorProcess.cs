using EQX.Core.Common;
using EQX.Core.Device.SpeedController;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Device.SpeedController;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Documents;

namespace FrontCameraAssembleEquipment.Process
{
    public class TrayOutCVProcess : ChangeReadyProcess
    {
        #region Inputs

        private IDInput In_TrayOutCv1DetectStart => _devices.Inputs.TrayOutCv1DetectStart;
        private IDInput In_TrayOutCv1DetectEnd => _devices.Inputs.TrayOutCv1DetectEnd;
        private IDInput In_TrayOutElevatorEnd => _devices.Inputs.TrayOutCv2DetectEnd;
        protected override IDInput In_LightCurtainMuting => _devices.Inputs.LightCurtainMutingSW;
        protected override IDInput In_LightCurtain => _devices.Inputs.AreaSensorDetect;
        private IDInput In_UnloadAGVArrival => _devices.Inputs.UnloadAGVArrival;
        private IDInput In_UnloadAGVCvRun => _devices.Inputs.UnloadAGVCvRun;
        private IDInput In_UnloadAGVComplete => _devices.Inputs.UnloadAGVComplete;
        #endregion

        #region Outputs
        private IDOutput Out_UnloadAGVReady => _devices.Outputs.UnloadAGVReady;
        private IDOutput Out_UnloadAGVReadyInput => _devices.Outputs.UnloadAGVReadyInput;
        private IDOutput Out_UnloadAGVTranferDone => _devices.Outputs.UnloadAGVTranferDone;

        protected override IDOutput AreaSensorBypassOn => _devices.Outputs.AreaSensorBypassOn;
        protected override IDOutput Out_MutingSWLamp => _devices.Outputs.MutingLamp;
        private IDOutput Buffer_OutCV => _devices.Outputs.TrayOutExtCvRun;
        //private IDOutput Out_AgvRequestUnload => _devices.Outputs.UnloadAGVRequest;

        #endregion

        #region Cylinders
        #endregion

        #region Conveyors
        private IConveyor Cv_TrayOutExternal => _devices.CVs.TrayOut_ExternalCv;
        private IConveyor Cv_TrayOut => _devices.CVs.TrayOutConveyor;

        #endregion

        #region Rollers
        private BD201SRollerController Roller_TrayOutCV => _devices.RollerList.TrayOutCVRoller;
        #endregion

        #region Flags
        ///
        /// Input
        /// 
        private bool FlagIn_TrayOutElevatorRequest
        {
            get => _trayOutCVInput[(int)ETrayOutCvInput.TRAY_OUT_ELEVATOR_REQUEST];
        }
        /// <summary>
        /// OutPut
        /// </summary>
        private bool FlagOut_TrayOutCVReady
        {
            set => _trayOutCVOutput[(int)ETrayOutCvOutput.TRAY_OUT_ELEVATOR_READY] = value;
        }


        #endregion

        #region Override Methods
        public override bool ProcessToRun()
        {
            switch ((ETrayOutputCV_ToRunStep)Step.ToRunStep)
            {
                case ETrayOutputCV_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETrayOutputCV_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayOutputCV_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ETrayOutCvOutput>)_trayOutCVOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ETrayOutputCV_ToRunStep.SetSpeedAccDecForRoller:
                    //RollerSetSpeed();
                    //RollerSetAcc();
                    //RollerSetDec();
                    Log.Debug("Set Speed Acc Dec For Roller Tray Out CV");
                    Step.ToRunStep++;
                    break;
                case ETrayOutputCV_ToRunStep.End:
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
        //    switch ((ETrayOutputCV_OriginStep)Step.OriginStep)
        //    {
        //        case ETrayOutputCV_OriginStep.Start:
        //            Log.Debug("TrayOutCV Origin Start");
        //            Step.OriginStep++;
        //            break;
        //        case ETrayOutputCV_OriginStep.Roller_Stop:
        //            Roller_TrayOutCV.Stop();
        //            Log.Debug($"{Roller_TrayOutCV.Name} Stopped");
        //            Step.OriginStep++;
        //            break;
        //        case ETrayOutputCV_OriginStep.End:
        //            Log.Debug("Origin End");
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

            if (Sequence != ESequence.Stop && ProcessStatus != EProcessStatus.OriginDone && Sequence != ESequence.None)
            {
                StopRun();
                ((MappableOutputDevice<ETrayOutCvOutput>)_trayOutCVOutput).ClearOutputs();

            }

            return base.ProcessToStop();
        }
        public override bool ProcessToAlarm()
        {
            ;
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
                case ESequence.TrayOutElevator_Unload:
                    Sequence_TrayOutCV_Load();
                    break;
                case ESequence.TrayOutCV_Unload:
                    Sequence_TrayOutCV_Unload();
                    break;
                case ESequence.Change:
                    Sequence_Change();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }
            CurrentSequence = Sequence;
            CurrentStep = Step.RunStep;
            return true;
        }
        private void Sequence_Ready()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            ((MappableOutputDevice<ETrayOutCvOutput>)_trayOutCVOutput).ClearOutputs();
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }

        public override string ToString()
        {
            return EProcess.TrayOutCV.GetDescription();
        }
        private void Sequence_AutoRun()
        {
            if (_machineStatus.IsByPassMode) return;

            if (In_TrayOutCv1DetectEnd.Value == true)
            {
                // Tray detected at the end sensor, start the CV to unload the tray
                Sequence = ESequence.TrayOutCV_Unload;
            }
            else
            {
                // No tray detected, stop the CV
                Sequence = ESequence.TrayOutElevator_Unload;
            }
        }
        private void Sequence_Change()
        {
            Out_MutingSWLamp.Value = true;
        }
        private void Sequence_TrayOutCV_Load()
        {
            switch ((ETrayOutputCV_LoadStep)Step.RunStep)
            {
                case ETrayOutputCV_LoadStep.Start:
                    Log.Debug("TrayOutCV Load Step Start");
                    Task.Run(() =>
                    {
                        RollerRunStop(false);
                    });
                    Cv_TrayOutExternal.Stop();
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.Check_Tray_Out_CV_Detect_Start:
                    if ((In_TrayOutCv1DetectStart.Value == true && In_TrayOutCv1DetectEnd.Value == false && In_TrayOutElevatorEnd.Value == true) || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Tray Out CV1 SenSor End Not Detect => CV Run");
                        Step.RunStep++;
                        break;
                    }
                    if ((In_TrayOutCv1DetectStart.Value == true || In_TrayOutCv1DetectEnd.Value == true) || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Tray Out CV1 SenSor End Detect End");
                        Step.RunStep = (int)ETrayOutputCV_LoadStep.End;
                    }
                    Step.RunStep = (int)ETrayOutputCV_LoadStep.Tray_Out_CV_Request;
                    break;
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Run:
                    AreaSensorBypassOn.Value = true;
                    Task.Run(() => 
                    {
                        RollerRunStop(true);
                    });
                    Cv_TrayOutExternal.Run();
                    if (In_TrayOutElevatorEnd.Value) // Check Tray Exist On Elevator
                    {
                        FlagOut_TrayOutCVReady = true; // Request to load tray
                    }
                    Wait(10000, () => In_TrayOutCv1DetectEnd.Value == true || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.Wait_Tray_Out_CV_Detect_End:
                    if (WaitTimeOutOccurred) // If waiting for tray to reach end sensor times out
                    {
                        Log.Warn("TrayOutCV Load Step: Timeout waiting for tray to reach end sensor");
                        Task.Run(() => 
                        {
                            RollerRunStop(false);
                        }); // Stop the CV on timeout
                        Cv_TrayOutExternal.Stop();
                        RaiseWarning((int)EWarning.TrayOUTBuffer_Detect_End_Timeout);
                        break;
                    }

                    Log.Debug("Tray Out CV1 SenSor End Detect => CV Stop");
                    RollerRunStop(false);
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Stop:
                    AreaSensorBypassOn.Value = false;
                    Step.RunStep = (int)ETrayOutputCV_LoadStep.End;
                    break;
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Request:
                    Log.Debug("Requesting Tray Out Elevator to load tray");
                    FlagOut_TrayOutCVReady = true; // Request to load tray
                    Step.RunStep++;
                    Log.Debug("Waiting for Tray Out Elevator to be ready...");
                    break;
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Request_Check:
                    if (FlagIn_TrayOutElevatorRequest == true) // Check if Tray Out Elevator is Ready Out
                    {
                        Log.Debug("TrayOutCV Load Step: Tray Out Elevator is ready to tranfer tray");
                        Wait(500);
                        Step.RunStep++; // Proceed to next step
                    }
                    break; // Still not ready, wait                   
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Run_Again:
                    Log.Debug("TrayOutCV Load Step: Starting Tray Out CV to load tray");
                    AreaSensorBypassOn.Value = true;
                    RollerRunStop(true); // Start the CV to load tray
                    Cv_TrayOutExternal.Run();
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_TrayOutCv1DetectEnd, true);
#endif
                    Wait(10000, () => In_TrayOutCv1DetectEnd.Value == true || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.Wait_Tray_Out_CV_Detect_Sensor_End:
                    if (WaitTimeOutOccurred) // Wait until the tray reaches the end sensor
                    {
                        Task.Run(() => 
                        {
                            RollerRunStop(false);
                        }); // Stop the CV on timeout
                        Cv_TrayOutExternal.Stop();
                        RaiseWarning((int)EWarning.TrayOUTBuffer_Detect_End_Timeout);
                        break;
                    }

                    Log.Debug("TrayOutCV Load Step: Tray has reached the end sensor");
                    RollerRunStop(false);
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.Tray_Out_CV_Stop_Again:
                    AreaSensorBypassOn.Value = false;
                    Cv_TrayOutExternal.Stop();
                    Log.Debug("TrayOutCV Load Step: Tray Out CV has stopped");
                    _trayOutCVOutput[(int)ETrayOutCvOutput.TRAY_OUT_ELEVATOR_READY] = false; // Reset the request signal
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_LoadStep.End:
                    Log.Debug("TrayOutCV Load Step End");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.TrayOutCV_Unload;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayOutCV_Unload()
        {
            switch ((ETrayOutputCV_UnLoadStep)Step.RunStep)
            {
                case ETrayOutputCV_UnLoadStep.Start:
                    {
                        Log.Debug("Tray Out Unload Start");
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Cv_Stop:
                    {
                        Log.Debug("Roller Stop");
                        Task.Run(() => 
                        {
                            RollerRunStop(false); 
                        });
                        Cv_TrayOutExternal.Stop();
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Request_Agv:
                    {
                        if (_devRecipe.UseAGV == false)
                        {
                            Step.RunStep = (int)ETrayOutputCV_UnLoadStep.End;
                            break;
                        }
                        Log.Debug("Request Agv Unload Tray");
                        Log.Debug("Agv Requesting!");
                        //if (_devices.AGV.CallAGV(true) == false)
                        //{
                        //    RaiseWarning((int)EWarning.AGV_NetworkCommunication_Fail);
                        //    break;
                        //}
                        _arm.RequestARMOutput(true);
                        Step.RunStep++;
                        Log.Debug("Agv Requested!");
                        break;
                    }
                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Wait_Agv_Ready:
                    {
                        if (In_UnloadAGVArrival.Value)
                        {
                            Log.Debug("Agv Ready to Unload Tray");
                            Out_UnloadAGVReady.Value = true;
                            Out_UnloadAGVReadyInput.Value = true;
                            Step.RunStep++;
                            break;
                        }
                        Wait(10);
                        break; ;
                    }

                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Run:
                    {
                        if (In_UnloadAGVCvRun.Value == true)
                        {
                            Log.Debug("Tray Out C/V Run");
                            Cv_TrayOutExternal.Run();
                            Wait(12000, () => (In_TrayOutCv1DetectStart.Value == false) && (In_TrayOutCv1DetectEnd.Value == false));
                            Step.RunStep++;
                            break;
                        }
                        Wait(10);
                        break; ;
                    }
                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Wait_Unload_Done:
                    {
                        if (WaitTimeOutOccurred)
                        {
                            RaiseWarning((int)EWarning.TrayOUTBuffer_Detect_End_Timeout);
                            break;
                        }
                        Log.Debug("Unload Tray Done");
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputCV_UnLoadStep.Tray_Out_CV_Stop_End:
                    {
                        Out_UnloadAGVTranferDone.Value = true;
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputCV_UnLoadStep.Wait_AGV_UnloadTray:
                    if (In_UnloadAGVArrival.Value == true)
                    {
                        Wait(20);
                        break;
                    }
                    Log.Debug("Tray Out CV Stop");
                    Cv_TrayOutExternal.Stop();

                    Out_UnloadAGVReady.Value = false;
                    Out_UnloadAGVReadyInput.Value = false;
                    Out_UnloadAGVTranferDone.Value = false;
                    _arm.RequestARMOutput(false);
                    Step.RunStep++;
                    break;
                case ETrayOutputCV_UnLoadStep.End:
                    {
                        Log.Debug("Tray Out CV Unload End");
                        if (Parent?.Sequence != ESequence.AutoRun)
                        {
                            Sequence = ESequence.Stop;
                            break;
                        }
                        Sequence = ESequence.TrayOutElevator_Unload;
                        break;
                    }
            }
        }
        #endregion

        #region Constructors
        public TrayOutCVProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            RecipeSelector recipeSelector,
            DevRecipe devRecipe,
            ARM arm,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer,
            [FromKeyedServices("TrayOutCvInput")] IDInputDevice<ETrayOutCvInput> trayOutCVInput,
            [FromKeyedServices("TrayOutCvOutput")] IDOutputDevice<ETrayOutCvOutput> trayOutCVOutput) : base(globalRecipe, machineStatus, devices, blinkTimer)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _recipeSelector = recipeSelector;
            _devRecipe = devRecipe;
            _arm = arm;
            _trayOutCVInput = trayOutCVInput;
            _trayOutCVOutput = trayOutCVOutput;
            _recipeSelector.RecipeSaved += RollerSet;
        }
        #endregion

        #region Privates

        private async void RollerSet()
        {
            await Task.Run(() =>
            {
                if (_devRecipe.IsUseRollerIOControl) return;

                RollerSetSpeed();
                RollerSetAcc();
                RollerSetDec();
            });
        }

        private void RollerRunStop(bool isRun)
        {
            if (isRun)
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayOutConveyor.Run();
                    return;
                }
                Roller_TrayOutCV.Run();
            }
            else
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayOutConveyor.Stop();
                    return;
                }
                Roller_TrayOutCV.Stop();
            }
        }
        private void RollerSetSpeed()
        {
            Roller_TrayOutCV.SetSpeed((int)_traySuplierRecipe.CVTrayOutSpeed);
        }
        private void RollerSetAcc()
        {
            Roller_TrayOutCV.SetAcceleration((int)_traySuplierRecipe.Acceleration);
        }
        private void RollerSetDec()
        {
            Roller_TrayOutCV.SetDeceleration((int)_traySuplierRecipe.Deceleration);
        }
        private void StopRun()
        {
            Task.Run(() =>
            {
                ((ProcessTimer)ProcessTimer).WaitTime = 0;
                Log.Debug("Stop Run");
                RollerRunStop(false);
                Log.Debug("Stop Roller Done");
                Cv_TrayOutExternal.Stop();
                Log.Debug("Stop CV Done");
            });
        }
        private readonly IDInputDevice _trayOutCVInput;
        private readonly IDOutputDevice _trayOutCVOutput;
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private RecipeSelector _recipeSelector;
        private readonly DevRecipe _devRecipe;
        private readonly ARM _arm;

        private TraySuplierRecipe _traySuplierRecipe => _recipeList.TraySuplierRecipe;
        #endregion
    }
}
