using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EQX.Core.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.MVVM.ViewModels;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for InitializeWindowView.xaml
    /// </summary>
    public partial class InitializeWindowView : Window
    {
        public InitializeWindowView()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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

            if (initVM.Processes.CameraFlipperProcess.IsOriginOrInitSelected)
            {
                initVM.Processes.CameraAssembleProcess.IsOriginOrInitSelected = true;

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is InitializeViewModel initVM == false) return;

            initVM.Processes.RootProcess?.Childs?.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
        }
    }
}
