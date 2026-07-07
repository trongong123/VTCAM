using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Device.SpeedController;
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

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for CVManualControlView.xaml
    /// </summary>
    public partial class CVManualControlView : UserControl
    {
        public CVManualControlView()
        {
            InitializeComponent();
        }

        // In Set Cv
        public ObservableCollection<ICylinder> CylindersInCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersInCvProperty); }
            set { SetValue(CylindersInCvProperty, value); }
        }
        public static readonly DependencyProperty CylindersInCvProperty =
            DependencyProperty.Register(nameof(CylindersInCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public IConveyor CVsInCv
        {
            get { return (IConveyor)GetValue(CVsInCvProperty); }
            set { SetValue(CVsInCvProperty, value); }
        }
        public static readonly DependencyProperty CVsInCvProperty =
            DependencyProperty.Register(nameof(CVsInCv), typeof(IConveyor), typeof(CVManualControlView));
        public IConveyor ManualInCv
        {
            get { return (IConveyor)GetValue(ManualInCvCvProperty); }
            set { SetValue(ManualInCvCvProperty, value); }
        }
        public static readonly DependencyProperty ManualInCvCvProperty =
            DependencyProperty.Register(nameof(ManualInCv), typeof(IConveyor), typeof(CVManualControlView));
        // Detach Set Cv
        public ObservableCollection<ICylinder> CylindersDetachCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersDetachCvProperty); }
            set { SetValue(CylindersDetachCvProperty, value); }
        }
        public static readonly DependencyProperty CylindersDetachCvProperty =
            DependencyProperty.Register(nameof(CylindersDetachCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> CVsDetachCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(CVsDetachCvProperty); }
            set { SetValue(CVsDetachCvProperty, value); }
        }
        public static readonly DependencyProperty CVsDetachCvProperty =
            DependencyProperty.Register(nameof(CVsDetachCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));
        
        // Assemble Set Cv
        public ObservableCollection<ICylinder> CylindersAssembleCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersAssembleCvProperty); }
            set { SetValue(CylindersAssembleCvProperty, value); }
        }
        public static readonly DependencyProperty CylindersAssembleCvProperty =
            DependencyProperty.Register(nameof(CylindersAssembleCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> CVsAssembleCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(CVsAssembleCvProperty); }
            set { SetValue(CVsAssembleCvProperty, value); }
        }
        public static readonly DependencyProperty CVsAssembleCvProperty =
            DependencyProperty.Register(nameof(CVsAssembleCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));
        
        // Out Set Cv
        public ObservableCollection<ICylinder> CylindersOutCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(CylindersOutCvProperty); }
            set { SetValue(CylindersOutCvProperty, value); }
        }
        public static readonly DependencyProperty CylindersOutCvProperty =
            DependencyProperty.Register(nameof(CylindersOutCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> CVsOutCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(CVsOutCvProperty); }
            set { SetValue(CVsOutCvProperty, value); }
        }
        public static readonly DependencyProperty CVsOutCvProperty =
            DependencyProperty.Register(nameof(CVsOutCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));


        // REAR
        // In Set Cv
        public ObservableCollection<ICylinder> RearCylindersInCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(RearCylindersInCvProperty); }
            set { SetValue(RearCylindersInCvProperty, value); }
        }
        public static readonly DependencyProperty RearCylindersInCvProperty =
            DependencyProperty.Register(nameof(RearCylindersInCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public IConveyor RearCVsInCv
        {
            get { return (IConveyor)GetValue(RearCVsInCvProperty); }
            set { SetValue(RearCVsInCvProperty, value); }
        }
        public static readonly DependencyProperty RearCVsInCvProperty =
            DependencyProperty.Register(nameof(RearCVsInCv), typeof(IConveyor), typeof(CVManualControlView));
        public IConveyor RearManualInCv
        {
            get { return (IConveyor)GetValue(RearManualInCvCvProperty); }
            set { SetValue(RearManualInCvCvProperty, value); }
        }
        public static readonly DependencyProperty RearManualInCvCvProperty =
            DependencyProperty.Register(nameof(RearManualInCv), typeof(IConveyor), typeof(CVManualControlView));

        // Detach Set Cv
        public ObservableCollection<ICylinder> RearCylindersDetachCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(RearCylindersDetachCvProperty); }
            set { SetValue(RearCylindersDetachCvProperty, value); }
        }
        public static readonly DependencyProperty RearCylindersDetachCvProperty =
            DependencyProperty.Register(nameof(RearCylindersDetachCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> RearCVsDetachCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(RearCVsDetachCvProperty); }
            set { SetValue(RearCVsDetachCvProperty, value); }
        }
        public static readonly DependencyProperty RearCVsDetachCvProperty =
            DependencyProperty.Register(nameof(RearCVsDetachCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));
        
        // Assemble Set Cv
        public ObservableCollection<ICylinder> RearCylindersAssembleCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(RearCylindersAssembleCvProperty); }
            set { SetValue(RearCylindersAssembleCvProperty, value); }
        }
        public static readonly DependencyProperty RearCylindersAssembleCvProperty =
            DependencyProperty.Register(nameof(RearCylindersAssembleCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> RearCVsAssembleCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(RearCVsAssembleCvProperty); }
            set { SetValue(RearCVsAssembleCvProperty, value); }
        }
        public static readonly DependencyProperty RearCVsAssembleCvProperty =
            DependencyProperty.Register(nameof(RearCVsAssembleCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));
        
        // Out Set Cv
        public ObservableCollection<ICylinder> RearCylindersOutCv
        {
            get { return (ObservableCollection<ICylinder>)GetValue(RearCylindersOutCvProperty); }
            set { SetValue(RearCylindersOutCvProperty, value); }
        }
        public static readonly DependencyProperty RearCylindersOutCvProperty =
            DependencyProperty.Register(nameof(RearCylindersOutCv), typeof(ObservableCollection<ICylinder>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<ICylinder> { }));
        public ObservableCollection<IConveyor> RearCVsOutCv
        {
            get { return (ObservableCollection<IConveyor>)GetValue(RearCVsOutCvProperty); }
            set { SetValue(RearCVsOutCvProperty, value); }
        }
        public static readonly DependencyProperty RearCVsOutCvProperty =
            DependencyProperty.Register(nameof(RearCVsOutCv), typeof(ObservableCollection<IConveyor>), typeof(CVManualControlView), new PropertyMetadata(new ObservableCollection<IConveyor> { }));


    }

}
