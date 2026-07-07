using EQX.Core.Common;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace EQX.UI.MVVM
{
    public class CIMMessageEntry
    {
        public string Time { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
    }

    public class CIMHostMessageViewModelBase : ViewModelBase
    {
        private readonly Regex CIMHostMessageLogRegex = new Regex(
            @"^\[(?<time>[^\]]+)\]\s*,\s*[^,]*\s*,\s*[^,]*\s*,\s*(?<id>[^,]*)\s*,\s*(?<message>.*)$",
            RegexOptions.Compiled);

        public virtual string Header { get; }
        protected virtual string CIMHostMessageLogFolder { get; }
        public ObservableCollection<CIMMessageEntry> LoadMessageEntries()
        {
            try
            {
                var files = Directory.GetFiles(CIMHostMessageLogFolder);

                List<CIMMessageEntry> oPCallEntries = new List<CIMMessageEntry>();
                foreach (var file in files)
                {
                    oPCallEntries.AddRange(LoadOPCallEntries(file));
                }

                return new ObservableCollection<CIMMessageEntry>(oPCallEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new ObservableCollection<CIMMessageEntry>();
            }
        }

        private List<CIMMessageEntry> LoadOPCallEntries(string filePath)
        {
            var opCallEntries = new List<CIMMessageEntry>();

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = CIMHostMessageLogRegex.Match(line);
                if (!match.Success)
                    continue;

                var time = match.Groups["time"].Value.Trim();
                var id = match.Groups["id"].Value.Trim();
                var message = match.Groups["message"].Value.Trim();

                opCallEntries.Add(new CIMMessageEntry
                {
                    Time = time,
                    Id = id,
                    Message = message
                });
            }

            return opCallEntries;
        }
    }
}
