using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TactTimeListView.xaml
    /// </summary>
    public partial class TactTimeListView : UserControl
    {
        public TactTimeListView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<double> TactTimes
        {
            get { return (ObservableCollection<double>)GetValue(TactTimesProperty); }
            set { SetValue(TactTimesProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TactTimesProperty =
            DependencyProperty.Register(nameof(TactTimes), typeof(ObservableCollection<double>), typeof(TactTimeListView), new PropertyMetadata(new ObservableCollection<double>()));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(TactTimeListView), new PropertyMetadata(3));
    }
}
