using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for JogControlView.xaml
    /// </summary>
    public partial class JogControlView : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(JogControlView),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty JogSpeedItemsSourceProperty =
            DependencyProperty.Register(
                nameof(JogSpeedItemsSource),
                typeof(IEnumerable),
                typeof(JogControlView),
                new PropertyMetadata(null));

        public IEnumerable JogSpeedItemsSource
        {
            get => (IEnumerable)GetValue(JogSpeedItemsSourceProperty);
            set => SetValue(JogSpeedItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedJogSpeedProperty =
            DependencyProperty.Register(nameof(SelectedJogSpeed), typeof(object), typeof(JogControlView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedJogSpeed
        {
            get => GetValue(SelectedJogSpeedProperty);
            set => SetValue(SelectedJogSpeedProperty, value);
        }

        public JogControlView()
        {
            InitializeComponent();
        }
    }
}