using System.Windows;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for IOMonitorWindowView.xaml
    /// </summary>
    public partial class IOMonitorWindowView : Window
    {
        public IOMonitorWindowView()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
