using System.Windows.Controls;
using System.Windows.Input;

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
