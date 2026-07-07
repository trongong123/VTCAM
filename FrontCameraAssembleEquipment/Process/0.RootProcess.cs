using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Sequence;
using EQX.Core.Units;
using EQX.Process;
using EQX.UI.Controls;
using EQX.UI.Language;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using SuperSimpleTcp;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using TopComponent.Controls;

namespace FrontCameraAssembleEquipment.Process
{
    public class RootProcess<TESequence, TESemiSequence> : ProcessBase<ESequence>
        where TESequence : Enum
    {
        private readonly Devices _devices;
        private readonly MachineStatus _machineStatus;
        private readonly IAlertService _alarmService;
        private readonly IAlertService _warningService;
        private int raisedAlarmCode = -1;
        private int raisedWarningCode = -1;
        private readonly object _lockAlarm = new object();
        string[] strEDMPara = new string[4];
        private EDMLogger _edmLogger;
        private int m_nTowerLampCurrentData = 0;
        private int m_nTowerLampPreviousData = 0;
        private int Wait_feeded = 60;
        private CVision_FrontCamera _vision;
        private readonly MaterialStatusList _materialStatusList;
        private TotalTackTime _totalTackTime;
        public DateTime begin_time { get; set; }
        public DateTime end_time { get; set; }
        public Stopwatch m_swSaveEDMLog;
        private int Current_LampTower_Status;
        private readonly TrayList _trayList;
        private readonly RecipeSelector _recipeSelector;
        private readonly NavigationStore _navigationStore;

        public EventHandler RootAlarmEvent;
        public EventHandler RootWarningEvent;

        private bool IsModelChange { get; set; } = false;

        private ITray<ETrayCellStatus> CurrentJig => _trayList.TrayCamera;

        public RootProcess(Devices devices,
            MachineStatus machineStatus,
            EDMLogger edmLogger,
            CVision_FrontCamera vision,
            TotalTackTime totalTackTime,
            MaterialStatusList materialStatusList,
            TrayList traylist,
            RecipeSelector recipeSelector,
            NavigationStore navigationStore,
            [FromKeyedServices("AlarmService")] IAlertService alarmService,
            [FromKeyedServices("WarningService")] IAlertService warningService)
        {
            _devices = devices;
            _machineStatus = machineStatus;
            _edmLogger = edmLogger;
            _vision = vision;
            _totalTackTime = totalTackTime;
            _materialStatusList = materialStatusList;
            _alarmService = alarmService;
            _warningService = warningService;
            _trayList = traylist;
            _recipeSelector = recipeSelector;
            _navigationStore = navigationStore;
            this.ProcessModeUpdated += ProcessModeUpdatedHandler;

            this.AlarmRaised += (alarmId, alarmSource) =>
            {
                RootProcess_AlarmRaised(alarmId, alarmSource);
            };
            this.WarningRaised += (warningId, warningSource) =>
            {
                RootProcess_WarningRaised(warningId, warningSource);
            };

            _devices.Inputs.FrontDetachCvEnd.ValueChanged += Event_InputSetEventEDMLog;
            _devices.Inputs.RearDetachCvEnd.ValueChanged += Event_InputSetEventEDMLog;

            _materialStatusList.FrontSetAssembleCvMaterialStatus.StateChanged += OnMaterialSetStatusChanged_FrontAssy;
            _materialStatusList.RearSetAssembleCvMaterialStatus.StateChanged += OnMaterialSetStatusChanged_RearAssy;
            _materialStatusList.FrontSetDetachCvMaterialStatus.StateChanged += OnMaterialSetStatusChanged_FrontDetach;
            _materialStatusList.RearSetDetachCvMaterialStatus.StateChanged += OnMaterialSetStatusChanged_RearDetach;

            AlertNotifyView.BuzzerOff += AlertNotifyView_BuzzerOff;
            AlertNotifyViewV2.BuzzerOff += AlertNotifyView_BuzzerOff;

            _devices.Inputs.FrontResetSW.ValueChanged += FrontResetSW_ValueChanged;

            _devices.Inputs.FrontDoor.ValueChanged += Door_ValueChanged;
            _devices.Inputs.RightDoor.ValueChanged += Door_ValueChanged;
            _devices.Inputs.RearDoor.ValueChanged += Door_ValueChanged;

            _recipeSelector.RecipeChanged += RecipeChangedHandler;
        }

        private void RecipeChangedHandler(object? sender, EventArgs e)
        {
            IsModelChange = true;
            _machineStatus.MachineReadyDone = false;
        }

        private void Door_ValueChanged(object? sender, EventArgs e)
        {
            if (sender is IDInput input)
            {
                if (input.Value == false)
                {
                    foreach (var motion in _devices.Motions.AjinMotions.All!)
                    {
                        motion.Stop();
                    }
                }
            }
        }

        private void AlertNotifyView_BuzzerOff()
        {
            _devices.Outputs.Buzzer1.Value = false;
        }

        private void FrontResetSW_ValueChanged(object? sender, EventArgs e)
        {
            if (_devices.Inputs.FrontResetSW.Value == true)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bool isShown = Application.Current.Windows.OfType<AlertNotifyView>().Any();
                    if (isShown) Application.Current.Windows.OfType<AlertNotifyView>().First().Close();

                    bool isShownV2 = Application.Current.Windows.OfType<AlertNotifyViewV2>().Any();
                    if (isShownV2) Application.Current.Windows.OfType<AlertNotifyViewV2>().First().Close();

                    isShown = Application.Current.Windows.OfType<MessageBoxEx>().Any();
                    if (isShown) Application.Current.Windows.OfType<MessageBoxEx>().First().Close();
                });
            }

        }

        private bool IsLightCurtainBypass => _devices.Outputs.AreaSensorBypassOn.Value;
        private bool IsLightCurtainDetect => _devices.Inputs.AreaSensorDetect.Value;
        private bool IsEmergencyStopPressed => _devices.Inputs.FrontEmergencyStop.Value == true || _devices.Inputs.RearEmergencyStop.Value == true;
        private bool IsMcPowerOn => _devices.Inputs.FrontPowerOn.Value;
        private bool ResetSW => _devices.Inputs.FrontResetSW.Value;
        private bool FrontStopSW => _devices.Inputs.FrontStopSW.Value;
        private bool RearStopSw => _devices.Inputs.RearStopSW.Value;
        private bool RearInputDetachCVSensorStartDetect => _devices.Inputs.RearDetachCvStart.Value;
        private bool FrontInputDetachCVSensorStartDetect => _devices.Inputs.FrontDetachCvStart.Value;
        private bool RearInputDetachCVSensorEndDetect => _devices.Inputs.RearDetachCvEnd.Value;
        private bool FrontInputDetachCVSensorEndDetect => _devices.Inputs.FrontDetachCvEnd.Value;
        private bool RearAssembleCVSensorEndDetect => _devices.Inputs.RearAssembleCvEnd.Value;
        private bool FrontAssembleCVSensorEndDetect => _devices.Inputs.FrontAssembleCvEnd.Value;

        #region Override Methods
        public override bool PreProcess()
        {
            if (_devices.Outputs.TowerLampGreen.Value == true)
            {
                Current_LampTower_Status = 1;
            }
            if (_devices.Outputs.TowerLampYellow.Value == true)
            {
                Current_LampTower_Status = 2;
            }
            if (_devices.Outputs.TowerLampRed.Value == true)
            {
                Current_LampTower_Status = 3;
            }
            if (_devices.Outputs.TowerLampYellow.Value == true && _devices.Outputs.TowerLampGreen.Value == true)
            {
                Current_LampTower_Status = 4;
            }

            // 1. CHECK ALARM STATUS (Utils, Motion, Safety...)
            if (ProcessMode != EProcessMode.ToAlarm && ProcessMode != EProcessMode.Alarm)
            {
                if(_navigationStore.CurrentViewModel is InitDeinitViewModel == false)
                {
                    CheckRealTimeAlarmStatus();
                }
            }
            if (ProcessMode == EProcessMode.Run)
            {
                CheckMetearialFeeded();
            }
            if (ProcessMode == EProcessMode.ToOrigin || ProcessMode == EProcessMode.Origin || ProcessMode == EProcessMode.ToRun || ProcessMode == EProcessMode.Run)
            {
                if ((IsLightCurtainBypass == false) && IsLightCurtainDetect)
                {
                    RaiseWarning((int)EWarning.LightCurtainDetected);
                }
                if (DoorSensors == false)
                {
                    RaiseWarning((int)EWarning.DoorOpen);
                }
            }
            else
            {
                if (DoorSensors == false && Current_LampTower_Status != 3)
                {
                    _devices.Outputs.TowerLamp_DoorOpen();
                }
                if (DoorSensors == true && Current_LampTower_Status != 2 && IsEmergencyStopPressed == false)
                {
                    _devices.Outputs.TowerLamp_Stop();
                }
                if (IsEmergencyStopPressed)
                {
                    _devices.Outputs.TowerLampBuzzer_Alarm();
                }
            }


            //2.CHECK USER OPERATION COMMAND(Origin / Ready / Start / Stop /Reset/ Semiauto...)
            EOperationCommand command = EOperationCommand.None;
            ESemiSequence eSemiAutoSequence = ESemiSequence.None;
            if (_machineStatus.IsRunningProcessMode)
            {
                // Machine is doing something : Run, Origin, To-Action
                // Can Stop
                if (_machineStatus.OPCommand == EOperationCommand.Stop
                    || _devices.Inputs.FrontStopSW.Value == true
                    || _devices.Inputs.RearStopSW.Value == true
                    || _devices.Inputs.LightCurtainMutingSW.Value)
                {
                    if (_devices.Inputs.LightCurtainMutingSW.Value)
                    {
                        while (_devices.Inputs.LightCurtainMutingSW.Value)
                        {
                            Thread.Sleep(10);
                        }

                        Thread.Sleep(50);
                        _machineStatus.IsOnMuting = true;
                    }

                    command = EOperationCommand.Stop;
                }
                // Block run OPCommand actived while machine is Runiing
                _machineStatus.OPCommand = EOperationCommand.None;
            }
            else
            {
                if ((_machineStatus.OPCommand == EOperationCommand.Ready))
                {
                    command = EOperationCommand.Ready;
                }

                // Machine is in standby mode (doing nothing)
                // Can Origin / Ready / Start
                if (_machineStatus.OPCommand == EOperationCommand.Origin)
                {
                    command = EOperationCommand.Origin;
                }

                if (ProcessMode == EProcessMode.Alarm || ProcessMode == EProcessMode.Warning ||
                    ProcessMode == EProcessMode.ToAlarm || ProcessMode == EProcessMode.ToWarning || ProcessMode == EProcessMode.ToStop || ProcessMode == EProcessMode.Stop)
                {
                    if (ResetSW == true)
                    {
                        //command = EOperationCommand.Reset;
                    }
                }

                if (ProcessMode != EProcessMode.ToAlarm &&
                    ProcessMode != EProcessMode.Alarm && ProcessMode != EProcessMode.None)
                {
                     if (((_machineStatus.OPCommand == EOperationCommand.Start
                        || _devices.Inputs.FrontStartSW.Value == true
                        || _devices.Inputs.RearStartSW.Value == true)
                        && (_navigationStore.CurrentViewModel is AutoViewModel)
                        && (_machineStatus.IsInterfaceOnlyMode == false) &&
                            _machineStatus.IsExistWindowPopup == false) ||
                        (_machineStatus.IsOnMuting && _devices.Inputs.LightCurtainMutingSW.Value))
                    {
                        if (_devices.Inputs.LightCurtainMutingSW.Value)
                        {
                            while (_devices.Inputs.LightCurtainMutingSW.Value)
                            {
                                Thread.Sleep(10);
                            }

                            Thread.Sleep(50);

                            _machineStatus.IsOnMuting = false;
                        }

                        command = EOperationCommand.Start;
                    }
                    else if (_machineStatus.OPCommand == EOperationCommand.SemiAuto && (_machineStatus.IsInterfaceOnlyMode == false))
                    {
                        command = EOperationCommand.SemiAuto;
                    }
                    else if (_devices.Inputs.FrontStopSW.Value == true || _devices.Inputs.RearStopSW.Value == true)
                    {
                        foreach (var motion in _devices.Motions.AjinMotions.All!)
                        {
                            motion.Stop();
                        }
                    }
                }
                else if ((_machineStatus.OPCommand == EOperationCommand.Ready
                    || _machineStatus.OPCommand == EOperationCommand.Start
                    || _devices.Inputs.FrontStartSW.Value == true
                    || _devices.Inputs.RearStartSW.Value == true) && _navigationStore.CurrentViewModel is AutoViewModel)
                {
                    if((ProcessMode == EProcessMode.None && _machineStatus.OPCommand == EOperationCommand.Ready) == false)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_ResetAlarmBeforeRun"], false, (string)Application.Current.Resources["str_Confirm"]);
                        _machineStatus.OPCommand = EOperationCommand.None;
                    }
                }
            }
            //3.HANDLE USER OPERATION COMMAND
            CheckHomeStatus();
            HandleOPCommand(command);
            checkTowerLampStatus();
            CheckMaterialEDMLog();
            return base.PreProcess();
        }

        private void CheckHomeStatus()
        {
            if (_devices.Motions.All.Count(p => p.Status.IsHomeDone != true) == 0)
            {
                _machineStatus.OriginDone = true;
            }
            else
            {
                _machineStatus.OriginDone = false;
            }
        }

        public override bool ProcessToAlarm()
        {
            _devices.Outputs.ManualMode.Value = true;

            ((ProcessTimer)ProcessTimer).WaitTime = 0;
            if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.ToAlarmDone) == 0)
            {
                _totalTackTime.StopAllStopWatch();

                foreach (var motion in _devices.Motions.AjinMotions.All!) { motion.Stop(); }

                _machineStatus.OriginDone = false;
                _devices.Outputs.TowerLampBuzzer_Alarm();

                ProcessMode = EProcessMode.Alarm;

                Log.Debug("ToAlarm Done, Alarm");
                if (AlertNotifyViewV2.ShowDialog(_alarmService.GetById(raisedAlarmCode)) == false)
                {
                    _devices.Outputs.ResetTowerLampBuzzer_Alarm();
                }
                raisedAlarmCode = -1;
            }
            else
            {
                Thread.Sleep(10);
            }

            return true;
        }

        public override bool ProcessToWarning()
        {
            _devices.Outputs.ManualMode.Value = true;

            if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.ToWarningDone) == 0)
            {
                _totalTackTime.StopAllStopWatch();
                foreach (var motion in _devices.Motions.All!) { motion.Stop(); }

                if (DoorSensors == false || IsLightCurtainDetect == true)
                {
                    _devices.Outputs.TowerLampBuzzer_Alarm();
                }
                else
                {
                    _devices.Outputs.TowerLampBuzzer_Warning();
                }

                ProcessMode = EProcessMode.Warning;
                Log.Debug("ToWarning Done, Warning");
                if (AlertNotifyViewV2.ShowDialog(_warningService.GetById(raisedWarningCode), true) == false)
                {
                    _devices.Outputs.ResetTowerLampBuzzer_Alarm();
                }
                raisedWarningCode = -1;

            }
            else
            {
                Thread.Sleep(10);
            }

            return true;
        }

        public override bool ProcessOrigin()
        {
            if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.OriginDone) == 0)
            {
                _machineStatus.OriginDone = true;

                MessageBoxEx.ShowDialog(Application.Current.Resources["str_OriginSuccess"].ToString(), false);

                ProcessMode = EProcessMode.Stop;
                Log.Debug("Origin Done, Stop");
            }
            else
            {
                Thread.Sleep(10);
            }

            return true;
        }

        public override bool ProcessToOrigin()
        {
            switch ((ERootProcess_ToOriginStep)Step.OriginStep)
            {
                case ERootProcess_ToOriginStep.Start:
                    Log.Debug("To Origin Start");
                    _devices.Outputs.TowerLamp_Origin();
                    _devices.Outputs.Buzzer1.Value = false;
                    _devices.Outputs.AreaSensorBypassOn.Value = false;
                    Step.OriginStep++;
                    break;
                case ERootProcess_ToOriginStep.CheckLightCurtain:
                    if (IsLightCurtainDetect)
                    {
                        RaiseWarning((int)EWarning.LightCurtainDetected);
                        _devices.Outputs.TowerLampBuzzer_Alarm();
                        break;
                    }

                    Log.Debug("Light Curtain safe");
                    Step.OriginStep++;
                    break;
                case ERootProcess_ToOriginStep.DoorSensorCheck:
                    if (DoorSensors == false)
                    {
                        RaiseWarning((int)EWarning.DoorOpen);
                        break;
                    }

                    Log.Debug("Doors closed.");
                    Step.OriginStep++;
                    break;
                case ERootProcess_ToOriginStep.AreaByPassCheck:
                    _devices.Outputs.AreaSensorBypassOn.Value = false;
                    Step.OriginStep++;
                    break;
                case ERootProcess_ToOriginStep.ChildsToOriginDoneWait:
                    if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.ToOriginDone) != 0)
                    {
                        Wait(10);
                        break;
                    }

                    Step.OriginStep++;
                    break;
                case ERootProcess_ToOriginStep.End:
                    Log.Debug("To Origin Done");
                    ProcessMode = EProcessMode.Origin;
                    Step.OriginStep = 0;
                    break;
            }
            return true;
        }

        public override bool ProcessToStop()
        {
            if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.ToStopDone) == 0)
            {
                _totalTackTime.StopAllStopWatch();

                ProcessMode = EProcessMode.Stop;
                _devices.Outputs.ManualMode.Value = true;
                if (DoorSensors == true)
                {
                    _devices.Outputs.TowerLamp_Stop();
                }
                Log.Debug("ToStop Done, Stop");
            }
            else
            {
                Thread.Sleep(10);
            }

            return true;
        }

        public override bool ProcessToRun()
        {
            switch ((ERootProcess_ToRunStep)Step.ToRunStep)
            {
                case ERootProcess_ToRunStep.Start:
                    Log.Debug("ToRun Start");
                    _devices.Outputs.Buzzer1.Value = false;
                    _devices.Outputs.ManualMode.Value = true;
                    _devices.Outputs.AreaSensorBypassOn.Value = false;
                    Step.ToRunStep++;
                    break;
                case ERootProcess_ToRunStep.LightCurtainCheck:
                    if (IsLightCurtainDetect)
                    {
                        RaiseWarning((int)EWarning.LightCurtainDetected);
                        break;
                    }

                    Log.Debug("Light Curtain safe");
                    Step.ToRunStep++;
                    break;
                case ERootProcess_ToRunStep.DoorSensorCheck:
                    if (DoorSensors == false)
                    {
                        RaiseWarning((int)EWarning.DoorOpen);
                        break;
                    }

                    Log.Debug("Doors closed.");
                    Step.ToRunStep++;
                    break;
                case ERootProcess_ToRunStep.DoorBypassCheck:
                    _devices.Outputs.AreaSensorBypassOn.Value = false;
                    Step.ToRunStep++;
                    break;
                case ERootProcess_ToRunStep.ChildsToRunDone_Wait:
                    if (Childs!.Count(child => child.ProcessStatus != EProcessStatus.ToRunDone) != 0)
                    {
                        Wait(10);
                        break;
                    }
                    Step.ToRunStep++;
                    break;
                case ERootProcess_ToRunStep.End:
                    ProcessMode = EProcessMode.Run;
                    Log.Debug("ToRun Done, Running");
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
                    break;
                case ESequence.Ready:
                    if (Childs!.Count(child => child.Sequence != ESequence.Stop) == 0)
                    {
                        _machineStatus.MachineReadyDone = true;

                        ProcessMode = EProcessMode.ToStop;
                        Log.Debug("Initialize done");
                    }
                    break;
                default: // Semi Auto
                    if (Childs!.Count(child => child.Sequence != ESequence.Stop) == 0)
                    {
                        ProcessMode = EProcessMode.Stop;
                        _devices.Outputs.TowerLamp_Stop();
                        Log.Info("SemiAuto sequence done");
                    }
                    break;
            }

            return true;
        }

        private void HandleOPCommand(EOperationCommand command)
        {
            try
            {
                switch (command)
                {
                    case EOperationCommand.None:
                        Thread.Sleep(10);
                        break;
                    case EOperationCommand.Origin:
                        if (_machineStatus.IsInterfaceOnlyMode)
                        {
                            if (_machineStatus.IsInterfaceOnlyMode)
                            {
                                MessageBoxEx.ShowDialog("Machine is Manual Interface Mode , can not Origin Machine", false);

                                _machineStatus.OPCommand = EOperationCommand.None;
                                return;
                            }
                        }

                        foreach (var motion in _devices.Motions.All!)
                        {
                            if (motion.Status.IsAlarm)
                            {
                                motion.AlarmReset();
                                Thread.Sleep(200);
                            }

                            motion.MotionOn();
                        }

                        Thread.Sleep(1000);

                        if (_devices.Motions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                        {
                            MessageBoxEx.ShowDialog("Motions turn on failed");
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        Step.OriginStep = 0;
                        Childs?.ToList().ForEach(p => p.Step.OriginStep = 0);
                        ProcessMode = EProcessMode.ToOrigin;
                        _machineStatus.OPCommand = EOperationCommand.None;
                        break;
                    case EOperationCommand.Ready:
                        if (_machineStatus.IsInterfaceOnlyMode)
                        {
                            MessageBoxEx.ShowDialog("Machine is Manual Interface Mode , can not Init Machine", false);

                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_devices.Motions.AjinMotions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], false, (string)Application.Current.Resources["str_Confirm"]);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }
                        if (_machineStatus.OriginDone == false)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        Sequence = ESequence.Ready;
                        foreach (var process in Childs!)
                        {
                            process.ProcessStatus = EProcessStatus.None;
                            process.Sequence = Sequence;
                        }

                        ProcessMode = EProcessMode.ToRun;
                        _machineStatus.OPCommand = EOperationCommand.None;
                        break;
                    case EOperationCommand.Start:
                        if (_machineStatus.IsInterfaceOnlyMode)
                        {
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_machineStatus.IsOnMuting)
                        {
                            MessageBoxEx.ShowDialog("Machine is On Muting Mode , Press Muting Button to Start Machine", false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_devices.Motions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], false, (string)Application.Current.Resources["str_Confirm"]);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }
                        if (_machineStatus.OriginDone == false)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_machineStatus.MachineReadyDone == false && IsModelChange == true)
                        {
                            _devices.Outputs.TowerLampBuzzer_Warning();
                            MessageBoxEx.ShowDialog("Machine Need Init and Remove All Old Camera On Gripper , Head Before Run New Model", false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_machineStatus.MachineReadyDone == false)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeReadyBeforeRun"], false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_machineStatus.IsReadyToRunProcessMode == false)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeReadyBeforeRun"], false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        _machineStatus.IsOnMuting = false;

                        if (IsModelChange == true)
                        {
                            if (_devices.Inputs.VtCamPrealignVacOn.Value ||
                                _devices.Inputs.VtCamAssemblePnPVacOn.Value ||
                                _devices.Inputs.VtCamSupplyPnPVacOn.Value ||
                                _devices.Inputs.VtCamRotatorDetect.Value)
                            {
                                _devices.Outputs.TowerLampBuzzer_Warning();
                                MessageBoxEx.ShowDialog("Remove All Old Camera On Gripper , Head Before Run New Model", false);
                                _machineStatus.OPCommand = EOperationCommand.None;
                                return;
                            }
                        }

                        IsModelChange = false;
                        //if (Childs!.Any(p => p.IsAlarm))
                        //{
                        //    MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                        //    _machineStatus.OPCommand = EOperationCommand.None;
                        //    return;
                        //}

                        if (_vision.IsVisionConnected == false)
                        {
                            MessageBoxEx.ShowDialog("Please Check Vision Connection Status Before Run", false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        //if(CVEndSensors == true)
                        //{
                        //    if (MessageBoxEx.ShowDialog("CV End Sensor detect. Do you want to Up Stopper?") == true) 
                        //    {
                        //        _machineStatus.IsCVConditionConfirm = true;
                        //    }
                        //    else
                        //    {
                        //        _machineStatus.IsCVConditionConfirm = false;
                        //    }
                        //}

                        Log.Info("Start Command");

                        Sequence = ESequence.AutoRun;
                        begin_time = DateTime.Now;
                        foreach (var process in Childs!)
                        {
                            process.ProcessStatus = EProcessStatus.None;
                            process.Sequence = Sequence;
                        }

                        ProcessMode = EProcessMode.ToRun;
                        _machineStatus.OPCommand = EOperationCommand.None;
                        _machineStatus.SemiAutoSequence = ESemiSequence.None;

                        //EDM
                        strEDMPara[0] = ",";
                        strEDMPara[1] = "0,";
                        if (_machineStatus.MachineRunMode == Defines.Process.EMachineRunMode.Dryrun) strEDMPara[1] = "1,";
                        if (_machineStatus.MachineRunMode == Defines.Process.EMachineRunMode.ByPass) strEDMPara[1] = "2,";
                        strEDMPara[2] = ",";
                        strEDMPara[3] = ",";
                        _edmLogger.AddEDMLog("9000", "00000002", strEDMPara);
                        //EDM
                        break;
                    case EOperationCommand.Stop:
                        Log.Info("Stop Command");

                        foreach (var motion in _devices.Motions.AjinMotions.All!) { motion.Stop(); }
                        ProcessMode = EProcessMode.ToStop;
                        _machineStatus.OPCommand = EOperationCommand.None;

                        if (FrontStopSW || RearStopSw)
                        {
                            strEDMPara[0] = ",";
                            strEDMPara[1] = ",";
                            strEDMPara[2] = ",";
                            strEDMPara[3] = ",";
                            _edmLogger.AddEDMLog("9001", "00000000", strEDMPara);
                        }
                        else
                        {
                            strEDMPara[0] = ",";
                            strEDMPara[1] = ",";
                            strEDMPara[2] = ",";
                            strEDMPara[3] = ",";
                            _edmLogger.AddEDMLog("9001", "00000002", strEDMPara);
                        }
                        break;
                        //case EOperationCommand.Reset:
                        _devices.Outputs.ResetTowerLampBuzzer_Alarm();
                        _machineStatus.OPCommand = EOperationCommand.None;
                        break;
                    case EOperationCommand.SemiAuto:
                        if (_devices.Motions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], false, (string)Application.Current.Resources["str_Confirm"]);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        if (_machineStatus.OriginDone == false)
                        {
                            MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                            _machineStatus.OPCommand = EOperationCommand.None;
                            return;
                        }

                        Sequence = (ESequence)Enum.Parse(typeof(ESequence), _machineStatus.SemiAutoSequence.ToString());

                        foreach (var process in Childs!)
                        {
                            process.ProcessStatus = EProcessStatus.None;
                            process.Sequence = Sequence;
                        }

                        ProcessMode = EProcessMode.ToRun;

                        _machineStatus.OPCommand = EOperationCommand.None;
                        _machineStatus.SemiAutoSequence = ESemiSequence.None;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Process {Name} exception: {ex.Message}");

            }
        }

        #endregion

        #region Privates Methos
        private void CheckRealTimeAlarmStatus()
        {
            if (!IsMcPowerOn)
            {
                Childs!.ToList().ForEach(p => p.IsAlarm = true);
                RaiseAlarm((int)EAlarm.PowerMcOff);
            }
            if (IsEmergencyStopPressed)
            {
                Childs!.ToList().ForEach(p => p.IsAlarm = true);
                RaiseAlarm((int)EAlarm.EmergencyStopPressed);
            }
        }

        private void ProcessModeUpdatedHandler(object? sender, EventArgs e)
        {
            _machineStatus.CurrentProcessMode = this.ProcessMode;
        }

        private void RootProcess_AlarmRaised(int alarmId, string alarmSource)
        {
            lock (_lockAlarm)
            {
                //if (this.IsInAlarmMode()) return;

                Log.Error($"[{(int)(EAlarm)alarmId}] {((EAlarm)alarmId).GetDescription()}");
                raisedAlarmCode = alarmId;
                ProcessMode = EProcessMode.ToAlarm;

                RootAlarmEvent?.Invoke(raisedAlarmCode, new EventArgs());

                strEDMPara[0] = ",";
                strEDMPara[1] = ",";
                strEDMPara[2] = ",";
                strEDMPara[3] = ",";
                _edmLogger.AddEDMLog(alarmSource, "00000002", strEDMPara);
            }
        }

        private void RootProcess_WarningRaised(int warningId, string warningSource)
        {
            lock (_lockAlarm)
            {
                if (this.IsInWarningMode() || this.IsInAlarmMode()) return;

                Log.Warn($"[{(int)(EWarning)warningId}] {((EWarning)warningId).GetDescription()}");
                raisedWarningCode = warningId;
                ProcessMode = EProcessMode.ToWarning;

                RootWarningEvent?.Invoke(raisedWarningCode, new EventArgs());

                strEDMPara[0] = ",";
                strEDMPara[1] = ",";
                strEDMPara[2] = ",";
                strEDMPara[3] = ",";
                _edmLogger.AddEDMLog(warningSource, "00000002", strEDMPara);
            }
        }

        #endregion

        private EOperationCommand GetUserCommand()
        {
            if (/*_systemState.CommandState.HMICommand == EOperationCommand.Stop ||*/
                _devices.Inputs.FrontStopSW.Value == true ||
                _devices.Inputs.RearStopSW.Value == true)
            {
                return EOperationCommand.Stop;
            }
            if (/*_systemState.CommandState.HMICommand == EOperationCommand.Stop ||*/
                _devices.Inputs.FrontStopSW.Value == true ||
                _devices.Inputs.RearStopSW.Value == true)
            {
                return EOperationCommand.Stop;
            }

            return EOperationCommand.None;
        }

        private bool DoorSensors
        {
            get
            {
                // TODO: Change when Run Test Done
                //return true;
                return _devices.Inputs.FrontDoor.Value &&
                        _devices.Inputs.RearDoor.Value &&
                        _devices.Inputs.RightDoor.Value;
            }
        }

        private bool CVEndSensors
        {
            get
            {
                return (_devices.Inputs.FrontAssembleCvEnd.Value && !_devices.Inputs.FrontAssembleCvStopperUp.Value)
                    || (_devices.Inputs.FrontDetachCvEnd.Value && !_devices.Inputs.FrontDetachCvStopperUp.Value)
                    || (_devices.Inputs.RearAssembleCvEnd.Value && !_devices.Inputs.RearAssembleCvStopperUp.Value)
                    || (_devices.Inputs.RearDetachCvEnd.Value && !_devices.Inputs.RearDetachCvStopperUp.Value);
            }
        }
        private void CheckMetearialFeeded()
        {


            if (RearInputDetachCVSensorStartDetect || FrontInputDetachCVSensorStartDetect)
            {
                begin_time = DateTime.Now;
            }
            else
            {
                end_time = DateTime.Now;
            }
            if (((end_time - begin_time).TotalSeconds > Wait_feeded) && Current_LampTower_Status != 2 && Current_LampTower_Status != 4 && _machineStatus.IsDryRunMode == false)
            {
                _devices.Outputs.TowerLamp_InputStop();
            }
            if (((end_time - begin_time).TotalSeconds < Wait_feeded)
                && Current_LampTower_Status != 1 && Current_LampTower_Status != 4
                && (Childs!.Count(child => child.Sequence == ESequence.Change) == 0))
            {
                _devices.Outputs.TowerLamp_Run();
            }
        }
        public int time
        {
            get
            {
                if (begin_time != null && end_time != null)
                {
                    return (int)(end_time - begin_time).TotalMilliseconds;
                }
                else
                {
                    return -1;
                }
            }
        }
        private void checkTowerLampStatus()
        {
            m_nTowerLampCurrentData = 0;
            if (_devices.Outputs.TowerLampRed.Value) m_nTowerLampCurrentData += 1;
            if (_devices.Outputs.TowerLampYellow.Value) m_nTowerLampCurrentData += 10;
            if (_devices.Outputs.TowerLampGreen.Value) m_nTowerLampCurrentData += 100;
            if ((m_nTowerLampCurrentData != m_nTowerLampPreviousData) && (m_nTowerLampCurrentData != 0))
            {
                strEDMPara[0] = ",";
                strEDMPara[1] = string.Format("{0:D3},", m_nTowerLampCurrentData);
                strEDMPara[2] = ",";
                strEDMPara[3] = ",";
                _edmLogger.AddEDMLog("9009", "00000002", strEDMPara);
                m_nTowerLampPreviousData = m_nTowerLampCurrentData;
            }
        }

        private void CheckMaterialEDMLog()
        {

            if (_totalTackTime.EDMLogStopWatch.ElapsedSecond >= 60)
            {
                int mMaterialCount = 0;
                strEDMPara[0] = ",";
                if (RearInputDetachCVSensorEndDetect) mMaterialCount++;
                if (FrontInputDetachCVSensorEndDetect) mMaterialCount++;
                if (RearAssembleCVSensorEndDetect) mMaterialCount++;
                if (FrontAssembleCVSensorEndDetect) mMaterialCount++;
                strEDMPara[1] = string.Format("{0},", mMaterialCount);
                strEDMPara[2] = ",";
                strEDMPara[3] = ",";
                _edmLogger.AddEDMLog("9200", "00000002", strEDMPara);
                _totalTackTime.EDMLogStopWatch.RestartTime();
            }
        }

        private void Event_InputSetEventEDMLog(object? sender, EventArgs e)
        {
            if (ProcessMode == EProcessMode.Run && (sender is IDInput input && input.Value == true))
            {
                strEDMPara[0] = "0,";
                strEDMPara[1] = ",";
                strEDMPara[2] = ",";
                strEDMPara[3] = ",";
                _edmLogger.AddEDMLog("9004", "00000002", strEDMPara);
                m_nTowerLampPreviousData = m_nTowerLampCurrentData;
            }
        }

        private void OnMaterialSetStatusChanged_FrontAssy(bool obj)
        {
            if (obj)
            {
                _devices.Cylinders.SetCV_FrontAssembleCentering.Forward();
            }
            else
            {
                _devices.Cylinders.SetCV_FrontAssembleCentering.Backward();
            }
        }
        private void OnMaterialSetStatusChanged_RearAssy(bool obj)
        {
            if (obj)
            {
                _devices.Cylinders.SetCV_RearAssembleCentering.Forward();
            }
            else
            {
                _devices.Cylinders.SetCV_RearAssembleCentering.Backward();
            }
        }
        private void OnMaterialSetStatusChanged_FrontDetach(bool obj)
        {
            if (obj)
            {
                _devices.Cylinders.SetCV_FrontDetachCentering.Forward();
            }
            else
            {
                _devices.Cylinders.SetCV_FrontDetachCentering.Backward();
            }
        }
        private void OnMaterialSetStatusChanged_RearDetach(bool obj)
        {
            if (obj)
            {
                _devices.Cylinders.SetCV_RearDetachCentering.Forward();
            }
            else
            {
                _devices.Cylinders.SetCV_RearDetachCentering.Backward();
            }
        }
    }
}
