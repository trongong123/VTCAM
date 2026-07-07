using System.Windows;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for MessageBoxEx.xaml
    /// </summary>
    public partial class MessageBoxEx : Window
    {
        public bool CloseItSelf { get; set; } = false;

        public MessageBoxEx()
        {
            InitializeComponent();
            Topmost = true;
        }

        public static void Show(string message, bool confirmRequest = true, string caption = "Confirm", bool closeItSelf = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                bool isShown = Application.Current.Windows.OfType<AlertNotifyView>().Any();
                if (isShown) Application.Current.Windows.OfType<AlertNotifyView>().First().Close();

                isShown = Application.Current.Windows.OfType<MessageBoxEx>().Any(mbe => mbe.CloseItSelf == false);
                if (isShown) Application.Current.Windows.OfType<MessageBoxEx>().First(mbe => mbe.CloseItSelf == false).Close();

                MessageBoxEx messageBoxEx = new MessageBoxEx();
                messageBoxEx.CloseItSelf = closeItSelf;
                ((MessageBoxExViewModel)messageBoxEx.DataContext).ConfirmRequest = confirmRequest;
                ((MessageBoxExViewModel)messageBoxEx.DataContext).Show(message, caption);
                messageBoxEx.Show();
            });
        }

        public static bool? ShowDialog(string message, string caption = "Confirm", bool closeItSelf = false)
        {
            return ShowDialog(message, true, caption, closeItSelf);
        }

        public static bool? ShowDialog(string message, bool confirmRequest, string caption = "Confirm", bool closeItSelf = false)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                bool isShown = Application.Current.Windows.OfType<AlertNotifyView>().Any();
                if (isShown) Application.Current.Windows.OfType<AlertNotifyView>().First().Close();

                isShown = Application.Current.Windows.OfType<MessageBoxEx>().Any(mbe => mbe.CloseItSelf == false);
                if (isShown) Application.Current.Windows.OfType<MessageBoxEx>().First(mbe => mbe.CloseItSelf == false).Close();

                MessageBoxEx messageBoxEx = new MessageBoxEx();
                messageBoxEx.CloseItSelf = closeItSelf;
                ((MessageBoxExViewModel)messageBoxEx.DataContext).ConfirmRequest = confirmRequest;
                ((MessageBoxExViewModel)messageBoxEx.DataContext).ShowDialog(message, caption);
                messageBoxEx.ShowDialog();
                return messageBoxEx.DialogResult;
            });
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                DialogResult = true;
            }
            else
            {
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                DialogResult = false;
            }
            else
            {
                Close();
            }
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                DialogResult = true;
            }
            else
            {
                Close();
            }
        }
    }
}
