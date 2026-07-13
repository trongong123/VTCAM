using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Core.Units;
using EQX.InOut;
using EQX.InOut.Virtual;
using EQX.Process;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Process.Step._05.TransferHeadProcess;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using System.Security.Policy;
using System.Windows;

namespace FrontCameraAssembleEquipment.Process
{
    public class TrayHeadProcess : ChangeReadyProcess
    {
        #region Inputs
        private IDInput In_TrayPickerVacOn => _devices.Inputs.TrayPickerVacOn;
        private IDInput In_VtCamSupplyPnPVacOn => _devices.Inputs.VtCamSupplyPnPVacOn;
        private IDInput In_VtCamPrealignVacOn => _devices.Inputs.VtCamPrealignVacOn;
        protected override IDInput In_LightCurtain => _devices.Inputs.AreaSensorDetect;
        protected override IDInput In_LightCurtainMuting => _devices.Inputs.LightCurtainMutingSW;
        #endregion

        #region Outputs
        private IDOutput Out_TrayPickerVacOn => _devices.Outputs.TrayPickerVacOn;
        private IDOutput Out_TrayPickerVacOff => _devices.Outputs.TrayPickerVacOff;
        private IDOutput Out_VtCamSupplyPnPVacOn => _devices.Outputs.VtCamSupplyPnPVacOn;
        private IDOutput Out_VtCamSupplyPnPVacOff => _devices.Outputs.VtCamSupplyPnPVacOff;

        private IDOutput Out_VtCamPreAlignVacOn => _devices.Outputs.VtCamPrealignVacOn;
        private IDOutput Out_VtCamPrealignVacOff => _devices.Outputs.VtCamPrealignVacOff;
        protected override IDOutput AreaSensorBypassOn => _devices.Outputs.AreaSensorBypassOn;
        protected override IDOutput Out_MutingSWLamp => _devices.Outputs.MutingLamp;

        #endregion

        #region Cylinders
        private ICylinder Cyl_TrayPicker => _devices.Cylinders.TrayPicker;


        // Use this Cylinder for PreCentering - not on this Process
        private ICylinder Cyl_PreAlign => _devices.Cylinders.FlipperSpongeDetach_VtCamCentering;
        #endregion

        #region Motions
        protected override IMotion XAxis => _devices.Motions.TrayHeadXAxis;
        protected override IMotion YAxis => _devices.Motions.TrayHeadYAxis;
        protected override IMotion ZAxis => _devices.Motions.TrayHeadZAxis;
        #endregion

        #region Flags
        /// <summary>
        /// Input
        /// </summary>
        private bool Flag_TrayInElevatorUnloadTrayREQ
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ];
        }
        private bool Flag_TrayInElevatorUnAlignDone
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_IN_ELEVATOR_UNALIGN_DONE];
        }

        private bool Flag_TrayInElevatorUnloadCamREQ
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ];
        }

        private bool Flag_SpongeDetachCamInREQ
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TAPE_DETACH_CAM_IN_REQ];
        }

        private bool FlagIn_TrayInElevatorCamUnloadDoneReceived
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED];
        }

        private bool Flag_TrayOutElevatorReadyPlace
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_OUT_ELEVATOR_READY_PLACE];
        }

        private bool FlagIn_ScanBarCodeError
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_HEAD_SCAN_BARCODE_ERROR];
        }
        private bool FlagIn_ScanBarCodeRun
        {
            get => _trayHeadInput[(int)ETrayHeadInput.TRAY_HEAD_SCAN_BARCODE_RUN];
        }

        /// <summary>
        /// Output 
        /// </summary>

        private bool Flag_TrayInElevatorUnloadCamDone
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAY_IN_ELEVATOR_CAM_UNLOAD_DONE] = value;
        }
        private bool Flag_TrayInElevatorUnloadCamPickFail
        {
            get => _trayHeadOutput[(int)ETrayHeadOutput.TRAYHEAD_CAM_UNLOAD_PICK_FAIL];
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAYHEAD_CAM_UNLOAD_PICK_FAIL] = value;
        }

        private bool Flag_TrayHeadTrayPickVacOnDone
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAY_IN_ELEVATOR_TRAYHEAD_VAC_ON] = value;
        }
        private bool Flag_TrayInElevatorUnloadTrayDone
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_DONE] = value;
        }

        private bool Flag_SpongeDetachCamInDone
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TAPE_DETACH_CAM_IN_DONE] = value;
        }

        private bool Flag_TrayOutElevatorPlaceDone
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAY_OUT_ELEVATOR_PLACE_DONE] = value;
        }

        private bool Flag_TrayHeadSafetyOut
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAYHEAD_OUT_OF_PLACE_AREA] = value;
        }

        private bool Flag_TrayHeadZUpDone
        {
            set
            {
                _trayHeadOutput[(int)ETrayHeadOutput.TRAYHEAD_Z_UP_DONE] = value;
            }
        }
        private bool FlagOut_TrayHeadPickCameDoneRecived
        {
            set => _trayHeadOutput[(int)ETrayHeadOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED] = value;
        }
        #endregion

        #region Vaccum
        private Vaccum TrayPickerVaccum => _vaccumList.TrayHead_TrayPickerVac;
        private Vaccum TrayCamPickerVaccum => _vaccumList.TrayHead_CamPickerVac;
        #endregion

        private double XYContiMoveSpeed => (_motions.TrayHeadXAxis.Parameter.Velocity + _motions.TrayHeadYAxis.Parameter.Velocity) / 2;

        #region Override Methods
        public override bool PreProcess()
        {
            if(ProcessMode == EProcessMode.Run && Sequence == ESequence.TrayHead_Tray_Place)
            {
                _machineStatus.IsTrayHeadTrayPlacing = true;
            }    
            else
            {
                _machineStatus.IsTrayHeadTrayPlacing = false;
            }
            //materialStatus.IsEditable = (_machineStatus.IsRunningProcessMode == false ? true : false);
            if (ProcessMode == EProcessMode.Run)
            {
                if (In_VtCamSupplyPnPVacOn.Value || In_TrayPickerVacOn.Value)
                {
                    if (In_VtCamSupplyPnPVacOn.Value)
                    {
                        materialStatus.Type = EMaterialType.Camera;
                    }
                    else if (In_TrayPickerVacOn.Value)
                    {
                        materialStatus.Type = EMaterialType.Tray;
                    }

                    materialStatus.Set();
                }
                else materialStatus.Clear();

                // Flag Out Of Place Area set
                //if(YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlaceReadyPosition))
                //{
                //    Flag_TrayHeadSafetyOut = true;
                //}
                //                else
                //                {
                //                    //Flag_TrayHeadSafetyOut = false;
                //                }
                //                if ((Flag_TraySearchInElevatorRecived || Flag_TraySearchOutElevatorRecived) && (ZAxis.Status.ActualPosition>10 || Cyl_TrayPicker.IsForward))
                //                {
                //////// ad sequence interlock

                //                }
                /// Tray Place prior

            }
            return base.PreProcess();
        }
        public override string ToString()
        {
            return EProcess.TrayHead.GetDescription();
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
        public override bool ProcessToRun()
        {
            switch ((ETrayHead_ToRunStep)Step.ToRunStep)
            {
                case ETrayHead_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)ETrayHead_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ETrayHeadOutput>)_trayHeadOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.MaterialDataMatching_VacOn: // Change to up TrayPick Cylinder
                    //CamPickerVac(true);
                    //Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamSupplyPnPVacOn.Value);

                    // Change to up TrayPick Cylinder
                    if (Cyl_TrayPicker.IsForward)
                    {
                        Cyl_TrayPicker.Backward();
                        Wait(10000, () => Cyl_TrayPicker.IsBackward);
                        Step.ToRunStep++;
                        break;
                    }

                    Step.ToRunStep = (int)ETrayHead_ToRunStep.CheckZReadyForTraySearch;
                    break;
                case ETrayHead_ToRunStep.MaterialDataMatching_Check: // Change to up TrayPick Cylinder
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }
                    //Log.Debug("Material Data Matching Check");
                    //bool bCamExist = In_VtCamSupplyPnPVacOn.Value;
                    //bool bTrayExist = In_TrayPickerVacOn.Value;

                    //bool bExistCheck = (materialStatus.Type == EMaterialType.Camera) ? bCamExist : bTrayExist;

                    //bool bMaterialStatus = (materialStatus.Status == EMaterialStatus.Existing ? true : false);

                    //if (bCamExist == false)
                    //{
                    //    CamPickerVac(false);
                    //}

                    //if (bTrayExist == false)
                    //{
                    //    TrayPickerVac(false);
                    //}

                    //if (bExistCheck != bMaterialStatus)
                    //{
                    //    RaiseWarning((int)EWarning.MaterialDataNotMatching);
                    //    break;
                    //}
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.CheckZReadyForTraySearch:
                    //if ((ZAxis.Status.ActualPosition >= 5|| Cyl_TrayPicker.IsForward) && YAxis.Status.ActualPosition <220)
                    //{
                    //    Log.Debug("Z Axis and Cylinder of Tray head  Not Safe ");
                    //    Step.ToRunStep++;
                    //    break;
                    //}
                    Step.ToRunStep = (int)ETrayHead_ToRunStep.Cylinder_Up;
                    break;

                case ETrayHead_ToRunStep.ZAxisMovetoReady:
                    Log.Debug("Z Axis and Cylinder Tray Picker Move to Ready");
                    Cyl_TrayPicker.Backward();
                    ZAxis.MoveAbs(0);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(0) && Cyl_TrayPicker.IsBackward);
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.ZAxisMovetoReady_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Move to Z Ready or Cylinder Up Fail");
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPickCameraPosition_Fail);
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.Cylinder_Up:
                    if (Cyl_TrayPicker.IsBackward)
                    {
                        Log.Debug("Cylinder Tray Picker Up Ready");
                        Step.ToRunStep = (int)ETrayHead_ToRunStep.End;
                        break;
                    }
                    Cyl_TrayPicker.Backward();
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.Cylinder_Up_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Cylinder Move Up Timeout");
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }
                    Log.Debug("Cylinder Tray Picker Up Ready");
                    Step.ToRunStep++;
                    break;
                case ETrayHead_ToRunStep.End:
                    Log.Debug("To Run end.");
                    ProcessStatus = EProcessStatus.ToRunDone;
                    Step.ToRunStep++;
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool _isXAxisOriginSelected => MotionSelection.IsSelected(XAxis);
        private bool _isYAxisOriginSelected => MotionSelection.IsSelected(YAxis);
        public override bool ProcessOrigin()
        {
            switch ((ETrayHead_OriginStep)Step.OriginStep)
            {
                case ETrayHead_OriginStep.Start:
                    Log.Debug("TrayHead Origin start.");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.CheckOriginSelected:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.Cyl_TrayPicker_Up:
                    Log.Debug("Tray Picker UP.");
                    Cyl_TrayPicker.Backward();
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayPickerUp, true);
#endif
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.Cyl_TrayPicker_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }

                    Log.Debug("Tray Picker UP Done.");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.ZAxis_Origin:
                    Log.Debug("Z Axis Origin Start.");
                    ZAxis.SearchOrigin();
                    Wait((int)(_globalRecipe.MotionOriginTimeout * 1000), () => { return ZAxis.Status.IsHomeDone; });
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.ZAxis_Origin_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_OriginFail);
                        break;
                    }

                    Log.Debug("Z Axis Origin Done.");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.Set_FlagTrayHeadZAxisHomeDone:
                    Log.Debug("Set Flag Tray Head Z Axis Home Done.");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.XYAxis_Origin:
                    if (_isXAxisOriginSelected == false && _isXAxisOriginSelected == false)
                    {
                        Step.OriginStep = (int)ETrayHead_OriginStep.ZAxis_MoveToWaitPos;
                        break;
                    }
                    Log.Debug("XY Axis Origin Start.");
                    if (_isXAxisOriginSelected) { XAxis.SearchOrigin(); }
                    if (_isYAxisOriginSelected) { YAxis.SearchOrigin(); }

                    Wait((int)_globalRecipe.MotionOriginTimeout * 1000, () =>
                    {
                        if (_isXAxisOriginSelected && _isYAxisOriginSelected)
                        {
                            return XAxis.Status.IsHomeDone && YAxis.Status.IsHomeDone;
                        }
                        else if (_isXAxisOriginSelected)
                        {
                            return XAxis.Status.IsHomeDone;
                        }
                        else
                        {
                            return YAxis.Status.IsHomeDone;
                        }
                    });
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.XYAxis_Origin_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_OriginFail);
                        break;
                    }

                    Log.Debug("XY Axis Origin Done.");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.ZAxis_MoveToWaitPos:
                    ZAxis.MoveAbs(0);
                    Log.Debug("Z Axis Move to Wait Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(0));
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.ZAxis_MoveToWaitPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveWaitPosition_Fail);
                        break;
                    }
                    Log.Debug("Z Axis Move to Wait Pos Done");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.XYAxis_MoveToWaitPos:
                    if(_isXAxisOriginSelected == false || _isYAxisOriginSelected == false)
                    {
                        Step.OriginStep = (int)ETrayHead_OriginStep.End;
                        break;
                    }
                    Log.Debug("XY Axis Move to Wait Pos");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisCamPickPosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisCamPickPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPickPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPickPosition));
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.XYAxis_MoveToWaitPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MoveWaitPosition_Fail);
                        break;
                    }
                    Log.Debug("XY Axis Move to Wait Pos Done");
                    Step.OriginStep++;
                    break;
                case ETrayHead_OriginStep.End:
                    Flag_TrayHeadZUpDone = true;
                    Log.Debug("TrayHead Origin End.");
                    ProcessStatus = EProcessStatus.OriginDone;
                    Step.OriginStep++;
                    break;

                default:
                    Wait(20);
                    break;
            }
            return true;
        }
        private void Sequence_Ready()
        {
            switch ((ETrayHead_Init)Step.RunStep)
            {
                case ETrayHead_Init.Start:
                    {
                        if (IsOriginOrInitSelected == false)
                        {
                            Sequence = ESequence.Stop;
                            break;
                        }
                        IsTrayScanBarcodeSuccess = false;
                        isScaned = false;
                        Log.Debug("Start Init");
                        Step.RunStep++;
                        break;
                    }
                case ETrayHead_Init.ZAxisUp:
                    {
                        Log.Debug("Z Up to Ready");
                        ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                        Step.RunStep++;
                        break;
                    }
                case ETrayHead_Init.TrayPickerUp:
                    Log.Debug("Tray Picker UP.");
                    Cyl_TrayPicker.Backward();
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayPickerUp, true);
#endif
                    Step.RunStep++;
                    break;
                case ETrayHead_Init.TrayPickerUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }
                    Log.Debug("Tray Picker UP Done.");
                    Step.RunStep++;
                    break;
                case ETrayHead_Init.ZAxisUp_Check:
                    {
                        Log.Debug("Z Up to Ready Check");
                        if (WaitTimeOutOccurred)
                        {
                            Log.Debug("Move to Z Ready Fail");
                            RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPickCameraPosition_Fail);
                            break;
                        }
                        Flag_TrayHeadZUpDone = true;
                        Log.Debug("Move to Z Ready Done");
                        Step.RunStep++;
                        break;
                    }
                case ETrayHead_Init.MoveXY_Ready_Pos:
                    {
                        Log.Debug("Move to X Y Ready");
                        XAxis.MoveAbs(_trayHeadRecipe.XAxisWaitPosition);
                        YAxis.MoveAbs(_trayHeadRecipe.YAxisWaitPosition);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisWaitPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisWaitPosition));
                        Step.RunStep++;
                        break;
                    }
                case ETrayHead_Init.MoveXY_Ready_Pos_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
                            Log.Debug("Move to X Y Ready Fail");
                            RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MoveWaitPosition_Fail);
                            break;
                        }
                        Log.Debug("Move to XY Ready Done");
                        Step.RunStep++;
                        break;
                    }
                case ETrayHead_Init.End:
                    {
                        ((MappableOutputDevice<ETrayHeadOutput>)_trayHeadOutput).ClearOutputs();
                        Log.Debug("Ready End");
                        Flag_TrayHeadZUpDone = true;
                        Sequence = ESequence.Stop;
                        break;
                    }

            }
        }

        public override bool ProcessToStop()
        {
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
                    Sequence_AutoRun();
                    break;
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.Change:
                    Sequence_Change();
                    break;
                case ESequence.TrayHead_Tray_Pick:
                    Sequence_TrayHead_TrayPick();
                    break;
                case ESequence.TrayHead_Tray_Place:
                    Sequence_TrayHead_TrayPlace();
                    break;
                case ESequence.TrayHead_Cam_Pick:
                    Sequence_TrayHead_CamPick();
                    break;
                case ESequence.TrayHead_Cam_Place:
                    Sequence_TrayHead_CamPlace();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }

            return true;
        }

        private void Sequence_Change()
        {
            switch ((ETrayHead_ChangeStep)Step.RunStep)
            {
                case ETrayHead_ChangeStep.Start:
                    Log.Debug("Tray Head Change Step Start");
                    Step.RunStep++;
                    break;

                case ETrayHead_ChangeStep.Stop_XYZAxis:
                    Log.Debug("Stop XYZ Axis ");
                    XAxis.Stop();
                    YAxis.Stop();
                    ZAxis.Stop();
                    Wait(3000, () => XAxis.Status.IsMotioning == false && YAxis.Status.IsMotioning == false && ZAxis.Status.IsMotioning == false);
                    Step.RunStep++;
                    break;
                case ETrayHead_ChangeStep.Stop_XYZAxis_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayINLift_StopFail);
                        Log.Debug("Stop XYZ Axis Fail");
                        break;
                    }
                    Log.Debug("Stop Z Axis Done");
                    Step.RunStep++;
                    break;
                case ETrayHead_ChangeStep.End:
                    Log.Debug("Change End");
                    Step.RunStep++;
                    break;
                default:
                    break;
            }
        }
        private void Sequence_TrayHead_TrayPick()
        {
            switch ((ETrayHead_TrayPickStep)Step.RunStep)
            {
                case ETrayHead_TrayPickStep.Start:
                    Log.Debug("Tray Pick Start.");
                    //FlagIn_TrayInElevatorUnloadTrayDone = false;
                    Step.RunStep++;
                    break;

                case ETrayHead_TrayPickStep.Wait_TrayInElevatorUnloadTrayRequest:
                    if (Flag_TrayInElevatorUnloadTrayREQ)
                    {
                        Log.Debug("Tray In Elevator Unload Tray Request.");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ETrayHead_TrayPickStep.ZAxis_MoveWait:

                    Log.Debug("Z Axis Move Wait.");
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Flag_TrayInElevatorUnloadCamDone = false;
                    Flag_TrayInElevatorUnloadCamPickFail = false;
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.ZAxis_MoveWait_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveWaitPosition_Fail);
                        break;
                    }

                    Log.Debug("Z Axis Move Wait Position Done.");
                    Flag_TrayHeadZUpDone = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.XYAxis_MovePickPosition:
                    if (_trayHeadRecipe.UseWarningWhenPickTray == 1)
                    {
                        if (CurrentJig.GetFirstIndex(ETrayCellStatus.PickFail) != -1)
                        {
                            Log.Debug("XY Axis Move Place Tray Position.");

                            XAxis.MoveAbs(_trayHeadRecipe.XAxisTrayPlacePosition);
                            YAxis.MoveAbs(_trayHeadRecipe.YAxisTrayPlacePosition);
                            Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisTrayPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisTrayPlacePosition));
                            Step.RunStep++;
                            break;
                        }
                    }

                    Log.Debug("XY Axis Move Pick Tray Position.");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisTrayPickPosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisTrayPickPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisTrayPickPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisTrayPickPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.XYAxis_MovePickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        if (_trayHeadRecipe.UseWarningWhenPickTray == 1)
                        {
                            if (CurrentJig.GetFirstIndex(ETrayCellStatus.PickFail) != -1)
                            {
                                RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePlaceTrayPosition_Fail);
                                break;
                            }
                        }
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePickTrayPosition_Fail);
                        break;
                    }

                    if (_trayHeadRecipe.UseWarningWhenPickTray == 1)
                    {
                        if (CurrentJig.GetFirstIndex(ETrayCellStatus.PickFail) != -1)
                        {
                            Log.Debug("XY Axis Move Place Position Done.");
                            Wait(100);
                            Step.RunStep++;
                            break;
                        }
                    }
                    Log.Debug("XY Axis Move Pick Position Done.");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.TrayPick_Warning_Check_Metarial_In_Tray_Empty:
                    if (_trayHeadRecipe.UseWarningWhenPickTray == 1)
                    {
                        if (CurrentJig.GetFirstIndex(ETrayCellStatus.PickFail) != -1)
                        {
                            RaiseWarning((int)EWarning.TrayCAMLoader_Camera_PickFail);
                            break;
                            //_devices.Outputs.TowerLampBuzzer_Alarm();
                            //if (MessageBoxEx.ShowDialog("WARNING: Check Metarial In Tray !") == true)
                            //{
                            //    _machineStatus.IsTrayEmptyConfirm = true;
                            //}
                            //else
                            //{
                            //    _machineStatus.IsTrayEmptyConfirm = false;
                            //}

                            //_devices.Outputs.Buzzer1.Value = false;
                            //if (_machineStatus.IsTrayEmptyConfirm)
                            //{
                            //    Log.Debug("Tray Empty was Confirm OK");
                            //    Step.RunStep++;
                            //    break;
                            //}
                        }
                    }
                    Log.Debug("Tray Empty OK");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_Down:
                    Log.Debug("Tray Picker Down.");
                    Cyl_TrayPicker.Forward();
                    Wait(10000, () => Cyl_TrayPicker.IsForward);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_Down_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Down_Fail);
                        break;
                    }
                    Log.Debug("Tray Picker Down Done.");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_VacOn:
                    if (In_TrayPickerVacOn.Value)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Tray Picker Vaccum On.");
                    TrayPickerVac(true);
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_TrayPickerVacOn.Value);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_VacOn_Check:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_VacuumOn_Fail);
                        break;
                    }

                    Log.Debug("Tray Picker Vaccum On Done.");
                    Wait(1000);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.SetFlag_TrayPickerVacOnDone:
                    Log.Debug("Set Flag Tray Picker Vac On Done");
                    Flag_TrayHeadTrayPickVacOnDone = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Wait_TrayInElevatorUnAlignDone:
                    if (Flag_TrayInElevatorUnAlignDone)
                    {
                        Flag_TrayHeadTrayPickVacOnDone = false;
                        Log.Debug("Tray In Elevator UnAling Done");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_Up:
                    if (Cyl_TrayPicker.IsBackward)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Tray Picker Up.");
                    Cyl_TrayPicker.Backward();
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayPickerUp, true);
#endif
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Cyl_TrayPicker_Up_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }

                    Log.Debug("Tray Picker Up Done.");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.SetFlag_TrayInElevatorUnloadTrayDone:
                    if (In_TrayPickerVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_VacuumOn_Fail);
                        break;
                    }

                    Log.Debug("Set Flag Tray In Elevator Unload Tray Done.");
                    Flag_TrayInElevatorUnloadTrayDone = true;
                    Log.Debug("Wait Tray In Elevator Unload Tray Done Received.");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPickStep.Wait_TrayInElevatorUnloadTrayReveived:
                    if (Flag_TrayInElevatorUnloadTrayREQ == false)
                    {
                        Log.Debug("Tray In Elevator Unload Tray Done Received.");
                        Flag_TrayInElevatorUnloadTrayDone = false;
                        Step.RunStep++;
                        break;
                    }
                    break;
                case ETrayHead_TrayPickStep.End:
                    Log.Debug("Pick Tray end");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        Parent.ProcessMode = EProcessMode.ToStop;
                        break;
                    }
                    IsTrayScanBarcodeSuccess = false;

                    Log.Debug("Sequence Tray Head Tray Place");
                    Sequence = ESequence.TrayHead_Tray_Place;
                    break;
                default:
                    break;
            }

        }

        private void Sequence_TrayHead_TrayPlace()
        {
            switch ((ETrayHead_TrayPlaceStep)Step.RunStep)
            {
                case ETrayHead_TrayPlaceStep.Start:
                    Log.Debug("Tray Place Start");

                    Flag_TrayOutElevatorPlaceDone = false;
                    foreach (var cell in CurrentJig.Cells)
                    {
                        cell.Status = ETrayCellStatus.Ready;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveUp_1st:
                    Log.Debug("Cylinder Tray Picker move up");
                    Cyl_TrayPicker.Backward();
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayPickerUp, true);
#endif
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveUp_1St_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }
                    Flag_TrayHeadTrayPickVacOnDone = false;
                    Log.Debug("Cylinder Tray Picker move up done");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.ZAxis_MoveWaitPosition:
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Log.Debug("Z Axis move wait position");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.ZAxis_MoveWaitPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveWaitPosition_Fail);
                        break;
                    }
                    Log.Debug("Z Axis move wait position done");
                    Step.RunStep++;
                    Flag_TrayHeadZUpDone = true;
                    break;
                case ETrayHead_TrayPlaceStep.Wait_TrayOutElevator_ReadyPlace:
                    if (Flag_TrayOutElevatorReadyPlace)
                    {
                        Log.Debug("Tray Out Elevator ready place.");
                        Wait(100);
                        Step.RunStep++;
                        break;
                    }
                    Wait(100);
                    break;
                case ETrayHead_TrayPlaceStep.XYAxis_MoveReadyPlacePosition:
                    Log.Debug("X, Y Axis move place tray position");
                    XAxis.MoveAbs(_trayHeadRecipe.XAxisTrayPlacePosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisTrayPlacePosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisTrayPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisTrayPlacePosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.XYAxis_MoveReadyPlacePosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePlaceTrayPosition_Fail);
                        break;
                    }

                    Log.Debug("X, Y Axis move place tray done");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.TrayPicker_Vacuum_Check:
                    if (In_TrayPickerVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_VacuumOn_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveDown:
                    if (Cyl_TrayPicker.IsForward)
                    {
                        Step.RunStep = (int)ETrayHead_TrayPlaceStep.Vac_TrayPicker_Off;
                        break;
                    }
                    Log.Debug("Cyl TrayPicker move down");
                    Cyl_TrayPicker.Forward();
                    Wait(10000, () => Cyl_TrayPicker.IsForward);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveDown_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Down_Fail);
                        break;
                    }

                    Log.Debug("Cyl TrayPicker move down done");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Vac_TrayPicker_Off:
                    //if (!In_TrayPickerVacOn.Value)
                    //{
                    //    Step.RunStep++;
                    //    break;
                    //}
                    Log.Debug("Tray Picker Vaccum Off");
                    TrayPickerVac(false);
                    Wait(3000, () => !In_TrayPickerVacOn.Value);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Vac_TrayPicker_Off_Wait:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_VacuumOff_Fail);
                        break;
                    }

                    Wait(300);
                    Log.Debug("Tray Picker Vacuum Off done");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveUp:
                    if (Cyl_TrayPicker.IsBackward)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Cylinder Tray Picker move up");
                    Cyl_TrayPicker.Backward();
#if SIMULATION
                    SimulationInputSetter.SetSimInput(_devices.Inputs.TrayPickerUp, true);
#endif
                    Wait(10000, () => Cyl_TrayPicker.IsBackward);
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Cyl_TrayPicker_MoveUp_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_TrayPicker_Up_Fail);
                        break;
                    }

                    Log.Debug("Cylinder Tray Picker move up done");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Set_Flag_TrayOutElevatorPlaceDone:
                    Log.Debug("Set flag TrayOutElevatorPlaceDone");
                    Flag_TrayOutElevatorPlaceDone = true;
                    Log.Debug("Wait flag TrayOutElevatorPlaceDone Received");
                    Step.RunStep++;
                    break;
                case ETrayHead_TrayPlaceStep.Wait_TrayOutElevatorPlaceDoneReceived:
                    if (Flag_TrayOutElevatorReadyPlace == false)
                    {
                        Log.Debug("TrayOutElevatorPlaceDone Received");
                        Flag_TrayOutElevatorPlaceDone = false;
                        Step.RunStep++;
                    }

                    break;
                case ETrayHead_TrayPlaceStep.End:
                    Log.Debug("Place tray done");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Log.Debug("Sequence Tray Head Pick Camera");
                    Sequence = ESequence.TrayHead_Cam_Pick;
                    break;
                default:
                    break;
            }

        }

        private int RetryVacc = 2;
        private double RetryZUpGap = 20.0;

        private void Sequence_TrayHead_CamPick()
        {
            switch ((ETrayHead_CamPickStep)Step.RunStep)
            {
                case ETrayHead_CamPickStep.Start:
                    Log.Debug("Tray Head pick camera start");
                    Step.RunStep++;
                    Flag_TrayInElevatorUnloadCamPickFail = false;
                    break;
                case ETrayHead_CamPickStep.Index_Initiation:
                    if (Flag_TrayInElevatorUnloadTrayREQ)
                    {
                        Sequence = ESequence.TrayHead_Tray_Pick;
                        break;
                    }
                    if (Flag_TrayInElevatorUnloadCamREQ == false)
                    {
                        Wait(20);
                        break;
                    }
                    if (_trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Working) == -1)
                    {
                        if (_trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Ready) == -1)
                        {
                            Sequence = ESequence.TrayHead_Tray_Pick;
                            break;
                        }
                        Wait(10);
                        break;
                    }
                    _countRetryVacc = 1;
                    currentCellIndex = _trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Working);
                    trayIndexX = _trayList.TrayCamera.GetFirstColumn(ETrayCellStatus.Working);
                    trayIndexY = _trayList.TrayCamera.GetFirstRow(ETrayCellStatus.Working);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.ZAxis_Move_ReadyPickPosition:
                    ZAxis.MoveAbs(_countRetryVacc <= 1 ? _trayHeadRecipe.ZAxisReadyPosition : _trayHeadRecipe.ZAxisCamPickPosition - RetryZUpGap);
                    Log.Debug("Z Axis move ready pick position");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => ZAxis.IsOnPosition(
                            _countRetryVacc <= 1
                        ? _trayHeadRecipe.ZAxisReadyPosition
                        : _trayHeadRecipe.ZAxisCamPickPosition - RetryZUpGap)
                        );
                    Step.RunStep++;
                    break;

                case ETrayHead_CamPickStep.ZAxis_Move_ReadyPickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                    Step.RunStep++;
                    break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPickCameraPosition_Fail);
                        break;
                    }
                    Flag_TrayHeadZUpDone = true;
                    Log.Debug("ZAxis move ready pick position done.");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.TrayInElevatorCamUnload_Request_Check:
                    {
                        if (Flag_TrayInElevatorUnloadCamREQ)
                        {
                            Log.Debug("Tray In Elevator request unload Camera");
                            Step.RunStep = (int)ETrayHead_CamPickStep.XYAxis_Move_PickPosition;
                            break;
                        }
                        Step.RunStep++;
                        Wait(20);

                    }
                    break;
                case ETrayHead_CamPickStep.XYAxis_Move_WaitPickPosition:
                    Log.Debug("X, Y Axis move pick camera Wait position");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisWaitPosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisWaitPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisWaitPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisWaitPosition));
                    Step.RunStep++;
                    break;

                case ETrayHead_CamPickStep.XYAxis_Move_WaitPickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePickCameraPosition_Fail);
                        break;
                    }

                    Log.Debug("X, Y Axis move pick camera Wait position done");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.Wait_TrayInElevatorCamUnload_Request:
                    if (Flag_TrayInElevatorUnloadCamREQ && !_machineStatus.IsPickUpStop)
                    {
                        Log.Debug("Tray In Elevator request unload Camera");
                        Step.RunStep++;
                        break;
                    }

                    Wait(20);
                    break;
                case ETrayHead_CamPickStep.XYAxis_Move_PickPosition:
                    Log.Debug("X, Y Axis move pick camera position");

                    double positionX = _trayHeadRecipe.XAxisCamPickPosition + ((trayIndexX - 1) * _traySuplierRecipe.TrayPitchX);
                    double positionY = _trayHeadRecipe.YAxisCamPickPosition - ((trayIndexY - 1) * _traySuplierRecipe.TrayPitchY);
                    XAxis.MoveAbs(positionX);
                    YAxis.MoveAbs(positionY);

                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(positionX) && YAxis.IsOnPosition(positionY));
                    Step.RunStep++;
                    break;

                case ETrayHead_CamPickStep.XYAxis_Move_PickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePickCameraPosition_Fail);
                        break;
                    }

                    Log.Debug("X, Y Axis move pick camera position done");
                    Wait_move = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.ZAxis_Move_PickPosition:
                    if (_countRetryVacc < RetryVacc)
                    {
                        ZAxis.MoveAbs(_trayHeadRecipe.ZAxisCamPickPosition);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisCamPickPosition));
                    }
                    else
                    {
                        double Zretry = _trayHeadRecipe.ZAxisCamPickPosition + 2;
                        ZAxis.MoveAbs(Zretry);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(Zretry));
                    }
                    Log.Debug("Z Axis move pick camera position");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.ZAxis_Move_PickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MovePickCameraPosition_Fail);
                        break;
                    }

                    Log.Debug("ZAxis move pick camera position done");
                    Wait_move = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.VacuumOn:
                    Log.Debug("Vacuum On");
                    CamPickerVac(true);
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamSupplyPnPVacOn.Value);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.VacuumOnCheck:
                    Log.Debug("Vaccum On delay.");
                    if (_machineStatus.IsDryRunMode)
                    {
                        Flag_TrayInElevatorUnloadCamDone = true;
                        Step.RunStep++;
                        break;
                    }
                    if (WaitTimeOutOccurred)
                    {
                        _countRetryVacc++;
                        Log.Debug(" Vaccum On Fail");
                        //CamPickerVac(false);
                        if (_countRetryVacc <= RetryVacc)
                        {
                            Step.RunStep = (int)ETrayHead_CamPickStep.ZAxis_Move_ReadyPickPosition;
                            break;
                        }
                        Flag_TrayInElevatorUnloadCamPickFail = true;
                        Step.RunStep = (int)ETrayHead_CamPickStep.Wait_Flag_TrayInElevatorCamUnloadDone_Received;
                        if (_trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Ready) == -1
                            && _trayList.TrayCamera.GetFirstIndex(ETrayCellStatus.Working) == -1)
                        {
                            Sequence = ESequence.TrayHead_Tray_Pick;
                        }
                        break;
                    }
                    Flag_TrayInElevatorUnloadCamDone = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.Wait_Flag_TrayInElevatorCamUnloadDone_Received:
                    if (Flag_TrayInElevatorUnloadCamREQ == false || FlagIn_TrayInElevatorCamUnloadDoneReceived == true)
                    {
                        if (Flag_TrayInElevatorUnloadCamPickFail == true)
                        {
                            Sequence = ESequence.TrayHead_Cam_Pick;
                            break;
                        }
                        Log.Debug("Tray In Elevator Cam Unload Done Received");
                        Flag_TrayInElevatorUnloadCamDone = false;
                        Flag_TrayInElevatorUnloadCamPickFail = false;
                        FlagOut_TrayHeadPickCameDoneRecived = true;
                        Step.RunStep++;
                        break;
                    }
                    break;
                case ETrayHead_CamPickStep.SetFlag_TrayInElevatorCamUnload_Done:
                    Log.Debug("Set flag Tray In Elevator Cam Unload done.");
                    Wait(500);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.ZAxis_MoveBack_ReadyPickPosition:
                    if (ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition) && !_machineStatus.IsDryRunMode)
                    {
                        Step.RunStep++;
                        break;
                    }
                    FlagOut_TrayHeadPickCameDoneRecived = false;

                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Log.Debug("Z Axis move back ready pick position");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.ZAxis_MoveBack_ReadyPickPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                    Step.RunStep++;
                    break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPickCameraPosition_Fail);
                        break;
                    }
                    Flag_TrayHeadZUpDone = true;
                    Log.Debug("ZAxis move back ready pick position done.");
                    if (In_VtCamSupplyPnPVacOn.Value == false && _machineStatus.IsDryRunMode == false )
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_VtCamSupplyPnP_VacOn_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPickStep.End:
                    Log.Debug("Sequence Tray Head Pick Camera done.");
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    Log.Debug("Sequence Tray Head Cam Place");
                    Sequence = ESequence.TrayHead_Cam_Place;
                    break;
                default:
                    break;
            }
        }

        private void Sequence_TrayHead_CamPlace()
        {
            switch ((ETrayHead_CamPlaceStep)Step.RunStep)
            {
                case ETrayHead_CamPlaceStep.Start:
                    Log.Debug("Tray Head Camera place start");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Check_Status_Tray_Head_Is_Placing:
                    double ZAxisActual = ZAxis.Status.ActualPosition;
                    if (((_trayHeadRecipe.ZAxisCamPlacePosition - ZAxisActual) < 10) && XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlacePosition) && isScaned)
                    {
                        Log.Debug("Tray Head Is On Placing Pos");
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.VacuumOff;
                        break;
                    }
                    Log.Debug("Tray Head is Placing Processing ");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Ready_Position:
                    if (ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition) && isScaned)
                    {
                        Flag_TrayHeadZUpDone = true;
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.XYAxis_Move_ReadyPlacePosition;
                        break;
                    }
                    Log.Debug("ZAxis move ready place camera");
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Ready_Position_Check_1st:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Move to Ready Z fail");
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveWaitPosition_Fail);
                        break;
                    }
                    Flag_TrayHeadZUpDone = true;
                    Log.Debug("Z Move to Ready Ok");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_ReadyPlacePosition_Check_Status:
                    if (XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPlacePosition) && (YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlacePosition)))
                    {
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.Wait_FlagSpongeDetachCamInRequestBeforePlace;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Barcode_Use_Check:
                    if ((_globalRecipe.UseScaner == false) || (trayIndexX == 0 && trayIndexY ==0))
                    {
                        isScaned = true;
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.ZAxis_Move_ReadyPlacePosition;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.No_Camera_Check:
                    //If not use Scan Only One Camemra => Scan All Camera
                    if(_trayHeadRecipe.UseScanOnlyOneCam == 0)
                    {
                        Step.RunStep++;
                        break;
                    }
                    if ((trayIndexX == 1 && trayIndexY == 1 && IsTrayScanBarcodeSuccess == false) || (IsTrayScanBarcodeSuccess == false && _devRecipe.UseRetryBarcodeScan))
                    {
                        Log.Debug("First Cam");
                        Step.RunStep++;
                        break;
                    }

                    isScaned = true;
                    Log.Debug("Move to Z Ready place Pos");
                    Step.RunStep = (int)ETrayHead_CamPlaceStep.ZAxis_Move_ReadyPlacePosition;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_ScanPosition:
                    if (XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamScanPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamScanPosition))
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("XY Axis move to Scan Position");
                    XAxis.MoveAbs(_trayHeadRecipe.XAxisCamScanPosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisCamScanPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamScanPosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamScanPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_ScanPosition_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("XY Axis move to Scan Position fail");
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MoveScanCameraPosition_Fail);
                        break;
                    }
                    Log.Debug("XY Axis move to Scan Position done");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Scan_Barcode_Request:
                    if (_globalRecipe.UseScaner == false)
                    {
                        isScaned = true;
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.ZAxis_Move_ReadyPlacePosition;
                        break;
                    }
                    _visionProcess.Vision_job(EVisionCmd.CMD_BARCODE_SEARCH);
                    Log.Debug("Scan BarCode Request");
                    Wait(200);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Wait_Result_Barcode:
                    Wait(4000, () => FlagIn_ScanBarCodeRun == false);
                    Log.Debug("Wait Result Barcode");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Check_Reseult_Barcode:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Scan BarCode TimeOut");
                        if (_devRecipe.UseRetryBarcodeScan && _trayHeadRecipe.UseScanOnlyOneCam != 0)
                        {
                            Step.RunStep = (int)ETrayHead_CamPlaceStep.XYAxis_Move_ReturnPosition;
                            break;
                        }
                        RaiseWarning((int)EWarning.TrayCAMLoader_ScanBarCode_Fail);
                        break;
                    }
                    if (FlagIn_ScanBarCodeError)
                    {
                        Log.Debug("Scan BarCode Error");
                        if (_devRecipe.UseRetryBarcodeScan && _trayHeadRecipe.UseScanOnlyOneCam != 0)
                        {
                            Step.RunStep = (int)ETrayHead_CamPlaceStep.XYAxis_Move_ReturnPosition;
                            break;
                        }
                        RaiseWarning((int)EWarning.TrayCAMLoader_ScanBarCode_Fail);
                        break;
                    }

                    if (_visionProcess.IsBarcodeIdMatched == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_Barcode_NotMatched);
                        break;
                    }
                    Log.Debug($"Barcode: {_visionProcess.BarcodeId}");
                    IsTrayScanBarcodeSuccess = true;

                    isScaned = true;
                    Wait(100);
                    Log.Debug("Scan OK ");
                    Step.RunStep++;
                    break;

                case ETrayHead_CamPlaceStep.ZAxis_Move_ReadyPlacePosition:
                    Log.Debug("ZAxis move ready place camera");
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Log.Debug("Z Axis move ready place camera");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_ReadyPlacePosition_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
#if SIMULATION
                Step.RunStep++;
                break;
#endif
                            RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPlaceCameraPosition_Fail);
                            break;
                        }
                        Flag_TrayHeadZUpDone = true;
                        Log.Debug("Z Axis move ready place camera done");
                        Step.RunStep++;
                    }
                    break;

                case ETrayHead_CamPlaceStep.XYAxis_Move_ReadyPlacePosition:
                    Log.Debug("X, Y Axis move ready place camera position");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisCamPlacePosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisCamPlacePosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlacePosition));
                    Step.RunStep++;
                    break;

                case ETrayHead_CamPlaceStep.XYAxis_Move_ReadyPlacePosition_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
#if SIMULATION
                Step.RunStep++;
                break;
#endif
                            Log.Debug("X, Y Axis move ready place camera position Fail");
                            RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MoveReadyPlaceCameraPosition_Fail);
                            break;
                        }
                        Log.Debug("X, Y Axis move ready place camera position done");
                        Step.RunStep++;
                    }
                    break;
                case ETrayHead_CamPlaceStep.Wait_FlagSpongeDetachCamInRequest:
                    if (Flag_SpongeDetachCamInREQ)
                    {
                        Log.Debug("Sponge detach cam in request");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_PlacePosition:
                    Log.Debug("X, Y Axis move place camera position");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisCamPlacePosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisCamPlacePosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlacePosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_PlacePosition_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
#if SIMULATION
                Step.RunStep++;
                break;
#endif
                            RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePlaceCameraPosition_Fail);
                            break;
                        }
                        Log.Debug("X, Y Axis move place camera position done");
                        Step.RunStep++;
                    }
                    break;
                case ETrayHead_CamPlaceStep.Wait_FlagSpongeDetachCamInRequestBeforePlace:
                    if ((In_VtCamPrealignVacOn.Value || PreAlignMaterialStatus.Status == EMaterialStatus.Existing) && _machineStatus.IsDryRunMode == false)
                    {
                        Log.Debug("PreAlign already has camera. Stop Tray Head camera place to prevent double camera stack");
                        RaiseWarning((int)EWarning.CamSpongeDetach_CameraExist);
                        break;
                    }
                    if (Flag_SpongeDetachCamInREQ && _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.IsBackward && _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.IsBackward)
                    {
                        Log.Debug("Sponge detach cam in request before place done");
                        Step.RunStep++;
                        break;
                    }

                    Wait(20);
                    break;

                case ETrayHead_CamPlaceStep.ZAxis_Move_PlacePosition:
                    Flag_TrayHeadZUpDone = false;
                    if (ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisCamPlacePosition))
                    {
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("ZAxis move place camera");
                    Flag_TrayHeadSafetyOut = false;
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisCamPlacePosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisCamPlacePosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_PlacePosition_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
#if SIMULATION
                Step.RunStep++;
                break;
#endif
                            RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MovePlaceCameraPosition_Fail);
                            break;
                        }

                        Log.Debug("Z Axis move place camera done");
                        Step.RunStep++;
                    }
                    break;
                case ETrayHead_CamPlaceStep.PreCentering_Option_Check:
                    if (usePreCentering == false)
                    {
                        Step.RunStep = (int)ETrayHead_CamPlaceStep.VacuumOff;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.CamPreCenteringOn:
                    Cyl_PreAlignOn(true);
                    Log.Debug("Cam PreCentering On");
                    Wait(10000, () => Cyl_PreAlign.IsForward);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.CamPreCenteringOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Cyl_PreAlignOn(false);
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOn_Fail);
                        break;
                    }

                    Log.Debug("Cam PreCentering On Done");
                    Wait(300);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.VacPreAlignOn:
                    Log.Debug("Prealign Vac On");
                    PreaAlignVac(true);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.CamPreCenteringOff:
                    Log.Debug("Cam PreCentering Off");
                    Cyl_PreAlignOn(false);
                    Wait(10000, () => Cyl_PreAlign.IsBackward);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.CamPreCenteringOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_CenteringOff_Fail);
                        break;
                    }
                    Log.Debug("Cam PreCentering Off Done");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamPrealignVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.VacPreAlign_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CamSpongeDetach_PreAlignVacCheck_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.VacuumOff:
                    if (In_VtCamSupplyPnPVacOn.Value == false)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Flag_TrayHeadZUpDone = false;
                    Log.Debug("Vacuum Off");
                    CamPickerVac(false);
                    Wait((_globalRecipe.VacCheckWaitTime), () => In_VtCamSupplyPnPVacOn.Value == false);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.VacuumOff_Wait:
                    if (WaitTimeOutOccurred && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_VtCamSupplyPnP_VacOff_Fail);
                        break;
                    }
                    PreAlignMaterialStatus.Set();
                    PreAlignMaterialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                    Flag_SpongeDetachCamInDone = true;
                    Log.Debug("Vacuum off done");
                    Wait(100);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_MoveBack_ReadyPlacePosition:
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Log.Debug("ZAxis move back ready place camera");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_MoveBack_ReadyPlacePosition_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
#if SIMULATION
                Step.RunStep++;
                break;
#endif
                            RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveReadyPlaceCameraPosition_Fail);
                            break;
                        }
                        Log.Debug("Z Axis move back ready place camera done");
                        Flag_SpongeDetachCamInDone = false;
                        Flag_TrayHeadZUpDone = true;
                        Step.RunStep++;
                    }
                    break;

                case ETrayHead_CamPlaceStep.XYAxis_MoveBack_ReadyPlacePosition:
                    Log.Debug("X, Y Axis move place camera position");

                    XAxis.MoveAbs(_trayHeadRecipe.XAxisCamPlacePosition);
                    YAxis.MoveAbs(_trayHeadRecipe.YAxisCamPlacePosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(_trayHeadRecipe.XAxisCamPlacePosition) && YAxis.IsOnPosition(_trayHeadRecipe.YAxisCamPlacePosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_MoveBack_ReadyPlacePosition_Check:
                    if (WaitTimeOutOccurred)
                    {
                        Log.Debug("Move Back to XY Ready Place Timeout");
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MoveReadyPlaceCameraPosition_Fail);
                        break;
                    }
                    Flag_TrayHeadSafetyOut = true;
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Set_FlagSpongeDetachCamInDone:
                    Log.Debug("Set flag SpongeDetachCamInDone");
                    isScaned = false;
                    Step.RunStep = (int)ETrayHead_CamPlaceStep.End;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_ReturnPosition:
                    double positionX = _trayHeadRecipe.XAxisCamPickPosition + ((trayIndexX - 1) * _traySuplierRecipe.TrayPitchX);
                    double positionY = _trayHeadRecipe.YAxisCamPickPosition - ((trayIndexY - 1) * _traySuplierRecipe.TrayPitchY);
                    XAxis.MoveAbs(positionX);
                    YAxis.MoveAbs(positionY);

                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000),
                        () => XAxis.IsOnPosition(positionX) && YAxis.IsOnPosition(positionY));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.XYAxis_Move_ReturnPosition_Wait:
                    if (WaitTimeOutOccurred)
                    {
#if SIMULATION
                        Step.RunStep++;
                        break;
#endif
                        RaiseWarning((int)EWarning.TrayCAMLoader_XYAxis_MovePickCameraPosition_Fail);
                        break;
                    }

                    Log.Debug("X, Y Axis return camera position done");
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Return_Position:
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisCamPickPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisCamPickPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Return_Position_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MovePickCameraPosition_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Vacuum_Return_Off:
                    CamPickerVac(false);
                    Wait(300);
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.Set_Tray_Status:
                    _trayList.TrayCamera.Cells.FirstOrDefault(c => _trayList.TrayCamera.GetColumn(c.Id) == trayIndexX && _trayList.TrayCamera.GetRow(c.Id) == trayIndexY)!.Status = ETrayCellStatus.PickFail;
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Return_ReadyPisition:
                    ZAxis.MoveAbs(_trayHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_trayHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.ZAxis_Move_Return_ReadyPisition_Wait:
                    if(WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_ZAxis_MoveWaitPosition_Fail);
                        break;
                    }
                    if ((trayIndexX == _recipeList.TraySuplierRecipe.Columns) && (trayIndexY == _recipeList.TraySuplierRecipe.Rows))
                    {
                        RaiseWarning((int)EWarning.TrayCAMLoader_ScanBarCode_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_CamPlaceStep.End:
                    if (Parent?.Sequence != ESequence.AutoRun)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }

                    if (Flag_TrayInElevatorUnloadTrayREQ)
                    {
                        Log.Debug("Sequence TrayHead Tray Pick");
                        Sequence = ESequence.TrayHead_Tray_Pick;
                        break;
                    }
                    Log.Debug("Sequence Auto");
                    Sequence = ESequence.AutoRun;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private Methods
        private void Sequence_AutoRun()
        {
            switch ((ETrayHead_AutoRunStep)Step.RunStep)
            {
                case ETrayHead_AutoRunStep.Start:
                    if(_machineStatus.IsByPassMode)
                    {
                        Wait(20);
                        break;
                    }

                    Step.RunStep++;
                    Wait(200);
                    break;
                case ETrayHead_AutoRunStep.Check_Camera_Exis:
                    //Log.Debug("Check Camera Exist.");
                    if (In_VtCamSupplyPnPVacOn.Value)
                    {
                        Log.Debug("Sequence TrayHead Cam Place.");
                        Sequence = ESequence.TrayHead_Cam_Place;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_AutoRunStep.Check_Tray_Exist:
                    if (In_TrayPickerVacOn.Value && Cyl_TrayPicker.IsBackward)
                    {
                        Flag_TrayHeadTrayPickVacOnDone = true;
                        Log.Debug("Sequence TrayHead Tray Place.");
                        Sequence = ESequence.TrayHead_Tray_Place;
                        break;
                    }
                    if (In_TrayPickerVacOn.Value && Cyl_TrayPicker.IsForward
                        && XAxis.IsOnPosition(_trayHeadRecipe.XAxisTrayPickPosition)
                        && YAxis.IsOnPosition(_trayHeadRecipe.YAxisTrayPickPosition))
                    {
                        Flag_TrayHeadTrayPickVacOnDone = true;
                        Log.Debug("Sequence TrayHead Tray Place.");
                        Sequence = ESequence.TrayHead_Tray_Pick;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ETrayHead_AutoRunStep.Check_Flags:
                    if (Flag_TrayInElevatorUnloadCamREQ && !_machineStatus.IsPickUpStop)
                    {
                        Sequence = ESequence.TrayHead_Cam_Pick;
                        break;
                    }
                    if (Flag_TrayInElevatorUnloadTrayREQ)
                    {
                        Sequence = ESequence.TrayHead_Tray_Pick;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ETrayHead_AutoRunStep.End:
                    Sequence = ESequence.AutoRun;
                    break;
                default:
                    break;
            }
        }

        private void TrayPickerVac(bool bOnOff)
        {
            Out_TrayPickerVacOn.Value = bOnOff;
            Out_TrayPickerVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(300).ContinueWith(t =>
                {
                    Out_TrayPickerVacOff.Value = false;
                });
            }
        }
        private void CamPickerVac(bool bOnOff)
        {
            Out_VtCamSupplyPnPVacOn.Value = bOnOff;
            Out_VtCamSupplyPnPVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(100).ContinueWith(t =>
                {
                    Out_VtCamSupplyPnPVacOff.Value = false;
                });
            }
#if SIMULATION
            SimulationInputSetter.SetSimInput(_devices.Inputs.VtCamSupplyPnPVacOn, bOnOff);
#endif
        }

        private void Cyl_PreAlignOn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_PreAlign.Forward();
            }
            else
            {
                Cyl_PreAlign.Backward();
            }
        }

        private void PreaAlignVac(bool bOnOff)
        {
            Out_VtCamPreAlignVacOn.Value = bOnOff;
            Out_VtCamPrealignVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(300).ContinueWith(t =>
                {
                    Out_VtCamPrealignVacOff.Value = false;
                });
            }
        }
        private void StopRun()
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            XAxis.Stop();
            YAxis.Stop();
            ZAxis.Stop();
        }
        #endregion

        #region Constructors
        public TrayHeadProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            TrayHeadRecipe trayHeadRecipe,
            TrayList trayList,
            TraySuplierRecipe traySuplierRecipe,
            MaterialStatusList materialStatusList,
            MachineStatus machineStatus,
            VisionProcess visionProcess,
            Motions motions,
            VaccumList vaccumList,
            DevRecipe devRecipe,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer,
            [FromKeyedServices("TrayHeadInput")] IDInputDevice<ETrayHeadInput> trayHeadInput,
            [FromKeyedServices("TrayHeadOutput")] IDOutputDevice<ETrayHeadOutput> trayHeadOutput) : base(globalRecipe, machineStatus, devices, blinkTimer)
        {
            {
                _devices = devices;
                _visionProcess = visionProcess;
                _globalRecipe = globalRecipe;
                _recipeList = recipeList;
                _trayHeadRecipe = trayHeadRecipe;
                _trayList = trayList;
                _traySuplierRecipe = traySuplierRecipe;
                _materialStatusList = materialStatusList;
                _machineStatus = machineStatus;
                _motions = motions;
                _vaccumList = vaccumList;
                _trayHeadInput = trayHeadInput;
                _trayHeadOutput = trayHeadOutput;
            }

            _devRecipe = devRecipe;
        }
        #endregion

        #region Privates
        private int currentCellIndex, trayIndexX = 0, trayIndexY = 0;
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly TrayHeadRecipe _trayHeadRecipe;
        private readonly TrayList _trayList;
        private readonly TraySuplierRecipe _traySuplierRecipe;
        private readonly MachineStatus _machineStatus;
        private readonly Motions _motions;
        private readonly IDInputDevice _trayHeadInput;
        private readonly IDOutputDevice _trayHeadOutput;
        private readonly DevRecipe _devRecipe;
        private bool isScaned = false;
        private MaterialStatusList _materialStatusList;
        private VaccumList _vaccumList;
        private VisionProcess _visionProcess;
        private ITray<ETrayCellStatus> CurrentJig => _trayList.TrayCamera;
        public MaterialStatus materialStatus => _materialStatusList.TrayHeadMaterialStatus;
        private MaterialStatus PreAlignMaterialStatus => _materialStatusList.PreAlignMaterialStatus;
        private bool usePreCentering => _trayHeadRecipe.UsePreCentering == 1;
        private int _countRetryVacc = 0;

        private bool IsTrayScanBarcodeSuccess { get; set; } = false;
        #endregion
    }
}
