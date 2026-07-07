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

namespace EQX.UI.MVVM
{
    /// <summary>
    /// Interaction logic for CIMHostMessageView.xaml
    /// </summary>
    public partial class CIMHostMessageView : UserControl
    {
        public CIMHostMessageView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CIMHostMessageViewModelBase cIMHostMessageViewModelBase == false) return;

            CIMHostMsgDataGrid.ItemsSource = cIMHostMessageViewModelBase.LoadMessageEntries();
        }
    }
}
