using EQX.Core.Recipe;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EQX.UI.Controls
{
    public partial class PositionPointGridView : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(ObservableCollection<PositionPoint>),
                typeof(PositionPointGridView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public ObservableCollection<PositionPoint> ItemsSource
        {
            get => (ObservableCollection<PositionPoint>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public PositionPointGridView()
        {
            InitializeComponent();
            // Thêm các tiêu đề hàng tĩnh
            AddStaticRowHeaders();
        }

        private void AddStaticRowHeaders()
        {
            var emptyHeader = new TextBlock { Text = "", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(emptyHeader, 0, 0));

            var targetHeader = new TextBlock { Text = "Target Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(targetHeader, 1, 0));

            var fixedHeader = new TextBlock { Text = "Fixed Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(fixedHeader, 2, 0));

            var offsetHeader = new TextBlock { Text = "Offset Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(offsetHeader, 3, 0));

            var modelHeader = new TextBlock { Text = "Model Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(modelHeader, 4, 0));

            var visionHeader = new TextBlock { Text = "Vision Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(visionHeader, 5, 0));

            var currentHeader = new TextBlock { Text = "Current Pos", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(currentHeader, 6, 0));

            var diffHeader = new TextBlock { Text = "Difference", FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap };
            MainGrid.Children.Add(CreateCell(diffHeader, 7, 0));
        }

        private UIElement CreateCell(UIElement content, int row, int column)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                Child = content
            };
            border.SetResourceReference(Border.BorderBrushProperty, "DarkBackgroundColor");

            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            return border;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PositionPointGridView view) return;

            view.RebuildGrid();

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= view.OnCollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += view.OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Để đơn giản, ta xây dựng lại toàn bộ grid khi collection thay đổi.
            Dispatcher.Invoke(RebuildGrid);
        }

        private void RebuildGrid()
        {
            // Xóa các cột và nội dung động đã có
            if (MainGrid.ColumnDefinitions.Count > 1)
                MainGrid.ColumnDefinitions.RemoveRange(1, MainGrid.ColumnDefinitions.Count - 1);

            var childrenToRemove = MainGrid.Children.Cast<UIElement>()
                                         .Where(c => Grid.GetColumn(c) > 0).ToList();
            foreach (var child in childrenToRemove)
            {
                MainGrid.Children.Remove(child);
            }

            if (ItemsSource == null || !ItemsSource.Any()) return;

            var points = ItemsSource.ToList();

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                int columnIndex = i + 1;

                // 1. Thêm một cột mới cho trục
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // 2. Thêm tiêu đề cột (tên trục)
                var axisHeader = new TextBlock { Text = point.Motion.Name, FontWeight = FontWeights.Bold };
                MainGrid.Children.Add(CreateCell(axisHeader, 0, columnIndex));

                // 3. Thêm ô hiển thị Target Position (tổng)
                var targetTextBlock = new TextBlock();
                targetTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(PositionPoint.TargetPos)) { Source = point, Mode = BindingMode.OneWay, StringFormat = "F3" });
                MainGrid.Children.Add(CreateCell(targetTextBlock, 1, columnIndex));

                // 4. Thêm ô nhập liệu cho Fixed Position
                var fixedTextBox = CreateEditablePositionTextBox(point, nameof(PositionPoint.FixedPos));
                MainGrid.Children.Add(CreateCell(fixedTextBox, 2, columnIndex));

                // 5. Thêm ô nhập liệu cho Offset Position
                var offsetTextBox = CreateEditablePositionTextBox(point, nameof(PositionPoint.OffsetPos));
                MainGrid.Children.Add(CreateCell(offsetTextBox, 3, columnIndex));

                // 6. Thêm ô hiển thị Model Position
                var modelTextBlock = new TextBlock();
                modelTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(PositionPoint.ModelPos)) { Source = point, Mode = BindingMode.OneWay, StringFormat = "F3" });
                MainGrid.Children.Add(CreateCell(modelTextBlock, 4, columnIndex));

                // 7. Thêm ô hiển thị Vision Position
                var visionTextBlock = new TextBlock();
                visionTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(PositionPoint.VisionPos)) { Source = point, Mode = BindingMode.OneWay, StringFormat = "F3" });
                MainGrid.Children.Add(CreateCell(visionTextBlock, 5, columnIndex));

                // 8. Thêm ô hiển thị Current Position
                var currentTextBlock = new TextBlock();
                currentTextBlock.SetBinding(TextBlock.TextProperty, new Binding("Motion.Status.ActualPosition") { Source = point, Mode = BindingMode.OneWay, StringFormat = "F3" });
                MainGrid.Children.Add(CreateCell(currentTextBlock, 6, columnIndex));

                // 9. Thêm ô hiển thị Difference
                var diffTextBlock = new TextBlock();
                var multiBinding = new MultiBinding { Converter = (IMultiValueConverter)Resources["DifferenceConverter"], StringFormat = "F3" };
                multiBinding.Bindings.Add(new Binding(nameof(PositionPoint.TargetPos)) { Source = point, Mode = BindingMode.OneWay });
                multiBinding.Bindings.Add(new Binding("Motion.Status.ActualPosition") { Source = point, Mode = BindingMode.OneWay });
                diffTextBlock.SetBinding(TextBlock.TextProperty, multiBinding);
                MainGrid.Children.Add(CreateCell(diffTextBlock, 7, columnIndex));
            }
        }

        private TextBox CreateEditablePositionTextBox(PositionPoint point, string propertyName)
        {
            var textBox = new TextBox();
            textBox.IsReadOnly = true;
            textBox.PreviewMouseUp += TargetTextBox_PreviewMouseUp;
            textBox.SetBinding(TextBox.TextProperty, new Binding(propertyName)
            {
                Source = point,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                StringFormat = "F3"
            });
            return textBox;
        }

        private void TargetTextBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox == false) return;

            DataEditor dataEditor = new DataEditor(Convert.ToDouble(textBox.Text), null!);
            if (dataEditor.ShowDialog() == true)
            {
                textBox.Text = dataEditor.NewValue.ToString();
            }
        }
    }
}