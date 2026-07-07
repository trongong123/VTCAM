using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines.LogHistory;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        private readonly IConfiguration _configuration;
        private string LogFolder => _configuration["Folders:LogFolder"] ?? "";
        public LogViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ObservableCollection<FileSystemNode> LogFiles { get; set; }

        public List<LogEntry> LoadLogEntries(string filePath)
        {
            var logEntries = new List<LogEntry>();
            var lines = File.ReadAllLines(filePath).Reverse();

            // Mẫu regex tương ứng với format: [HH:mm:ss.fff],LEVEL ,LOGGER ,MESSAGE
            var regex = new Regex(@"\[(?<time>[0-9:\.]+)\],(?<type>\w+)\s*,(?<source>.{0,30}),(?<message>.*)");

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    logEntries.Add(new LogEntry
                    {
                        Time = DateTime.Parse(match.Groups["time"].Value.Trim()).ToString("HH:mm:ss"),
                        Type = match.Groups["type"].Value.Trim(),
                        Source = match.Groups["source"].Value.Trim(),
                        Description = match.Groups["message"].Value.Trim()
                    });
                }
            }
            return logEntries;
        }

        public void LoadLogFiles()
        {
            try
            {
                var rootNode = new FileSystemNode
                {
                    Name = "Log",
                    Path = LogFolder,
                    IsDirectory = true
                };

                PopulateTreeView(rootNode, LogFolder);

                LogFiles = new ObservableCollection<FileSystemNode> { rootNode };

                OnPropertyChanged(nameof(LogFiles));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateTreeView(FileSystemNode parentNode, string path)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(path).OrderByDescending(d => d))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    var dirNode = new FileSystemNode
                    {
                        Name = dirInfo.Name,
                        Path = dir,
                        IsDirectory = true
                    };

                    PopulateTreeView(dirNode, dir);
                    parentNode.Children.Add(dirNode);
                }

                foreach (var file in Directory.GetFiles(path, "*.txt"))
                {
                    var fileInfo = new FileInfo(file);
                    var fileNode = new FileSystemNode
                    {
                        Name = fileInfo.Name,
                        Path = file,
                        IsDirectory = false
                    };
                    parentNode.Children.Add(fileNode);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating TreeView for {path}: {ex}");
            }
        }
    }
}
