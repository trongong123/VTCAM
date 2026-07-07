using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Communication.CIM;
using EQX.Core.Communication.CIM.Custom;
using EQX.Core.Communication.CIM.Custom.WordArea;
using System.Windows.Input;
using TOPENG_Device;

namespace EQX.UI.MVVM
{
    public class MaterialPort : MaterialKittingPlcToCimArea
    {
        private readonly ICIMMapHelper _mapHelper;
        private bool _IsCurrentActived;
        #region Properties
        public EMaterialKittingCEID LastKittingCEID { get; private set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsCurrentActived
        {
            get { return _IsCurrentActived; }
            set
            {
                _IsCurrentActived = value;
                OnPropertyChanged(nameof(IsCurrentActived));
            }
        }

        public string ManualInputID { get; set; }

        public string Result { get; set; }
        #endregion

        public MaterialPort(ICIMMapHelper mapHelper)
        {
            _mapHelper = mapHelper;
        }

        public void UIUpdate()
        {
            OnPropertyChanged(nameof(MaterialState));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(TotalQty));
            OnPropertyChanged(nameof(RemainQty));
            OnPropertyChanged(nameof(UseQty));
            OnPropertyChanged(nameof(Result));
        }

        #region Commands
        public ICommand ActiveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsCurrentActived = true;
                    OnPropertyChanged(nameof(IsCurrentActived));
                });
            }
        }

        public ICommand ManualIDInput
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ManualInputID = $"abcdefghij01234567890123456789_{PortID}";
                    BatchID = ManualInputID;
                    OnPropertyChanged(nameof(BatchID));
                    OnPropertyChanged(nameof(ManualInputID));
                });
            }
        }

        public ICommand MaterialKitting
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Result = "Kitting...";
                    OnPropertyChanged(nameof(Result));

                    base.MaterialState = "1";
                    base.PortID = Id.ToString();
                    base.State = "1";
                    LastKittingCEID = EMaterialKittingCEID.KITTING;

                    EquipEventHelpers.MaterialKitting(this);
                });
            }
        }

        public ICommand MaterialCancel
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Result = "Cancelling...";
                    OnPropertyChanged(nameof(Result));

                    base.MaterialState = "1";
                    base.PortID = Id.ToString();
                    base.State = "1";
                    LastKittingCEID = EMaterialKittingCEID.KITTING_CANCEL;

                    EquipEventHelpers.MaterialCancel(this);
                });
            }
        }
        #endregion
    }
}
