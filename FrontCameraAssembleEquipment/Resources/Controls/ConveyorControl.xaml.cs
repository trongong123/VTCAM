using EQX.Core.InOut;
using EQX.InOut;
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
