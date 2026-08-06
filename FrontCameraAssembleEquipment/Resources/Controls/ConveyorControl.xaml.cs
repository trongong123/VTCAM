using System.Windows;
using System.Windows.Controls;
using EQX.Core.InOut;
using EQX.InOut;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for RollerControl.xaml
    /// </summary>
    public partial class CVControl : UserControl
    {
        public IDOutput VacOnOutput { get; set; }

        public CVControl()
        {
            InitializeComponent();
        }
        private void RunClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ConveyorBase CV)
            {
                CV.Run();
            }
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ConveyorBase CV)
            {
                CV.Stop();
            }
        }
    }
}
