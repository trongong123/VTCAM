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
using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace FrontCameraAssembleEquipment.Process
{
    public class CameraAssembleProcess : ProcessBase<ESequence>
    {
        #region Inputs
        private IDInput In_VtCamAssemblePnPVacOn => _devices.Inputs.VtCamAssemblePnPVacOn;
        private IDInput In_VtCamAssemblePnPOverload => _devices.Inputs.VtCamAssemblePnPOverload;
        #endregion

        #region Outputs
        private IDOutput Out_VtCamAssemblePnPVacOn => _devices.Outputs.VtCamAssemblePnPVacOn;
        private IDOutput Out_VtCamAssemblePnPPurgeOn => _devices.Outputs.VtCamAssemblePnPPurgeOn;
        #endregion

        #region Cylinders
        //private ICylinder Cyl_MoverUpDown => _devices.Cylinders.CamHead_MoverUpDn;
        #endregion

        #region Motions
        private IMotion XAxis => _devices.Motions.AssemblePickPlaceX;
        private IMotion YAxis => _devices.Motions.AssemblePickPlaceY;
        private IMotion ZAxis => _devices.Motions.AssemblePickPlaceZ;
        private IMotion RXAxis => _devices.Motions.AssemblePickPlaceRX;
        #endregion

        #region Vaccum
        private Vaccum CamHeadVac => _vaccumList.CamHead_CamPickerVac;
        #endregion

        #region Flags
        // Inputs
        private bool FlagIn_FlipperGripOffDone => _camAssembleInput[(int)ECameraAssembleHeadInput.GRIPPER_OFF_DONE];
        private bool FlagIn_FlipperCamOutRequest => _camAssembleInput[(int)ECameraAssembleHeadInput.FLIPPER_CAM_OUT_REQUEST];
        private bool FlagIn_RearCamAssembleRequest => _camAssembleInput[(int)ECameraAssembleHeadInput.REAR_CAM_ASSEMBLE_REQUEST];
        private bool FlagIn_FrontCamAssembleRequest => _camAssembleInput[(int)ECameraAssembleHeadInput.FRONT_CAM_ASSEMBLE_REQUEST];
        private bool FlagIn_VisionRunning => _camAssembleInput[(int)ECameraAssembleHeadInput.VISION_RUNNING];

        // Outputs
        private bool FlagOut_CamAssembleFrontDone { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_ASSEMBLE_FRONT_DONE] = value; }
        private bool FlagOut_CamAssembleRearDone { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_ASSEMBLE_REAR_DONE] = value; }
        private bool FlagOut_CamPickUpDone { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_PICKUP_DONE] = value; }
        private bool FlagOut_VacOnOk { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.VAC_ON_OK] = value; }
        private bool FlagOut_CamAssembleMoveAvoidToVisionFront { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_ASSEMBLE_AVOID_TO_VISION_FRONT] = value; }
        private bool FlagOut_CamAssembleMoveAvoidToVisionRear { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_ASSEMBLE_AVOID_TO_VISION_REAR] = value; }
        private bool FlagOut_CamAssembleReadyDone { set => _camAssembleOutput[(int)ECameraAssembleHeadOutput.CAM_ASSEMBLE_HEAD_READY_DONE] = value; }


        private double PlaceOffsetX
        {
            get
            {
                double value = 0;
                if (UseAlignVision)
                {
                    value = _isFrontDetachRequest ? _visionProcess.LastFrontResult.dPosX :
                                                    _visionProcess.LastRearResult.dPosX;
                }
                else
                {
                    value = 0;
                }

                return value;
            }
        }

        private double PlaceOffsetY
        {
            get
            {
                double value = 0;
                if (UseAlignVision)
                {
                    value = _isFrontDetachRequest ? _visionProcess.LastFrontResult.dPosY :
                                                    _visionProcess.LastRearResult.dPosY;
                }
                else
                {
                    value = 0;
                }

                return value;
            }
        }

        private MaterialStatus materialStatus => _materialStatusList.CamHeadMaterialStatus;
        #endregion

        private double XYMoveContiSpeed => (XAxis.Parameter.Velocity + YAxis.Parameter.Velocity) / 2;
        private double XYMoveContiAcc => (XAxis.Parameter.Acceleration + YAxis.Parameter.Acceleration) / 2;
        private double XYMoveContiDec => (XAxis.Parameter.Deceleration + YAxis.Parameter.Deceleration) / 2;
        private double XZRXMoveContiSpeed => (XAxis.Parameter.Velocity + ZAxis.Parameter.Velocity + RXAxis.Parameter.Velocity) / 3;
        private double XZRXMoveContiAcc => (XAxis.Parameter.Acceleration + ZAxis.Parameter.Acceleration + RXAxis.Parameter.Acceleration) / 3;
        private double XZRXMoveContiDec => (XAxis.Parameter.Deceleration + ZAxis.Parameter.Deceleration + RXAxis.Parameter.Deceleration) / 3;

        #region Override Methods
        public override bool PreProcess()
        {
            //materialStatus.IsEditable = (_machineStatus.IsRunningProcessMode == false ? true : false);
            if (ProcessMode == EProcessMode.Run)
            {
                if (In_VtCamAssemblePnPVacOn.Value == true) materialStatus.Set();
                else materialStatus.Status = EMaterialStatus.NotExist;

                if (YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition) ||
                YAxis.IsOnPosition(_cameraHeadRecipe.YAxisReadyPosition))
                {

                    FlagOut_CamAssembleMoveAvoidToVisionFront = true;
                }
                FlagOut_CamAssembleMoveAvoidToVisionRear = (YAxis.IsOnPosition(_cameraHeadRecipe.YAxisReadyPosition)
                                                    || YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition));

            }
            return base.PreProcess();
        }

        private bool _isXOriginSelected => MotionSelection.IsSelected(XAxis);
        private bool _isYOriginSelected => MotionSelection.IsSelected(YAxis);
        private bool _isRXOriginSelected => MotionSelection.IsSelected(RXAxis);
        public override bool ProcessOrigin()
        {
            switch ((ECamAssembleHead_OriginStep)Step.OriginStep)
            {
                case ECamAssembleHead_OriginStep.Start:
                    Log.Debug("Camera Assemble Origin Start");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.CheckOriginSelected:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.ZAXisAssembleHeadOrigin:
                    ZAxis.SearchOrigin();
                    Log.Debug($"{ZAxis.Name} Home Search Start");
                    Wait((int)(_globalRecipe.MotionOriginTimeout * 1000), () => ZAxis.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.ZAXisAssembleHeadOrigin_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_OriginFail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Home Search Done");
                    Step.OriginStep++;
                    break;
                // SPARE Cylinder - Remove Comment when USE
                case ECamAssembleHead_OriginStep.CylinderUp:
                    //Cyl_MoverUpDown.Backward();
                    //Log.Debug($"{Cyl_MoverUpDown} Move Up");
                    //Wait((int)_globalRecipe.CylinderMoveTimeout * 1000, () => Cyl_MoverUpDown.IsBackward);
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.CylinderUp_Check:
                    //if (WaitTimeOutOccurred)
                    //{}
                    //    RaiseWarning((int)EWarning.CamAssembleHead_CylinderUp_Fail);
                    //    break;
                    //}
                    //Log.Debug($"{Cyl_MoverUpDown} Move Up Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.RXAisAssembleHeadOrigin:
                    if (_isRXOriginSelected)
                    {
                        Log.Debug($"{RXAxis.Name} Home Search Start");
                        RXAxis.SearchOrigin();
                        Wait((int)(_globalRecipe.MotionOriginTimeout * 1000), () => RXAxis.Status.IsHomeDone);
                        Step.OriginStep++;
                        break;
                    }
                    Step.OriginStep = (int)ECamAssembleHead_OriginStep.XYAisAssembleHeadOrigin;
                    break;
                case ECamAssembleHead_OriginStep.RXAisAssembleHeadOrigin_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_RXAxis_OriginFail);
                        break;
                    }
                    Log.Debug($"{RXAxis.Name} Home Search Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.XYAisAssembleHeadOrigin:
                    if (_isXOriginSelected == false && _isYOriginSelected == false)
                    {
                        Step.OriginStep = (int)ECamAssembleHead_OriginStep.ZAxis_MoveToReadyPickPos;
                        break;
                    }
                    if (_isXOriginSelected) { XAxis.SearchOrigin(); }
                    if (_isYOriginSelected) { YAxis.SearchOrigin(); }
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Home Search Start");
                    Wait((int)_globalRecipe.MotionOriginTimeout * 1000, () =>
                    {
                        if (_isXOriginSelected && _isYOriginSelected)
                        {
                            return XAxis.Status.IsHomeDone && YAxis.Status.IsHomeDone;
                        }
                        else if (_isXOriginSelected)
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
                case ECamAssembleHead_OriginStep.XYAisAssembleHeadOrigin_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_OriginFail);
                        break;
                    }
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Home Search Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.ZAxis_MoveToReadyPickPos:
                    ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisReadyPosition));
                    Log.Debug($"{ZAxis.Name} Move to Ready Position");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.ZAxis_MoveToReadyPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Move To Ready Pos Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.RXAxis_MoveToReadyPickPos:
                    if (_isRXOriginSelected == false)
                    {
                        Step.OriginStep = (int)ECamAssembleHead_OriginStep.XYAxis_MoveToReadyPickPos;
                        break;
                    }
                    RXAxis.MoveAbs(_cameraHeadRecipe.RXAxisPickPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => RXAxis.IsOnPosition(_cameraHeadRecipe.RXAxisPickPosition));
                    Log.Debug($"{RXAxis.Name} Move to Ready Position");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.RXAxis_MoveToReadyPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_RXAxis_MoveToPick_Fail);
                        break;
                    }
                    Log.Debug($"{RXAxis.Name} Move To Ready Pos Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.XYAxis_MoveToReadyPickPos:
                    if (_isXOriginSelected == false || _isYOriginSelected == false)
                    {
                        Step.OriginStep = (int)ECamAssembleHead_OriginStep.Set_FlagCamAssembleHeadHomeDone;
                        break;
                    }
                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Ready Pick Pos");

                    XAxis.MoveAbs(_cameraHeadRecipe.XAxisReadyPosition);
                    YAxis.MoveAbs(_cameraHeadRecipe.YAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_cameraHeadRecipe.XAxisReadyPosition) &&
                                                                                YAxis.IsOnPosition(_cameraHeadRecipe.YAxisReadyPosition));
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.XYAxis_MoveToReadyPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Ready Pick Pos Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.Set_FlagCamAssembleHeadHomeDone:
                    Log.Debug("Set Flag Cam Assemble Head Z Axis Home Done");
                    Step.OriginStep++;
                    break;
                case ECamAssembleHead_OriginStep.End:
                    Log.Debug("Cam Assemble Head Origin End");
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
            if (Sequence != ESequence.Stop && ProcessStatus != EProcessStatus.OriginDone && Sequence != ESequence.None)
            {
                RunStop();
            }
            ProcessStatus = EProcessStatus.ToStopDone;
            return base.ProcessToStop();
        }
        public override bool ProcessToAlarm()
        {
            if (ProcessStatus == EProcessStatus.ToAlarmDone)
            {
                Thread.Sleep(50);
                return true;
            }
            RunStop();
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
            RunStop();
            ProcessStatus = EProcessStatus.ToWarningDone;
            return base.ProcessToWarning();
        }
        public override bool ProcessToRun()
        {
            switch ((ECamAssembleHead_ToRunStep)Step.ToRunStep)
            {
                case ECamAssembleHead_ToRunStep.Start:
                    Log.Debug("Cam Assemble Head To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        FlagOut_CamAssembleReadyDone = false;

                        Step.ToRunStep = (int)ECamAssembleHead_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ECamAssembleHead_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<ECameraAssembleHeadOutput>)_camAssembleOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case ECamAssembleHead_ToRunStep.MaterialDataMatching_VacOn:
                    //AssemblePnPVacOn(true);
                    //Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamAssemblePnPVacOn.Value);
                    Step.ToRunStep++;
                    break;
                case ECamAssembleHead_ToRunStep.MaterialDataMatching_Check:
                    //Log.Debug("Material Data Matching Check");
                    //bool bCamExist = In_VtCamAssemblePnPVacOn.Value;
                    //bool bMaterialStatus = (materialStatus.Status == EMaterialStatus.Existing ? true : false);

                    //if (bCamExist == false)
                    //{
                    //    AssemblePnPVacOn(false);
                    //}

                    //if (bCamExist != bMaterialStatus)
                    //{
                    //    RaiseWarning((int)EWarning.MaterialDataNotMatching);
                    //    break;
                    //}
                    Step.ToRunStep++;
                    break;
                case ECamAssembleHead_ToRunStep.End:
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
                case ESequence.CamHead_Pick:
                    Sequence_CamHeadPick();
                    break;
                case ESequence.CamHead_Place:
                    Sequence_CamHeadPlace();
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
            return EProcess.CameraAssemble.GetDescription();
        }
        #endregion

        #region Private Methods
        private void Sequence_Ready()
        {
            switch ((ECamAssembleHead_InitStep)Step.RunStep)
            {
                case ECamAssembleHead_InitStep.Start:
                    {
                        if (IsOriginOrInitSelected == false)
                        {
                            Sequence = ESequence.Stop;
                            break;
                        }
                        Log.Debug("Start Init");
                        Step.RunStep++;
                        break;
                    }
                case ECamAssembleHead_InitStep.CamHeadStateCheck:
                    if (_isAssembling == false)
                    {
                        Step.RunStep = (int)ECamAssembleHead_InitStep.ZAxisUp;
                        break;
                    }
                    Log.Debug("Cam Head Is Assembling Check");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_InitStep.MoveBackTo1stPos:
                    double positionX_1st = _isFrontDetachRequest ? _cameraHeadRecipe.XAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.XAxis1stPlaceRearPosition;
                    double positionZ_1st = _isFrontDetachRequest ? _cameraHeadRecipe.ZAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.ZAxis1stPlaceRearPosition;
                    double positionRX_1st = _isFrontDetachRequest ? _cameraHeadRecipe.RXAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.RXAxis1stPlaceRearPosition;

                    double[] firstPosArr = { positionX_1st, positionZ_1st, positionRX_1st };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, firstPosArr, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Log.Debug($"Move to first Place Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_InitStep.MoveBackTo1stPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToFirstPlace_Fail);
                        break;
                    }
                    _isAssembling = false;
                    _isPushingIn = false;
                    Log.Debug($"Move to first Place Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_InitStep.ZAxisUp:
                    {
                        Log.Debug("Z Up to Ready Pos");
                        ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisReadyPosition);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisReadyPosition));
                        Step.RunStep++;
                        break;
                    }
                case ECamAssembleHead_InitStep.ZAxisUp_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
                            Log.Debug("Z Move to Ready Pos NG");
                            RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToReadyPick_Fail);
                            break;

                        }
                        Log.Debug("Z Move to Ready Pos Done");
                        Step.RunStep++;
                        break;
                    }
                case ECamAssembleHead_InitStep.MoveXY_Ready_Pos:
                    {
                        Log.Debug("Move XY to Ready Pos");
                        XAxis.MoveAbs(_cameraHeadRecipe.XAxisReadyPosition);
                        YAxis.MoveAbs(_cameraHeadRecipe.YAxisReadyPosition);
                        Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_cameraHeadRecipe.XAxisReadyPosition) && YAxis.IsOnPosition(_cameraHeadRecipe.YAxisReadyPosition));
                        Step.RunStep++;
                        break;
                    }
                case ECamAssembleHead_InitStep.MoveXY_Ready_Pos_Check:
                    {
                        if (WaitTimeOutOccurred)
                        {
                            Log.Debug("Move to XY Ready Pos Fail");
                            RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToReadyPlace_Fail);
                            break;
                        }
                        Log.Debug("Move XY to Ready Pos Done");
                        Step.RunStep++;
                        break;

                    }
                case ECamAssembleHead_InitStep.End:
                    {
                        ((MappableOutputDevice<ECameraAssembleHeadOutput>)_camAssembleOutput).ClearOutputs();

                        FlagOut_CamAssembleReadyDone = true;

                        Log.Debug("Ready End");
                        Sequence = ESequence.Stop;
                        break;
                    }

            }

        }
        private void Sequence_AutoRun()
        {
            if (_machineStatus.IsByPassMode)
            {
                return;
            }

            if (In_VtCamAssemblePnPVacOn.Value == true || _isPushingIn == true || _isAssembling == true)
            {
                Log.Debug("Sequence Cam Place");

                Sequence = ESequence.CamHead_Place;
            }
            else
            {
                Log.Debug("Sequence Cam Pick");

                Sequence = ESequence.CamHead_Pick;
            }
        }

        private void Sequence_CamHeadPick()
        {
            switch ((ECamAssembleHead_PickStep)Step.RunStep)
            {
                case ECamAssembleHead_PickStep.Start:
                    _pickRetryCount = 0;
                    _isAssembling = false;
                    Log.Debug("Cam Assemble Pick Start");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAxis_MoveToReadyPickPos:
                    ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisReadyPosition);
                    Log.Debug($"{ZAxis.Name} Move to Ready Pick Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAxis_MoveToReadyPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Move to Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.RXAxis_MoveToPickPos:
                    RXAxis.MoveAbs(_cameraHeadRecipe.RXAxisPickPosition);
                    Log.Debug($"{RXAxis.Name} Move to Pick Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => RXAxis.IsOnPosition(_cameraHeadRecipe.RXAxisPickPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.RXAxis_MoveToPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_RXAxis_MoveToPick_Fail);
                        break;
                    }
                    Log.Debug($"{RXAxis.Name} Move to Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.CamHeadPositionCheck:
                    if ((XAxis.IsOnPosition(_cameraHeadRecipe.XAxisPickPosition) && YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition)))
                    {
                        Step.RunStep = (int)ECamAssembleHead_PickStep.Flipper_CamUnloadRequest_Wait;
                        break;
                    }

                    Step.RunStep++;
                    break;
                //case ECamAssembleHead_PickStep.XYAxis_MoveToReadyPickPos:
                //    double[] positionArr = { _cameraHeadRecipe.XAxisReadyPosition, _cameraHeadRecipe.YAxisReadyPosition };
                //    _motions.MoveContiMotion(ECoordinate.Assemble_XY, positionArr, _cameraHeadRecipe.XYPickSpeed);
                //    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Ready Pick Pos");
                //    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XY) == false);
                //    Step.RunStep++;
                //    break;
                //case ECamAssembleHead_PickStep.XYAxis_MoveToReadyPickPos_Check:
                //    if (WaitTimeOutOccurred)
                //    {
                //        RaiseAlarm((int)EAlarm.CAMAssemble_XYAxis_MoveToReadyPick_Fail);
                //        break;
                //    }
                //    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Ready Pick Pos Done");
                //    FlagOut_CamAssembleMoveAvoidToVisionRear = true;
                //    Step.RunStep++;
                //    break;
                case ECamAssembleHead_PickStep.XYAixs_MoveToPickPos:
                    double[] positionPickArr = { _cameraHeadRecipe.XAxisPickPosition, _cameraHeadRecipe.YAxisPickPosition };
                    //_motions.MoveContiMotion(ECoordinate.Assemble_XY, positionPickArr, XYMoveContiSpeed, XYMoveContiAcc, XYMoveContiDec);
                    XAxis.MoveAbs(_cameraHeadRecipe.XAxisPickPosition);
                    YAxis.MoveAbs(_cameraHeadRecipe.YAxisPickPosition);
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Pick Pos");
                    //Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XY) == false);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_cameraHeadRecipe.XAxisPickPosition) && YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition));

                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.XYAixs_MoveToPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToPick_Fail);
                        break;
                    }
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.Flipper_CamUnloadRequest_Wait:
                    if (!FlagIn_FlipperCamOutRequest)
                    {
                        break;
                    }
                    materialStatus.ProcessStatus = EMaterialProcessStatus.None;
                    Log.Debug("Flag Cam Out Request Received");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAixs_MoveToPickPos:
                    ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisPickPosition);
                    Log.Debug($"{ZAxis.Name} Move to Pick Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisPickPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAixs_MoveToPickPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToPick_Fail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Move to Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.CamPickVacOn:
                    AssemblePnPVacOn(true);
                    Log.Debug("Cam Head Vac On");
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamAssemblePnPVacOn.Value || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.CamPickVacOn_Check:
                    if (WaitTimeOutOccurred && _pickRetryCount < 2)
                    {
                        Step.RunStep = (int)ECamAssembleHead_PickStep.ZAixs_MoveToPickPosRetry;
                        break;
                    }
                    else if (WaitTimeOutOccurred)
                    {
                        _pickRetryCount = 0;
                        Step.RunStep = (int)ECamAssembleHead_PickStep.ZAxis_MoveToReadyPickPosAgain;
                        break;
                    }
                    Log.Debug("Cam Head Vac On Done");
                    Step.RunStep = (int)ECamAssembleHead_PickStep.CamHead_VacOnDone_Set;
                    break;
                case ECamAssembleHead_PickStep.ZAixs_MoveToPickPosRetry:
                    _pickRetryCount++;
                    double positionZ = _cameraHeadRecipe.ZAxisPickPosition + (_pickRetryCount * 0.4);
                    ZAxis.MoveAbs(positionZ);
                    Log.Debug($"{ZAxis.Name} Move to Pick Pos Retry");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(positionZ));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAixs_MoveToPickPosRetry_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToPick_Fail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Move to Pick Pos Retry Done");
                    Step.RunStep = (int)ECamAssembleHead_PickStep.CamPickVacOn;
                    break;
                case ECamAssembleHead_PickStep.CamHead_VacOnDone_Set:
                    if (In_VtCamAssemblePnPVacOn.Value || _machineStatus.IsDryRunMode)
                    {
                        materialStatus.ProcessStatus = EMaterialProcessStatus.Processing;
                        FlagOut_VacOnOk = true;
                        Step.RunStep++;
                        break;
                    }

                    Wait(20);
                    break;
                case ECamAssembleHead_PickStep.Flipper_GripOffDone_Wait:
                    if (FlagIn_FlipperGripOffDone)
                    {
                        FlagOut_VacOnOk = false;
                        Log.Debug("Flag Flipper Grip Off Done Received");
                        Step.RunStep++;
                        break;
                    }
                    Wait(20);
                    break;
                case ECamAssembleHead_PickStep.ZAxis_MoveToReadyPickPosAgain:
                    ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisReadyPosition);
                    Log.Debug($"{ZAxis.Name} Move to Ready Pick Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.ZAxis_MoveToReadyPickPosAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"{ZAxis.Name} Move to Ready Pick Pos Done");
                    if (In_VtCamAssemblePnPVacOn.Value == false && _machineStatus.IsDryRunMode == false)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_PickUpVacOn_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.Set_Flag_CameraOutDone:
                    FlagOut_CamPickUpDone = true;
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.Wait_FlagOut_GripOffDone_Clear:
                    if (FlagIn_FlipperGripOffDone)
                    {
                        Wait(20);
                        break;
                    }

                    FlagOut_CamPickUpDone = false;
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.AssembleRequestCheck:
                    if (FlagIn_FrontCamAssembleRequest || FlagIn_RearCamAssembleRequest)
                    {
                        Step.RunStep = (int)ECamAssembleHead_PickStep.CamHead_PickOutDone_Set;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.XYAxis_MoveToReadyPickPosAgain:
                    XAxis.MoveAbs(_cameraHeadRecipe.XAxisPickPosition);
                    YAxis.MoveAbs(_cameraHeadRecipe.YAxisPickPosition);
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Ready Pick Again Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_cameraHeadRecipe.XAxisPickPosition) && YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition));

                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.XYAxis_MoveToReadyPickPosAgain_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Ready Pick Pos Again Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.CamHead_PickOutDone_Set:
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PickStep.End:
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
        private void Sequence_CamHeadPlace()
        {
            switch ((ECamAssembleHead_PlaceStep)Step.RunStep)
            {
                case ECamAssembleHead_PlaceStep.Start:
                    Log.Debug("Cam Assemble Place Start");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.CamAssembleStateCheck:
                    if (_isAssembling)
                    {
                        Step.RunStep = (int)ECamAssembleHead_PlaceStep.MovetoSecondPlacePos;
                        break;
                    }

                    if (_isPushingIn)
                    {
                        Step.RunStep = (int)ECamAssembleHead_PlaceStep.YAxis_MoveToReadyPlacePosToPush;
                        break;
                    }

                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.ZAXis_MoveToReadyPos:
                    ZAxis.MoveAbs(_cameraHeadRecipe.ZAxisReadyPosition);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(_cameraHeadRecipe.ZAxisReadyPosition));
                    Log.Debug("Move Z Axis To Ready Pos");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.ZAXis_MoveToReadyPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug("Move Z Axis To Ready Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.CamExistVacCheck:
                    if (In_VtCamAssemblePnPVacOn.Value == true || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Cam Exist Check OK");
                        Step.RunStep++;
                        break;
                    }

                    Sequence = ESequence.CamHead_Pick;
                    break;
                case ECamAssembleHead_PlaceStep.CVAssemble_CamAssembleRequest_Wait:
                    if ((FlagIn_FrontCamAssembleRequest || FlagIn_RearCamAssembleRequest) && In_VtCamAssemblePnPVacOn.Value == true || _machineStatus.IsDryRunMode)
                    {
                        Log.Debug("Cam Assemble Signal Detect");
                        Step.RunStep++;
                        break;
                    }

                    if (In_VtCamAssemblePnPVacOn.Value == false && !_machineStatus.IsDryRunMode)
                    {
                        Sequence = ESequence.CamHead_Pick;
                        break;
                    }

                    Wait(10);
                    break;
                case ECamAssembleHead_PlaceStep.CamAssembleRequest_Check:
                    if (FlagIn_FrontCamAssembleRequest && FlagIn_RearCamAssembleRequest) //Both request signal
                    {
                        if (_lastRequest == ECVLine.Front)
                        {
                            _isFrontDetachRequest = false;
                            _lastRequest = ECVLine.Rear;
                        }
                        else
                        {
                            _isFrontDetachRequest = true;
                            _lastRequest = ECVLine.Front;
                        }
                    }
                    else
                    {
                        _isFrontDetachRequest = FlagIn_FrontCamAssembleRequest;
                    }
                    Log.Debug("Cam Assemble Signal Detect Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XYAxis_MoveToReadyPlacePos:
                    FlagOut_CamAssembleMoveAvoidToVisionRear = false;
                    if (FlagIn_VisionRunning == true)
                    {
                        break;
                    }
                    string line = _isFrontDetachRequest ? "Front" : "Rear";
                    Log.Debug($"Move to {line} with Offset X: {PlaceOffsetX}");
                    Log.Debug($"Move to {line} with Offset Y: {PlaceOffsetY}");

                    double positionX = _isFrontDetachRequest ? _cameraHeadRecipe.XAxisReadyPlaceFrontPosition
                                                               : _cameraHeadRecipe.XAxisReadyPlaceRearPosition;
                    double positionY = (_isFrontDetachRequest ? _cameraHeadRecipe.YAxisPlaceFrontPosition
                                                              : _cameraHeadRecipe.YAxisPlaceRearPosition) - PlaceOffsetY;
                    //double[] positonArr = { positionX, positionY };
                    //_motions.MoveContiMotion(ECoordinate.Assemble_XY, positonArr, XYMoveContiSpeed, XYMoveContiAcc,XYMoveContiDec);

                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Ready Place Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XY) == false);
                    XAxis.MoveAbs(positionX);
                    YAxis.MoveAbs(positionY);
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Ready Pick Again Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(positionX) && YAxis.IsOnPosition(positionY));

                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XYAxis_MoveToReadyPlacePos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToReadyPlace_Fail);
                        break;
                    }
                    FlagOut_CamAssembleMoveAvoidToVisionFront = false;
                    Log.Debug($"{XAxis.Name}, {YAxis.Name} Move to Ready Place Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MovetoFirstPlacePos:
                    double positionX_1st = _isFrontDetachRequest ? _cameraHeadRecipe.XAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.XAxis1stPlaceRearPosition + PlaceOffsetX;
                    double positionZ_1st = _isFrontDetachRequest ? _cameraHeadRecipe.ZAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.ZAxis1stPlaceRearPosition;
                    double positionRX_1st = _isFrontDetachRequest ? _cameraHeadRecipe.RXAxis1stPlaceFrontPosition
                                            : _cameraHeadRecipe.RXAxis1stPlaceRearPosition;

                    double[] firstPosArr = { positionX_1st, positionZ_1st, positionRX_1st };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, firstPosArr, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Log.Debug($"Move to first Place Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MovetoFirstPlacePos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToFirstPlace_Fail);
                        break;
                    }
                    Log.Debug($"Move to first Place Pos Done");
                    _isAssembling = true;
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MovetoSecondPlacePos:
                    double positionX_2nd = (_isFrontDetachRequest ? _cameraHeadRecipe.XAxis2ndPlaceFrontPosition
                                                                : _cameraHeadRecipe.XAxis2ndPlaceRearPosition) + PlaceOffsetX;
                    double positionZ_2nd = _isFrontDetachRequest ? _cameraHeadRecipe.ZAxis2ndPlaceFrontPosition
                                                                : _cameraHeadRecipe.ZAxis2ndPlaceRearPosition;
                    double positionRX_2nd = _isFrontDetachRequest ? _cameraHeadRecipe.RXAxis2ndPlaceFrontPosition
                                                                : _cameraHeadRecipe.RXAxis2ndPlaceRearPosition;
                    string line_1 = _isFrontDetachRequest ? "Front" : "Rear";
                    Log.Debug($"Move to {line_1} with Offset X: {PlaceOffsetX}");
                    double[] secondPosArr = { positionX_2nd, positionZ_2nd, positionRX_2nd };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, secondPosArr, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Log.Debug($"Move to second Place Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MovetoSecondPlacePos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToSecondPlace_Fail);
                        break;
                    }
                    Log.Debug($"Move to second Place Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.RAxis_Back:
                    Step.RunStep++;
                    break;
                    double actualRAxisPos = RXAxis.Status.ActualPosition;
                    double actualZAxisPos = ZAxis.Status.ActualPosition;
                    double actualXAxisPos = XAxis.Status.ActualPosition;
                    XAxis.MoveAbs(actualXAxisPos + 1);
                    RXAxis.MoveAbs(actualRAxisPos + 1);
                    ZAxis.MoveAbs(actualZAxisPos + 1);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => RXAxis.IsOnPosition(actualRAxisPos + 1) && ZAxis.IsOnPosition(actualZAxisPos + 1) && XAxis.IsOnPosition(actualXAxisPos + 1));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.RAxis_Back_Wait:
                    Step.RunStep++;
                    break;
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToSecondPlace_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.CamPickVacOff:
                    materialStatus.ProcessStatus = EMaterialProcessStatus.Done;
                    AssemblePnPVacOn(false);
                    _isAssembling = false;
                    Log.Debug("Cam Head Vac Off");

                    Log.Debug("Set Flag Out Cam Assemble Done");
                    if (_isFrontDetachRequest) FlagOut_CamAssembleFrontDone = true;
                    else FlagOut_CamAssembleRearDone = true;
                    _isPushingIn = true;
                    Wait(_globalRecipe.VacCheckWaitTime, () => In_VtCamAssemblePnPVacOn.Value == false || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;

                case ECamAssembleHead_PlaceStep.CamPickVacOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_PickUpVacOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.CamHead_CamPlaceDone_Set:
                    Log.Debug("Place Done");
                    Wait(200);
                    Step.RunStep = (int)ECamAssembleHead_PlaceStep.MoveToPrePushInUpPos;
                    break;
                // Push in cam to set
                case ECamAssembleHead_PlaceStep.YAxis_MoveToReadyPlacePosToPush:
                    double positionReadyY = _isFrontDetachRequest ? _cameraHeadRecipe.YAxisPlaceFrontPosition
                                                              : _cameraHeadRecipe.YAxisPlaceRearPosition;
                    YAxis.MoveAbs(positionReadyY);
                    if (_isFrontDetachRequest) FlagOut_CamAssembleFrontDone = true;
                    else FlagOut_CamAssembleRearDone = true;
                    Log.Debug($"{YAxis.Name} Move to Ready Place Pos To Push");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxis.IsOnPosition(positionReadyY));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.YAxis_MoveToReadyPlacePosToPush_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_YAxis_MoveToReadyPlace_Fail);
                        break;
                    }

                    Log.Debug($"{YAxis.Name} Move to Ready Place Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPrePushInUpPos:
                    double positionX_preuppush = _isFrontDetachRequest ? _cameraHeadRecipe.XAxisPrePushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.XAxisPrePushInPlaceRearPosition;
                    double positionZ_preuppush = (_isFrontDetachRequest ? _cameraHeadRecipe.ZAxisPushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.ZAxisPushInPlaceRearPosition) - _cameraHeadRecipe.PrePushOffsetZ;
                    double positionRX_preuppush = _isFrontDetachRequest ? _cameraHeadRecipe.RXAxisPushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.RXAxisPushInPlaceRearPosition;
                    double[] prePushUpPosArr = { positionX_preuppush, positionZ_preuppush, positionRX_preuppush };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, prePushUpPosArr, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Log.Debug("Move To Pre Push In Up Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPrePushInUpPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToPreUpPushInPosPlace_Fail);
                        break;
                    }
                    Log.Debug("Move To Pre Push In Up Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPrePushInPos:
                    double positionZ_prepush = (_isFrontDetachRequest ? _cameraHeadRecipe.ZAxisPushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.ZAxisPushInPlaceRearPosition);
                    //double speedZ_place = _cameraHeadRecipe.ZAxisPlaceSpeed;
                    ZAxis.MoveAbs(positionZ_prepush);
                    Log.Debug("Move To Pre Push In Down Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => ZAxis.IsOnPosition(positionZ_prepush));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPrePushInPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_ZAxis_MoveToPrePushInPosPlace_Fail);
                        break;
                    }
                    Log.Debug("Move To Pre Push In Down Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPushInPos:
                    double positionX_push = (_isFrontDetachRequest ? _cameraHeadRecipe.XAxisPrePushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.XAxisPrePushInPlaceRearPosition) - _cameraHeadRecipe.PushOffsetX;
                    //double speedX_push = _cameraHeadRecipe.XAxisPlaceSpeed;
                    XAxis.MoveAbs(positionX_push);
                    Log.Debug("Move To Push In Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveToPushInPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XAxis_MoveToPushInPosPlace_Fail);
                        break;
                    }
                    Log.Debug("Move To Push In Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveBackToPrePushInUpPos:
                    double positionX_preuppush2 = (_isFrontDetachRequest ? _cameraHeadRecipe.XAxisPrePushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.XAxisPrePushInPlaceRearPosition);
                    double positionZ_preuppush2 = (_isFrontDetachRequest ? _cameraHeadRecipe.ZAxisPushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.ZAxisPushInPlaceRearPosition) - _cameraHeadRecipe.PrePushOffsetZ;
                    double positionRX_preuppush2 = _isFrontDetachRequest ? _cameraHeadRecipe.RXAxisPushInPlaceFrontPosition
                                                                    : _cameraHeadRecipe.RXAxisPushInPlaceRearPosition;
                    double[] prePushUpPosArr2 = { positionX_preuppush2, positionZ_preuppush2, positionRX_preuppush2 };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, prePushUpPosArr2, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Log.Debug("Move Back To Pre Push In Up Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.MoveBackToPrePushInUpPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToPreUpPushInPosPlace_Fail);
                        break;
                    }
                    _isPushingIn = false;
                    Log.Debug("Move Back To Pre Push In Up Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.CamHead_CamAssembleDone_Set:

                    //EDM
                    strEDMPara[0] = "0,";
                    strEDMPara[1] = ",";
                    strEDMPara[2] = ",";
                    strEDMPara[3] = ",";
                    _edmLogger.AddEDMLog("9002", "00000002", strEDMPara);
                    //
                    Step.RunStep++;
                    break;

                case ECamAssembleHead_PlaceStep.XZRXAxis_MoveToReadyPick:
                    double positionReadyPickX = _cameraHeadRecipe.XAxisReadyPosition;
                    double positionReadyPickZ = _cameraHeadRecipe.ZAxisReadyPosition;
                    double positionReadyPickRX = _cameraHeadRecipe.RXAxisPickPosition;

                    double[] positionReadyPick = { positionReadyPickX, positionReadyPickZ, positionReadyPickRX };
                    _motions.MoveContiMotion(ECoordinate.Assemble_XZRX, positionReadyPick, XZRXMoveContiSpeed, XZRXMoveContiAcc, XZRXMoveContiDec);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => _motions.IsContiMotioning(ECoordinate.Assemble_XZRX) == false);
                    Log.Debug($"X Z RX Move to Ready Pick Pos");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XZRXAxis_MoveToReadyPick_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XZRXAxis_MoveToReadyPick_Fail);
                        break;
                    }
                    Log.Debug($"X Z RX Move to Ready Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XYAxis_MoveOutVisionFrontWork:
                    XAxis.MoveAbs(_cameraHeadRecipe.XAxisPickPosition);
                    YAxis.MoveAbs(_cameraHeadRecipe.YAxisPickPosition);

                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Avoid Vision Pos");
                    Wait(3000, () => YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XYAxis_MoveOutVisionFrontWork_Check:
                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Avoid Vision Pos Done");

                    if (_isFrontDetachRequest)
                    {
                        FlagOut_CamAssembleMoveAvoidToVisionFront = true;
                    }

                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => XAxis.IsOnPosition(_cameraHeadRecipe.XAxisPickPosition) &&
                                                                              YAxis.IsOnPosition(_cameraHeadRecipe.YAxisPickPosition));
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.XYAxis_MoveOutVisionFrontWorkDone_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.CAMAssemble_XYAxis_MoveToReadyPick_Fail);
                        break;
                    }

                    if (_isFrontDetachRequest == false)
                    {
                        FlagOut_CamAssembleMoveAvoidToVisionRear = true;
                    }

                    Log.Debug($"{XAxis.Name} and {YAxis.Name} Move to Pick Pos Done");
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.Wait_CVAssembleRequest_Clear:
                    if (_isFrontDetachRequest)
                    {
                        if (FlagIn_FrontCamAssembleRequest)
                        {
                            Wait(20);
                            break;
                        }

                        Log.Debug("Clear FlagOut_CamAssembleFrontDone");
                        FlagOut_CamAssembleFrontDone = false;
                        Step.RunStep++;
                        break;
                    }

                    if (FlagIn_RearCamAssembleRequest)
                    {
                        Wait(20);
                        break;
                    }
                    materialStatus.ProcessStatus = EMaterialProcessStatus.None;
                    Log.Debug("Clear FlagOut_CamAssembleRearDone");
                    FlagOut_CamAssembleRearDone = false;
                    Step.RunStep++;
                    break;
                case ECamAssembleHead_PlaceStep.End:
                    FlagOut_CamAssembleMoveAvoidToVisionFront = false;
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

        private void RunStop()
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            XAxis.Stop();
            YAxis.Stop();
            ZAxis.Stop();
            RXAxis.Stop();
        }

        private void AssemblePnPVacOn(bool bOnOff)
        {
            Out_VtCamAssemblePnPVacOn.Value = bOnOff;
            Out_VtCamAssemblePnPPurgeOn.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(400).ContinueWith(t =>
                {
                    Out_VtCamAssemblePnPPurgeOn.Value = false;
                });
            }
        }
        #endregion

        #region Constructors
        public CameraAssembleProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            EDMLogger edmLogger,
            Motions motions,
            MachineStatus machineStatus,
            MaterialStatusList materialStatusList,
            VaccumList vaccumList,
            VisionProcess visionProcess,
            [FromKeyedServices("CameraAssembleHeadInput")] IDInputDevice<ECameraAssembleHeadInput> cameraAssembleHeadInput,
            [FromKeyedServices("CameraAssembleHeadOutput")] IDOutputDevice<ECameraAssembleHeadOutput> cameraAssembleHeadOutput)
        {
            _edmLogger = edmLogger;
            _motions = motions;
            _machineStatus = machineStatus;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _materialStatusList = materialStatusList;
            _vaccumList = vaccumList;
            _visionProcess = visionProcess;
            _camAssembleInput = cameraAssembleHeadInput;
            _camAssembleOutput = cameraAssembleHeadOutput;
        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly IDInputDevice _camAssembleInput;
        private readonly IDOutputDevice _camAssembleOutput;
        private readonly Motions _motions;
        private readonly MachineStatus _machineStatus;
        private readonly VaccumList _vaccumList;
        private readonly VisionProcess _visionProcess;

        private ECVLine _lastRequest = ECVLine.Front;
        private bool _isFrontDetachRequest;
        private bool _isAssembling { get; set; }
        private bool _isPushingIn;
        string[] strEDMPara = new string[4];
        private EDMLogger _edmLogger;
        private int _pickRetryCount = 0;
        private CameraHeadRecipe _cameraHeadRecipe => _recipeList.CameraHeadRecipe;
        private MaterialStatusList _materialStatusList;

        private bool UseAlignVision => _cameraHeadRecipe.UseAlignVision == 1;
        #endregion
    }
}
