using EQX.InOut;
using System.Windows;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for CylinderSimpleView.xaml
    /// </summary>
    public partial class CylinderSimpleView : UserControl
    {
        public CylinderSimpleView()
        {
            InitializeComponent();
        }
        private void ForwardButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CylinderBase cylinder)
            {
                cylinder.Forward();
            }
        }

        private void BackwardButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CylinderBase cylinder)
            {
                cylinder.Backward();
            }
        }
    }
}
