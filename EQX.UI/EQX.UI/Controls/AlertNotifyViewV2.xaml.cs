using EQX.Core.Common;
using EQX.UI.Language;
using System.Windows;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for AlertNotifyView.xaml
    /// </summary>
    public partial class AlertNotifyViewV2 : Window
    {
        public AlertModel AlertModel { get; set; }
        public bool IsWarning { get; set; }
        public static event Action? BuzzerOff;
        public event Action? CloseDialog;
        public AlertNotifyViewV2()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public static bool? ShowDialog(AlertModel alarmModel, bool isWarning = false)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                bool isShown = Application.Current.Windows.OfType<AlertNotifyView>().Any();
                if (isShown) Application.Current.Windows.OfType<AlertNotifyView>().First().Close();

                isShown = Application.Current.Windows.OfType<MessageBoxEx>().Any();
                if (isShown) Application.Current.Windows.OfType<MessageBoxEx>().First().Close();


                var mainWindow = Application.Current.MainWindow;
                double width = mainWindow.ActualWidth;
                double height = mainWindow.ActualHeight;

                AlertNotifyView messageBoxEx = new AlertNotifyView()
                {
                    IsWarning = isWarning,
                    AlertModel = alarmModel,
                    Width = width * 0.7,
                    Height = height * 0.7,
                    
                };

                messageBoxEx.ShowDialog();
                return messageBoxEx.DialogResult;
            });
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

        private void BuzzerOff_Click(object sender, RoutedEventArgs e)
        {
            OnBuzzerOff();
        }

        private void OnBuzzerOff()
        {
            BuzzerOff?.Invoke();
        }
    }
}
