using System.Collections.ObjectModel;

namespace FrontCameraAssembleEquipment.Defines.LogHistory
{
    public class FileSystemNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public ObservableCollection<FileSystemNode> Children { get; set; } = new ObservableCollection<FileSystemNode>();
    }
}
