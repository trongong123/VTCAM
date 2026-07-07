using EQX.Core.Recipe;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for DevView.xaml
    /// </summary>
    public partial class DevView : UserControl
    {
        public DevView()
        {
            InitializeComponent();
        }

        private void LoadRecipe(IRecipe recipe)
        {
            //Clear Recipe
            OptionsRecipe_StackPanel.Children.Clear();

            int index = 0;

            PropertyInfo[] props = recipe.GetType().GetProperties();

            foreach (PropertyInfo prop in props)
            {
                // 1. Get all attributes of property
                List<object> attrs = prop.GetCustomAttributes(false).ToList();

                // 2. Ignore properties of base clas (Head / Name properties)
                if (attrs.Count <= 0) continue;

                // 3. Get DataDescriptionAttribute
                if (attrs.FirstOrDefault(att => (att is SingleRecipeDescriptionAttribute)) == null)
                {
                    continue;
                }
                SingleRecipeDescriptionAttribute dataAttr = (SingleRecipeDescriptionAttribute)attrs.First(att => (att as SingleRecipeDescriptionAttribute) != null);
                if (dataAttr.DescriptionKey != null)
                    dataAttr.Description = Application.Current.Resources[dataAttr.DescriptionKey].ToString();
                if (dataAttr.DetailKey != null)
                    dataAttr.Detail = Application.Current.Resources[dataAttr.DetailKey].ToString();
                // 4. Adding spacer if it's
                if (dataAttr == null)
                {
                    throw new Exception("Attribute need to be add to recipe properties");
                }
                else if (dataAttr.IsSpacer)
                {
                    continue;
                }

                // 5. Extract DataMinMaxAtrribute
                SingleRecipeMinMaxAttribute minMaxAttribute = null;
                if (attrs.FirstOrDefault(att => att is SingleRecipeMinMaxAttribute) != null)
                {
                    minMaxAttribute = (SingleRecipeMinMaxAttribute)attrs.FirstOrDefault(att => att is SingleRecipeMinMaxAttribute);
                }

                bool hasOptionRecipe = attrs.Any(att => att is OptionRecipeAttribute);

                // 6. Add recipe DataView to the view
                if (hasOptionRecipe
                    && (prop.PropertyType.Name == nameof(Double)
                        || prop.PropertyType.Name == nameof(Int32) || prop.PropertyType.Name == nameof(UInt32)
                        || prop.PropertyType.Name == nameof(Int64) || prop.PropertyType.Name == nameof(UInt64)))
                {
                    dataAttr.Index = ++index;
                    Binding binding = new Binding(prop.Name)
                    {
                        Source = recipe,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    SingleOptionRecipe singleOptionRecipe = new SingleOptionRecipe(dataAttr, minMaxAttribute);
                    singleOptionRecipe.SetBinding(SingleOptionRecipe.ValueProperty, binding);
                    OptionsRecipe_StackPanel.Children.Add(singleOptionRecipe);
                }
                else if (prop.PropertyType.Name == nameof(Double)
                    || prop.PropertyType.Name == nameof(Int32) || prop.PropertyType.Name == nameof(UInt32)
                    || prop.PropertyType.Name == nameof(Int64) || prop.PropertyType.Name == nameof(UInt64))
                {
                    dataAttr.Index = ++index;
                    Binding binding = new Binding(prop.Name)
                    {
                        Source = recipe,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    SingleRecipe singleRecipe = new SingleRecipe(dataAttr, minMaxAttribute);
                    singleRecipe.SetBinding(SingleRecipe.ValueProperty, binding);
                    OptionsRecipe_StackPanel.Children.Add(singleRecipe);
                }
                else if (prop.PropertyType.Name == nameof(Boolean))
                {
                    Binding binding = new Binding(prop.Name)
                    {
                        Source = recipe,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    CheckBox checkBox = new CheckBox()
                    {
                        Content = dataAttr.Description,
                        Tag = dataAttr.Detail,
                        Margin = new Thickness(5),
                    };

                    checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);

                    OptionsRecipe_StackPanel.Children.Add(checkBox);

                    continue;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is DevViewModel devVM == false) return;

            LoadRecipe(devVM.DevRecipe);
        }
    }
}
