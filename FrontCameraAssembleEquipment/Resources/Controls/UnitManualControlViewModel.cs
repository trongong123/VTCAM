using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Process;
using EQX.Core.Sequence;
using EQX.Core.Units;
using EQX.Device.SpeedController;
using EQX.InOut;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public class UnitManualControlViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public ObservableCollection<BD201SRollerController> Rollers
        {
            get => _rollers;
            set
            {
                _rollers = value;
                OnPropertyChanged(nameof(Rollers));
            }
        }

        public ObservableCollection<ICylinder> Cylinders
        {
            get => _cylinders;
            set
            {
                _cylinders = value;
                OnPropertyChanged(nameof(Cylinders));
            }
        }
        public ObservableCollection<IConveyor> CVs
        {
            get => _conveyors;
            set
            {
                _conveyors = value;
                OnPropertyChanged(nameof(CVs));
            }
        }
        public ObservableCollection<Vaccum> Vaccums
        {
            get => _vaccums;
            set
            {
                _vaccums = value;
                OnPropertyChanged(nameof(Vaccums));
            }
        }

        public bool IsMaterialEditUse => Name == _processes.RearCVSetCamAssembleProcess.Name
                                        || Name == _processes.FrontCVSetCamAssembleProcess.Name
                                        || Name == _processes.RearCVSetFilmDetachProcess.Name
                                        || Name == _processes.FrontCVSetFilmDetachProcess.Name;

        public ObservableCollection<PositionGroup> TeachingPositions
        {
            get { return _teachingPositions; }
            set { _teachingPositions = value; OnPropertyChanged(); }
        }

        public MachineStatus MachineStatus { get; set; }

        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _processes.RootProcess?.Childs?.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
                    if (_devices.Motions.AjinMotions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], false, (string)Application.Current.Resources["str_Confirm"]);
                        return;
                    }

                    if (MachineStatus.OriginDone == false)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"], false);
                        return;
                    }

                    foreach (var item in _processes.RootProcess.Childs!)
                    {
                        if (item.Name == Name)
                        {
                            item.IsOriginOrInitSelected = true;
                        }

                        if (item.Name == EProcess.SpongeDetach.ToString() && item.Name == Name)
                        {
                            _processes.CameraFlipperProcess.IsOriginOrInitSelected = true;
                            _processes.CameraAssembleProcess.IsOriginOrInitSelected = true;
                        }

                        if (item.Name == EProcess.CameraRotator.ToString() && item.Name == Name)
                        {
                            _processes.SpongeDetachProcess.IsOriginOrInitSelected = true;
                            _processes.CameraAssembleProcess.IsOriginOrInitSelected = true;
                        }
                    }

                    _processes.RootProcess.Sequence = ESequence.Ready;

                    foreach (var process in _processes.RootProcess.Childs!)
                    {
                        process.ProcessStatus = EProcessStatus.None;
                        process.Sequence = ESequence.Ready;
                    }

                    _processes.RootProcess.ProcessMode = EProcessMode.ToRun;
                });
            }
        }

        public bool IsTrayOutUnit => Name == _processes.TrayOutElevatorProcess.Name;

        public bool AllDoorClose => _devices.Inputs.FrontDoor.Value &&
                                    _devices.Inputs.RearDoor.Value &&
                                    _devices.Inputs.RightDoor.Value;
        public bool LightCurtainSensor => _devices.Inputs.AreaSensorDetect.Value;
        private bool FrontStopSW => _devices.Inputs.FrontStopSW.Value;
        private bool RearStopSW => _devices.Inputs.RearStopSW.Value;
        public ICommand PositionMoveCommand => new AsyncRelayCommand<object>(MoveTeachingPosAsync);

        public ICommand TrayOutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        public DevRecipe DevRecipe { get; }

        private async Task MoveTeachingPosAsync(object parameter)
        {
            if (parameter is not PositionGroup group) return;
            if (MessageBoxEx.ShowDialog($"Move to [{group.Name}]") == false) return;

            await Task.Run(() =>
            {
                var ctx = new TeachingContext
                {
                    PositionGroup = group,
                    State = ETeachingState.SafetyCheck
                };
                RunTeachingPositionStateMachine(ctx);
            });
        }

        private void RunTeachingPositionStateMachine(TeachingContext ctx)
        {
            while (ctx.State != ETeachingState.Idle)
            {
                try
                {
                    switch (ctx.State)
                    {
                        case ETeachingState.SafetyCheck:
                            if (!AllDoorClose)
                                throw new Exception("Door Open!");

                            if (LightCurtainSensor)
                                throw new Exception("Light Curtain Detect!");

                            ctx.Enumerator = ctx.PositionGroup.Positions.GetEnumerator();
                            if (ctx.PositionGroup.AxisUnit.HasZAxis == true)
                            {
                                ctx.State = ETeachingState.ZReady;
                            }
                            else
                            {
                                ctx.State = ETeachingState.MoveAxisFirst;
                            }
                            break;

                        case ETeachingState.ZReady:
                            var zAxis = ctx.PositionGroup.AxisUnit.AxisList.FirstOrDefault(p => p.Name.Contains("Z"));

                            if (zAxis != null)
                            {
                                zAxis.MoveAbs(0);
                                WaitAxis(zAxis, 0);
                            }

                            ctx.State = ETeachingState.MoveAxisFirst;
                            break;

                        case ETeachingState.MoveAxisFirst:
                            //if (!ctx.Enumerator.MoveNext())
                            //{
                            //    ctx.State = ETeachingState.Done;
                            //    break;
                            //}

                            var xyAxisList = ctx.PositionGroup.Positions.Where(x => x.AxisName.Contains("X") || x.AxisName.Contains("Y")).ToList();
                            if (xyAxisList != null)
                            {
                                foreach (var axis in xyAxisList)
                                {
                                    CheckInterlock(axis);
                                    axis.Axis.MoveAbs(axis.PositionValue);
                                }

                                WaitAxis(xyAxisList);
                            }

                            ctx.State = ETeachingState.MoveAxisSecond;
                            break;

                        case ETeachingState.MoveAxisSecond:

                            var zrxAxisList = ctx.PositionGroup.Positions.Where(x => x.AxisName.Contains("Z") || x.AxisName.Contains("RX")).ToList();

                            if (zrxAxisList != null)
                            {
                                foreach (var axis in zrxAxisList)
                                {
                                    CheckInterlock(axis);
                                    axis.Axis.MoveAbs(axis.PositionValue);
                                }

                                WaitAxis(zrxAxisList);
                            }

                            ctx.State = ETeachingState.Done;
                            break;

                        case ETeachingState.Done:
                            MessageBoxEx.ShowDialog("Move To Teaching Pos Done!", false);
                            ctx.State = ETeachingState.Idle;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ctx.Error = ex.Message;
                    ctx.State = ETeachingState.Error;
                }

                if (ctx.State == ETeachingState.Error)
                {
                    MessageBoxEx.ShowDialog(ctx.Error, false);
                    ctx.State = ETeachingState.Idle;
                }
            }
        }

        private void WaitAxis(IMotion axis, double target)
        {
            var start = DateTime.Now;

            while (true)
            {
                if ((DateTime.Now - start).TotalSeconds > 30)
                    throw new Exception($"{axis.Name} Move Timeout");

                if (!AllDoorClose)
                    throw new Exception("Door Open!!!");

                if (FrontStopSW || RearStopSW)
                    throw new Exception("Stop!!!");

                if (axis.IsOnPosition(target))
                    return;

                Thread.Sleep(5);
            }
        }

        private void WaitAxis(List<Position> positionList)
        {
            var start = DateTime.Now;

            while (true)
            {
                if ((DateTime.Now - start).TotalSeconds > 30)
                    throw new Exception($"Position Move Timeout");

                if (!AllDoorClose)
                    throw new Exception("Door Open!!!");

                if (FrontStopSW || RearStopSW)
                    throw new Exception("Stop!!!");

                if (positionList.Count(m => m.Axis.IsOnPosition(m.PositionValue)) == positionList.Count)
                    return;

                Thread.Sleep(5);
            }
        }

        private void CheckInterlock(Position pos)
        {
            if (!pos.Axis.Status.IsMotionOn)
                throw new Exception($"{pos.Axis.Name} Motion OFF");

            if (pos.Axis.Id == (int)EMotion.FILM_DETACH_Y && (_devices.Cylinders.FilmDetach_MoverUpDn.IsForward))
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }
            if (pos.Axis.Id == (int)EMotion.TRAY_INPUT_Z && (_devices.Cylinders.TraySupplier_TrayCentering1.IsForward || _devices.Cylinders.TraySupplier_TrayCentering2.IsForward || _devices.Cylinders.TrayPicker.IsForward))
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }
            if (pos.Axis.Id == (int)EMotion.TRAY_OUTPUT_Z && _devices.Cylinders.TrayPicker.IsForward)
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }

            if ((pos.Axis.Id == (int)EMotion.TRAY_HEAD_X || pos.Axis.Id == (int)EMotion.TRAY_HEAD_Y) && _devices.Cylinders.TrayPicker.IsForward)
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }

            // Add interlock check here
        }


        public UnitManualControlViewModel(Processes processes, Devices devices , DevRecipe devRecipe)
        {
            _processes = processes;
            _devices = devices;
            DevRecipe = devRecipe;
        }

        // Auto Get Properties(In/Out, Cylinder, Motion, ...) from Process
        public static ObservableCollection<TProp> GetProcessProperties<TProp>(object processInstance)
        {
            var props = processInstance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => typeof(TProp).IsAssignableFrom(p.PropertyType));

            ObservableCollection<TProp> result = new();
            foreach (var prop in props)
            {
                var value = prop.GetValue(processInstance);
                if (value is TProp typedValue)
                {
                    result.Add(typedValue);
                }
            }

            return result;
        }

        public void ControlUnitLoad(IProcess<ESequence> processInstance)
        {
            Name = processInstance.Name;

            LoadProperties(processInstance);
        }

        private void LoadProperties(IProcess<ESequence> processInstance)
        {
            Rollers = GetProcessProperties<BD201SRollerController>(processInstance);
            CVs = GetProcessProperties<IConveyor>(processInstance);
            Cylinders = GetProcessProperties<ICylinder>(processInstance);
            Vaccums = GetProcessProperties<Vaccum>(processInstance);
            TeachingPositions = GetPositionTeachingList(processInstance);
            Name = processInstance.Name;
            MachineStatus = App.AppHost!.Services.GetRequiredService<MachineStatus>();
        }


        private ObservableCollection<PositionGroup> GetPositionTeachingList(IProcess<ESequence> selectedProcess)
        {
            PositionList _positionList = App.AppHost!.Services.GetRequiredService<PositionList>();

            ObservableCollection<PositionGroup> teachingPositions = new();
            if (selectedProcess == _processes.TrayInElevatorProcess)
            {
                teachingPositions.Add(_positionList.TrayInElevator_LimitUpPos);
                teachingPositions.Add(_positionList.TrayInElevator_InputTrayPos);
            }
            else if (selectedProcess == _processes.TrayOutElevatorProcess)
            {
                teachingPositions.Add(_positionList.TrayOutElevator_ReadyPlacePos);
                teachingPositions.Add(_positionList.TrayOutElevator_LimitDownTraySearchPos);
                teachingPositions.Add(_positionList.TrayOutElevator_OutputTrayPos);
            }
            else if (selectedProcess == _processes.TransferHeadProcess)
            {
                teachingPositions.Add(_positionList.TrayHead_CamPickPos);
                teachingPositions.Add(_positionList.TrayHead_CamScanPos);
                teachingPositions.Add(_positionList.TrayHead_CamPlacePos);
                teachingPositions.Add(_positionList.TrayHead_TrayPickPos);
                teachingPositions.Add(_positionList.TrayHead_TrayPlacePos);
                teachingPositions.Add(_positionList.TrayHead_WaitPos);
            }
            else if (selectedProcess == _processes.FilmDetachProcess)
            {
                teachingPositions.Add(_positionList.FilmDetachHead_RearSuctionPos);
                teachingPositions.Add(_positionList.FilmDetachHead_RearDetachPos);
                teachingPositions.Add(_positionList.FilmDetachHead_ReadyPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontSuctionPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontDetachPos);
            }
            else if (selectedProcess == _processes.CameraAssembleProcess)
            {
                teachingPositions.Add(_positionList.CamHead_ReadyPickPos);
                teachingPositions.Add(_positionList.CamHead_PickPos);

                teachingPositions.Add(_positionList.CamHead_FrontReadyPlacePos);
                teachingPositions.Add(_positionList.CamHead_FrontPlace1stPos);
                teachingPositions.Add(_positionList.CamHead_FrontPlace2ndPos);
                teachingPositions.Add(_positionList.CamHead_FrontPrePushInPlacePos);
                teachingPositions.Add(_positionList.CamHead_RearReadyPlacePos);
                teachingPositions.Add(_positionList.CamHead_RearPlace1stPos);
                teachingPositions.Add(_positionList.CamHead_RearPlace2ndPos);
                teachingPositions.Add(_positionList.CamHead_RearPrePushInPlacePos);
            }

            return teachingPositions;
        }


        private readonly Processes _processes;
        private readonly Devices _devices;
        private ObservableCollection<ICylinder> _cylinders;
        private ObservableCollection<IConveyor> _conveyors;
        private ObservableCollection<Vaccum> _vaccums;
        private ObservableCollection<BD201SRollerController> _rollers;
        private ObservableCollection<PositionGroup> _teachingPositions;

    }
}
