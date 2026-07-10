using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class AppManualViewModel : MaintenanceViewModel<ESemiSequence, RecipeList>
    {
        protected readonly Processes _processes;
        protected readonly ProcessConfig _processConfig;
        protected readonly PositionList _positionList;
        protected readonly RecipeList _recipeList;
        private uint _selectedInputPage;
        private ObservableCollection<PositionGroup> _teachingPositions;
        private static uint MAX_IO_PER_PAGE = 9;

        private bool FrontStopSW => Devices.Inputs.FrontStopSW.Value;
        private bool RearStopSW => Devices.Inputs.RearStopSW.Value;

        public Devices Devices { get; }

        public AppManualViewModel(NavigationStore navigationStore, MachineStatus machineStatus, Processes processes, PositionList positionList , RecipeList recipeList , Devices devices, ProcessConfig processConfig)
            : base(navigationStore, machineStatus)
        {
            _processes = processes;
            _processConfig = processConfig;
            _positionList = positionList;
            _recipeList = recipeList;
            Devices = devices;
        }

        public ICommand DecreaseIOPageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedInputPage == 0)
                    {
                        return;
                    }
                    else
                    {
                        SelectedInputPage--;
                    }
                });
            }
        }

        public ICommand IncreaseIOPageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedInputPage >= ((Inputs.Count() / MAX_IO_PER_PAGE) - 1))
                    {
                        return;
                    }
                    else
                    {
                        SelectedInputPage++;
                    }
                });
            }
        }

        public uint SelectedInputPage
        {
            get => _selectedInputPage;
            set
            {
                _selectedInputPage = value;
                OnPropertyChanged(nameof(SelectedInputPage));
            }
        }

        public bool IsMaxIOPerPage => Inputs.Count() > MAX_IO_PER_PAGE;


        public ObservableCollection<PositionGroup> TeachingPositions
        {
            get { return _teachingPositions; }
            set { _teachingPositions = value; OnPropertyChanged(); }
        }

        public bool AllDoorClose
        {
            get
            {
                return Devices.Inputs.FrontDoor.Value && Devices.Inputs.RearDoor.Value && Devices.Inputs.RightDoor.Value;
            }
        }

        public bool LightCurtainSensor
        {
            get
            {
                return Devices.Inputs.AreaSensorDetect.Value;
            }
        }

        protected void AddRange<T>(IEnumerable<T> source, ICollection<T> target)
        {
            foreach (var item in source)
                target.Add(item);
        }

        protected override RecipePositionManagerBase<RecipeList> UpdatePositionManager()
        {
            return new RecipePositionManagerBase<RecipeList>(_recipeList,Devices.Motions.All);
        }

        protected ObservableCollection<PositionGroup> GetPositionTeachingList(object selectedProcess)
        {
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
                if(_processConfig.IsTwoConveyor)
                {
                    teachingPositions.Add(_positionList.FilmDetachHead_RearSuctionPos);
                    teachingPositions.Add(_positionList.FilmDetachHead_RearDetachPos);
                }    
               
                teachingPositions.Add(_positionList.FilmDetachHead_ReadyPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontSuctionPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontDetachPos);
            }
            else if (selectedProcess == _processes.CameraAssembleProcess)
            {
                
                    teachingPositions.Add(_positionList.CamHead_ReadyPickPos);
                    teachingPositions.Add(_positionList.CamHead_PickPos);

                    if (_processConfig.IsTwoConveyor)
                    {
                        teachingPositions.Add(_positionList.CamHead_RearReadyPlacePos);
                        teachingPositions.Add(_positionList.CamHead_RearPlace1stPos);
                        teachingPositions.Add(_positionList.CamHead_RearPlace2ndPos);
                        teachingPositions.Add(_positionList.CamHead_RearPrePushInPlacePos);
                    }

                    teachingPositions.Add(_positionList.CamHead_FrontReadyPlacePos);
                    teachingPositions.Add(_positionList.CamHead_FrontPlace1stPos);
                    teachingPositions.Add(_positionList.CamHead_FrontPlace2ndPos);
                    teachingPositions.Add(_positionList.CamHead_FrontPrePushInPlacePos);
                
            }

            return teachingPositions;
        }

        public ICommand PositionMoveCommand => new AsyncRelayCommand<object>(MoveTeachingPosAsync);

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
                if (axis.Status.HwNegLimitDetect || axis.Status.HwPosLimitDetect)
                    throw new Exception("Z Search Hit Limit!!!");
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

            if (pos.Axis.Id == (int)EMotion.FILM_DETACH_Y && (Devices.Cylinders.FilmDetach_MoverUpDn.IsForward))
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }
            if (pos.Axis.Id == (int)EMotion.TRAY_INPUT_Z && (Devices.Cylinders.TraySupplier_TrayCentering1.IsForward || Devices.Cylinders.TraySupplier_TrayCentering2.IsForward || Devices.Cylinders.TrayPicker.IsForward))
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }
            if (pos.Axis.Id == (int)EMotion.TRAY_OUTPUT_Z && Devices.Cylinders.TrayPicker.IsForward)
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }

            if ((pos.Axis.Id == (int)EMotion.TRAY_HEAD_X || pos.Axis.Id == (int)EMotion.TRAY_HEAD_Y) && Devices.Cylinders.TrayPicker.IsForward)
            {
                throw new Exception($"{pos.Axis.Name} Motion Interlock");
            }

            // Add interlock check here
        }
    }
}
