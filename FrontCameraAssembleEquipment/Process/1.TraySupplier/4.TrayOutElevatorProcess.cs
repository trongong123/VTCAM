using EQX.Core.Common;
using EQX.Core.Device.SpeedController;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Device.SpeedController;
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
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;

namespace FrontCameraAssembleEquipment.Process
{
    public class TrayOutElevatorProcess : ChangeReadyProcess
    {
        #region Inputs
        private IDInput In_TrayOutCVDetectStart => _devices.Inputs.TrayOutCv1DetectStart;
        private IDInput In_TrayOutCv2DetectStart => _devices.Inputs.TrayOutCv2DetectStart;
        private IDInput In_TrayOutCv2DetectEnd => _devices.Inputs.TrayOutCv2DetectEnd;
        private IDInput In_TrayOutCv2DetectExist => _devices.Inputs.TrayOutCv2DetectExist;
        private IDInput In_TrayOutCV2Level => _devices.Inputs.TrayOutCV2Level;
        //protected override IDInput In_LightCurtainMuting => _devices.Inputs.LightCurtainMutingSW;
        //protected override IDInput In_LightCurtain => _devices.Inputs.AreaSensorDetect;
        #endregion

        #region Outputs
        protected override IDOutput AreaSensorBypassOn => _devices.Outputs.AreaSensorBypassOn;
        protected override IDOutput Out_MutingSWLamp => _devices.Outputs.MutingLamp;
        #endregion

        #region Cylinders
        #endregion

        private IConveyor Cv_TrayOutLift => _devices.CVs.TrayOutLiftConveyor;

        #region Rollers
        private BD201SRollerController Roller_TrayOutElevator => _devices.RollerList.TrayOutElevatorRoller;
        #endregion

        #region Motions
        protected override IMotion z_Axis_Elevator => _devices.Motions.TrayOutputZ;
        #endregion

        #region Flags
        private bool Flag_TrayOutElevatorPlaceDone
        {
            get => _trayOutElevatorInput[(int)ETrayOutElevatorInput.TRAY_OUT_ELEVATOR_PLACE_DONE];
        }

        private bool Flag_TrayOutElevatorReadyPlace
        {
            set => _trayOutElevatorOutput[(int)ETrayOutElevatorOutput.TRAY_OUT_ELEVATOR_READY_PLACE] = value;
        }
        #endregion

        #region Override Methods
        public override bool PreProcess()
        {
            if (In_TrayOutCv2DetectExist.Value) materialStatus.Set();
            else materialStatus.Clear();
            return base.PreProcess();
        }
        public override bool ProcessToRun()
        {
            switch ((ETrayOutputElevator_ToRunStep)Step.ToRunStep)
            {
                case ETrayOutputElevator_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETrayOutputElevator_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayOutputElevator_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ETrayOutElevatorOutput>)_trayOutElevatorOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ETrayOutputElevator_ToRunStep.SetSpeedAccDecForRoller:
                    //RollerSetSpeed();
                    //RollerSetAcc();
                    //RollerSetDec();
                    Log.Debug("Set Speed Acc Dec For Roller Tray Out Elevator");
                    Step.ToRunStep++;
                    break;
                case ETrayOutputElevator_ToRunStep.End:
                    Log.Debug("To Run End.");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }
            return true;
        }
        public override bool ProcessOrigin()
        {
            switch ((ETrayOutputElevator_OriginStep)Step.OriginStep)
            {
                case (ETrayOutputElevator_OriginStep.Start):
                    Log.Debug("TrayOutElevator - Origin Start");
                    Step.OriginStep++;
                    break;
                case ETrayOutputElevator_OriginStep.CheckOriginSelected:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.OriginStep++;
                    break;
                case (ETrayOutputElevator_OriginStep.ZAxisElevatorHomeSearch):
                    Log.Debug(" Tray Out Elevator Origin - Z Axis Origin Start ");
                    z_Axis_Elevator.SearchOrigin();
                    Wait(60000, (Func<bool>?)(() => { return (bool)this.z_Axis_Elevator.Status.IsHomeDone; }));
                    Step.OriginStep++;
                    break;
                case (ETrayOutputElevator_OriginStep.ZAxisElevatorHomeSearch_Check):
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_ZAxis_OriginFail);
                        break;
                    }

                    Log.Debug(" Tray Out Elevator Origin - Z Axis Origin Done ");
                    Step.OriginStep++;
                    break;
                case ETrayOutputElevator_OriginStep.End:
                    _istraySearch = false;
                    Log.Debug("TrayOutElevator - Origin End");
                    ProcessStatus = EProcessStatus.OriginDone;
                    Step.OriginStep++;
                    break;
                default:
                    Wait(10);
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
        public override bool ProcessRun()
        {
            switch (Sequence)
            {
                case ESequence.Stop:
                    break;
                case ESequence.AutoRun:
                    Sequence_Auto();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.TraySearch:
                    Sequence_TraySearch();
                    break;
                case ESequence.TrayOutElevator_Unload:
                    Sequence_TrayOutElevator_Unload();
                    break;
                case ESequence.TrayHead_Tray_Place:
                    Sequence_TrayOutElevator_Load();
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
        private void Sequence_Change()
        { 
            switch((ETrayOutPutElevator_ChangeStep)Step.RunStep)
            {
                case ETrayOutPutElevator_ChangeStep.Start:
                    Log.Debug("Tray Head Change Step Start");
                    Step.RunStep++;
                    break;

                case ETrayOutPutElevator_ChangeStep.Stop_ZAxis:
                    Log.Debug("Stop XYZ Axis ");                   
                    Out_MutingSWLamp.Value = true;                   
                    z_Axis_Elevator.Stop();
                    Wait(3000, () => z_Axis_Elevator.Status.IsMotioning == false);
                    Step.RunStep++;
                    break;
                case ETrayOutPutElevator_ChangeStep.Stop_ZAxis_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_StopFail);
                        Log.Debug("Stop Z Axis Fail");
                        break;
                    }
                    Log.Debug("Stop Z Axis Done");
                    Step.RunStep++;
                    break;
                case ETrayOutPutElevator_ChangeStep.End:
                    Log.Debug("Change End");
                    //Step.RunStep++;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_Ready()
        {
            if (IsOriginOrInitSelected == false)
            {
                Sequence = ESequence.Stop;
                return;
            }
            ((MappableOutputDevice<ETrayOutElevatorOutput>)_trayOutElevatorOutput).ClearOutputs();
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
            _istraySearch = false;
        }

        public override string ToString()
        {
            return EProcess.TrayOutElevator.GetDescription();
        }
        private void Sequence_Auto()
        {
            if (_machineStatus.IsByPassMode) return;

            if (z_Axis_Elevator.Status.ActualPosition == _traySuplierRecipe.ZAxisReadyOutputTrayPosition && In_TrayOutCv2DetectEnd.Value == true && In_TrayOutCVDetectStart.Value == true)
            {
                Sequence = ESequence.TrayOutElevator_Unload;              
            }
            else if((In_TrayOutCv2DetectExist.Value == true || In_TrayOutCv2DetectStart.Value == true) && _istraySearch == false || _machineStatus.IsDryRunMode || In_TrayOutCV2Level.Value == true)
            {
                Sequence = ESequence.TraySearch;
            }
            else
            {
                Sequence = ESequence.TrayHead_Tray_Place;
            }
        }
        private void Sequence_TraySearch()
        {
            switch ((ETrayOutputElevator_TraySearchStep)Step.RunStep)
            {
                case (ETrayOutputElevator_TraySearchStep.Start):
                    {
                        Log.Debug("TrayOutElevator - Tray Search Start");
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputElevator_TraySearchStep.Check_CheckExist:
                    if(In_TrayOutCv2DetectExist.Value == false && In_TrayOutCv2DetectExist.Value == false)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.Check_IsTraySearch;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxis_Move_ReadyPosition:
                    Log.Debug("Z Axis Move Ready Position");
                    z_Axis_Elevator.MoveAbs(_traySuplierRecipe.ZAxisReadyPlaceTrayPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.IsOnPosition(_traySuplierRecipe.ZAxisReadyPlaceTrayPosition));
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxis_Move_ReadyPosition_Wait:
                    if(WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_Move_To_Ready_Place_Fail);
                        break;
                    }
                    Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.End;
                    break;
                case ETrayOutputElevator_TraySearchStep.Check_IsTraySearch:
                    if(_istrayFull == true)
                    {
                        Log.Debug("Tray On Out Elevator Full ");
                        Sequence = ESequence.TrayOutElevator_Unload;
                        break;
                    }
                    Log.Debug("Tray On Elevator Not Search");
                    ProcessTimer.SpareTime = Environment.TickCount;
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxisTrayEndUp_PreCheck:
                    if (In_TrayOutCV2Level.Value == false)
                    {
                        z_Axis_Elevator.Stop();

                        Log.Debug($"Tray end up not detect -> Z Up Search");
                        // Set timer for searching until endup sensor detected (move up timeout)
                        ProcessTimer.SpareTime = Environment.TickCount;
                        Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck;
                        break;
                    }

                    if (Environment.TickCount - ProcessTimer.SpareTime > _globalRecipe.MotionMoveTimeout * 1000)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_TraySearchFail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxisTrayDown_Move:
                    if (z_Axis_Elevator.Status.ActualPosition <= _traySuplierRecipe.ZAxisLimitDownTraySearchPosition)
                    {
                        Log.Debug($"{Name} Tray full -> Sequence Unload");
                        Sequence = ESequence.TrayOutElevator_Unload;
                        break;
                    }

                    z_Axis_Elevator.MoveInc(searchPitch * -1.0);
                    Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.ZAxisTrayEndUp_PreCheck;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck:
                    if (In_TrayOutCV2Level.Value == true)
                    {
                        Log.Debug($"Tray end up detected.");

                        Log.Debug("Z Axis Stop");
                        z_Axis_Elevator.Stop();

                        if (z_Axis_Elevator.Status.ActualPosition <= _traySuplierRecipe.ZAxisLimitDownTraySearchPosition)
                        {
                            Log.Debug($"{Name} Tray full -> Sequence Unload");
                            Sequence = ESequence.TrayOutElevator_Unload;
                            break;
                        }

                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.Status.IsMotioning == false);
                        Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.ZAxis_Stop_Wait;
                        break;
                    }

                    if (Environment.TickCount - ProcessTimer.SpareTime > _globalRecipe.MotionMoveTimeout * 1000)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_TraySearchFail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxisTrayUp_Move:
                    z_Axis_Elevator.MoveJog(100.0,true);
                    Step.RunStep = (int)ETrayOutputElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxis_Stop_Wait:
                    if(WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_TraySearchFail);
                        break;
                    }
                    Wait(650);
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxis_MoveDownOffset:
                    Log.Debug("Z Axis Move Down Offset");
                    double zTargetPosition = z_Axis_Elevator.Status.ActualPosition - 15;
                    z_Axis_Elevator.MoveAbs(zTargetPosition,400.0);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.IsOnPosition(zTargetPosition));
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.ZAxis_MoveDownOffset_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_TraySearchFail);
                        break;
                    }

                    Log.Debug("Z Axis Move Down Offset Done");
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_TraySearchStep.End:
                    _istraySearch = true;
                    Log.Debug("Tray Search End");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Sequence = ESequence.TrayHead_Tray_Place;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayOutElevator_Load()
        {
            switch ((ETrayOutputElevator_LoadStep)Step.RunStep)
            {
                case ETrayOutputElevator_LoadStep.Start:
                    Log.Debug("TrayOutElevator - Load Start");
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_LoadStep.Check_Status_Tray_Exis:
                    if( In_TrayOutCv2DetectEnd.Value == false 
                        && In_TrayOutCv2DetectExist.Value == false 
                        &&  In_TrayOutCv2DetectStart.Value == false)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (_istraySearch == true)
                    {
                        Step.RunStep = (int)ETrayOutputElevator_LoadStep.Set_Flag_TrayOutElevatorReadyPlace;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_LoadStep.Move_To_Ready_Place:
                    Log.Debug("Move to Ready Place");
                    z_Axis_Elevator.MoveAbs(_traySuplierRecipe.ZAxisReadyPlaceTrayPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.IsOnPosition(_traySuplierRecipe.ZAxisReadyPlaceTrayPosition));
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_LoadStep.Move_To_Ready_Place_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Move to Ready Place Erro");
                        RaiseWarning((int)EWarning.TrayOUTLift_Move_To_Ready_Place_Fail);
                        break;
                    }
                    Log.Debug(" Tray Out Elevator move to Ready Place Done");
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_LoadStep.Set_Flag_TrayOutElevatorReadyPlace:
                    Log.Debug("Set flag tray out elevator ready place");
                    Flag_TrayOutElevatorReadyPlace = true;
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_LoadStep.Wait_TrayOutElevatorPlaceDone:
                    if (Flag_TrayOutElevatorPlaceDone)
                    {
#if SIMULATION
                        SimulationInputSetter.SetSimInput(In_TrayOutCV2Level, true);
                        SimulationInputSetter.SetSimInput(In_TrayOutCv2DetectExist, true);

#endif
                        Log.Debug("Tray Out Elevator Place Done");
                        Flag_TrayOutElevatorReadyPlace = false;
                        Step.RunStep++;
                        break;
                    }

                    if(_machineStatus.IsTrayOut && (In_TrayOutCv2DetectStart.Value || In_TrayOutCv2DetectExist.Value || In_TrayOutCv2DetectEnd.Value) &&
                        _machineStatus.IsTrayHeadTrayPlacing == false)
                    {
                        Flag_TrayOutElevatorReadyPlace = false;
                        Sequence = ESequence.TrayOutElevator_Unload;
                        break;
                    }
                    Wait(20);
                    break;

                case ETrayOutputElevator_LoadStep.End:
                    Log.Debug("Load End.");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Log.Debug("Sequence TraySearch!");
                    Sequence = ESequence.TraySearch;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayOutElevator_Unload()
        {
            switch ((ETrayOutputElevator_UnloadStep)Step.RunStep)
            {
                case (ETrayOutputElevator_UnloadStep.Start):
                    {
                        Log.Debug("TrayOutElevator - Unload Start");
                        Step.RunStep++;
                        break;
                    }
                case (ETrayOutputElevator_UnloadStep.Tray_Output_Elevator_Cv_Stop):
                    {
                        Log.Debug(" Tray Out Elevator Unload - Stop CV2 ");
                        Task.Run(() => 
                        {
                            RollerRunStop(false);
                        });
                        Wait(500);
                        Step.RunStep++;
                        break;
                    }
                case (ETrayOutputElevator_UnloadStep.Move_Tray_Output_Elevator_to_Unload_Position):
                    Log.Debug(" Tray Out Elevator Unload - Move Z Axis to Unload Position ");
                    z_Axis_Elevator.MoveAbs(_traySuplierRecipe.ZAxisReadyOutputTrayPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => (z_Axis_Elevator.IsOnPosition(_traySuplierRecipe.ZAxisReadyOutputTrayPosition)));
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_UnloadStep.Move_Tray_Output_Elevator_to_Unload_Position_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayOUTLift_OutElevator_Up_Height_Position_Move_Timeout);
                        Log.Debug(" Tray Out Elevator Move to Unload Position Time Out Error ");
                        break;
                    }
                    z_Axis_Elevator.Stop();
                    Log.Debug(" Tray Out Elevator Move to Unload Position Done ");
                    Step.RunStep++;
                    break;
                case ETrayOutputElevator_UnloadStep.Tray_Out_Elevator_Request:
                    {
                        Log.Debug(" Tray Out Elevator Unload - Request to Unload Tray from Elevator to CV2 ");
                        _trayOutElevatorOutput[(int)ETrayOutElevatorOutput.TRAY_OUT_ELEVATOR_REQUEST] = true;
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputElevator_UnloadStep.Tray_Out_Elevator_Request_Check:
                    {
                        if (_trayOutElevatorInput[(int)ETrayOutElevatorInput.TRAY_OUT_ELEVATOR_READY] == true)
                        {
                            Log.Debug(" Tray Out Elevator Unload - Ready Signal Received ");
                            Step.RunStep++;
                            break;
                        }
                        break;
                    }
                case ETrayOutputElevator_UnloadStep.CV_Tray_Output_Elevator_Run:
                    {
                        Task.Run(() => 
                        {
                            RollerRunStop(true);
                        });
                        Log.Debug(" Tray Out Elevator Unload - Start CV2 to Unload Tray ");
#if SIMULATION
                        SimulationInputSetter.SetSimInput(In_TrayOutCV2Level, true);
                        SimulationInputSetter.SetSimInput(In_TrayOutCv2DetectExist, true);
                        SimulationInputSetter.SetSimInput(In_TrayOutCv2DetectEnd, true);
#endif
                        Wait(10000, () => (In_TrayOutCv2DetectEnd.Value == false && In_TrayOutCv2DetectStart.Value == false && In_TrayOutCv2DetectExist.Value == false));
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputElevator_UnloadStep.Wait_Tray_Output_Elevator_Cv_NotDetect_End:
                    {
                        if (WaitTimeOutOccurred)
                        {
                            Log.Debug(" Tray Out Elevator Unload - Unload Tray Time Out Error ");
                            RaiseWarning((int)EWarning.TrayOUTLift_Out_SenSor_EndDetect);
                            break;
                        }
                        Task.Run(() => 
                        {
                            RollerRunStop(false);
                        });
                        Log.Debug(" Tray Out Elevator Unload - Unload Tray Done ");
                        _trayOutElevatorOutput[(int)ETrayOutElevatorOutput.TRAY_OUT_ELEVATOR_REQUEST] = false;
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputElevator_UnloadStep.Tray_Output_Elevator_Cv_Stop_Again:
                    {
                        Log.Debug(" Tray Out Elevator Unload - Stop CV2 Again ");
                        Wait(500);
                        Task.Run(() => 
                        {
                            RollerRunStop(false); 
                        });
                        Step.RunStep++;
                        break;
                    }
                case ETrayOutputElevator_UnloadStep.End:
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _machineStatus.IsTrayOut = false;
                        });

                        _istrayFull = false;
                        _istraySearch = false;
                        Log.Debug("TrayOutElevator - Unload End");
                        if (Parent?.Sequence != ESequence.AutoRun)
                        {
                            Sequence = ESequence.Stop;
                            break;
                        }
                        Sequence = ESequence.TraySearch;
                        break;
                    }
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

        private void RollerRunStop(bool isRun)
        {
            if (isRun)
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayOutLiftConveyor.Run();
                    return;
                }
                Roller_TrayOutElevator.Run();
            }
            else
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayOutLiftConveyor
                        .Stop();
                    return;
                }
                Roller_TrayOutElevator.Stop();
            }
        }
        private void RollerSetSpeed()
        {
            Roller_TrayOutElevator.SetSpeed((int)_traySuplierRecipe.CVTrayOutElevatorSpeed);
        }
        private void RollerSetAcc()
        {
            Roller_TrayOutElevator.SetAcceleration((int)_traySuplierRecipe.Acceleration);
        }
        private void RollerSetDec()
        {
            Roller_TrayOutElevator.SetDeceleration((int)_traySuplierRecipe.Deceleration);
        }
        private void StopRun()
        {
            Task.Run(() =>
            {
                ((ProcessTimer)ProcessTimer).WaitTime = 0;
                Log.Debug("Stop Run");
                RollerRunStop(false);
                Log.Debug("Stop Roller Done");
                z_Axis_Elevator.Stop();
                Log.Debug("Stop Z Axis Done");
            });
        }
        #endregion

        #region Constructors
        public TrayOutElevatorProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            RecipeSelector recipeSelector,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer,
            [FromKeyedServices("TrayOutElevatorInput")] IDInputDevice<ETrayOutElevatorInput> trayOutElevatorInput,
            [FromKeyedServices("TrayOutElevatorOutput")] IDOutputDevice<ETrayOutElevatorOutput> trayOutElevatorOutput,
            DevRecipe devRecipe) 
            : base(globalRecipe, machineStatus, devices, blinkTimer)
        {
            this.blinkTimer = blinkTimer;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _machineStatus = machineStatus;
            _materialStatusList = materialStatusList;
            _recipeSelector=recipeSelector;
            _trayOutElevatorInput = trayOutElevatorInput;
            _trayOutElevatorOutput = trayOutElevatorOutput;
            _devRecipe = devRecipe;
            _recipeSelector.RecipeSaved += RollerSet;
        }
        #endregion

        #region Privates


        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly IDInputDevice _trayOutElevatorInput;
        private readonly IDOutputDevice _trayOutElevatorOutput;
        private readonly DevRecipe _devRecipe;
        private readonly MaterialStatusList _materialStatusList;
        private readonly MachineStatus _machineStatus;
        private bool _istraySearch = false;
        private bool _istrayFull = false;
        private RecipeSelector _recipeSelector;
        private TraySuplierRecipe _traySuplierRecipe => _recipeList.TraySuplierRecipe;
        private MaterialStatus materialStatus => _materialStatusList.TrayOutMaterialStatus;
        protected readonly ActionAssignableTimer blinkTimer;
        private double searchPitch = 1.0;

        #endregion
    }
}
