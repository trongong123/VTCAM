using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for InterfaceView.xaml
    /// </summary>
    public partial class InterfaceView : Window
    {
        public InterfaceView()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
