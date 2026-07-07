using EQX.InOut;
using EQX.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FrontCameraAssembleEquipment.Resources.Style
{
    public partial class ConveyorControlStyle : ResourceDictionary
    {
        public ConveyorControlStyle()
        {
            InitializeComponent();
        }
        private void ConveyorRun_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ConveyorBase cv)
            {
                cv.Run();
            }
        }

        private void ConveyorStop_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ConveyorBase cv)
            {

                cv.Stop();
            }
        }

    }
}
