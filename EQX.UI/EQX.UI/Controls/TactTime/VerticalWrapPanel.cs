using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    public class VerticalWrapPanel : Panel
    {
        public int MaxColumns
        {
            get { return (int)GetValue(MaxColumnsProperty); }
            set { SetValue(MaxColumnsProperty, value); }
        }

        public static readonly DependencyProperty MaxColumnsProperty =
            DependencyProperty.Register(
                nameof(MaxColumns),
                typeof(int),
                typeof(VerticalWrapPanel),
                new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = InternalChildren.Count;
            if (count == 0) return finalSize;

            int columns = MaxColumns;
            int rows = (int)Math.Ceiling((double)count / columns);

            double itemWidth = finalSize.Width / columns;
            double itemHeight = finalSize.Height / rows;

            for (int i = 0; i < count; i++)
            {
                UIElement child = InternalChildren[i];

                // 👉 Layout theo dọc
                int row = i % rows;
                int col = i / rows;

                double x = col * itemWidth;
                double y = row * itemHeight;

                child.Arrange(new Rect(x, y, itemWidth, itemHeight));

                // 👉 TÍNH INDEX THEO DỌC
                int displayIndex = row * columns + col + 1;

                // 👉 GÁN vào ContentPresenter
                if (child is FrameworkElement fe)
                {
                    fe.Tag = displayIndex;
                }
            }

            return finalSize;
        }
    }
}
