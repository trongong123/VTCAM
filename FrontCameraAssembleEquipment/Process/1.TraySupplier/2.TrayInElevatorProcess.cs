using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Core.Units;
using EQX.Device.SpeedController;
using EQX.InOut;
using EQX.InOut.InOut.Analog;
using EQX.InOut.Virtual;
using EQX.Process;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Windows.Media.Animation;

namespace FrontCameraAssembleEquipment.Process
{
    public class TrayInElevatorProcess : ChangeReadyProcess
    {
        #region Inputs
        private IDInput In_TrayInCv2Level => _devices.Inputs.TrayInCV2Level;
        private IDInput In_TrayInCv2DetectStart => _devices.Inputs.TrayInCv2DetectStart;
        private IDInput In_TrayInCv2DetectEnd => _devices.Inputs.TrayInCv2DetectEnd;
        private IDInput In_TrayInCv2DetectExist => _devices.Inputs.TrayInCv2DetectExist;
        //protected override IDInput In_LightCurtain => _devices.Inputs.AreaSensorDetect;
        //protected override IDInput In_LightCurtainMuting => _devices.Inputs.LightCurtainMutingSW;
        #endregion

        #region Outputs
        protected override IDOutput AreaSensorBypassOn => _devices.Outputs.AreaSensorBypassOn;
        protected override IDOutput Out_MutingSWLamp => _devices.Outputs.MutingLamp;

        #endregion

        #region Cylinders
        private ICylinder Cyl_TrayCentering1 => _devices.Cylinders.TraySupplier_TrayCentering1;
        #endregion

        #region Motions
        protected override IMotion z_Axis_Elevator => _devices.Motions.TrayInputZ;
        #endregion

        private IConveyor Cv_TrayInLift => _devices.CVs.TrayInLiftConveyor;

        #region Rollers
        private BD201SRollerController Roller_TrayInElevator => _devices.RollerList.TrayInElevatorRoller;
        #endregion

        #region Flags
        private bool FlagIn_TrayInElevatorUnloadCamDone
        {
            get => _trayInElevatorInput[(int)ETrayInElevatorInput.TRAY_IN_ELEVATOR_CAM_UNLOAD_DONE];
        }
        private bool FlagIn_TrayInElevatorUnloadCamPickFail
        {
            get => _trayInElevatorInput[(int)ETrayInElevatorInput.TRAYHEAD_CAM_UNLOAD_PICK_FAIL];
        }
        private bool FlagIn_TrayHeadTrayPickVacOnOk
        {
            get => _trayInElevatorInput[(int)ETrayInElevatorInput.TRAY_IN_ELEVATOR_TRAYHEAD_VAC_ON];
        }
        private bool FlagIn_TrayInElevatorUnloadTrayDone
        {
            get => _trayInElevatorInput[(int)ETrayInElevatorInput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_DONE];
        }
        private bool FlagIn_TrayInElevatorUnloadTrayDoneReceived
        {
            get => _trayInElevatorInput[(int)ETrayInElevatorInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED];
        }
        /// <summary>
        /// //Out Put
        /// </summary>
        private bool FlagOut_TrayInElevatorUnloadCamRequest
        {
            set => _trayInElevatorOutput[(int)ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ] = value;
        }
        private bool FlagOut_TrayInElevatorUnloadTrayRequest
        {
            set => _trayInElevatorOutput[(int)ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ] = value;
        }
        private bool FlagOut_TrayInElevatorUnAlignDone
        {
            set => _trayInElevatorOutput[(int)ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNALIGN_DONE] = value;
        }
        private bool FlagOut_TrayInElevatorLoadRequest
        {
            set => _trayInElevatorOutput[(int)ETrayInElevatorOutput.TRAY_INPUT_ELEVATOR_REQUEST] = value;
        }
        private bool FlagOut_TrayInElevatorUnloadCamDoneReceicved
        {
            set => _trayInElevatorOutput[(int)ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED] = value;
        }


        private MaterialStatus materialStatus => _materialStatusList.TrayInMaterialStatus;
        #endregion

        #region Override Methods
        public override bool PreProcess()
        {
            //if(In_TrayInCv2DetectEnd.Value == false && In_TrayInCv2DetectExist.Value == false)
            //{
            //    foreach (var cell in CurrentJig.Cells)
            //    {
            //        cell.Status = ETrayCellStatus.Skip;
            //    }
            //}
            if (In_TrayInCv2DetectExist.Value) materialStatus.Set();
            else materialStatus.Clear();
            return base.PreProcess();
        }
        public override bool ProcessToAlarm()
        {
            if (ProcessStatus == EProcessStatus.ToAlarmDone)
            {
                Thread.Sleep(50);
                return true;
            }
            z_Axis_Elevator.Stop();

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
            z_Axis_Elevator.Stop();

            StopRun();
            ProcessStatus = EProcessStatus.ToWarningDone;
            return base.ProcessToWarning();
        }
        public override bool ProcessToRun()
        {
            switch ((ETrayInPutElevator_ToRunStep)Step.ToRunStep)
            {
                case ETrayInPutElevator_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETrayInPutElevator_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayInPutElevator_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ETrayInElevatorOutput>)_trayInElevatorOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ETrayInPutElevator_ToRunStep.SetSpeedAccDeccForRoller:
                    //RollerSetSpeed();
                    //RollerSetAcc();
                    //RollerSetDec();
                    Log.Debug("Set Speed Acc Dec For Roller Tray In Elevator");
                    Step.ToRunStep++;
                    break;
                case ETrayInPutElevator_ToRunStep.End:
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
            switch ((ETrayInputElevator_OriginStep)Step.OriginStep)
            {
                case ETrayInputElevator_OriginStep.Start:
                    Log.Debug("TrayInElevator Origin Start");
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.CheckOriginSelected:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.Stop_CV:
                    Log.Debug("Stop Input CV");
                    Task.Run(() => 
                    {
                        RollerRunStop(false);
                    });
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.TrayCenteringOff:
                    Log.Debug("Tray Align UnAlign");
                    Cyl_TrayCentering1.Backward();
                    _isTraySearch = false;
                    Wait(10000, () => Cyl_TrayCentering1.IsBackward && _devices.Inputs.TrayCentering2Off.Value);
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.TrayCenteringOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_UnAlign_Timeout);
                        break;
                    }

                    Log.Debug("Tray ALign UnAlign done");
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.ZAxisElevatorHomeSearch:
                    Log.Debug("Z Axis Move Origin");
                    z_Axis_Elevator.SearchOrigin();
                    Wait((int)(_globalRecipe.MotionOriginTimeout * 1000), () => z_Axis_Elevator.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.ZAxisElevatorHomeSearch_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_ZAxis_OriginFail);
                        break;
                    }

                    Log.Debug("Z Axis Move Origin Done");
                    Step.OriginStep++;
                    break;
                case ETrayInputElevator_OriginStep.End:
                    Log.Debug("Origin End");
                    ProcessStatus = EProcessStatus.OriginDone;
                    Step.OriginStep++;
                    break;
                default:
                    Wait(10);
                    break;
            }
            return true;
        }
        public override bool ProcessToStop()
        {
            if (ProcessStatus == EProcessStatus.ToStopDone)
            {
                Thread.Sleep(50);
                return true;
            }

            z_Axis_Elevator.Stop();

            if (CurrentJig.GetFirstIndex(ETrayCellStatus.Working) != -1)
            {
                CurrentJig[(uint)CurrentJig.GetFirstIndex(ETrayCellStatus.Working)] = ETrayCellStatus.Ready;
            }

            if (Sequence != ESequence.Stop && ProcessStatus != EProcessStatus.OriginDone && Sequence != ESequence.None)
            {
                //Roller_TrayInElevator.Stop();
                //z_Axis_Elevator.Stop();
                //_savedSequence = Sequence;
                //_savedRunStep = Step.RunStep;
                //_isPausedFromRun = true;
                //Wait_move = true;

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
                    Sequence_AutoRun();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.TrayInElevator_Load:
                    Sequence_TrayInElevator_Load();
                    break;
                case ESequence.TraySearch:
                    Sequence_TrayInElevator_TraySearch();
                    break;
                case ESequence.TrayHead_Tray_Pick:
                    Sequence_TrayInElevator_TrayUnload();
                    break;
                case ESequence.TrayHead_Cam_Pick:
                    Sequence_TrayInElevator_CamUnload();
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
           ((MappableOutputDevice<ETrayInElevatorOutput>)_trayInElevatorOutput).ClearOutputs();
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
            _isTraySearch = false;
            _canApplyAutoUiCameraSettingForNextTray = true;
        }
        private void Sequence_AutoRun()
        {
            if (_machineStatus.IsByPassMode) return;

            ((MappableOutputDevice<ETrayInElevatorOutput>)_trayInElevatorOutput).ClearOutputs();
            if (In_TrayInCv2DetectExist.Value == false && In_TrayInCv2DetectEnd.Value == false && In_TrayInCv2DetectStart.Value == false && _machineStatus.IsDryRunMode == false)
            {
                _isSetCamCount = false;
                _isTraySearch = false;
                Sequence = ESequence.TrayInElevator_Load;
                CaptureAutoUiCameraSettingForNextTray();
                foreach (var cell in CurrentJig.Cells)
                {
                    cell.Status = ETrayCellStatus.Skip;
                }
            }
            if (In_TrayInCv2DetectExist.Value == false && In_TrayInCv2DetectEnd.Value == false && _isTraySearch == false || (In_TrayInCv2DetectStart.Value == true && _machineStatus.IsDryRunMode == false))
            {
                Sequence = ESequence.TrayInElevator_Load;

            }
            else if (_isTraySearch == false || _machineStatus.IsDryRunMode)
            {
                Sequence = ESequence.TraySearch;

            }
            else
            {
                Sequence = ESequence.TrayHead_Cam_Pick;

            }
            if ((z_Axis_Elevator.Status.ActualPosition >= _traySuplierRecipe.ZAxixWarningEmptyMaterialPos && _devices.Inputs.TrayInCv1DetectExist.Value == false) ||
                (_devices.Inputs.TrayInCv1DetectExist.Value == false && _devices.Inputs.TrayInCv1DetectStart.Value == false && In_TrayInCv2DetectExist.Value == false))
            {
                _devices.Outputs.TowerLampBuzzer_Full_Empty();
                Log.Debug("Warning Empty Metarial ");

            }
            else
            {
                _devices.Outputs.TowerLamp_Run();
                Log.Debug("Metarial iS Full ");
            }

        }
        private void Sequence_Change()
        {
            switch ((ETrayInPutElevator_ChangeStep)Step.RunStep)
            {
                case ETrayInPutElevator_ChangeStep.Start:
                    Log.Debug("Tray Head Change Step Start");
                    Step.RunStep++;
                    break;

                case ETrayInPutElevator_ChangeStep.Stop_ZAxis:
                    Log.Debug("Stop Z Axis ");
                    z_Axis_Elevator.Stop();
                    Wait(2000, () => z_Axis_Elevator.Status.IsMotioning == false);
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_ChangeStep.Stop_ZAxis_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_StopFail);
                        Log.Debug("Stop Z Axis Fail");
                        break;
                    }
                    Log.Debug("Stop Z Axis Done");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_ChangeStep.End:
                    Log.Debug("Change End");
                    Step.RunStep++;
                    break;
                default:
                    break;
            }

        }
        private void Sequence_TrayInElevator_Load()
        {
            switch ((ETrayInputElevator_LoadStep)Step.RunStep)
            {
                case ETrayInputElevator_LoadStep.Start:
                    Log.Debug("TrayInElevator Load Start");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Tray_Input_Elevator_Cv_Exist_Check:
                    if (In_TrayInCv2DetectEnd.Value || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Tray already exists at elevator end position" + "Move to initialize camera status");

                        Step.RunStep = (int)ETrayInputElevator_LoadStep.Reset_Status_Camera;
                        break;
                    }

                    Log.Debug("Tray not at end position. Start tray loading sequence.");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Elevator_Input_Position_Move:
                    z_Axis_Elevator.MoveAbs(_traySuplierRecipe.ZAxisInputTrayPosition);

                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => (z_Axis_Elevator.IsOnPosition(_traySuplierRecipe.ZAxisInputTrayPosition)));
                    Log.Debug("TrayInElevator Move Input Position  ");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Elevator_Input_Position_Move_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_Input_Position_Move_Timeout);
                        break;
                    }
                    Log.Debug("Elevator Move Input Position Check OK");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Elevator_Input_Request_Input_Tray:
                    FlagOut_TrayInElevatorLoadRequest = true;
                    Log.Debug("Set Flag Request Tray Input ");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Tray_Start_Sensor_Check:
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_TrayInCv2DetectStart, true);
#endif
                    if (_machineStatus.IsDryRunMode == true)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if (In_TrayInCv2DetectStart.Value == false)
                    {
                        break;
                    }
                    Log.Debug("TrayInElevator Start Sensor Check OK");
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Input_Elevator_Cv_Start:
                    Task.Run(() => 
                    {
                        RollerRunStop(true);
                    });
                    Log.Debug("TrayInElevator Roller Start");
                    Wait((int)(_globalRecipe.CVMoveTimeout * 1000), () => In_TrayInCv2DetectEnd.Value || _machineStatus.IsDryRunMode);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(In_TrayInCv2DetectEnd, true);
#endif
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Tray_End_Sensor_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_Detect_End_Timeout);
                        break;
                    }
                    Log.Debug("TrayInElevator Tray End Sensor Check OK");
                    Wait(1500);
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Input_Elevator_Cv_Stop:
                    Task.Run(() => 
                    {
                        RollerRunStop(false);
                    });
                    Log.Debug("TrayInElevator Cv Stop");
                    FlagOut_TrayInElevatorLoadRequest = false;
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.Reset_Status_Camera:
                    bool isTrayStatusNotInitialized = CurrentJig.Cells.All(cell => cell.Status == ETrayCellStatus.Skip);

                    if (!_isSetCamCount && _autoUiCameraSettingForNextTray != null)
                    {
                        ApplyAutoUiCameraSettingForNextTray();
                    }
                    else if (!_isSetCamCount && isTrayStatusNotInitialized)
                    {
                        foreach (var cell in CurrentJig.Cells)
                        {
                            cell.Status = ETrayCellStatus.Ready;
                        }

                        Log.Debug($"New tray camera status initialized: " + $"{CurrentJig.Cells.Count} cells Ready");
                    }
                    else
                    {
                        Log.Debug("Keep current tray camera status because tray " + "has already been processed partially");
                    }

                    _isSetCamCount = true;
                    Step.RunStep++;
                    break;
                case ETrayInputElevator_LoadStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.TraySearch;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayInElevator_TraySearch()
        {
            switch ((ETrayInPutElevator_TraySearchStep)Step.RunStep)
            {
                case ETrayInPutElevator_TraySearchStep.Start:
                    Log.Debug("TrayInElevator Sequence_TraySupIn_ElevatorLoad Start");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Check_Status_IsTraySearch:
                    if (_isTraySearch)
                    {
                        Log.Debug("Tray Search Is Already");
                        Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.End;
                        break;
                    }
                    Log.Debug("Tray Search Start");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_UnAlign:
                    Log.Debug(" UnAlign ");
                    Cyl_TrayCentering1.Backward();
                    _isTraySearch = false;
                    Wait(10000, () => Cyl_TrayCentering1.IsBackward == true && _devices.Cylinders.TraySupplier_TrayCentering2.IsBackward == true);
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_UnAlign_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Un Align NG");
                        RaiseWarning((int)EWarning.TrayINLift_UnAlign_Timeout);
                        break;
                    }
                    ProcessTimer.SpareTime = Environment.TickCount;

                    Log.Debug("Un Align OK");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PreCheck:
                    if (In_TrayInCv2Level.Value == false)
                    {
                        z_Axis_Elevator.Stop();

                        Log.Debug($"Tray end up not detect -> Z Up Search");
                        // Set timer for searching until endup sensor detected (move up timeout)
                        ProcessTimer.SpareTime = Environment.TickCount;
                        Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck_1st;
                        break;
                    }

                    if (Environment.TickCount - ProcessTimer.SpareTime > _globalRecipe.MotionMoveTimeout * 1000)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayDown_Move:
                    z_Axis_Elevator.MoveInc(searchPitch * -1.0);
                    Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PreCheck;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck_1st:
                    if (In_TrayInCv2Level.Value == true)
                    {
                        Log.Debug($"Tray end up detected 1st.");

                        Log.Debug("Z Axis Stop");
                        z_Axis_Elevator.Stop();

                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.Status.IsMotioning == false);
                        Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_1st;
                        break;
                    }

                    if (z_Axis_Elevator.Status.ActualPosition >= (_traySuplierRecipe.ZAxisLimitUpPosition - 5))
                    {
                        Log.Debug("Z Axis Stop Because Actual Position >= Z Limit Up Position - 5");
                        z_Axis_Elevator.Stop();
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.Status.IsMotioning == false);
                        Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_Before_MoveZLimitUp;
                        break;
                    }
                    if (z_Axis_Elevator.Status.ActualPosition >= _traySuplierRecipe.ZAxisLimitUpPosition)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_Up_Height_Position_Move_To_ZUpLimit);
                        break;
                    }

                    if (Environment.TickCount - ProcessTimer.SpareTime > _globalRecipe.MotionMoveTimeout * 1000)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayUp_Move_1st:
                    z_Axis_Elevator.MoveJog(100.0, true);
                    Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck_1st;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_1st:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_Move_DownOffset:
                    Log.Debug("Z Axis Move Down Offset");
                    double zTargetPosition = z_Axis_Elevator.Status.ActualPosition - 10;
                    z_Axis_Elevator.MoveAbs(zTargetPosition, 400.0);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.IsOnPosition(zTargetPosition));
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_Move_DownOffset_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }

                    ProcessTimer.SpareTime = Environment.TickCount;
                    Log.Debug("Z Axis Move Down Offset Done");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck_2nd:
                    if (In_TrayInCv2Level.Value == true)
                    {
                        Log.Debug($"Tray end up detected 2nd.");

                        Log.Debug("Z Axis Stop");
                        z_Axis_Elevator.Stop();

                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.Status.IsMotioning == false);
                        Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_2nd;
                        break;
                    }

                    if (z_Axis_Elevator.Status.ActualPosition >= _traySuplierRecipe.ZAxisLimitUpPosition)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_Up_Height_Position_Move_To_ZUpLimit);
                        break;
                    }

                    if (Environment.TickCount - ProcessTimer.SpareTime > _globalRecipe.MotionMoveTimeout * 1000)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxisTrayUp_Move_2nd:
                    z_Axis_Elevator.MoveInc(searchPitch);
                    Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.ZAxisTrayEndUp_PostCheck_2nd;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_2nd:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }
                    Step.RunStep = (int)ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_Align;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_Stop_Wait_Before_MoveZLimitUp:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }

                    Log.Debug("Z Axis Stop Done");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_MoveZLimitUp:
                    Log.Debug("Z Axis Move Limit Up Position");
                    z_Axis_Elevator.MoveAbs(_recipeList.TraySuplierRecipe.ZAxisLimitUpPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => z_Axis_Elevator.IsOnPosition(_recipeList.TraySuplierRecipe.ZAxisLimitUpPosition));
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.ZAxis_MoveZLimitUp_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_Up_Height_Position_Move_To_ZUpLimit);
                        break;
                    }

                    Log.Debug("Z Axis Move Limit Up Position Done");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.TrayEndUp_Detect_Check:
                    if (In_TrayInCv2Level.Value == false)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_TraySearchFail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_Align:
                    if (Cyl_TrayCentering1.IsForward == true && _devices.Cylinders.TraySupplier_TrayCentering2.IsForward == true)
                    {
                        Log.Debug(" Centering On already ");
                        Step.RunStep++;
                        break;
                    }
                    Cyl_TrayCentering1.Forward();
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayCentering1On, true);
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayCentering2On, true);
#endif
                    Wait(10000, () => Cyl_TrayCentering1.IsForward == true && _devices.Cylinders.TraySupplier_TrayCentering2.IsForward == true);
                    Log.Debug(" Centering On ");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_Align_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("TrayInElevator Align NG ");
                        RaiseWarning((int)EWarning.TrayINLift_Align_Timeout);
                        break;
                    }
                    Log.Debug("TrayInElevator Align OK");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.Tray_In_Elevator_Check_Warning_Empty_Material:
                    if (z_Axis_Elevator.Status.ActualPosition >= _traySuplierRecipe.ZAxixWarningEmptyMaterialPos && _devices.Inputs.TrayInCv1DetectExist.Value == false)
                    {
                        _devices.Outputs.TowerLampBuzzer_Full_Empty();
                        Log.Debug("Warning Empty Metarial ");
                        Step.RunStep++;
                        break;
                    }
                    _devices.Outputs.TowerLamp_Run();
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TraySearchStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    _isTraySearch = true;
                    Sequence = ESequence.TrayHead_Cam_Pick;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayInElevator_CamUnload()
        {
            try
            {
                switch ((ETrayInPutElevator_CamUnloadStep)Step.RunStep)
                {
                    case ETrayInPutElevator_CamUnloadStep.Start:
                        Log.Debug("Unload Camera start");
                        Step.RunStep++;
                        break;
                    case ETrayInPutElevator_CamUnloadStep.Tray_WorkCondition_Check:
                        bool isAllCellSkip = CurrentJig.Cells.All(cell => cell.Status == ETrayCellStatus.Skip);

                        if (_isTraySearch && isAllCellSkip)
                        {
                            _isSetCamCount = true;

                            Log.Debug("All camera cells are Skip after tray search. Keep Auto UI camera setting and unload tray without picking camera");

                            Sequence = ESequence.TrayHead_Tray_Pick;
                            break;
                        }

                        if (JigWorkDone)
                        {
                            if (_devRecipe.UseRetryPickFail == true)
                            {
                                if (_isTraySetToRetryPickDone == true)
                                {
                                    _isTraySetToRetryPickDone = false;
                                    Sequence = ESequence.TrayHead_Tray_Pick;
                                    break;
                                }

                                if (CurrentJig.Cells.Count(c => c.Status == ETrayCellStatus.PickFail) > 0
                                    && _isTraySetToRetryPickDone == false)
                                {
                                    _isTraySetToRetryPickDone = true;
                                    CurrentJig.Cells.Where(c => c.Status == ETrayCellStatus.PickFail).ToList().ForEach(c => c.Status = ETrayCellStatus.Ready);
                                    break;
                                }

                            }
                            Sequence = ESequence.TrayHead_Tray_Pick;
                            break;
                        }

                        if (CurrentJig.GetFirstIndex(ETrayCellStatus.Working) == -1)
                        {
                            CurrentJig[(uint)CurrentJig.GetFirstIndex(ETrayCellStatus.Ready)] = ETrayCellStatus.Working;
                        }
                        else
                        {
                            Wait(30);
                            Step.RunStep++;
                            break;
                        }
                        Step.RunStep++;
                        break;
                    case ETrayInPutElevator_CamUnloadStep.Set_FlagTrayInElevatorUnloadCamRequest:
                        Log.Debug("Set flag Tray In Elevator ready pick camera for Tray Head");
                        FlagOut_TrayInElevatorUnloadCamRequest = true;
                        Wait(500);
                        Step.RunStep++;
                        break;
                    case ETrayInPutElevator_CamUnloadStep.Wait_TrayHeadPickCameraAndResetStatusCamera:
                        int currentCellIndex = _trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Working);
                        if (FlagIn_TrayInElevatorUnloadCamDone == true)
                        {
                            //CurrentJig[(uint)CurrentJig.GetFirstIndex(ETrayCellStatus.Ready)] = ETrayCellStatus.Done;
                            _trayList.TrayCamera[(uint)currentCellIndex] = ETrayCellStatus.Done;
                            FlagOut_TrayInElevatorUnloadCamRequest = false;
                            Log.Debug("Tray In Elevator Unload Camera done");
                            Step.RunStep++;
                            break;
                        }
                        if (FlagIn_TrayInElevatorUnloadCamPickFail == true)
                        {
                            /// Reset Status camera Skip
                            _trayList.TrayCamera[(uint)currentCellIndex] = ETrayCellStatus.PickFail;
                            FlagOut_TrayInElevatorUnloadCamRequest = false;
                            Log.Debug("Tray In Elevator Unload Camera Fail");
                            Step.RunStep = (int)ETrayInPutElevator_CamUnloadStep.End;
                            break;
                        }

                        Wait(20);
                        break;
                    case ETrayInPutElevator_CamUnloadStep.SetFlagTrayHeadPickCamera_Received:
                        FlagOut_TrayInElevatorUnloadCamDoneReceicved = true;
                        Log.Debug("Tray Head Pick Camera Received");
                        Step.RunStep++;
                        break;
                    case ETrayInPutElevator_CamUnloadStep.Wait_TrayHeadPickCamera_Received:
                        if (FlagIn_TrayInElevatorUnloadTrayDoneReceived == true)
                        {
                            FlagOut_TrayInElevatorUnloadCamDoneReceicved = false;
                            Step.RunStep++;
                            Log.Debug(" Check Received Tray Head Came Done");
                            break;
                        }
                        break;
                    case ETrayInPutElevator_CamUnloadStep.End:
                        if (Parent?.Sequence != ESequence.AutoRun)
                        {
                            Sequence = ESequence.Stop;
                            break;
                        }

                        Step.RunStep = (int)ETrayInPutElevator_CamUnloadStep.Tray_WorkCondition_Check;
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"Exception in Sequence_TrayInElevator_CamUnload: {ex.Message}");
            }
        }
        private void Sequence_TrayInElevator_TrayUnload()
        {
            //to do
            switch ((ETrayInPutElevator_TrayUnloadStep)Step.RunStep)
            {
                case ETrayInPutElevator_TrayUnloadStep.Start:
                    Log.Debug("TrayInElevator Sequence_TraySupIn_ElevatorLoad Start");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_Input_Elevator_Cv_Out_Request:
                    FlagOut_TrayInElevatorUnloadTrayRequest = true;
                    Log.Debug("TrayInElevator Cv Out Request ");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_In_Elevator_Wait_TrayHead_VacOn:
                    if (FlagIn_TrayHeadTrayPickVacOnOk)
                    {
                        Log.Debug("Tray Picker VacOn Done");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_In_Elevator_UnAlign:
                    if (Cyl_TrayCentering1.IsBackward && _devices.Cylinders.TraySupplier_TrayCentering2.IsBackward)
                    {
                        Log.Debug(" Centering Off already ");
                        Step.RunStep = (int)ETrayInPutElevator_TrayUnloadStep.Tray_Input_Elevator_Cv_Out_Request;
                        break;
                    }
                    Cyl_TrayCentering1.Backward();
                    _isTraySearch = false;
                    Wait(10000, () => Cyl_TrayCentering1.IsBackward && _devices.Inputs.TrayCentering2Off.Value);
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_In_Elevator_UnAlign_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_UnAlign_Timeout);
                        break;
                    }

                    Wait(200);
                    Log.Debug("TrayInElevator UnAlign Check OK");
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.SetFlag_Tray_In_Elevator_UnAlign_Done:
                    Log.Debug("Tray In Elevator UnAlign Done");
                    FlagOut_TrayInElevatorUnAlignDone = true;
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_Input_Elevator_Cv_Out_Done_Check:
                    if (FlagIn_TrayInElevatorUnloadTrayDone == false)
                    {
                        break;
                    }
                    Log.Debug("TrayInElevator Cv Out Done Check OK");
                    FlagOut_TrayInElevatorUnloadTrayRequest = false;
                    FlagOut_TrayInElevatorUnAlignDone = false;
                    _isTraySearch = false;

                    Wait(3000);

                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_In_Elevator_Reset:
                    Log.Debug("Reset amount Camera in Tray ");
                    foreach (var cell in CurrentJig.Cells)
                    {
                        cell.Status = ETrayCellStatus.Skip;
                    }
                    _isSetCamCount = false;
                    Step.RunStep++;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.Tray_in_Elevator_Check_Status_Exist:
                    if (In_TrayInCv2DetectExist.Value == true)
                    {
                        foreach (var cell in CurrentJig.Cells)
                        {
                            cell.Status = ETrayCellStatus.Ready;
                        }
                        _isSetCamCount = true;
                        Log.Debug("Tray In Elevator Exist. Move to Tray Search Sequence");
                        Sequence = ESequence.TraySearch;
                        break;
                    }
                    Log.Debug("Tray In Elevator Not Exist. Move to Tray Load Sequence");
                    Sequence = ESequence.TrayInElevator_Load;
                    break;
                case ETrayInPutElevator_TrayUnloadStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Sequence = ESequence.TraySearch;
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

        private void RollerRunStop(bool isRun)
        {
            if (isRun)
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayInLiftConveyor.Run();
                    return;
                }
                Roller_TrayInElevator.Run();
            }
            else
            {
                if (_devRecipe.IsUseRollerIOControl)
                {
                    _devices.CVs.TrayInLiftConveyor.Stop();
                    return;
                }
                Roller_TrayInElevator.Stop();
            }
        }

        private void RollerSetSpeed()
        {
            Roller_TrayInElevator.SetSpeed((int)_traySuplierRecipe.CVTrayInElevatorSpeed);
        }
        private void RollerSetAcc()
        {
            Roller_TrayInElevator.SetAcceleration((int)_traySuplierRecipe.Acceleration);
        }
        private void RollerSetDec()
        {
            Roller_TrayInElevator.SetDeceleration((int)_traySuplierRecipe.Deceleration);
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
        public override string ToString()
        {
            return EProcess.TrayInElevator.GetDescription();
        }
        #endregion

        #region Constructors
        public TrayInElevatorProcess(
            MachineStatus machineStatus,
            Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            TrayList trayList,
            MaterialStatusList materialStatusList,
            RecipeSelector recipeSelector,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer,
            [FromKeyedServices("TrayInElevatorInput")] IDInputDevice<ETrayInElevatorInput> trayInElevatorInput,
            [FromKeyedServices("TrayInElevatorOutput")] IDOutputDevice<ETrayInElevatorOutput> trayInElevatorOutput,
            DevRecipe devRecipe) : base(globalRecipe, machineStatus, devices, blinkTimer)
        {
            _machineStatus = machineStatus;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _trayList = trayList;
            _materialStatusList = materialStatusList;
            _recipeSelector = recipeSelector;
            _trayInElevatorInput = trayInElevatorInput;
            _trayInElevatorOutput = trayInElevatorOutput;
            _recipeSelector.RecipeSaved += RollerSet;
            _devRecipe = devRecipe;
        }
        #endregion

        #region Privates
        private readonly IDInputDevice _trayInElevatorInput;
        private readonly IDOutputDevice _trayInElevatorOutput;
        private readonly MachineStatus _machineStatus;
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly TrayList _trayList;
        private readonly MaterialStatusList _materialStatusList;
        private bool _isTraySearch { get; set; } = false;
        private RecipeSelector _recipeSelector;
        private TraySuplierRecipe _traySuplierRecipe => _recipeList.TraySuplierRecipe;
        private DevRecipe _devRecipe;
        private bool _isSetCamCount = false;
        private bool _canApplyAutoUiCameraSettingForNextTray = true;
        private List<ETrayCellStatus>? _autoUiCameraSettingForNextTray;

        private bool _isTraySetToRetryPickDone = false;

        private void CaptureAutoUiCameraSettingForNextTray()
        {
            if (!_canApplyAutoUiCameraSettingForNextTray || _autoUiCameraSettingForNextTray != null) return;
            _autoUiCameraSettingForNextTray = CurrentJig.Cells.Select(cell => cell.Status).ToList();
            Log.Debug("Captured Auto UI camera setting for next tray");
        }

        private void ApplyAutoUiCameraSettingForNextTray()
        {
            int count = Math.Min(CurrentJig.Cells.Count, _autoUiCameraSettingForNextTray!.Count);

            for (int i = 0; i < count; i++)
            {
                CurrentJig.Cells[i].Status = _autoUiCameraSettingForNextTray[i];
            }

            if (CurrentJig.Cells.Count > count)
            {
                for (int i = count; i < CurrentJig.Cells.Count; i++)
                {
                    CurrentJig.Cells[i].Status = ETrayCellStatus.Ready;
                }
            }

            Log.Debug("Applied Auto UI camera setting for current tray");
            _autoUiCameraSettingForNextTray = null;
            _canApplyAutoUiCameraSettingForNextTray = false;
        }

        #endregion

        private double searchPitch = 0.5;
        private ITray<ETrayCellStatus> CurrentJig => _trayList.TrayCamera;
        private bool JigWorkDone => CurrentJig.Cells.Count(c => c.Status == ETrayCellStatus.Ready || c.Status == ETrayCellStatus.Working) == 0;

    }
}
