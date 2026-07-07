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
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Animation;

namespace FrontCameraAssembleEquipment.Process
{
    public class TrayInCVProcess : ChangeReadyProcess
    {
        #region Inputs
        private IDInput In_TrayInCv1DetectStart => _devices.Inputs.TrayInCv1DetectStart;
        private IDInput In_TrayInCv1DetectEnd => _devices.Inputs.TrayInCv1DetectEnd;
        private IDInput In_TrayInCv1DetectExist => _devices.Inputs.TrayInCv1DetectExist;
        protected override IDInput In_LightCurtain => _devices.Inputs.AreaSensorDetect;
        protected override IDInput In_LightCurtainMuting => _devices.Inputs.LightCurtainMutingSW;
        #endregion

        #region Outputs
        protected override IDOutput AreaSensorBypassOn => _devices.Outputs.AreaSensorBypassOn;
        protected override IDOutput Out_MutingSWLamp => _devices.Outputs.MutingLamp;
        //private IDOutput Out_AgvRequest => _devices.Outputs.LoadAGVRequest;
        #endregion

        #region Cylinders
        private ICylinder Cyl_TrayInStopperUpDown => _devices.Cylinders.TraySupplier_TrayInStopper;
        #endregion

        #region Conveyors
        private IConveyor Cv_TrayInExternal => _devices.CVs.TrayIn_ExternalCv;
        private IConveyor Cv_TrayIn => _devices.CVs.TrayInConveyor;
        #endregion

        #region Motions

        #endregion

        #region Rollers
        private BD201SRollerController Roller_TrayInCV => _devices.RollerList.TrayInCVRoller;
        #endregion

        #region Flags
        private bool Flag_TrayInElevator_Load_Request
        {
            get => _trayInCvInput[(int)ETrayInCvInput.TRAY_INPUT_ELEVATOR_REQUEST];
        }

        #endregion

        #region Override Methods
        public override bool PreProcess()
        {
            if (Flag_TrayInElevator_Load_Request && _machineStatus.IsDryRunMode)
            {
                Sequence = ESequence.TrayInElevator_Load;
            }
            return base.PreProcess();
        }
        //public override bool ProcessOrigin()
        //{
        //    switch ((ETrayInCV_OriginStep)Step.OriginStep)
        //    {
        //        case ETrayInCV_OriginStep.Start:
        //            Log.Debug("Tray In CV Origin Start");
        //            Roller_TrayInCV.Stop();
        //            Step.OriginStep++;
        //            break;
        //        case ETrayInCV_OriginStep.Cylinder_Up:
        //            Cyl_TrayInStopperUpDown.Forward();
        //            Wait((int)(_globalRecipe.CylinderMoveTimeout * 1000), () => Cyl_TrayInStopperUpDown.IsForward);
        //            Log.Debug("Tray In CV Stopper Up");
        //            Step.OriginStep++;
        //            break;
        //        case ETrayInCV_OriginStep.Cylinder_Up_Check:
        //            if (WaitTimeOutOccurred)
        //            {
        //                RaiseWarning((int)EWarning.Cylinder_MoveUp_Timeout);
        //                break;
        //            }

        //            Log.Debug("Tray In CV Stopper Up Done");
        //            Step.OriginStep++;
        //            break;
        //        case ETrayInCV_OriginStep.End:
        //            Log.Debug("Origin End");
        //            ProcessStatus = EProcessStatus.OriginDone;
        //            Step.OriginStep++;
        //            break;
        //        default:
        //            Wait(20);
        //            break;
        //    }
        //    return true;
        //}
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
            if (Sequence != ESequence.Stop && ProcessStatus != EProcessStatus.OriginDone && Sequence != ESequence.None)
            {
                StopRun();
            }
            return base.ProcessToStop();
        }

        public override bool ProcessToRun()
        {
            switch ((ETrayInCV_ToRunStep)Step.ToRunStep)
            {
                case ETrayInCV_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETrayInPutElevator_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayInCV_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ETrayInCvOutput>)_trayInCvOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ETrayInCV_ToRunStep.SetSpeedAccDeccForRoller:
                    //RollerSetSpeed();
                    //RollerSetAcc();
                    //RollerSetDec();
                    Log.Debug("Set Speed Acc Dec For Roller Tray In CV");
                    Step.ToRunStep++;
                    break;
                case ETrayInCV_ToRunStep.End:
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
                case ESequence.TrayInCV_Load:
                    Sequence_TrayInCV_Load();
                    break;
                case ESequence.TrayInElevator_Load:
                    Sequence_TrayInElevator_Load();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.Change:
                    Sequence_Change();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }

            return true;
        }
        private void Sequence_Ready()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            ((MappableOutputDevice<ETrayInCvOutput>)_trayInCvOutput).ClearOutputs();
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }
        public void Sequence_Change()
        {

        }
        public override string ToString()
        {
            return EProcess.TrayInCV.GetDescription();
        }
        #endregion

        #region Constructors
        public TrayInCVProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            RecipeSelector recipeSelector,
            DevRecipe devRecipe,
            ARM arm,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer,
            [FromKeyedServices("TrayInCvInput")] IDInputDevice<ETrayInCvInput> trayInCvInput,
            [FromKeyedServices("TrayInCvOutput")] IDOutputDevice<ETrayInCvOutput> trayInCvOutput) : base(globalRecipe, machineStatus, devices, blinkTimer)
        {
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _recipeSelector = recipeSelector;
            _devRecipe = devRecipe;
            _arm = arm;
            _trayInCvInput = trayInCvInput;
            _trayInCvOutput = trayInCvOutput;

            _recipeSelector.RecipeSaved += RollerSet;
        }
        #endregion

        #region Privates
        private void Sequence_AutoRun()
        {
            if (_machineStatus.IsByPassMode) return;
#if SIMULATION
            SimulationInputSetter.SetSimInput(In_TrayInCv1DetectExist, true);
#endif

            if ((In_TrayInCv1DetectExist.Value == true || In_TrayInCv1DetectEnd.Value == true) && _devices.Inputs.TrayInCv2DetectExist.Value == false)
            {
                // CV -> Elevator
                Sequence = ESequence.TrayInElevator_Load;
            }
            else if ((In_TrayInCv1DetectStart.Value || In_TrayInCv1DetectExist.Value || In_TrayInCv1DetectEnd.Value) == false)
            {
                // AGV / OP -> CV
                Sequence = ESequence.TrayInCV_Load;
            }
        }
        private void Sequence_TrayInCV_Load()
        {
            switch ((ETrayInCV_LoadStep)Step.RunStep)
            {
                case ETrayInCV_LoadStep.Start:
                    Log.Debug("Tray In CV Load Start");
                    Task.Run(() =>
                    {
                        RollerRunStop(false);
                    });
                    Cv_TrayInExternal.Stop();
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.Stopper_Up:
                    if (Cyl_TrayInStopperUpDown.IsForward)
                    {
                        Log.Debug("Stopper Up Done");
                        Step.RunStep = (int)ETrayInCV_LoadStep.AGV_Call;
                        break;
                    }
                    Cyl_TrayInStopperUpDown.Forward();
                    Wait((int)(10000), () => Cyl_TrayInStopperUpDown.IsForward || _machineStatus.IsDryRunMode);
                    break;
                case ETrayInCV_LoadStep.Stopper_Up_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINBuffer_StopperUp_Timeout);
                        break;
                    }
                    Log.Debug("Stopper Up Done");
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.AGV_Call:
                    if (_devRecipe.UseAGV == false)
                    {
                        Step.RunStep = (int)ETrayInCV_LoadStep.Wait_TrayDetect;
                        break;
                    }

                    //Log.Info("Agv Requesting!");
                    //if (_devices.AGV.CallAGV(true) == false)
                    //{
                    //    RaiseWarning((int)EWarning.AGV_NetworkCommunication_Fail);
                    //    break;
                    //}

                    _arm.RequestARMInput(true);
                    Step.RunStep++;
                    Log.Debug("Agv Requested!");
                    break;
                case ETrayInCV_LoadStep.AGV_Arrival_Check:
                    if (_devices.Inputs.InAGV_InWorkPosition.Value == false)
                    {
                        Wait(50);
                        break;
                    }

                    _devices.Outputs.LoadAGV_MachineReady.Value = true;
                    Log.Debug("Agv Arrival!");
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.AGV_SendWorkPermission:
                    if (false) // TODO: Confirm before work? Sensor? RaisingWarning?
                    {
                        Wait(50);
                        break;
                    }

                    _devices.Outputs.LoadAGV_PermitToWork.Value = true;
                    Log.Debug("Agv Send Work Permission!");

                    Wait(5000, () => _devices.Inputs.InAGV_StartWork.Value == true);
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.AGV_Wait_StartWorkSignal:
                    if (WaitTimeOutOccurred)
                    {
                        // Could not received AGV's Start work signal
                        RaiseWarning((int)EWarning.AGV_PIOCommunication_Fail);
                        break;
                    }
                    Task.Run(() =>
                    {
                        RollerRunStop(true);
                    });
                    Cv_TrayInExternal.Run();
                    Log.Debug("Agv Received Start Work Signal!");
                    _devices.Outputs.LoadAGV_PermitToWork.Value = false;
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.Wait_TrayDetect:
                    if (In_TrayInCv1DetectStart.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        Wait(20);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.Enable_LightCurtainMuting:
                    _devices.Outputs.AreaSensorBypassOn.Value = true;
                    Log.Debug("Light curtain muting on");
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.CV_Run:
                    Task.Run(() =>
                    {
                        RollerRunStop(true);
                    });
                    Cv_TrayInExternal.Run();
                    Wait((int)(_globalRecipe.CVMoveTimeout * 1000), () => In_TrayInCv1DetectEnd.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.TrayInCV_DetectEnd_Wait:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayINBuffer_Detect_End_Timeout);
                        break;
                    }

                    Log.Debug("Tray reached the end of TrayInCV1.");
                    Log.Debug("Light curtain muting off");
                    _devices.Outputs.AreaSensorBypassOn.Value = false;
                    Wait(500);
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.CV_Stop:
                    Task.Run(() =>
                    {
                        RollerRunStop(false);
                    });

                    Cv_TrayInExternal.Stop();
                    Log.Debug($"Roller CV stopped!");
                    _devices.Outputs.TowerLamp_Run();
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.AGV_SendGoPermission:
                    if (_devRecipe.UseAGV == false)
                    {
                        Step.RunStep = (int)ETrayInCV_LoadStep.End;
                        break;
                    }
                    _arm.RequestARMInput(false);
                    Log.Debug("Agv Send WorkDone");
                    _devices.Outputs.LoadAGV_GoPermission.Value = true;
                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.AGV_GoPermissionConfirm_Wait:
                    //if (WaitTimeOutOccurred)
                    //{
                    //    // Could not received AGV's Go Permission Confirm signal
                    //    RaiseWarning((int)EWarning.AGV_PIOCommunication_Fail);
                    //    break;
                    //}

                    // Clear AGV Call Request
                    //if (_devices.AGV.CallAGV(false) == false)
                    //{
                    //    RaiseWarning((int)EWarning.AGV_NetworkCommunication_Fail);
                    //    break;
                    //}

                    Step.RunStep++;
                    break;
                case ETrayInCV_LoadStep.End:
                    Log.Debug("Tray In CV Load End");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.AutoRun;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_TrayInElevator_Load()
        {
            switch ((ETrayInCV_UnloadStep)Step.RunStep)
            {
                case ETrayInCV_UnloadStep.Start:
                    Log.Debug("Tray In CV Unload Start");
                    Task.Run(() => 
                    {
                        RollerRunStop(false);
                    });
                    Cv_TrayInExternal.Stop();
                    Log.Debug("Wait Tray In Elevator Request Load");
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.TrayInElevator_TrayRequest_Wait:
                    if (Flag_TrayInElevator_Load_Request == true)
                    {
                        Log.Debug("Tray In CV Unload Tray In Elevator Request");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ETrayInCV_UnloadStep.Stopper_MoveDown:
                    Cyl_TrayInStopperUpDown.Backward();
                    Wait((10000), () => Cyl_TrayInStopperUpDown.IsBackward);
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.Stopper_Down_Check:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayINBuffer_StopperDown_Timeout);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.CV_Run:
                    Task.Run(() => 
                    {
                        RollerRunStop(true);
                    });
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.Tray_Unload_Done_Check:
                    if ((In_TrayInCv1DetectEnd.Value == false && Flag_TrayInElevator_Load_Request == false) || _machineStatus.IsDryRunMode)
                    {
                        Wait(2000);
                        Step.RunStep++;
                        break;
                    }

                    Wait(10);
                    break;
                case ETrayInCV_UnloadStep.CV_Stop:
                    Task.Run(() => 
                    {
                        RollerRunStop(true);
                    });
                    Cv_TrayInExternal.Stop();
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.Stopper_MoveUp:
                    Cyl_TrayInStopperUpDown.Forward();
                    Wait(10000, () => Cyl_TrayInStopperUpDown.IsForward);
                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.Stopper_Up_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINBuffer_StopperUp_Timeout);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayInCV_UnloadStep.End:
                    Log.Debug("Tray In CV Unload End");

                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.TrayInCV_Load;
                    break;
                default:
                    break;
            }
        }
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
        private void RollerSetSpeed()
        {
            Roller_TrayInCV.SetSpeed((int)_traySuplierRecipe.CVTrayInSpeed);
        }
        private void RollerSetAcc()
        {
            Roller_TrayInCV.SetAcceleration((int)_traySuplierRecipe.Acceleration);
        }
        private void RollerSetDec()
        {
            Roller_TrayInCV.SetDeceleration((int)_traySuplierRecipe.Deceleration);
        }

        private void RollerRunStop(bool isRun)
        {
            if (isRun)
            {
                if(_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayInConveyor.Run();
                    return;
                }
                Roller_TrayInCV.Run();
            }
            else
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayInConveyor.Stop();
                    return;
                }
                Roller_TrayInCV.Stop();
            }
        }
        private void StopRun()
        {
            Task.Run(() =>
            {
                Log.Debug("Stop Run");
                ((ProcessTimer)ProcessTimer).WaitTime = 0;
                RollerRunStop(false);
                Log.Debug("Stop Roller Done");
                Cv_TrayInExternal.Stop();
                Log.Debug("Stop Conveyor Done");
            });
        }      
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _trayInCvInput;
        private readonly IDOutputDevice _trayInCvOutput;
        private RecipeSelector _recipeSelector;
        private readonly DevRecipe _devRecipe;
        private readonly ARM _arm;

        private TraySuplierRecipe _traySuplierRecipe => _recipeList.TraySuplierRecipe;
        #endregion
    }
}
