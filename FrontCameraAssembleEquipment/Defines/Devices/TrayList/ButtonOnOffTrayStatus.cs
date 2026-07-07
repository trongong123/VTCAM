using CommunityToolkit.Mvvm.ComponentModel;

namespace FrontCameraAssembleEquipment.Defines
{
    public class ButtonOnOffTrayStatus : ObservableObject
    {
        #region Properties
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private int index;
        private string name;
        #endregion
    }
}
