using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Device.SpeedController;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Vision;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using static FrontCameraAssembleEquipment.MVVM.ViewModels.ManualViewModel;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for UnitManualControlView.xaml
    /// </summary>
    public partial class UnitManualControlView : UserControl
    {
        public ObservableCollection<IMotion> Motions
        {
            get { return (ObservableCollection<IMotion>)GetValue(MotionsProperty); }
            set { SetValue(MotionsProperty, value); }
        }
        public static readonly DependencyProperty MotionsProperty =
            DependencyProperty.Register("Motions", typeof(ObservableCollection<IMotion>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<IMotion> { }));
        public ObservableCollection<ICylinder> Cylinders
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersProperty); }
            set { SetValue(CylindersProperty, value); }
        }
        public static readonly DependencyProperty CylindersProperty =
            DependencyProperty.Register("Cylinders", typeof(ObservableCollection<ICylinder>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> CVs
        {
            get { return (ObservableCollection<IConveyor>)GetValue(CVsProperty); }
            set { SetValue(CVsProperty, value); }
        }
        public static readonly DependencyProperty CVsProperty =
            DependencyProperty.Register("CVs", typeof(ObservableCollection<IConveyor>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));
        public ObservableCollection<ESemiSequence> SemiAutoSequences
        {
            get { return (ObservableCollection<ESemiSequence>)GetValue(SemiSequencesProperty); }
            set { SetValue(SemiSequencesProperty, value); }
        }
        public static readonly DependencyProperty SemiSequencesProperty =
            DependencyProperty.Register("SemiAutoSequences", typeof(ObservableCollection<ESemiSequence>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<ESemiSequence> { }));
        public ObservableCollection<PositionGroup> TeachingPositions
        {
            get { return (ObservableCollection<PositionGroup>)GetValue(PositionTeachingsProperty); }
            set { SetValue(PositionTeachingsProperty, value); }
        }
        public static readonly DependencyProperty PositionTeachingsProperty =
            DependencyProperty.Register(nameof(TeachingPositions), typeof(ObservableCollection<PositionGroup>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<PositionGroup> { }));
        public ObservableCollection<BD201SRollerController> Rolllers
        {
            get { return (ObservableCollection<BD201SRollerController>)GetValue(RolllersProperty); }
            set { SetValue(RolllersProperty, value); }
        }
        public static readonly DependencyProperty RolllersProperty =
            DependencyProperty.Register("Rolllers", typeof(ObservableCollection<BD201SRollerController>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<BD201SRollerController> { }));
        public ObservableCollection<Vaccum> Vaccums
        {
            get { return (ObservableCollection<Vaccum>)GetValue(VaccumsProperty); }
            set { SetValue(VaccumsProperty, value); }
        }
        public static readonly DependencyProperty VaccumsProperty =
            DependencyProperty.Register("Vaccums", typeof(ObservableCollection<Vaccum>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<Vaccum> { }));
        public ObservableCollection<IDOutput> Outputs
        {
            get { return (ObservableCollection<IDOutput>)GetValue(OutputsProperty); }
            set { SetValue(OutputsProperty, value); }
        }
        public static readonly DependencyProperty OutputsProperty =
            DependencyProperty.Register("Outputs", typeof(ObservableCollection<IDOutput>), typeof(UnitManualControlView), new PropertyMetadata(new ObservableCollection<IDOutput> { }));
        public VisionProcess VisionProcess
        {
            get { return (VisionProcess)GetValue(VisionProcessProperty); }
            set { SetValue(VisionProcessProperty, value); }
        }
        public static readonly DependencyProperty VisionProcessProperty =
            DependencyProperty.Register("VisionProcess", typeof(VisionProcess), typeof(UnitManualControlView));
        public List<IDInput> Inputs
        {
            get { return (List<IDInput>)GetValue(InputsProperty); }
            set { SetValue(InputsProperty, value); }
        }
        public static readonly DependencyProperty InputsProperty =
            DependencyProperty.Register("Inputs", typeof(List<IDInput>), typeof(UnitManualControlView), new PropertyMetadata(new List<IDInput> { }));
        public ImageSource UnitImage
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("UnitImage", typeof(ImageSource), typeof(UnitManualControlView), new PropertyMetadata(null));

        public ICommand SemiAutoCommand
        {
            get { return (ICommand)GetValue(SemiAutoCommandCommandProperty); }
            set { SetValue(SemiAutoCommandCommandProperty, value); }
        }
        public static readonly DependencyProperty SemiAutoCommandCommandProperty =
            DependencyProperty.Register("SemiAutoCommand", typeof(ICommand), typeof(UnitManualControlView), new PropertyMetadata());
        public ICommand PositionMoveCommand
        {
            get { return (ICommand)GetValue(PositionMoveCommandProperty); }
            set { SetValue(PositionMoveCommandProperty, value); }
        }
        public static readonly DependencyProperty PositionMoveCommandProperty =
            DependencyProperty.Register(nameof(PositionMoveCommand), typeof(ICommand), typeof(UnitManualControlView), new PropertyMetadata());

        public ICommand DecreaseIOPageCommand
        {
            get { return (ICommand)GetValue(DecreaseIOPageCommandProperty); }
            set { SetValue(DecreaseIOPageCommandProperty, value); }
        }
        public static readonly DependencyProperty DecreaseIOPageCommandProperty =
            DependencyProperty.Register(nameof(DecreaseIOPageCommand), typeof(ICommand), typeof(UnitManualControlView), new PropertyMetadata());
        public ICommand IncreaseIOPageCommand
        {
            get { return (ICommand)GetValue(IncreaseIOPageCommandProperty); }
            set { SetValue(IncreaseIOPageCommandProperty, value); }
        }
        public static readonly DependencyProperty IncreaseIOPageCommandProperty =
            DependencyProperty.Register(nameof(IncreaseIOPageCommand), typeof(ICommand), typeof(UnitManualControlView), new PropertyMetadata());

        public bool IsIOShow
        {
            get { return (bool)GetValue(IsIOShowProperty); }
            set { SetValue(IsIOShowProperty, value); }
        }
        public static readonly DependencyProperty IsIOShowProperty =
            DependencyProperty.Register(nameof(IsIOShow), typeof(bool), typeof(UnitManualControlView), new PropertyMetadata());

        public bool IsFrontCv
        {
            get { return (bool)GetValue(IsFrontCvProperty); }
            set { SetValue(IsFrontCvProperty, value); }
        }
        public static readonly DependencyProperty IsFrontCvProperty =
            DependencyProperty.Register(nameof(IsFrontCv), typeof(bool), typeof(UnitManualControlView), new PropertyMetadata());

        public bool IsCamHeadProcess
        {
            get { return (bool)GetValue(IsCamHeadProcessProperty); }
            set { SetValue(IsCamHeadProcessProperty, value); }
        }
        public static readonly DependencyProperty IsCamHeadProcessProperty =
            DependencyProperty.Register(nameof(IsCamHeadProcess), typeof(bool), typeof(UnitManualControlView), new PropertyMetadata());

        public bool IsMaxIOPerPage
        {
            get { return (bool)GetValue(IsMaxIOPerPageProperty); }
            set { SetValue(IsMaxIOPerPageProperty, value); }
        }
        public static readonly DependencyProperty IsMaxIOPerPageProperty =
            DependencyProperty.Register(nameof(IsMaxIOPerPage), typeof(bool), typeof(UnitManualControlView), new PropertyMetadata());

        public uint SelectedInputPage
        {
            get { return (uint)GetValue(SelectedInputPageProperty); }
            set { SetValue(SelectedInputPageProperty, value); }
        }
        public static readonly DependencyProperty SelectedInputPageProperty =
            DependencyProperty.Register(nameof(SelectedInputPage), typeof(uint), typeof(UnitManualControlView), new PropertyMetadata());

        public UnitManualControlView()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
