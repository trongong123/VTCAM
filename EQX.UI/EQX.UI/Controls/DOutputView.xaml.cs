using EQX.Core.InOut;
using System.Windows;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for DOutputView.xaml
    /// </summary>
    public partial class DOutputView : UserControl
    {
        public DOutputView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null) return;
            if (this.DataContext.GetType().GetInterfaces().Contains(typeof(IDOutput)) == false) return;

            ((IDOutput)this.DataContext).Value = !((IDOutput)this.DataContext).Value;
        }
    }
}
