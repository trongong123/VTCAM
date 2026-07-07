using EQX.Core.InOut;
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

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for VacuumView.xaml
    /// </summary>
    public partial class VacuumView : UserControl
    {
        public VacuumView()
        {
            InitializeComponent();
        }

        private void VaccumOff_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is IEjector vacuum)
            {
                vacuum.VacuumOff();
            }
        }

        private void VaccumOn_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IEjector vacuum)
            {
                vacuum.VacuumOn();
            }
        }
    }
}
