using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for LogDisplayer.xaml
    /// </summary>
    public partial class LogDisplayer : UserControl
    {
        public LogDisplayer()
        {
            InitializeComponent();

            // TODO : Uncomment log
            //NotifyAppender.Appender.Notification.CollectionChanged += Notification_CollectionChanged;
        }

        private void Notification_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = logListBox.Items.Count - 1;
            if (selectedIndex < 0)
                return;

            logListBox.SelectedIndex = selectedIndex;
            logListBox.UpdateLayout();

            logListBox.ScrollIntoView(logListBox.SelectedItem);
        }
    }
}
