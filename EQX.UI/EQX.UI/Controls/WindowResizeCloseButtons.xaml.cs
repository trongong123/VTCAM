using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for WindowResizeCloseButtons.xaml
    /// </summary>
    public partial class WindowResizeCloseButtons : UserControl
    {
        public ICommand CloseButtonClickCommand
        {
            get { return (ICommand)GetValue(CloseButtonClickCommandProperty); }
            set { SetValue(CloseButtonClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseButtonClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseButtonClickCommandProperty =
            DependencyProperty.Register("CloseButtonClickCommand", typeof(ICommand), typeof(WindowResizeCloseButtons), new UIPropertyMetadata(null));

        public WindowState WindowState
        {
            get
            {
                if (Window.GetWindow(this) == null) return WindowState.Normal;

                return Window.GetWindow(this).WindowState;
            }
            set
            {
                if (Window.GetWindow(this) == null) return;

                Window.GetWindow(this).WindowState = value;
            }
        }

        public WindowResizeCloseButtons()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ?
                          WindowState.Normal
                        : WindowState.Maximized;
        }
    }
}
