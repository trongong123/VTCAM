using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Sequence;
using EQX.Core.Units;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Process
{
    public class ChangeReadyProcess : ProcessBase<ESequence>
    {
        protected readonly ActionAssignableTimer _blinkTimer;
        private readonly GlobalRecipe _globalRecipe;
        private readonly MachineStatus _machineStatus;
        private readonly Devices _devices;

        protected string ChangeLedBlinkActionKey => $"{Name}changeLedBlinkAction";
        protected virtual ITray<ETrayCellStatus> CurrentJig { get; }
        protected virtual bool JigWorkDone { get; set; }

        #region Inputs
        protected virtual IDInput In_LightCurtain { get; }
        protected virtual IDInput In_LightCurtainMuting { get; }
        protected virtual IDInput In_DoorByPass { get; }
        protected virtual IDInput In_SubDoor { get; }

        #endregion

        #region Outputs
        protected virtual IDOutput AreaSensorBypassOn { get; }
        protected virtual IDOutput Out_MutingSWLamp { get; }
        #endregion

        #region Cylinders
        protected virtual ICylinder Cyl_SliderLock { get; }
        protected virtual ICylinder Cyl_Doorlock { get; }
        #endregion

        #region Motions 
        protected virtual IMotion z_Axis_Elevator { get; }
        protected virtual IMotion XAxis { get; }
        protected virtual IMotion YAxis { get; }
        protected virtual IMotion ZAxis { get; }


        #endregion

        #region Outputs
        protected virtual IDOutput Out_TowerLampRed { get; }
        protected virtual IDOutput Out_TowerLampYellow { get; }
        protected virtual IDOutput Out_TowerLampGreen { get; }
        protected virtual IDOutput Out_Buzzer1 { get; }
        #endregion
        public ChangeReadyProcess(
            GlobalRecipe globalRecipe,
            MachineStatus machineStatus,
            Devices devices,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer)
        {
            this._blinkTimer = blinkTimer;
            _globalRecipe = globalRecipe;
            _machineStatus = machineStatus;
            _devices = devices;
        }
        protected void Sequence_Auto()
        {

        }
        public override bool ProcessToRun()
        {
            ProcessStatus = EProcessStatus.ToRunDone;
            return true;
        }
        public override bool ProcessToStop()
        {
            return base.ProcessToStop();
        }
        public override bool ProcessToWarning()
        {
            return base.ProcessToWarning();
        }
        public override bool ProcessToAlarm()
        {
            return base.ProcessToAlarm();
        }
        public override bool PreProcess()
        {
            //if (ProcessMode != EProcessMode.Run && _machineStatus.IsReadyToRunProcessMode == false) return base.PreProcess();
            //switch ((EUnitChangeReadyPreProcessStep)Step.PreProcessStep)
            //{
            //    case EUnitChangeReadyPreProcessStep.Start:
            //        {
            //            Step.PreProcessStep++;
            //            break;
            //        }

            //    case EUnitChangeReadyPreProcessStep.CrurrentSequence_Check:
            //        {
            //            if (ProcessMode == EProcessMode.Stop)
            //            {
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            if (Sequence != ESequence.Change && Sequence != ESequence.TrayInCV_Load && (Sequence != ESequence.TraySearch || Sequence != ESequence.TrayOutCV_Unload))
            //            {
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            if (Sequence == ESequence.Change)
            //            {
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.ReadySW_PressDetect;
            //                break;
            //            }
            //            break;

            //        }
            //    case EUnitChangeReadyPreProcessStep.Wait_Signal_LightCurtain_Or_MuttingSW:
            //        {
            //            AreaSensorBypassOn.Value = true;
            //            if (In_LightCurtainMuting.Value ==true)
            //            {
            //                Log.Debug("Muting SW Detected");
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.ChangeSW_PressDetect;
            //                break;

            //            }
            //            if (In_LightCurtain.Value == true)
            //            {
            //                Log.Debug("Light Curtain Detected");
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            break;
            //        }

            //    case EUnitChangeReadyPreProcessStep.Light_Curtain_Detect:
            //        {
            //            if (In_LightCurtain.Value == true)
            //            {
            //                Log.Debug("Change To Sequence Change");
            //                buttonPressCounter = Environment.TickCount;
            //                Sequence = ESequence.Change;
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.Light_Curtain_Stop_Axis:
            //        ((ProcessTimer)ProcessTimer).WaitTime = 0;
            //        Log.Debug("Stop All Axis");
            //        if (z_Axis_Elevator!=null)
            //        {
            //            z_Axis_Elevator.Stop();
            //        }
            //        if (XAxis!=null)
            //        {
            //            XAxis.Stop();
            //        }
            //        if (ZAxis!=null)
            //        {
            //            YAxis.Stop();
            //        }
            //        if (ZAxis!=null)
            //        {
            //            ZAxis.Stop();
            //        }
            //        Step.PreProcessStep++;
            //        break;
            //    case EUnitChangeReadyPreProcessStep.Light_Curtatin_Check_Time_Detect:
            //        {
            //            if (In_LightCurtain.Value == false)
            //            {
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.Ready_Enable;
            //                break;
            //            }
            //            if (Environment.TickCount - buttonPressCounter >= 10000)
            //            {
            //                Log.Info("LightCurtainMutingSW Press Hold 10s Detected");
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.Ready_Enable;

            //                break;
            //            }
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.Light_Curtain_Alarm_TimeOut:
            //        {
            //            AreaSensorBypassOn.Value = false;
            //            Log.Debug("Light Curtain Alarm TimeOut");
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //            break;
            //        }

            //    /// Change to sequence Change by SW                    
            //    case EUnitChangeReadyPreProcessStep.ChangeSW_PressDetect:
            //        {
            //            if (In_LightCurtainMuting.Value == true)
            //            {
            //                buttonPressCounter = Environment.TickCount;
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.ChangeSW_PressHold_Wait:
            //        {
            //            if (In_LightCurtainMuting.Value == false)
            //            {
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //                Sequence = ESequence.AutoRun;
            //                break;
            //            }
            //            if (Environment.TickCount - buttonPressCounter >200)
            //            {
            //                Log.Debug("LightCurtainMutingSW Press Hold 2s Detected");
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.ChangeSW_PressDetect_Confirm:
            //        {
            //            AreaSensorBypassOn.Value = true;
            //            Log.Debug("LightCurtainMutingSW Press Detect Confirmed");
            //            Step.PreProcessStep++;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.ChangeTray_Enable:
            //        {
            //            Log.Info("Change Tray Enabled");
            //            _devices.Outputs.TowerLamp_InputStop();
            //            Step.PreProcessStep++;
            //            Sequence = ESequence.Change;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.SW_Stop_Axis:
            //        ((ProcessTimer)ProcessTimer).WaitTime = 0;
            //        Log.Debug("Stop All Axis");
            //        if (z_Axis_Elevator!=null)
            //        {
            //            z_Axis_Elevator.Stop();
            //        }
            //        if (XAxis!=null)
            //        {
            //            XAxis.Stop();
            //        }
            //        if (YAxis!=null)
            //        {
            //            YAxis.Stop();
            //        }
            //        if (ZAxis!=null)
            //        {
            //            ZAxis.Stop();
            //        }
            //        Step.PreProcessStep++;
            //        break;
            //    case EUnitChangeReadyPreProcessStep.ReadySW_PressDetect:
            //        {
            //            if (In_LightCurtainMuting.Value == true)
            //            {
            //                buttonPressCounter = Environment.TickCount;
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.ReadySW_PressHold_Wait:
            //        {
            //            if (In_LightCurtainMuting.Value == false)
            //            {
            //                Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.CrurrentSequence_Check;
            //                break;
            //            }
            //            if (In_LightCurtain.Value == false)
            //            {
            //                //
            //                //\Log.Debug("L")
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            if (Environment.TickCount - buttonPressCounter >= 500)
            //            {
            //                Log.Debug("LightCurtainSW Press Hold 0.5s Detected");
            //                Step.PreProcessStep++;
            //                break;
            //            }
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.Ready_Enable:
            //        {
            //            //To do: Enable Ready 
            //            Log.Info("Ready Enabled, Set sequence to Load");
            //            _blinkTimer.DisableAction(ChangeLedBlinkActionKey);
            //            Out_MutingSWLamp.Value = false;
            //            if (_machineStatus.IsRunningProcessMode)
            //            {
            //                _devices.Outputs.TowerLamp_Run();
            //            }
            //            AreaSensorBypassOn.Value = false;
            //            Sequence = ESequence.AutoRun;
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.Start;
            //            break;
            //        }
            //    case EUnitChangeReadyPreProcessStep.End:
            //        {
            //            Log.Info("Change Ready PreProcess Ended");
            //            Step.PreProcessStep = (int)EUnitChangeReadyPreProcessStep.Start;
            //            break;
            //        }
            //    default:
            //        break;
            //}
            return base.PreProcess();
        }
        #region Privates
        int buttonPressCounter = 0;
        #endregion
    }
}

