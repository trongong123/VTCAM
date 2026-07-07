using EQX.Core.Recipe;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Giống <see cref="SingleRecipe"/>; cột Value hiển thị NotUse (0) / Use (1).
    /// </summary>
    public partial class SingleOptionRecipe : UserControl
    {
        public bool IsHeader
        {
            get => (bool)GetValue(IsHeaderProperty);
            set => SetValue(IsHeaderProperty, value);
        }

        public static readonly DependencyProperty IsHeaderProperty =
            DependencyProperty.Register(nameof(IsHeader), typeof(bool), typeof(SingleOptionRecipe), new PropertyMetadata(false));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(SingleOptionRecipe), new PropertyMetadata(0.0, OnValueChanged));

        public string OptionDisplayText
        {
            get => (string)GetValue(OptionDisplayTextProperty);
            set => SetValue(OptionDisplayTextProperty, value);
        }

        public static readonly DependencyProperty OptionDisplayTextProperty =
            DependencyProperty.Register(nameof(OptionDisplayText), typeof(string), typeof(SingleOptionRecipe), new PropertyMetadata("NotUse"));

        public SingleOptionRecipe(SingleRecipeDescriptionAttribute singleRecipeDescription,
            SingleRecipeMinMaxAttribute singleRecipeMinMax)
        {
            InitializeComponent();
            SingleRecipeDescription = singleRecipeDescription;
            SingleRecipeMinMax = singleRecipeMinMax;
            DataContext = this;
            RefreshOptionDisplayText();
        }

        public SingleRecipeDescriptionAttribute? SingleRecipeDescription { get; set; }
        public SingleRecipeMinMaxAttribute? SingleRecipeMinMax { get; set; }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SingleOptionRecipe c)
                c.RefreshOptionDisplayText();
        }

        private void RefreshOptionDisplayText()
        {
            double v = Value;
            if (Math.Abs(v) < 0.0001)
                OptionDisplayText = "NotUse";
            else if (Math.Abs(v - 1.0) < 0.0001)
                OptionDisplayText = "Use";
            else
                OptionDisplayText = v.ToString("0.###");
        }

        private void Value_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsHeader || SingleRecipeDescription?.IsSpacer == true)
                return;

            Value = Math.Abs(Value) < 0.5 ? 1.0 : 0.0;
        }
    }
}
