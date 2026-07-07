using log4net.Appender;
using log4net.Core;
using log4net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace EQX.UI
{
    /// <summary>
    /// The appender we are going to bind to for our logging.
    /// </summary>
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged
    {
        #region Members and events
        private const int MaxUILogRow = 200;
        private readonly object UILogObject = new object();

        private ObservableCollection<string> _notification = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Get or set the notification message.
        /// </summary>
        public ObservableCollection<string> Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                if (_notification == value) return;

                _notification = value;
                OnPropertyChanged("Notification");
            }
        }

        /// <summary>
        /// Raise the change notification.
        /// </summary>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Get a reference to the log instance.
        /// </summary>
        public static NotifyAppender Appender
        {
            get
            {
                return LogManager.GetRepository().GetAppenders().FirstOrDefault(a => a is NotifyAppender) as NotifyAppender;
            }
        }

        /// <summary>
        /// Append the log information to the notification.
        /// </summary>
        /// <param name="loggingEvent">The log event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(writer, loggingEvent);
            try  // In case, App is shutdown before Log did
            {
                if (Application.Current == null) return;

                Application.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    lock (UILogObject)
                    {
                        if (Notification.Count >= MaxUILogRow)
                        {
                            Notification.RemoveAt(MaxUILogRow - 1);
                        }
                        Notification.Insert(0,writer.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
