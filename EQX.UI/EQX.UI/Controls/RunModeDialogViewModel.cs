using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace EQX.UI.Controls
{
    public class RunModeDialogViewModel<T> : ObservableObject where T : struct, Enum
    {
        public RunModeDialogViewModel(IEnumerable<T> modes)
        {
            Modes = new ObservableCollection<T>(modes);
            SelectModeCommand = new RelayCommand<T>(mode =>
            {
                SelectedMode = mode;
                RequestClose?.Invoke();
            });
            CloseCommand = new RelayCommand(() => RequestClose?.Invoke());
        }

        public ObservableCollection<T> Modes { get; }

        public T? SelectedMode { get; private set; }

        public IRelayCommand<T> SelectModeCommand { get; }

        public IRelayCommand CloseCommand { get; }

        public event Action? RequestClose;
    }
}
