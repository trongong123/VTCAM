using EQX.InOut;
using FrontCameraAssembleEquipment.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
