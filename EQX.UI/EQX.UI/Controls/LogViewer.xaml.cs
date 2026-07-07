using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace EQX.UI.Controls
{
    public partial class LogViewer : UserControl
    {
        private List<string> level = new List<string> { "ALL", "DEBUG", "INFO", "WARN", "ERROR", "FATAL", "OFF" };

        public string LogDirectory
        {
            get { return (string)GetValue(LogDirectoryProperty); }
            set { SetValue(LogDirectoryProperty, value); }
        }

        public ObservableCollection<string> Unit
        {
            get { return (ObservableCollection<string>)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(ObservableCollection<string>), typeof(LogViewer), new PropertyMetadata(new ObservableCollection<string> { }));

        public static readonly DependencyProperty LogDirectoryProperty =
            DependencyProperty.Register("LogDirectory", typeof(string), typeof(LogViewer), new PropertyMetadata(""));

        public LogViewer()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void LoadDirectoriesAndFiles()
        {
            LogTreeView.Items.Clear();
            LoadDirectoryNodes(LogDirectory, null);
        }

        private void LoadDirectoryNodes(string directoryPath, TreeViewItem parentItem)
        {
            var directories = Directory.GetDirectories(directoryPath)
                                       .Select(d => new { Path = d, Name = Path.GetFileName(d) })
                                       .OrderByDescending(d => d.Name)
                                       .ToList();

            foreach (var directory in directories)
            {
                var directoryItem = new TreeViewItem { Header = directory.Name, Tag = directory.Path };
                LoadDirectoryNodes(directory.Path, directoryItem);
                LoadLogFiles(directory.Path, directoryItem);
                if (parentItem == null)
                {
                    LogTreeView.Items.Add(directoryItem);
                }
                else
                {
                    parentItem.Items.Add(directoryItem);
                }
            }
        }

        private void LoadLogFiles(string directoryPath, TreeViewItem parentItem)
        {
            var logFiles = Directory.GetFiles(directoryPath, "*.*")
                                    .Select(f => new { Path = f, Name = Path.GetFileName(f) })
                                    .OrderByDescending(f => f.Name)
                                    .ToList();

            foreach (var logFile in logFiles)
            {
                var fileItem = new TreeViewItem { Header = logFile.Name, Tag = logFile.Path };
                parentItem.Items.Add(fileItem);
            }
        }

        private void LoadLevel()
        {
            cboLevel.Items.Clear();
            foreach (var lvl in level)
            {
                cboLevel.Items.Add(lvl);
            }
            cboLevel.SelectedIndex = 0;
        }

        private void LogTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (LogTreeView.SelectedItem is TreeViewItem selectedItem && File.Exists(selectedItem.Tag.ToString()))
            {
                LoadData(selectedItem.Tag.ToString());
            }
        }

        private void cboUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LogTreeView.SelectedItem is TreeViewItem selectedItem && File.Exists(selectedItem.Tag.ToString()))
            {
                LoadData(selectedItem.Tag.ToString());
            }
        }

        private void cboLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LogTreeView.SelectedItem is TreeViewItem selectedItem && File.Exists(selectedItem.Tag.ToString()))
            {
                LoadData(selectedItem.Tag.ToString());
            }
        }

        private ObservableCollection<string> Filter(string unit, string level)
        {
            if (unit == "ALL" && level == "ALL") return logList;
            ObservableCollection<string> result = new ObservableCollection<string>();
            foreach (string item in logList)
            {
                if (unit == "ALL")
                {
                    if (item.Contains(level))
                    {
                        result.Add(item);
                    }
                    continue;
                }
                if (level == "ALL")
                {
                    if (item.Contains(unit))
                    {
                        result.Add(item);
                    }
                    continue;
                }
                if (item.Contains(level) && item.Contains(unit))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private void LoadData(string selectedFile)
        {
            if (string.IsNullOrEmpty(LogDirectory)) return;

            string logContent = File.ReadAllText(selectedFile);
            logList = new ObservableCollection<string>(logContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
            logList = Filter(cboLevel.SelectedItem.ToString(), cboUnit.SelectedItem.ToString());
            logListBox.ItemsSource = logList;
        }

        private ObservableCollection<string> logList = new ObservableCollection<string>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDirectoriesAndFiles();
            LoadLevel();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            logListBox.ItemsSource = null;
            LoadDirectoriesAndFiles();
            LoadLevel();
            cboUnit.SelectedIndex = 0;
        }
    }
}
