using EQX.Core.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for InitializeView.xaml
    /// </summary>
    public partial class InitializeView : UserControl
    {
        public InitializeView()
        {
            InitializeComponent();
        }
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is InitializeViewModel initVM == false) return;
            if (sender is Border border == false) return;
            if (border.DataContext is IProcess<ESequence> process == false) return;

            bool currentValue = process.IsOriginOrInitSelected;
            process.IsOriginOrInitSelected = !currentValue;

            if (initVM.Processes.SpongeDetachProcess.IsOriginOrInitSelected
                || initVM.Processes.CameraFlipperProcess.IsOriginOrInitSelected)
            {
                initVM.Processes.CameraFlipperProcess.IsOriginOrInitSelected = true;
                initVM.Processes.SpongeDetachProcess.IsOriginOrInitSelected = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is InitializeViewModel initVM == false) return;

            initVM.Processes.RootProcess?.Childs?.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
        }
    }
}
