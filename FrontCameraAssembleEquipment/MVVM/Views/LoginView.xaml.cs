using System.Windows;
using System.Windows.Input;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.MVVM.ViewModels;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            EnterEvent += LoginButton_Click;
        }

        private event EventHandler? EnterEvent;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.LoginCommand.Execute(passwordBox.Password);
                DialogResult = viewModel.WindowResult;
            }
        }

        private void passwordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualKeyboard virtualKeyboard = new VirtualKeyboard();
            if (virtualKeyboard.ShowDialog() == true)
            {
                passwordBox.Password = virtualKeyboard.InputText;
                EnterEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VirtualKeyboard virtualKeyboard = new VirtualKeyboard();
            if (virtualKeyboard.ShowDialog() == true)
            {
                passwordBox.Password = virtualKeyboard.InputText;
                EnterEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
