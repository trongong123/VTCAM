using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EQX.UI.Controls;
using Window = System.Windows.Window;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();

            EnterEvent += LoginButton_Click;
            this.PreviewKeyDown += LoginDialog_KeyDown;
        }

        private void LoginDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnterEvent?.Invoke(null, EventArgs.Empty);
            }
        }

        private event EventHandler? EnterEvent;
        public string InputPasswordToCheck;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            passwordBox.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            bool checkVal = PasswordCheck(passwordBox.Password, InputPasswordToCheck);

            if(checkVal == false)
            {
                MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_WrongPassword"], false);
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
            }

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn == false) return;

            passwordBox.Password += btn.Content.ToString();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn == false) return;

            passwordBox.Password = string.Empty;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private bool PasswordCheck(string inputPass, string checkPass)
        {
            return (string.Equals(inputPass, checkPass));
        }
    }
}
