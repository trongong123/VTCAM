using CommunityToolkit.Mvvm.ComponentModel;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public class MaterialStatus : ObservableObject
    {
        private string _name;
        private bool _isEditable;
        private EMaterialType _type;
        private EMaterialStatus _status;
        private ECameraStatus _cameraStatus;
        private EMaterialProcessStatus _processStatus;

        public event Action<bool> StateChanged;

        public void OnStateChangedEvent(bool obj)
        {
            StateChanged?.Invoke(obj);
        }

        public string Name 
        {  
            get => _name ;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public EMaterialType Type 
        { 
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            } 
        }

        public EMaterialStatus Status 
        { 
            get => _status; 
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public ECameraStatus CameraStatus
        {
            get => _cameraStatus;
            set
            {
                _cameraStatus = value;
                OnPropertyChanged(nameof(_cameraStatus));
            }
        }

        public EMaterialProcessStatus ProcessStatus
        {
            get => _processStatus;
            set
            {
                _processStatus = value;
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }

        public ECVLine CVLine { get; set; }
        public bool IsEditable 
        { 
            get => _isEditable; 
            set
            {
                _isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public void Set()
        {
            Status = EMaterialStatus.Existing;
        }

        public void Clear()
        {
            ProcessStatus = EMaterialProcessStatus.None;
            Status = EMaterialStatus.NotExist;
        }
    }
}
