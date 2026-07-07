using System;
using System.Collections;
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
    /// Interaction logic for IncControlView.xaml
    /// </summary>
    public partial class IncControlView : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(IncControlView),
                new PropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty IncStepProperty =
            DependencyProperty.Register("IncStep",
                                        typeof(double),
                                        typeof(IncControlView),
                                        new PropertyMetadata(0.0));
        public double IncStep
        {
            get { return (double)GetValue(IncStepProperty); }
            set { SetValue(IncStepProperty, value); }
        }

        public IncControlView()
        {
            InitializeComponent();
        }

        private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox == false) return;

            DataEditor dataEditor = new DataEditor(Convert.ToDouble(textBox.Text), null);
            dataEditor.ShowDialog();

            textBox.Text = dataEditor.NewValue.ToString();
        }
    }
}
