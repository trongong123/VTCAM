using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Device.BarCodeScanner;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Process;
using EQX.Core.Sequence;
using EQX.Device.CognexDataMan150X;
using EQX.Motion;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Resources.Controls;
using FrontCameraAssembleEquipment.Vision;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class TeachViewModel : ViewModelBase
    {
        #region Properties
        public Devices Devices { get; }
        public Processes Processes { get; }
        public MachineStatus MachineStatus { get; }
        public RecipeList RecipeList;
        public RecipeSelector RecipeSelector;

        public IBarCodeScanner BarcodeReader
        {
            get => _barcodeReader;
            set
            {
                _barcodeReader = value;
                OnPropertyChanged();
            }
        }

        public VisionProcess VisionProcess { get; }
        public double JogSpeed
        {
            get => _jogSpeed;
            set { _jogSpeed = value; OnPropertyChanged(); }
        }

        public double JogInc
        {
            get => _jogInc;
            set { _jogInc = value; OnPropertyChanged(); }
        }

        public bool IsMoveJog
        {
            get => _isPadMotionView;
            set { _isPadMotionView = value; OnPropertyChanged(); }
        }

        public bool IsMultipleAxisSelection
        {
            get
            {
                return (Motions.Count > 1);
            }
        }
        public bool IsFrontCv
        {
            get => _isFrontCv;
            set
            {
                _isFrontCv = value;
                TeachingPositions = GetPositionTeachingList(SelectedProcess);
                SelectedPropertyProcess();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IProcess<ESequence>> ProcessListTeaching
        {
            get => _processListTeaching;
            set { _processListTeaching = value; OnPropertyChanged(); }
        }
        public ObservableCollection<PositionGroup> TeachingPositions
        {
            get { return _teachingPositions; }
            set { _teachingPositions = value; OnPropertyChanged(); }
        }

        public IProcess<ESequence> SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (_selectedProcess != value)
                {
                    _selectedProcess = value;
                    OnPropertyChanged();
                    SelectedPropertyProcess();
                }
            }
        }

        public string MotionNameSelected
        {
            get => _motionNameSelected;
            set
            {
                _motionNameSelected = value;
                var motion = Motions.Where(m => m.Name == MotionNameSelected).FirstOrDefault();
                if (motion != null)
                {
                    MotionSelected = motion;
                }
                OnPropertyChanged(nameof(MotionNameSelected));
            }
        }

        public PositionGroup SelectedPositionGroup
        {
            get => _selectedPositionGroup;
            set { _selectedPositionGroup = value; OnPropertyChanged(nameof(SelectedPositionGroup)); OnPropertyChanged(nameof(IsTrayHeadPickPos)); OnPropertyChanged(nameof(IsZSearchPos)); }
        }

        public ImageSource UnitImageSource
        {
            get => _unitImageSource;
            set { _unitImageSource = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> Cylinders
        {
            get => _cylinders;
            set { _cylinders = value; OnPropertyChanged(nameof(Cylinders)); }
        }
        public ObservableCollection<IMotion> Motions
        {
            get => _motions;
            set { _motions = value; OnPropertyChanged(nameof(Motions)); }
        }
        public ObservableCollection<IDInput> Inputs
        {
            get => _inputs;
            set { _inputs = value; OnPropertyChanged(nameof(Inputs)); }
        }
        public ObservableCollection<IDOutput> Outputs
        {
            get => _outputs;
            set { _outputs = value; OnPropertyChanged(nameof(Outputs)); }
        }
        public ObservableCollection<Vaccum> Vaccums
        {
            get => _vaccums;
            set { _vaccums = value; OnPropertyChanged(nameof(Vaccums)); }
        }

        public uint XCamTray
        {
            get => _xcamtray;
            set { _xcamtray = value; OnPropertyChanged(nameof(XCamTray)); }
        }
        public uint YCamTray
        {
            get => _ycamtray;
            set { _ycamtray = value; OnPropertyChanged(nameof(YCamTray)); }
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
        #endregion

        #region Constructor
        public TeachViewModel(Devices devices,
            RecipeList recipeList,
            RecipeSelector recipeSelector,
            Processes processes,
            PositionList positionList,
            Motions motionList,
            MachineStatus machineStatus,
            VisionProcess visionProcess,
            IBarCodeScanner barcodeReader)
        {
            Devices = devices;
            RecipeList = recipeList;
            RecipeSelector = recipeSelector;
            Processes = processes;
            _positionList = positionList;

            ProcessListTeaching = GetProcessList();
            SelectedProcess = ProcessListTeaching.FirstOrDefault();
            _motionList = motionList;
            MachineStatus = machineStatus;
            VisionProcess = visionProcess;
            BarcodeReader = barcodeReader;
        }
        #endregion

        #region Privates Methods

        public List<double> IncStepList => new List<double>
        {
            0.001,
            0.010,
            0.1,
            1,
            10,
        };

        public List<string> JogSpeedList => new List<string>
        {
            "Super Slow",
            "Slow",
            "Medium",
            "Fast"
        };

        private List<double> jogSpeedRates = new List<double>
        {
            1.0,
            5.0,
            30.0,
            70.0,
        };
        public double IncStepSelected
        {
            get => _incStepSelected;
            set { _incStepSelected = value; OnPropertyChanged(nameof(IncStepSelected)); }
        }


        public int JogSpeedIndexSelected
        {
            get { return jogSpeedIndexSelected; }
            set { jogSpeedIndexSelected = value; OnPropertyChanged(); }
        }


        private ObservableCollection<IProcess<ESequence>> GetProcessList()
        {
            ObservableCollection<IProcess<ESequence>> processes;
            processes = new ObservableCollection<IProcess<ESequence>>
                {
                    Processes.TrayInElevatorProcess,
                    Processes.TrayOutElevatorProcess,
                    Processes.TransferHeadProcess,
                    Processes.FilmDetachProcess,
                    Processes.CameraAssembleProcess
                };
            return processes;
        }

        public bool IsTrayHeadProcess => (SelectedProcess == Processes.TransferHeadProcess);
        public bool IsCamHeadProcess => (SelectedProcess == Processes.CameraAssembleProcess || SelectedProcess == Processes.FilmDetachProcess);
        public bool IsTrayHeadPickPos => (SelectedPositionGroup == _positionList.TrayHead_CamPickPos);
        public bool IsZSearchPos => (SelectedPositionGroup == _positionList.TrayHead_CamPickPos || SelectedPositionGroup == _positionList.TrayHead_CamPlacePos || SelectedPositionGroup == _positionList.CamHead_PickPos);
        public bool IsCamTrayHeadProcess => (SelectedProcess == Processes.CameraAssembleProcess || SelectedProcess == Processes.TransferHeadProcess);

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

        public ObservableCollection<ESemiSequence> LoadSemiSequence(object selectedProcess)
        {
            ObservableCollection<ESemiSequence> semiSequences;
            if (selectedProcess == Processes.FilmDetachProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.Detach_FilmDetach,
                };
            }
            else if (selectedProcess == Processes.TrayInElevatorProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.TraySearch,
                };
            }
            else if (selectedProcess == Processes.TrayOutElevatorProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.TraySearch,
                };
            }
            else if (selectedProcess == Processes.TransferHeadProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.TrayHead_Tray_Pick,
                    ESemiSequence.TrayHead_Tray_Place,
                    ESemiSequence.TrayHead_Cam_Pick,
                    ESemiSequence.TrayHead_Cam_Place,
                };
            }
            else if (selectedProcess == Processes.SpongeDetachProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.SpongeDetach_RemoveSponge,
                };
            }
            else if (selectedProcess == Processes.CameraFlipperProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.Flipper_Pick,
                };
            }
            else if (selectedProcess == Processes.CameraAssembleProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.CamHead_Pick,
                    ESemiSequence.CamHead_Place,
                };
            }
            else if (selectedProcess == Processes.FrontCVSetUnloadProcess || selectedProcess == Processes.RearCVSetUnloadProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.CVOut_Load,
                    ESemiSequence.CVOut_Unload,
                };
            }
            else if (selectedProcess == Processes.FrontCVSetCamAssembleProcess || selectedProcess == Processes.RearCVSetCamAssembleProcess)
            {
                semiSequences = new()
                {
                    ESemiSequence.CVAssemble_Load,
                };
            }
            else
            {
                semiSequences = new();
            }

            return semiSequences;
        }

        public ObservableCollection<ESemiSequence> SemiAutoSequences
        {
            get => _semiAutoSequence;
            set { _semiAutoSequence = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> MotionNameList
        {
            get => _motionNameList;
            set { _motionNameList = value; OnPropertyChanged(nameof(MotionNameList)); }
        }

        private ObservableCollection<PositionGroup> GetPositionTeachingList(object selectedProcess)
        {
            ObservableCollection<PositionGroup> teachingPositions = new();
            if (selectedProcess == Processes.TrayInElevatorProcess)
            {
                teachingPositions.Add(_positionList.TrayInElevator_LimitUpPos);
                teachingPositions.Add(_positionList.TrayInElevator_InputTrayPos);
            }
            else if (selectedProcess == Processes.TrayOutElevatorProcess)
            {
                teachingPositions.Add(_positionList.TrayOutElevator_OutputTrayPos);
                teachingPositions.Add(_positionList.TrayOutElevator_ReadyPlacePos);
                teachingPositions.Add(_positionList.TrayOutElevator_LimitDownTraySearchPos);

            }
            else if (selectedProcess == Processes.TransferHeadProcess)
            {
                teachingPositions.Add(_positionList.TrayHead_CamPickPos);
                teachingPositions.Add(_positionList.TrayHead_CamScanPos);
                teachingPositions.Add(_positionList.TrayHead_CamPlacePos);
                teachingPositions.Add(_positionList.TrayHead_TrayPickPos);
                teachingPositions.Add(_positionList.TrayHead_TrayPlacePos);
                teachingPositions.Add(_positionList.TrayHead_WaitPos);
            }
            else if (selectedProcess == Processes.FilmDetachProcess)
            {
                teachingPositions.Add(_positionList.FilmDetachHead_RearSuctionPos);
                teachingPositions.Add(_positionList.FilmDetachHead_RearDetachPos);
                //teachingPositions.Add(_positionList.FilmDetachHead_RearCleanPos);
                teachingPositions.Add(_positionList.FilmDetachHead_ReadyPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontSuctionPos);
                teachingPositions.Add(_positionList.FilmDetachHead_FrontDetachPos);
                //teachingPositions.Add(_positionList.FilmDetachHead_FrontCleanPos);
            }
            else if (selectedProcess == Processes.CameraAssembleProcess)
            {
                teachingPositions.Add(_positionList.CamHead_ReadyPickPos);
                teachingPositions.Add(_positionList.CamHead_PickPos);

                teachingPositions.Add(_positionList.CamHead_RearReadyPlacePos);
                teachingPositions.Add(_positionList.CamHead_RearPlace1stPos);
                teachingPositions.Add(_positionList.CamHead_RearPlace2ndPos);
                teachingPositions.Add(_positionList.CamHead_RearPrePushInPlacePos);

                teachingPositions.Add(_positionList.CamHead_FrontReadyPlacePos);
                teachingPositions.Add(_positionList.CamHead_FrontPlace1stPos);
                teachingPositions.Add(_positionList.CamHead_FrontPlace2ndPos);
                teachingPositions.Add(_positionList.CamHead_FrontPrePushInPlacePos);
            }

            return teachingPositions;
        }

        private void SelectedPropertyProcess()
        {
            if (SelectedProcess == null)
                return;

            if (Enum.TryParse<EProcess>(SelectedProcess.Name, out var procType)
                && _processImages.TryGetValue(procType, out var resourceKey))
            {
                UnitImageSource = (ImageSource)Application.Current.FindResource(resourceKey);
            }

            Cylinders = GetProcessProperties<ICylinder>(SelectedProcess);
            Motions = GetProcessProperties<IMotion>(SelectedProcess);
            Inputs = GetProcessProperties<IDInput>(SelectedProcess);
            Outputs = GetProcessProperties<IDOutput>(SelectedProcess);
            Vaccums = GetProcessProperties<Vaccum>(SelectedProcess);
            TeachingPositions = GetPositionTeachingList(SelectedProcess);
            SemiAutoSequences = LoadSemiSequence(SelectedProcess);
            SelectedPositionGroup = TeachingPositions.FirstOrDefault();
            MotionNameSelected = Motions.FirstOrDefault().Name;

            if (SelectedProcess == Processes.CameraAssembleProcess)
            {
                Cylinders.Add(Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn);
                Cylinders.Add(Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorGripper);
                Cylinders.Add(Devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper);
            }

            var cylFilmHeadExtendList = IsFrontCv ? GetProcessProperties<ICylinder>(Processes.FrontCVSetFilmDetachProcess)
                                            : GetProcessProperties<ICylinder>(Processes.RearCVSetFilmDetachProcess);
            if (SelectedProcess == Processes.FilmDetachProcess)
            {
                foreach (var cyl in cylFilmHeadExtendList)
                {
                    Cylinders.Add(cyl);
                }
            }

            ObservableCollection<string> motionNameList = new ObservableCollection<string>();
            foreach (var motion in Motions)
            {
                motionNameList.Add(motion.Name);
            }
            MotionNameList = motionNameList;

            OnPropertyChanged(nameof(IsTrayHeadProcess));
            OnPropertyChanged(nameof(IsCamHeadProcess));
            OnPropertyChanged(nameof(IsCamTrayHeadProcess));
            OnPropertyChanged(nameof(IsMultipleAxisSelection));

            RecipeSelector.Load();
        }

        public IMotion MotionSelected
        {
            get => _motionSelected;
            set
            {
                _motionSelected = value;
                OnPropertyChanged(nameof(MotionSelected));
            }
        }
        #endregion

        #region Commands
        public ICommand SemiAutoCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (Devices.Motions.All.Count(motion => motion.Status.IsMotionOn != true) > 0)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_TurnOnAllMotionsFirst"], (string)Application.Current.Resources["str_Confirm"]);
                        return;
                    }

                    if (MachineStatus.OriginDone == false)
                    {
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_MachineNeedToBeOriginBeforeRun"]);
                        return;
                    }

                    if (o is ESemiSequence sequence == false) return;

                    MachineStatus.OPCommand = EOperationCommand.SemiAuto;
                    MachineStatus.SemiAutoSequence = sequence;
                });
            }
        }

        public ICommand SaveTargetCommand => new RelayCommand(SaveTargetPosition);
        private void SaveTargetPosition()
        {
            if (MessageBoxEx.ShowDialog($"{(string)Application.Current.Resources["str_Save"]}?") == true)
            {
                RecipeSelector.Save();
            }
        }

        public ICommand SaveCurrentCommand => new RelayCommand(SaveCurrentPosition);
        private void SaveCurrentPosition()
        {

            if (MessageBoxEx.ShowDialog($"{(string)Application.Current.Resources["str_Save"]}?") == true)
            {
                foreach (var position in SelectedPositionGroup.Positions)
                {
                    if (SelectedPositionGroup.AxisUnit.HasZAxis == true
                        && position.AxisName.Contains("Z")
                        && position.RecipePropertyPath.Contains("Ready")
                        && position.Axis.Status.ActualPosition > 10)
                    {
                        MessageBoxEx.ShowDialog($"Z Ready Pos must be smaller than 10 mm");
                        return;
                    }
                }
                foreach (var position in SelectedPositionGroup.Positions)
                {
                    position.SetCurrentPos();
                }
                RecipeSelector.Save();
            }
        }
    
        public ICommand JogCommand => new RelayCommand<object>(JogMove);
        private void JogMove(object parameter)
        {
            if (Motions == null || Motions.Count == 0) { return; }
            var motionName = parameter.ToString().Split('_').First();
            var motionDir = parameter.ToString().Split('_').Last();
            var direction = (motionDir == "Inc" ? true : false);

            var motion = Motions.FirstOrDefault(m => m.Name.Split('_').Last() == motionName);
            if ((motion.Id == (int)EMotion.TRAY_INPUT_Z) || (motion.Id == (int)EMotion.TRAY_OUTPUT_Z))
            {
                direction = !direction;
            }

            int incDir = direction ? 1 : -1;

            if (motion != null)
            {
                if (IsMoveJog == false)
                {
                    motion.MoveInc(IncStepSelected * incDir);
                }
                else
                {
                    motion.MoveJog(jogSpeedRates[JogSpeedIndexSelected], direction);
                }
            }
        }

        public ICommand SetJogSpeedCommand => new RelayCommand<object>(JogSpeedSet);
        private void JogSpeedSet(object parameter)
        {
            if (double.TryParse(parameter.ToString(), out double speed))
            {
                JogSpeed = speed;
            }
        }

        public ICommand StopCommand => new RelayCommand<object>(StopMove);
        private void StopMove(object parameter)
        {
            if (Motions == null || Motions.Count == 0) { return; }
            var motionName = parameter.ToString();
            var motion = Motions.FirstOrDefault(m => m.Name.Split('_').Last() == motionName);
            if (IsMoveJog == false) return;
            if (motion != null)
            {
                motion.Stop();
            }
        }

        public ICommand StopMoveTeachingPosCommand => new RelayCommand<object>(StopMoveTeachingPos);
        private void StopMoveTeachingPos(object parameter)
        {
            foreach (var motion in Motions)
            {
                motion.Stop();
            }
        }
        public ICommand SearchZPosCommand => new AsyncRelayCommand<object>(SearchZPos);
        private async Task SearchZPos(object parameter)
        {

            if (SelectedPositionGroup == null) return;
            await Task.Run(() =>
            {
                var ctx = new TeachingContext
                {
                    PositionGroup = SelectedPositionGroup,
                    Status = ESearchZPos.SafetyCheck
                };
                RunSearchingZPosition(ctx);

            });
        }
        private void RunSearchingZPosition(TeachingContext  ctx)
        {
            while (ctx.Status != ESearchZPos.Idle )
            {
                try
                {
                    switch (ctx.Status)
                    {
                        case ESearchZPos.SafetyCheck:
                            if (!AllDoorClose)
                                throw new Exception("Door Open!");

                            if (LightCurtainSensor)
                                throw new Exception("Light Curtain Detect!");
                            ctx.Status = ESearchZPos.ZReady;
                            break;
                        case ESearchZPos.ZReady:
                            var zAxis = ctx.PositionGroup.AxisUnit.AxisList.FirstOrDefault(p => p.Name.Contains("Z"));

                            if (zAxis != null)
                            {
                                zAxis.MoveAbs(0);
                                WaitAxis(zAxis, 0);
                            }

                            ctx.Status = ESearchZPos.MoveXYAxis;
                            break;
                        case ESearchZPos.MoveXYAxis:                          

                            var xyAxisList = ctx.PositionGroup.Positions.Where(x => (x.AxisName.Contains("X") || x.AxisName.Contains("Y")) && (x.IsSelected)).ToList();
                            if (xyAxisList != null)
                            {
                                foreach (var axis in xyAxisList)
                                {
                                    CheckInterlock(axis);
                                    axis.Axis.MoveAbs(axis.PositionValue);
                                }

                                WaitAxis(xyAxisList);
                            }
                            ctx.Status = ESearchZPos.MoveZAxis;
                            break;
                        case ESearchZPos.MoveZAxis:
                            var zAxisSearch = ctx.PositionGroup.AxisUnit.AxisList.FirstOrDefault(p => p.Name.Contains("Z"));

                            if (zAxisSearch != null)
                            {
                                WaitSearchZPos(zAxisSearch);
                            }
                            ctx.Status = ESearchZPos.Done;
                            break;
                        case ESearchZPos.Done:
                            if (MessageBoxEx.ShowDialog($"Search Z Pos {ctx.PositionGroup.Name.ToString()} Done!\r\n{(string)Application.Current.Resources["str_Save"]}?") == true)
                            {
                                foreach (var position in SelectedPositionGroup.Positions)
                                {
                                    position.SetCurrentPos();
                                }
                                RecipeSelector.Save();
                            }
                            ctx.Status = ESearchZPos.Idle;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ctx.Error = ex.Message;
                    ctx.Status = ESearchZPos.Error;
                }

                if (ctx.Status == ESearchZPos.Error)
                {
                    MessageBoxEx.ShowDialog(ctx.Error, false);
                    ctx.Status = ESearchZPos.Idle;
                }
            }
        }
        public ICommand MoveCommand => new AsyncRelayCommand<object>(MoveTeachingPosAsync);

        private async Task MoveTeachingPosAsync(object parameter)
        {
            if (SelectedPositionGroup == null) return;
            await Task.Run(() =>
            {
                var ctx = new TeachingContext
                {
                    PositionGroup = SelectedPositionGroup,
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

                            ctx.Enumerator = ctx.PositionGroup.Positions.Where(a => a.IsSelected).ToList().GetEnumerator();
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

                            var xyAxisList = ctx.PositionGroup.Positions.Where(x => (x.AxisName.Contains("X") || x.AxisName.Contains("Y")) && (x.IsSelected)).ToList();
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

                            var zrxAxisList = ctx.PositionGroup.Positions.Where(x => (x.AxisName.Contains("Z") || x.AxisName.Contains("RX")) && (x.IsSelected)).ToList();

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
                if ((DateTime.Now - start).TotalSeconds > 60)
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
        private void WaitSearchZPos(IMotion axis)
        {
            var start = DateTime.Now;
            bool  ret1 = false;
            while (true)
            {
                if ((DateTime.Now - start).TotalSeconds > 40)
                    throw new Exception($"{axis.Name} Move Timeout");

                if (!AllDoorClose)
                    throw new Exception("Door Open!!!");

                if (FrontStopSW || RearStopSW )
                    throw new Exception("Stop!!!");
                if (axis.Status.HwNegLimitDetect || axis.Status.HwPosLimitDetect)
                    throw new Exception("Z Search Hit Limit!!!");

                axis.MoveJog(5,true);

                if (axis.Id ==  (int)EMotion.TRAY_HEAD_Z)
                {
                    
                    if (VtCamSupplyPnPOverload == false)
                    {
                        axis.Stop();
                        return;
                    }
                }
                if (axis.Id == (int)EMotion.CAM_HEAD_Z)
                {
                    
                    if (VtCamAssemblePnPOverload == false)
                    {
                        axis.Stop();
                        return;
                    }
                }

                Thread.Sleep(5);
            }
        }

        private void CheckInterlock(Position pos)
        {
            if (!pos.Axis.Status.IsMotionOn)
                throw new Exception($"{pos.Axis.Name} Motion OFF");

            if (pos.Axis.Id == (int)EMotion.FILM_DETACH_Y
                && (Devices.Cylinders.FilmDetach_MoverUpDn.IsForward))
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
        public ICommand MoveContiCommand => new AsyncRelayCommand<object>(MoveContiTeachingPosAsync);
        public ICommand ZReadyMoveCommand => new AsyncRelayCommand<object>(MoveZReadyAsync);

        private async Task MoveZReadyAsync(object parameter)
        {
            if (SelectedPositionGroup == null) return;
            await Task.Run(() =>
            {
                var ctx = new TeachingContext
                {
                    PositionGroup = SelectedPositionGroup,
                    State = ETeachingState.SafetyCheck
                };
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

                                ctx.Enumerator = ctx.PositionGroup.Positions.Where(a => a.IsSelected).ToList().GetEnumerator();
                                if (ctx.PositionGroup.AxisUnit.HasZAxis == true)
                                {
                                    ctx.State = ETeachingState.ZReady;
                                }
                                else
                                {
                                    ctx.State = ETeachingState.Done;
                                }
                                break;

                            case ETeachingState.ZReady:
                                var zAxis = ctx.PositionGroup.AxisUnit.AxisList.FirstOrDefault(p => p.Name.Contains("Z"));

                                if (zAxis != null)
                                {
                                    zAxis.MoveAbs(0);
                                    WaitAxis(zAxis, 0);
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

            });
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
        private async Task MoveContiTeachingPosAsync(object parameter)
        {
            if (AllDoorClose == false)
            {
                MessageBoxEx.ShowDialog("WARNING: Door Open!");
                return;
            }

            if (LightCurtainSensor == true)
            {
                MessageBoxEx.ShowDialog("WARNING: Light Curtain Detect!");
                return;
            }
            await Task.Run(() =>
            {
                if (SelectedPositionGroup == null) return;
                var selectedPosList = SelectedPositionGroup.Positions.Where(x => x.IsSelected).ToList();
                if (selectedPosList.Count < 2)
                {
                    MessageBoxEx.ShowDialog("Axis Selected need more than one Axis!!!");
                    return;
                }
                uint coordinate = 1;
                int[] axisArr = new int[selectedPosList.Count];
                double[] posArr = new double[selectedPosList.Count];

                for (int i = 0; i < selectedPosList.Count; i++)
                {
                    axisArr[i] = (int)selectedPosList[i].Axis.Id;
                }
                for (int i = 0; i < selectedPosList.Count; i++)
                {
                    posArr[i] = (double)selectedPosList[i].PositionValue;
                }

                AXM.AxmContiWriteClear((int)coordinate);
                AXM.AxmContiSetAxisMap((int)coordinate, (uint)selectedPosList.Count, axisArr);
                AXM.AxmContiSetAbsRelMode((int)coordinate, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);
                AXM.AxmContiBeginNode((int)coordinate);
                AXM.AxmLineMove((int)coordinate, posArr, 200, 0.25, 0.25);

                AXM.AxmContiEndNode((int)coordinate);
                AXM.AxmContiStart((int)coordinate, 0, 0);
                while (true)
                {
                    uint inMotion = 0;
                    uint ret = AXM.AxmContiIsMotion((int)coordinate, ref inMotion);
                    if (inMotion == 0) break;
                    Task.Delay(10);
                }

            });

            MessageBoxEx.ShowDialog("Move To Teaching Pos Done!");
        }
        #endregion

        #region Privates
        private ObservableCollection<ICylinder> _cylinders;
        private ObservableCollection<IMotion> _motions;
        private ObservableCollection<IDOutput> _outputs;
        private ObservableCollection<IDInput> _inputs;
        private PositionList _positionList;
        private PositionGroup _selectedPositionGroup;
        private ImageSource _unitImageSource;
        private Motions _motionList;
        private ObservableCollection<PositionGroup> _teachingPositions;
        private ObservableCollection<ESemiSequence> _semiAutoSequence;
        private ObservableCollection<IProcess<ESequence>> _processListTeaching;
        private ObservableCollection<string> _motionNameList;
        private double _incStepSelected;
        private IMotion _motionSelected;
        private IProcess<ESequence> _selectedProcess;
        private double _jogSpeed = 20;
        private double _jogInc = 1.0;
        private bool _isPadMotionView = true;
        private bool _isFrontCv;
        private ObservableCollection<Vaccum> _vaccums;
        private uint _xcamtray = 1;
        private uint _ycamtray = 1;
        private IBarCodeScanner _barcodeReader;
        private int jogSpeedIndexSelected = 0;
        private string _motionNameSelected;
        private bool FrontStopSW => Devices.Inputs.FrontStopSW.Value;
        private bool RearStopSW => Devices.Inputs.RearStopSW.Value;
        private bool VtCamAssemblePnPOverload => Devices.Inputs.VtCamAssemblePnPOverload.Value;
        private bool VtCamSupplyPnPOverload => Devices.Inputs.VtCamSupplyPnPOverload.Value;

        private readonly Dictionary<EProcess, string> _processImages = new()
        {
            { EProcess.TrayInElevator, "image_tray_in_elevator" },
            { EProcess.TrayOutElevator, "image_tray_out_elevator" },
            { EProcess.TrayHead, "image_transfer_head" },
            { EProcess.FilmDetach, "image_film_detach" },
            { EProcess.CameraAssemble, "image_cam_assemble" },
        };
        #endregion
    }


}
