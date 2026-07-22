using EQX.Core.Common;
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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace FrontCameraAssembleEquipment.Process
{
    public class FilmDetachProcess : ProcessBase<ESequence>
    {
        #region Inputs
        private IDInput In_SetReverseDetect => _devices.Inputs.SetReverseDetect;
        private IDInput In_VinylDetect => _devices.Inputs.VinylDetect;
        #endregion

        #region Outputs
        private IDOutput Out_TrashSuctionOn => _devices.Outputs.TrashSuctionOn;
        private IDOutput Out_FilmDetachIonizerOn => _devices.Outputs.FilmDetachIonizerOn;
        private IDOutput Out_FilmDetachSuctionVacOn => _devices.Outputs.FilmDetachSuctionVacOn;
        private IDOutput Out_FilmDetachSuctionVacOff => _devices.Outputs.FilmDetachSuctionVacOff;
        #endregion

        #region Vaccum
        private Vaccum FilmSuctionVac => _vaccumList.VinylDetach_VinylSuctionVac;
        #endregion

        #region Cylinders
        private ICylinder Cyl_FilmDetachMoverUpDn => _devices.Cylinders.FilmDetach_MoverUpDn;
        private ICylinder Cyl_FilmDetachGripper => _devices.Cylinders.FilmDetach_GripperOnOff;
        #endregion

        #region Motions
        private IMotion YAxisFilmDetach => _devices.Motions.FilmDetachY;
        #endregion

        #region Flags
        // Inputs
        private bool FlagIn_FrontCvDetachRequest => _filmDetachInput[(int)EFilmDetachInput.FRONT_FILM_DETACH_REQUEST];
        private bool FlagIn_RearCvDetachRequest => _filmDetachInput[(int)EFilmDetachInput.REAR_FILM_DETACH_REQUEST];
        private bool FlagIn_FrontCvDetachStartWorkRequest => _filmDetachInput[(int)EFilmDetachInput.FRONT_FILM_DETACH_START_WORK_REQUEST];
        private bool FlagIn_RearCvDetachStartWorkRequest => _filmDetachInput[(int)EFilmDetachInput.REAR_FILM_DETACH_START_WORK_REQUEST];

        //Outputs
        private bool FlagOut_FrontFilmDetachDone { set => _filmDetachOutput[(int)EFilmDetachOutput.FRONT_FILM_DETACH_DONE] = value; }
        private bool FlagOut_RearFilmDetachDone { set => _filmDetachOutput[(int)EFilmDetachOutput.REAR_FILM_DETACH_DONE] = value; }
        #endregion

        #region Override Methods
        public override bool ProcessOrigin()
        {
            switch ((EFilmDetach_OriginStep)Step.OriginStep)
            {
                case EFilmDetach_OriginStep.Start:
                    Log.Debug("Film Detach Origin Start");
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.CheckOriginSelected:
                    if (IsOriginOrInitSelected == false)
                    {
                        Sequence = ESequence.Stop;
                        break;
                    }
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.FilmGripperOff:
                    Cyl_FilmDetachGrip(false);
                    Log.Debug($"{Cyl_FilmDetachGripper} Gripper Off");
                    Wait(10000, () => Cyl_FilmDetachGripper.IsBackward);
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.FilmGripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_GripOff_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachGripper} Gripper Off done");
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.FilmDetachUp:
                    Cyl_FilmDetachUpDn(false);
                    //Cyl_SuctionUpDn(false);
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move up");
                    Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsBackward);
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.FilmDetachUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move up done");
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.YAxisFilmOrigin:
                    YAxisFilmDetach.SearchOrigin();
                    Log.Debug("Y Axis Search Origin");
                    Wait((int)(_globalRecipe.MotionOriginTimeout * 1000), () => YAxisFilmDetach.Status.IsHomeDone);
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.YAxisFilmOrigin_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_OriginFail);
                        break;
                    }
                    Log.Debug("Y Axis Origin Done");
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.YAxisFilmHeadMoveToReadyPos:
                    double yAxisReadyPos = _filmDetachRecipe.YAxisReadyPosition;
                    //double yAxisReadySpeed = _filmDetachRecipe.YAxisReadySpeed;
                    YAxisFilmDetach.MoveAbs(yAxisReadyPos);
                    Log.Debug("Y Axis Move to Ready Pos");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(yAxisReadyPos));
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.YAxisFilmHeadMoveToReadyPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveReady_Fail);
                        break;
                    }
                    Log.Debug("Y Axis Move to Ready Pos Done");
                    Step.OriginStep++;
                    break;
                case EFilmDetach_OriginStep.End:
                    Log.Debug("Film Detach Origin End");
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
                StopRun();
            }
            return base.ProcessToStop();
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
            switch ((EFilmDetach_ToRunStep)Step.ToRunStep)
            {
                case EFilmDetach_ToRunStep.Start:
                    Log.Debug("To Run start.");
                    if (Sequence == ESequence.Ready)
                    {
                        Step.ToRunStep = (int)EFilmDetach_ToRunStep.End;
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case EFilmDetach_ToRunStep.InternalInOutSignal_Reset:
                    ((MappableOutputDevice<EFilmDetachOutput>)_filmDetachOutput).ClearOutputs();
                    Log.Debug("Internal Output Signal Reset");
                    Step.ToRunStep++;
                    break;
                case EFilmDetach_ToRunStep.End:
                    _countRetryFilmDetachShift = 0;
                    _countRetryFilmDetach = 0;
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
                case ESequence.Ready:
                    Sequence_Ready();
                    break;
                case ESequence.Detach_FilmDetach:
                    Sequence_FilmDetach();
                    break;
                default:
                    Sequence = ESequence.Stop;
                    break;
            }
            return true;
        }
        public override string ToString()
        {
            return EProcess.FilmDetach.GetDescription();
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
            ((MappableOutputDevice<EFilmDetachOutput>)_filmDetachOutput).ClearOutputs();
            Log.Debug("Ready End");
            Sequence = ESequence.Stop;
        }
        private void Sequence_AutoRun()
        {
            if (_machineStatus.IsByPassMode)
            {
                return;
            }

            Log.Debug("Sequence Film Detach");
            Sequence = ESequence.Detach_FilmDetach;

            // TODO : SET Step to drop
            if (YAxisFilmDetach.IsOnPosition(_filmDetachRecipe.YAxisReadyPosition))
            {
                Step.RunStep = (int)EFilmDetach_DetachStep.CylinderHeadDown;
            }
        }

        private bool firstStartAfterAutoRun = false;

        private void Sequence_FilmDetach()
        {
            switch ((EFilmDetach_DetachStep)Step.RunStep)
            {
                case EFilmDetach_DetachStep.Start:
                    Log.Debug("Film Detach Start");
                    _retryGripperCount = 0;
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CheckFilmExsit:
                    //if ((YAxisFilmDetach.IsOnPosition(_filmDetachRecipe.YAxisRearFilmSuctionPosition) && _currentRequest == ECVLine.Rear)
                    //    || (YAxisFilmDetach.IsOnPosition(_filmDetachRecipe.YAxisFrontFilmSuctionPosition) && _currentRequest == ECVLine.Front))
                    //{
                    //    if (In_VinylDetect.Value)
                    //    {
                    //        RaiseWarning(_currentRequest == ECVLine.Front ? EWarning.VinylDetach_FrontVinylDetach_Fail :
                    //                                                        EWarning.VinylDetach_RearVinylDetach_Fail);
                    //        break;
                    //    }
                    //}
                    if (Cyl_FilmDetachGripper.IsForward || _devices.Outputs.FilmDetachGrip.Value == true)
                    {
                        Step.RunStep = (int)EFilmDetach_DetachStep.MoveCylinderHeadUp;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylHeadUpDownSafety:
                    if (Cyl_FilmDetachMoverUpDn.IsBackward == false)
                    {
                        Cyl_FilmDetachUpDn(false);
                        Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsBackward);
                        Log.Debug("Film Detach Head Up for safety");
                        Step.RunStep++;
                        break;
                    }
                    Log.Debug("Film Detach Head is Up");
                    Step.RunStep = (int)EFilmDetach_DetachStep.MoveToReadyPos;
                    break;
                case EFilmDetach_DetachStep.CylHeadUpDownSafety_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveUp_Fail);
                        break;
                    }
                    Log.Debug("Film Detach Head Up for safety Done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CheckFlagFilmRequestCondition:
                    if (!FlagIn_FrontCvDetachRequest || !FlagIn_RearCvDetachRequest)
                    {
                        Wait(20);
                        Step.RunStep = (int)EFilmDetach_DetachStep.CheckFilmDetachRequest;
                        break;
                    }
                    Step.RunStep++;
                    break;

                case EFilmDetach_DetachStep.MoveToReadyPos:
                    YAxisFilmDetach.MoveAbs(_filmDetachRecipe.YAxisReadyPosition);
                    Log.Debug("Y Axis Move to Ready Position");
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(_filmDetachRecipe.YAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToReadyPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveReady_Fail);
                        break;
                    }
                    Log.Debug("Y Axis Move to Ready Position Done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylGripperOff:
                    Cyl_FilmDetachGrip(false);
                    Wait(10000, () => Cyl_FilmDetachGripper.IsBackward);
                    Log.Debug("Film Detach Gripper Off");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylGripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_GripOff_Fail);
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CheckFilmDetachRequest:
                    if (!FlagIn_FrontCvDetachRequest && !FlagIn_RearCvDetachRequest)
                    {
                        Wait(20);
                        break; //Non request signal
                    }

                    if (FlagIn_FrontCvDetachRequest && FlagIn_RearCvDetachRequest) //Both request signal
                    {
                        _currentRequest = (_lastRequest == ECVLine.Front) ? ECVLine.Rear : ECVLine.Front;
                        _lastRequest = _currentRequest;
                    }
                    else
                    {
                        _currentRequest = FlagIn_FrontCvDetachRequest ? ECVLine.Front : ECVLine.Rear;
                    }

                    Log.Debug("Request Film Detach check done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToFilmDetachPos:
                    if (_recipeList.FilmDetachHeadRecipe.UseSequenceFilmDetach == 0)
                    {
                        Step.RunStep = (int)EFilmDetach_DetachStep.MoveToGarbagePos;
                        break;
                    }
                    _countRetryFilmDetachShift = 0;
                    double position = (_currentRequest == ECVLine.Front) ? _filmDetachRecipe.YAxisFrontFilmDetachPosition
                                                                         : _filmDetachRecipe.YAxisRearFilmDetachPosition;

                    _filmDetachPeelStartYPos = position;

                    YAxisFilmDetach.MoveAbs(position);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(position));
                    Log.Debug("Y Axis Move to Film Detach Position");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToFilmDetachPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eAlarm = (_currentRequest == ECVLine.Front) ? EWarning.VinylDetach_MoveFrontDetachPos_Fail
                                                                : EWarning.VinylDetach_MoveRearDetachPos_Fail;
                        RaiseWarning((int)eAlarm);
                        break;
                    }
                    Log.Debug("Y Axis Move to Film Detach Position Done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.WaitFilmDetachStartSignal:
                    if (_currentRequest == ECVLine.Front)
                    {
                        if (FlagIn_FrontCvDetachStartWorkRequest == true)
                        {
                            Log.Debug("Front Detach Cv Start Work Signal Received");
                            Step.RunStep++;
                            break;
                        }
                    }
                    else
                    {
                        if (FlagIn_RearCvDetachStartWorkRequest == true)
                        {
                            Log.Debug("Rear Detach Cv Start Work Signal Received");
                            Step.RunStep++;
                            break;
                        }
                    }

                    Wait(10);
                    break;
                case EFilmDetach_DetachStep.DirectOfSetCheck:
                    Log.Debug("Set Reverse Check");
                    Wait(10000, () => In_SetReverseDetect.Value || _filmDetachRecipe.UseDetectSetReverse == 0 || _machineStatus.IsDryRunMode);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.DirectOfSetCheckWait:
                    if (WaitTimeOutOccurred && _filmDetachRecipe.UseDetectSetReverse == 1)
                    {
                        if (_currentRequest == ECVLine.Front)
                        {
                            _devices.Cylinders.SetCV_FrontDetachCentering.Backward();
                        }
                        if (_currentRequest == ECVLine.Rear)
                        {
                            _devices.Cylinders.SetCV_RearDetachCentering.Backward();
                        }
                        Log.Debug("Set Reverse Detect Fail !");
                        RaiseWarning((int)EWarning.VinylDetach_SetDetectReverse_Fail);
                        break;
                    }
                    Log.Debug("Set Reverse Detect OK !");
                    Step.RunStep = (int)EFilmDetach_DetachStep.MoveCylinderHeadDown;
                    break;
                case EFilmDetach_DetachStep.MoveToShiftDetachPos:
                    double shiftPos = ((_currentRequest == ECVLine.Front) ? _filmDetachRecipe.YAxisFrontFilmDetachPosition
                                                                          : _filmDetachRecipe.YAxisRearFilmDetachPosition) + _filmDetachRecipe.ShiftDetachMoveGap;

                    _filmDetachPeelStartYPos = shiftPos;
                    YAxisFilmDetach.MoveAbs(shiftPos);
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(shiftPos));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToShiftDetachPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = (_currentRequest == ECVLine.Front) ? EWarning.VinylDetach_MoveFrontDetachPos_Fail
                                                                               : EWarning.VinylDetach_MoveRearDetachPos_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"Move To Detach Pos Done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveCylinderHeadDown:
                    Cyl_FilmDetachUpDn(true);
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move down");
                    Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveCylinderHeadDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveDown_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move down done");
                    Step.RunStep++;
                    break;
       
                case EFilmDetach_DetachStep.FilmGripperOn:
                    Cyl_FilmDetachGrip(true);
                    Log.Debug($"{Cyl_FilmDetachGripper} gripper on");
                    Wait(10000, () => Cyl_FilmDetachGripper.IsForward);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.FilmGripperOn_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_GripOn_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachGripper} Gripper on done");
                    Wait(200);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveCylinderHeadUp:
                    _filmDetachPeelOffsetTargetYPos = _filmDetachPeelStartYPos + _filmDetachRecipe.YAxisOffsetPeel;
                    Cyl_FilmDetachUpDn(false);
                    YAxisFilmDetach.MoveAbs(_filmDetachPeelOffsetTargetYPos, _filmDetachRecipe.YAxisOffsetPeelVelocity);
                    Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsBackward /*&& Cyl_FilmDetachSuctionUpDn.IsBackward*/
                                    && YAxisFilmDetach.IsOnPosition(_filmDetachPeelOffsetTargetYPos));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveCylinderHeadUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        if (!Cyl_FilmDetachMoverUpDn.IsBackward)
                        {
                            RaiseWarning((int)EWarning.VinylDetach_MoveUp_Fail);
                            break;
                        }

                        EWarning eWarning = (_currentRequest == ECVLine.Front)
                            ? EWarning.VinylDetach_MoveFrontDetachPos_Fail
                            : EWarning.VinylDetach_MoveRearDetachPos_Fail;

                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move up done");

                    if (_isSetDetachRequest == false)
                    {
                        Step.RunStep = (int)EFilmDetach_DetachStep.MoveToGarbagePos;
                        break;
                    }

                    //if (_filmDetachRecipe.UseIonizer == 1) // Use Ionizer
                    //{
                    //    Step.RunStep++;
                    //    break;
                    //}
                    //if(_devRecipe.UseVinylDetachCheck)
                    //{
                    //    Step.RunStep = (int)EFilmDetach_DetachStep.MoveToVinylDetachCheckPos;
                    //    break;
                    //}
                    Step.RunStep = (int)EFilmDetach_DetachStep.MoveBackToPeelStartPos;
                    break;
                case EFilmDetach_DetachStep.MoveBackToPeelStartPos:
                    YAxisFilmDetach.MoveAbs(_filmDetachPeelStartYPos);
                    Log.Debug($"{YAxisFilmDetach} move back to peel start position: " + $"{_filmDetachPeelStartYPos}");
                    Wait(4000,() => YAxisFilmDetach.IsOnPosition(_filmDetachPeelStartYPos));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveBackToPeelStartPos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = (_currentRequest == ECVLine.Front)
                            ? EWarning.VinylDetach_MoveFrontDetachPos_Fail
                            : EWarning.VinylDetach_MoveRearDetachPos_Fail;

                        RaiseWarning((int)eWarning);
                        break;
                    }

                    Log.Debug($"{YAxisFilmDetach} move back to peel start position done: " + $"{_filmDetachPeelStartYPos}");

                    if (_filmDetachRecipe.UseIonizer == 1)
                    {
                        Step.RunStep = (int)EFilmDetach_DetachStep.IonizerOn;
                        break;
                    }

                    if (_devRecipe.UseVinylDetachCheck)
                    {
                        Step.RunStep = (int)EFilmDetach_DetachStep.MoveToVinylDetachCheckPos;
                        break;
                    }

                    Step.RunStep = (int)EFilmDetach_DetachStep.MoveToGarbagePos;
                    break;
                case EFilmDetach_DetachStep.IonizerOn:
                    FilmIonizerOn(true);
                    Wait((int)(_filmDetachRecipe.FilmBlowOnTime * 1000));
                    if (_devRecipe.UseVinylDetachCheck)
                    {
                        Step.RunStep++;
                        break;
                    }
                    Step.RunStep = (int)EFilmDetach_DetachStep.MoveToGarbagePos;
                    break;
                case EFilmDetach_DetachStep.MoveToVinylDetachCheckPos:
                    FilmIonizerOn(false);
                    double vinylDetachCheckPosition = (_currentRequest == ECVLine.Front) ? _filmDetachRecipe.YAxisFrontFilmSuctionPosition
                                                                                : _filmDetachRecipe.YAxisRearFilmSuctionPosition;
                    YAxisFilmDetach.MoveAbs(vinylDetachCheckPosition);
                    Log.Debug($"{YAxisFilmDetach} Move to Vinyl Detach check pos"); // Garbage position is the same as ready position
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(vinylDetachCheckPosition));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToVinylDetachCheckPos_Wait:
                    if (WaitTimeOutOccurred)
                    {
                        EWarning eWarning = (_currentRequest == ECVLine.Front) ? EWarning.VinylDetach_MoveFrontSuctionPos_Fail
                                                                  : EWarning.VinylDetach_MoveRearSuctionPos_Fail;
                        RaiseWarning((int)eWarning);
                        break;
                    }
                    Log.Debug($"{YAxisFilmDetach} Move to Vinyl Detach check pos Done");
                    Wait(200);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.VinylDetach_Check:
                    if(In_VinylDetect.Value)
                    {
                        if(_filmDetachRecipe.UseRetryDetachFilm == 1 && _countRetryFilmDetachShift < 1)
                        {
                            Log.Debug("Retry Move to Shift Detach Pos");
                            _countRetryFilmDetachShift++;
                            Cyl_FilmDetachGripper.Backward();
                            Wait(200);
                            Step.RunStep = (int)EFilmDetach_DetachStep.MoveToShiftDetachPos;
                            break;
                        }

                        if (_filmDetachRecipe.UseRetryDetachFilm == 1 && _countRetryFilmDetach < 1)
                        {
                            Log.Debug("Retry Film Detach Sub Sequence");
                            _countRetryFilmDetachShift = 0;
                            _countRetryFilmDetach++;
                            Cyl_FilmDetachGripper.Backward();
                            Wait(200);
                            Step.RunStep = (int)EFilmDetach_DetachStep.MoveToFilmDetachPos;
                            break;
                        }

                            RaiseWarning(_currentRequest == ECVLine.Front ? (int)EWarning.VinylDetach_FrontVinylDetach_Fail :
                                                                        (int)EWarning.VinylDetach_RearVinylDetach_Fail);
                            break;
                    }
                    _countRetryFilmDetachShift = 0;
                    _countRetryFilmDetach = 0;
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToGarbagePos:
                    if (_isSetDetachRequest == true)
                    {
                        if (_currentRequest == ECVLine.Front)
                        {
                            FlagOut_FrontFilmDetachDone = true;
                        }
                        else
                        {
                            FlagOut_RearFilmDetachDone = true;
                        }
                    }
                    FilmIonizerOn(false);
                    YAxisFilmDetach.MoveAbs(_filmDetachRecipe.YAxisReadyPosition);
                    Log.Debug($"{YAxisFilmDetach} Move to Garbage pos"); // Garbage position is the same as ready position
                    Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(_filmDetachRecipe.YAxisReadyPosition));
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToGarbagePos_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveReady_Fail);
                        break;
                    }
                    Log.Debug($"{YAxisFilmDetach} Move to Garbage pos done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.FilmDetachSignal_Reset:
                    FlagOut_FrontFilmDetachDone = false;
                    FlagOut_RearFilmDetachDone = false;
                    Log.Debug("Film Detach Done Signal Reset");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylinderHeadDown:
                    Cyl_FilmDetachUpDn(true);
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move Down");
                    Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsForward);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylinderHeadDown_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveDown_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move down done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.TrashSuctionOnToRemoveFilm:
                    TrashSuctionOn(true);
                    FilmIonizerOn(true);
                    Log.Debug("Ionizer On To Remove Film From Head");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.FilmGripperOff:
                    Cyl_FilmDetachGrip(false);
                    Log.Debug($"{Cyl_FilmDetachGripper} Gripper Off");
                    Wait(10000, () => Cyl_FilmDetachGripper.IsBackward);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.FilmGripperOff_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_GripOff_Fail);
                        break;
                    }
                    Log.Debug($"{Cyl_FilmDetachGripper} Gripper Off done");
                    //EDM
                    Log.Debug("Film Detach Vac Off done");
                    strEDMPara[0] = "3,";
                    strEDMPara[1] = "02,";
                    strEDMPara[2] = ",";
                    strEDMPara[3] = "TOPM38,";
                    _edmLogger.AddEDMLog("9020", "00000002", strEDMPara);
                    //
                    if (_retryGripperCount > 1)
                    {
                        _retryGripperCount = 0;
                        Step.RunStep = (int)EFilmDetach_DetachStep.MoveToCleanFilmPos;
                        break;
                    }
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.FilmGripperOnAgain:
                    _retryGripperCount++;
                    Cyl_FilmDetachGrip(true);
                    Wait(10000, () => Cyl_FilmDetachGripper.IsForward);
                    Step.RunStep = (int)EFilmDetach_DetachStep.FilmGripperOff;
                    break;
                case EFilmDetach_DetachStep.MoveToCleanFilmPos:
                    //double cleanPos = (_currentRequest == ECVLine.Front) ? _filmDetachRecipe.YAxisFrontCleanFilmPosition :
                    //                                          _filmDetachRecipe.YAxisRearCleanFilmPosition;
                    //YAxisFilmDetach.MoveAbs(cleanPos);
                    //Wait((int)(_globalRecipe.MotionMoveTimeout * 1000), () => YAxisFilmDetach.IsOnPosition(cleanPos));
                    //Log.Debug("Move to Clean Film Head Pos");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.MoveToCleanFilmPos_Check:
                    //if (WaitTimeOutOccurred)
                    //{
                    //    EWarning eWarning = (_currentRequest == ECVLine.Front) ? EWarning.FilmDetachHead_MoveFrontCleanPos_Fail :
                    //                                                EWarning.FilmDetachHead_MoveRearCleanPos_Fail;
                    //    RaiseWarning((int)eWarning);
                    //    break;
                    //}
                    //Log.Debug("Move to Clean Film Head Done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.TrashSuctionDelay:
                    //_gripperTimer.EnableAction(GripperActionKey, () => { Cyl_FilmDetachGrip(true); },
                    //                                             () => { Cyl_FilmDetachGrip(false); });
                    //FilmSuctionOn(true);
                    //Wait(_globalRecipe.TrashSuctionOnTime * 1000);
                    Wait(500);
                    Log.Debug("Film Detach Suction On Wait");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylinderHeadUp:
                    Cyl_FilmDetachUpDn(false);
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move Up");
                    Wait(10000, () => Cyl_FilmDetachMoverUpDn.IsBackward);
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.CylinderHeadUp_Check:
                    if (WaitTimeOutOccurred)
                    {
                        RaiseWarning((int)EWarning.VinylDetach_MoveUp_Fail);
                        break;
                    }
                    // TODO: REMOVE THIS WHEN RETURN SUCTION SEQUENCE
                    FilmIonizerOn(false);
                    //------------------------------------------------
                    Log.Debug($"{Cyl_FilmDetachMoverUpDn} move Up done");
                    Step.RunStep++;
                    break;
                case EFilmDetach_DetachStep.End:
                    //_gripperTimer.DisableAction(GripperActionKey);
                    //Cyl_FilmDetachGrip(false);
                    //FilmSuctionOn(false);
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

        private int _retryGripperCount;
        private void Cyl_FilmDetachUpDn(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_FilmDetachMoverUpDn.Forward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.FilmDetachDown, true);
                SimulationInputSetter.SetSimInput(_devices.Inputs.FilmDetachUp, false);
#endif
            }
            else
            {
                Cyl_FilmDetachMoverUpDn.Backward();
#if SIMULATION
                SimulationInputSetter.SetSimInput(_devices.Inputs.FilmDetachDown, false);
                SimulationInputSetter.SetSimInput(_devices.Inputs.FilmDetachUp, true);
#endif
            }
        }


        private void TrashSuctionOn(bool bOnOff)
        {
            Out_TrashSuctionOn.Value = bOnOff;
            _machineStatus.Vinyl_TrashSuctionOn = bOnOff;
            if (bOnOff)
            {
                Task.Delay((int)(_globalRecipe.TrashSuctionOnTime * 1000)).ContinueWith(t =>
                {
                    _machineStatus.Vinyl_TrashSuctionOn = false;
                    if (_machineStatus.Sponge_TrashSuctionOn) return;

                    TrashSuctionOn(false);
                });
            }
        }

        private void Cyl_FilmDetachGrip(bool bOnOff)
        {
            if (bOnOff)
            {
                Cyl_FilmDetachGripper.Forward();
            }
            else
            {
                Cyl_FilmDetachGripper.Backward();
            }
        }

        private void SuctionVaccumOn(bool bOnOff)
        {
            Out_FilmDetachSuctionVacOn.Value = bOnOff;
            Out_FilmDetachSuctionVacOff.Value = !bOnOff;

            if (bOnOff == false)
            {
                Task.Delay(300).ContinueWith(t =>
                {
                    Out_FilmDetachSuctionVacOff.Value = false;
                });
            }
        }

        private void FilmIonizerOn(bool bOnOff)
        {
            Out_FilmDetachIonizerOn.Value = bOnOff;
        }

        private void StopRun()
        {
            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            YAxisFilmDetach.Stop();
            Out_FilmDetachSuctionVacOn.Value = false;
            FilmIonizerOn(false);
            //Out_TrashSuctionOn.Value = false;
        }
        #endregion

        #region Constructors
        public FilmDetachProcess(Devices devices,
            GlobalRecipe globalRecipe,
            RecipeList recipeList,
            EDMLogger edmLogger,
            MachineStatus machineStatus,
            VaccumList vaccumList,
            DevRecipe devRecipe,
            FlipperTapeDetachRecipe flipperSpongeDetachRecipe,
            [FromKeyedServices("GripperTimer")] ActionAssignableTimer gripperTimer,
            [FromKeyedServices("FilmDetachInput")] IDInputDevice<EFilmDetachInput> filmDetachInput,
            [FromKeyedServices("FilmDetachOutput")] IDOutputDevice<EFilmDetachOutput> filmDetachOutput)
        {
            _edmLogger = edmLogger;
            _machineStatus = machineStatus;
            _vaccumList = vaccumList;
            _devRecipe = devRecipe;
            _flipperSpongeDetachRecipe = flipperSpongeDetachRecipe;
            _devices = devices;
            _globalRecipe = globalRecipe;
            _recipeList = recipeList;
            _filmDetachInput = filmDetachInput;
            _filmDetachOutput = filmDetachOutput;
            _gripperTimer = gripperTimer;

        }
        #endregion

        #region Privates
        private readonly Devices _devices;
        private readonly GlobalRecipe _globalRecipe;
        private readonly RecipeList _recipeList;
        private readonly MachineStatus _machineStatus;
        private readonly IDInputDevice _filmDetachInput;
        private readonly IDOutputDevice _filmDetachOutput;
        private readonly VaccumList _vaccumList;
        private readonly DevRecipe _devRecipe;
        private readonly FlipperTapeDetachRecipe _flipperSpongeDetachRecipe;
        private readonly ActionAssignableTimer _gripperTimer;

        private string GripperActionKey = "GripperAction";

        private ECVLine _lastRequest = ECVLine.Front;
        private ECVLine _currentRequest;

        private bool _isSetDetachRequest => FlagIn_FrontCvDetachRequest || FlagIn_RearCvDetachRequest;
        private int _countRetryFilmDetachShift;
        private int _countRetryFilmDetach;
        string[] strEDMPara = new string[4];
        private EDMLogger _edmLogger;
        private double _filmDetachPeelStartYPos;
        private double _filmDetachPeelOffsetTargetYPos;

        private FilmDetachHeadRecipe _filmDetachRecipe => _recipeList.FilmDetachHeadRecipe;

        #endregion
    }
}
