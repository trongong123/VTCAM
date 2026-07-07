using FrontCameraAssembleEquipment.Process;
using System.Windows;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for RunModeDialog.xaml
    /// </summary>
    public partial class RunModeDialog : Window
    {
        public RunModeDialog(MachineStatus machineStatus)
        {
            InitializeComponent();
            switch (machineStatus.MachineRunMode)
            {
                case Defines.Process.EMachineRunMode.Auto:
                    cbAutoRun.IsChecked = true;
                    break;
                case Defines.Process.EMachineRunMode.ByPass:
                    cbByPass.IsChecked = true;
                    break;
                case Defines.Process.EMachineRunMode.Dryrun:
                    cbDryRun.IsChecked = true;
                    break;
                case Defines.Process.EMachineRunMode.ManualInterface:
                    cbInterfaceOnly.IsChecked = true;
                    break;
            }
        }

        public static T? ShowRunModeDialog<T>(IEnumerable<T> modes, MachineStatus machineStatus) where T : struct, Enum
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var vm = new RunModeDialogViewModel<T>(modes);
                var dialog = new RunModeDialog(machineStatus) { DataContext = vm };
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
