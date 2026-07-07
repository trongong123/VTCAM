using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.Device.CognexDataMan150X;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using OpenCvSharp.Tracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UnitTeachControlView.xaml
    /// </summary>
    public partial class UnitTeachControlView : UserControl
    {

        public ObservableCollection<IMotion> Motions
        {
            get { return (ObservableCollection<IMotion>)GetValue(MotionsProperty); }
            set { SetValue(MotionsProperty, value); }
        }
        public static readonly DependencyProperty MotionsProperty =
            DependencyProperty.Register("Motions", typeof(ObservableCollection<IMotion>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<IMotion> { }));

        public IMotion MotionSelected
        {
            get { return (IMotion)GetValue(MotionSelectedProperty); }
            set { SetValue(MotionSelectedProperty, value); }
        }
        public static readonly DependencyProperty MotionSelectedProperty =
            DependencyProperty.Register(nameof(MotionSelected), typeof(IMotion), typeof(UnitTeachControlView));

        public string MotionNameSelected
        {
            get { return (string)GetValue(MotionNameSelectedProperty); }
            set { SetValue(MotionNameSelectedProperty, value); }
        }
        public static readonly DependencyProperty MotionNameSelectedProperty =
            DependencyProperty.Register(nameof(MotionNameSelected), typeof(string), typeof(UnitTeachControlView));

        public ObservableCollection<string> MotionNameList
        {
            get { return (ObservableCollection<string>)GetValue(MotionNameListProperty); }
            set { SetValue(MotionNameListProperty, value); }
        }
        public static readonly DependencyProperty MotionNameListProperty =
            DependencyProperty.Register(nameof(MotionNameList), typeof(ObservableCollection<string>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<string> { }));

        public ObservableCollection<ICylinder> Cylinders
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersProperty); }
            set { SetValue(CylindersProperty, value); }
        }
        public static readonly DependencyProperty CylindersProperty =
            DependencyProperty.Register("Cylinders", typeof(ObservableCollection<ICylinder>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IDOutput> Outputs
        {
            get { return (ObservableCollection<IDOutput>)GetValue(OutputsProperty); }
            set { SetValue(OutputsProperty, value); }
        }
        public static readonly DependencyProperty OutputsProperty =
            DependencyProperty.Register("Outputs", typeof(ObservableCollection<IDOutput>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<IDOutput> { }));
        public ObservableCollection<IDInput> Inputs
        {
            get { return (ObservableCollection<IDInput>)GetValue(InputsProperty); }
            set { SetValue(InputsProperty, value); }
        }
        public static readonly DependencyProperty InputsProperty =
            DependencyProperty.Register("Inputs", typeof(ObservableCollection<IDInput>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<IDInput> { }));
        public ObservableCollection<ESemiSequence> SemiAutoSequences
        {
            get { return (ObservableCollection<ESemiSequence>)GetValue(SemiAutoSequencesProperty); }
            set { SetValue(SemiAutoSequencesProperty, value); }
        }
        public static readonly DependencyProperty SemiAutoSequencesProperty =
            DependencyProperty.Register("SemiAutoSequences", typeof(ObservableCollection<ESemiSequence>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<ESemiSequence> { }));
        public ICommand SemiAutoCommand
        {
            get { return (ICommand)GetValue(SemiAutoCommandCommandProperty); }
            set { SetValue(SemiAutoCommandCommandProperty, value); }
        }
        public static readonly DependencyProperty SemiAutoCommandCommandProperty =
            DependencyProperty.Register("SemiAutoCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());

        public ImageSource UnitImage
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty SelectedPositionGroupProperty =
            DependencyProperty.Register("SelectedPositionGroup", typeof(PositionGroup), typeof(UnitTeachControlView), new PropertyMetadata(null)); 
        public PositionGroup SelectedPositionGroup
        {
            get { return (PositionGroup)GetValue(SelectedPositionGroupProperty); }
            set { SetValue(SelectedPositionGroupProperty, value); }
        }

        public static readonly DependencyProperty XCamTrayProperty =
            DependencyProperty.Register("XCamTray", typeof(uint), typeof(UnitTeachControlView), new PropertyMetadata(null)); 
        public uint XCamTray
        {
            get { return (uint)GetValue(XCamTrayProperty); }
            set { SetValue(XCamTrayProperty, value); }
        }

        public static readonly DependencyProperty YCamTrayProperty =
            DependencyProperty.Register("YCamTray", typeof(uint), typeof(UnitTeachControlView), new PropertyMetadata(null)); 
        public uint YCamTray
        {
            get { return (uint)GetValue(YCamTrayProperty); }
            set { SetValue(YCamTrayProperty, value); }
        }

        public static readonly DependencyProperty BarcodeReaderProperty =
            DependencyProperty.Register("BarcodeReader", typeof(BarCodeScannerBase), typeof(UnitTeachControlView), new PropertyMetadata(null)); 
        public BarCodeScannerBase BarcodeReader
        {
            get { return (BarCodeScannerBase)GetValue(BarcodeReaderProperty); }
            set { SetValue(BarcodeReaderProperty, value); }
        }

        public ObservableCollection<PositionGroup> TeachingPositions
        {
            get { return (ObservableCollection<PositionGroup>)GetValue(PositionTeachingsProperty); }
            set { SetValue(PositionTeachingsProperty, value); }
        }
        public static readonly DependencyProperty PositionTeachingsProperty =
            DependencyProperty.Register("TeachingPositions", typeof(ObservableCollection<PositionGroup>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<PositionGroup> { }));
        
        public ICommand SaveTargetCommand
        {
            get { return (ICommand)GetValue(SaveTargetCommandProperty); }
            set { SetValue(SaveTargetCommandProperty, value); }
        }
        public static readonly DependencyProperty SaveTargetCommandProperty =
            DependencyProperty.Register("SaveTargetCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());

        public ICommand SaveCurrentCommand
        {
            get { return (ICommand)GetValue(SaveCurrentCommandProperty); }
            set { SetValue(SaveCurrentCommandProperty, value); }
        }
        public static readonly DependencyProperty SaveCurrentCommandProperty =
            DependencyProperty.Register("SaveCurrentCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand StopMoveTeachingPosCommand
        {
            get { return (ICommand)GetValue(StopMoveTeachingPosCommandProperty); }
            set { SetValue(StopMoveTeachingPosCommandProperty, value); }
        }
        public static readonly DependencyProperty StopMoveTeachingPosCommandProperty =
            DependencyProperty.Register("StopMoveTeachingPosCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand SetJogSpeedCommand
        {
            get { return (ICommand)GetValue(SetJogSpeedCommandProperty); }
            set { SetValue(SetJogSpeedCommandProperty, value); }
        }
        public static readonly DependencyProperty SetJogSpeedCommandProperty =
            DependencyProperty.Register(nameof(SetJogSpeedCommand), typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());

        public bool IsFrontCv
        {
            get { return (bool)GetValue(IsFrontCvProperty); }
            set { SetValue(IsFrontCvProperty, value); }
        }
        public static readonly DependencyProperty IsFrontCvProperty =
            DependencyProperty.Register(nameof(IsFrontCv), typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

       
        public ICommand JogCommand
        {
            get { return (ICommand)GetValue(JogCommandProperty); }
            set { SetValue(JogCommandProperty, value); }
        }
        public static readonly DependencyProperty JogCommandProperty =
            DependencyProperty.Register("JogCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand MoveContiCommand
        {
            get { return (ICommand)GetValue(MoveContiCommandProperty); }
            set { SetValue(MoveContiCommandProperty, value); }
        }
        public static readonly DependencyProperty MoveContiCommandProperty =
            DependencyProperty.Register("MoveContiCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand StopCommand
        {
            get { return (ICommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }
        public static readonly DependencyProperty StopCommandProperty =
            DependencyProperty.Register("StopCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand SearchZPosCommand
        {
            get { return (ICommand)GetValue(SearchZPosCommandProperty); }
            set { SetValue(SearchZPosCommandProperty, value); }
        }
        public static readonly DependencyProperty SearchZPosCommandProperty =
            DependencyProperty.Register("SearchZPosCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand MoveCommand
        {
            get { return (ICommand)GetValue(MoveCommandProperty); }
            set { SetValue(MoveCommandProperty, value); }
        }
        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register("MoveCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ICommand ZReadyMoveCommand
        {
            get { return (ICommand)GetValue(ZReadyMoveCommandProperty); }
            set { SetValue(ZReadyMoveCommandProperty, value); }
        }
        public static readonly DependencyProperty ZReadyMoveCommandProperty =
            DependencyProperty.Register("ZReadyMoveCommand", typeof(ICommand), typeof(UnitTeachControlView), new PropertyMetadata());
        public ObservableCollection<Vaccum> Vaccums
        {
            get { return (ObservableCollection<Vaccum>)GetValue(VaccumsProperty); }
            set { SetValue(VaccumsProperty, value); }
        }
        public static readonly DependencyProperty VaccumsProperty =
            DependencyProperty.Register("Vaccums", typeof(ObservableCollection<Vaccum>), typeof(UnitTeachControlView), new PropertyMetadata(new ObservableCollection<Vaccum> { }));
        public List<double> IncStepList
        {
            get { return (List<double>)GetValue(IncStepListProperty); }
            set { SetValue(IncStepListProperty, value); }
        }
        public static readonly DependencyProperty IncStepListProperty =
            DependencyProperty.Register(nameof(IncStepList), typeof(List<double>), typeof(UnitTeachControlView), new PropertyMetadata(new List<double> { }));
        public double IncStepSelected
        {
            get { return (double)GetValue(IncStepSelectedProperty); }
            set { SetValue(IncStepSelectedProperty, value); }
        }
        public static readonly DependencyProperty IncStepSelectedProperty =
            DependencyProperty.Register(nameof(IncStepSelected), typeof(double), typeof(UnitTeachControlView), new PropertyMetadata(new double { }));

        public List<string> JogSpeedList
        {
            get { return (List<string>)GetValue(JogSpeedListProperty); }
            set { SetValue(JogSpeedListProperty, value); }
        }
        // Using a DependencyProperty as the backing store for JogSpeedList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JogSpeedListProperty =
            DependencyProperty.Register("JogSpeedList", typeof(List<string>), typeof(UnitTeachControlView), new PropertyMetadata(null));

        public int JogSpeedIndexSelected
        {
            get { return (int)GetValue(JogSpeedIndexSelectedProperty); }
            set { SetValue(JogSpeedIndexSelectedProperty, value); }
        }
        // Using a DependencyProperty as the backing store for JogSpeedIndexSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JogSpeedIndexSelectedProperty =
            DependencyProperty.Register("JogSpeedIndexSelected", typeof(int), typeof(UnitTeachControlView), new PropertyMetadata(0));

        public double JogSpeed
        {
            get { return (double)GetValue(JogSpeedProperty); }
            set { SetValue(JogSpeedProperty, value); }
        }
        public static readonly DependencyProperty JogSpeedProperty =
            DependencyProperty.Register(nameof(JogSpeed),
                                        typeof(double),
                                        typeof(UnitTeachControlView),
                                        new PropertyMetadata(0.0));
        public double JogInc
        {
            get { return (double)GetValue(JogIncProperty); }
            set { SetValue(JogIncProperty, value); }
        }
        public static readonly DependencyProperty JogIncProperty =
            DependencyProperty.Register(nameof(JogInc),
                                        typeof(double),
                                        typeof(UnitTeachControlView),
                                        new PropertyMetadata(1.0));

        public bool IsMoveJog
        {
            get { return (bool)GetValue(IsPadMotionViewProperty); }
            set { SetValue(IsPadMotionViewProperty, value); }
        }
        public static readonly DependencyProperty IsPadMotionViewProperty =
            DependencyProperty.Register("IsMoveJog", typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());
        public bool IsMultipleAxisSelection
        {
            get { return (bool)GetValue(IsMultipleAxisSelectionProperty); }
            set { SetValue(IsMultipleAxisSelectionProperty, value); }
        }
        public static readonly DependencyProperty IsMultipleAxisSelectionProperty =
            DependencyProperty.Register(nameof(IsMultipleAxisSelection), typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

        public bool IsCamTrayHeadProcess
        {
            get { return (bool)GetValue(IsCamTrayHeadProcessProperty); }
            set { SetValue(IsCamTrayHeadProcessProperty, value); }
        }
        public static readonly DependencyProperty IsCamTrayHeadProcessProperty =
            DependencyProperty.Register(nameof(IsCamTrayHeadProcess), typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

        public bool IsTrayHeadProcess
        {
            get { return (bool)GetValue(IsTrayHeadProcessProperty); }
            set { SetValue(IsTrayHeadProcessProperty, value); }
        }
        public static readonly DependencyProperty IsTrayHeadProcessProperty =
            DependencyProperty.Register("IsTrayHeadProcess", typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());
        public bool IsCamHeadProcess
        {
            get { return (bool)GetValue(IsCamHeadProcessProperty); }
            set { SetValue(IsCamHeadProcessProperty, value); }
        }
        public static readonly DependencyProperty IsCamHeadProcessProperty =
            DependencyProperty.Register(nameof(IsCamHeadProcess), typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

        public bool IsTrayHeadPickPos
        {
            get { return (bool)GetValue(IsTrayHeadPickPosProperty); }
            set { SetValue(IsTrayHeadPickPosProperty, value); }
        }
        public static readonly DependencyProperty IsTrayHeadPickPosProperty =
           DependencyProperty.Register("IsTrayHeadPickPos", typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

        public bool IsZSearchPos
        {
            get { return (bool)GetValue(IsZSearchPosProperty); }
            set { SetValue(IsZSearchPosProperty, value); }
        }
        public static readonly DependencyProperty IsZSearchPosProperty =
            DependencyProperty.Register("IsZSearchPos", typeof(bool), typeof(UnitTeachControlView), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("UnitImage", typeof(ImageSource), typeof(UnitTeachControlView), new PropertyMetadata(null));

        public UnitTeachControlView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            double.TryParse(textBox.Text , out double currentValue);

            var minMaxAttribute = new SingleRecipeMinMaxAttribute
            {
                Min = -100.0,
                Max = 10,
            };

            DataEditor dataEditor;

            if (((Position)textBox.DataContext).AxisName.Contains("Z")
                && ((Position)textBox.DataContext).RecipePropertyPath.Contains("Ready")
                && (((Position)textBox.DataContext).RecipePropertyPath.Contains("CameraHead") || ((Position)textBox.DataContext).RecipePropertyPath.Contains("TrayHead")))
            {
                dataEditor = new(currentValue, minMaxAttribute);
            }
            else
            {
                dataEditor = new(currentValue, null);
            }

            
            if (dataEditor.ShowDialog() == true)
            {
                textBox.Text = dataEditor.NewValue.ToString();
                PositionStatusDataGrid.Items.Refresh();
            }
        }
    }
}
