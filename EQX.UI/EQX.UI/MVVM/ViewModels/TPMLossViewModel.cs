using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Communication;
using System;
using System.Windows.Input;

namespace EQX.UI.MVVM
{
    public class TPMLossViewModel : ViewModelBase
    {
        private ETPMLossDesciption? _selectedTPMMode;
        public ETPMLossDesciption? SelectedTPMMode
        {
            get => _selectedTPMMode;
            set => SetProperty(ref _selectedTPMMode, value);
        }

        public Action<bool?> CloseAction { get; set; }

        public ICommand TPMModeSelectCommand { get; }

        public ICommand ConfirmCommand { get; }

        public TPMLossViewModel()
        {
            TPMModeSelectCommand = new RelayCommand<ETPMLossDesciption>((description) => SelectedTPMMode = description);

            // Chỉ cho phép nhấn Confirm khi đã có một lựa chọn
            ConfirmCommand = new RelayCommand(
                () => CloseAction?.Invoke(true),
                () => SelectedTPMMode.HasValue
            );

            // Cập nhật trạng thái của nút Confirm mỗi khi lựa chọn thay đổi
            PropertyChanged += (s, e) => { 
                if (e.PropertyName == nameof(SelectedTPMMode)) ((RelayCommand)ConfirmCommand).NotifyCanExecuteChanged(); 
            };
        }
    }
}
