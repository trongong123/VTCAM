using System.Windows;
using System.Windows.Controls;
using EQX.InOut;

namespace FrontCameraAssembleEquipment.Resources.Style
{
    public partial class CylinderControlStyle : ResourceDictionary
    {
        public CylinderControlStyle()
        {
            InitializeComponent();
        }
        private void CylinderForward_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CylinderBase cylinder)
            {
                cylinder.Forward();
            }
        }

        private void CylinderBackward_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CylinderBase cylinder)
            {
                cylinder.Backward();
            }
        }

    }
}
