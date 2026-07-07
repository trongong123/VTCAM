using EQX.Core.Units;
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
    /// Interaction logic for CamTrayCellView.xaml
    /// </summary>
    public partial class CamTrayCellView : UserControl
    {
        public CamTrayCellView()
        {
            InitializeComponent();
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (this.DataContext is TrayCell<ETrayCellStatus> trayCell == false) return;

            //trayCell.CellDoubleClick();
            //e.Handled = true;
        }
    }
}
