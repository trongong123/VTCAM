using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for TactTimeChartView.xaml
    /// </summary>
    public partial class TactTimeChartView : UserControl
    {
        public TactTimeChartView()
        {
            InitializeComponent();
        }

        private void RefreshChart()
        {
            if (this.DataContext is not TactTimeListViewModel viewModel)
                return;
            if (TactPlot == null)
                return;
            if (viewModel.TactTimes == null)
                return;

            var plot = TactPlot.Plot;
            plot.Clear();

            var list = viewModel.TactTimes;
            if (list == null || list.Count == 0)
            {
                plot.Axes.Bottom.Label.Text = "Step";
                plot.Axes.Left.Label.Text = "Tact (s)";
                TactPlot.Refresh();
                return;
            }

            double[] xs = Enumerable.Range(0, list.Count).Select(i => (double)i).ToArray();
            double[] ys = list.ToArray();

            var scatter = plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 1;
            scatter.MarkerSize = 6;

            plot.Axes.Bottom.Label.Text = "Index";
            plot.Axes.Left.Label.Text = "Tact (s)";
            plot.Axes.AutoScale();
            TactPlot.Refresh();
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is TactTimeListViewModel viewModel)
            {
                viewModel.TactTimes.CollectionChanged += TactTimes_CollectionChanged;

                RefreshChart();
            }
        }

        private void TactTimes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshChart();
        }
    }
}
