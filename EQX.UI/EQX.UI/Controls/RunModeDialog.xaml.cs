using System.Windows;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for RunModeDialog.xaml
    /// </summary>
    public partial class RunModeDialog : Window
    {
        public RunModeDialog()
        {
            InitializeComponent();
        }
        public static T? ShowRunModeDialog<T>(IEnumerable<T> modes) where T : struct, Enum
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var vm = new RunModeDialogViewModel<T>(modes);
                var dialog = new RunModeDialog { DataContext = vm };
                vm.RequestClose += () =>
                {
                    dialog.DialogResult = vm.SelectedMode.HasValue;
                    dialog.Close();
                };
                var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (owner != null)
                    dialog.Owner = owner;
                return dialog.ShowDialog() == true ? vm.SelectedMode : null;
            });
        }
    }
}
