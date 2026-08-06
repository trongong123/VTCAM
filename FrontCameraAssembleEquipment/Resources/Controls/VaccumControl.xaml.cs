using System.Windows;
using System.Windows.Controls;
using FrontCameraAssembleEquipment.Defines;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for VaccumControl.xaml
    /// </summary>
    public partial class VaccumControl : UserControl
    {
        public VaccumControl()
        {
            InitializeComponent();
        }

        private void VaccumOn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Vaccum vac)
            {
                vac.VaccumOn();
            }
        }

        private void VaccumOff_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Vaccum vac)
            {
                vac.VaccumOff();
            }
        }
    }
}
