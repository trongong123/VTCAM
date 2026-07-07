using EQX.Core.Recipe;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for SingleRecipe.xaml
    /// </summary>
    public partial class SingleRecipe : UserControl
    {
        public bool IsHeader
        {
            get { return (bool)GetValue(IsHeaderProperty); }
            set { SetValue(IsHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderProperty =
            DependencyProperty.Register("IsHeader", typeof(bool), typeof(SingleRecipe), new PropertyMetadata(false));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SingleRecipe), new PropertyMetadata(0.0));

        public SingleRecipe(SingleRecipeDescriptionAttribute singleRecipeDescription,
            SingleRecipeMinMaxAttribute singleRecipeMinMax)
        {
            InitializeComponent();

            SingleRecipeDescription = singleRecipeDescription;
            SingleRecipeMinMax = singleRecipeMinMax;
            this.DataContext = this;
        }

        public SingleRecipeDescriptionAttribute SingleRecipeDescription { get; set; }
        public SingleRecipeMinMaxAttribute SingleRecipeMinMax { get; set; }

        private void Value_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataEditor dataEditor = new DataEditor(Value, SingleRecipeMinMax);
            if(dataEditor.ShowDialog() == true)
            {
                Value = dataEditor.NewValue;
            }
        }
    }
}
