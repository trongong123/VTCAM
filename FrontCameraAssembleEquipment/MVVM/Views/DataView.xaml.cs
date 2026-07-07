using EQX.Core.Motion;
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
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : UserControl
    {
        public DataView()
        {
            InitializeComponent();
        }
        private int index = 0;

        private void LoadRecipe(IRecipe recipe, Panel mainPanel)
        {
            PropertyInfo[] props = recipe.GetType().GetProperties();

            mainPanel.Children.Add(new SingleRecipe(null, null) { IsHeader = true });

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

                // 4. Adding spacer if it's
                if (dataAttr == null)
                {
                    throw new Exception("Attribute need to be add to recipe properties");
                }
                else if (dataAttr.IsSpacer)
                {
                    mainPanel.Children.Add(new SingleRecipe(dataAttr, null));
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
                if (prop.PropertyType.Name == nameof(Boolean) || prop.Name.Contains("Position"))
                {
                    continue;
                }
                else if (hasOptionRecipe
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

                    mainPanel.Children.Add(singleOptionRecipe);
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

                    mainPanel.Children.Add(singleRecipe);
                }
            }
        }

        private void DataView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            index = 0;
            ((DataViewModel)this.DataContext).LoadRecipeEvent += LoadAllRecipe;
            ((DataViewModel)this.DataContext).RecipeSelector.Load();
            //((DataViewModel)this.DataContext).CurrentRecipe.GlobalRecipe.SelectedLanguageLoadAllRecipe += LoadAllRecipe;
            LoadAllRecipe();
            //this.Loaded -= DataView_Loaded;
        }

        private void LoadAllRecipe()
        {
            if (this.DataContext is DataViewModel dataContext)
            {
                index = 0;
                CurrentRecipe_TabControl.Items.Clear();
                CVDelayTimeStackPanel.Children.Clear();

                // 1. GlobalRecipe tab
                var globalTab = new TabItem { Header = "Global" , Width = double.NaN };
                var globalPanel = new StackPanel();
                globalTab.Content = new ScrollViewer { Content = globalPanel };
                LoadRecipe(dataContext.CurrentRecipe.GlobalRecipe, globalPanel);
                CurrentRecipe_TabControl.Items.Add(globalTab);

                // 2. TraySuplierRecipe tab
                var trayTab = new TabItem { Header = "Tray Supplier" ,Width = double.NaN };
                var trayPanel = new StackPanel();
                trayTab.Content = new ScrollViewer { Content = trayPanel };
                LoadRecipe(dataContext.CurrentRecipe.TraySuplierRecipe, trayPanel);
                CurrentRecipe_TabControl.Items.Add(trayTab);

                // 3. TransferHeadRecipe tab
                var transferTab = new TabItem { Header = "Transfer Head" , Width = double.NaN };
                var transferPanel = new StackPanel();
                transferTab.Content = new ScrollViewer { Content = transferPanel };
                LoadRecipe(dataContext.CurrentRecipe.TrayHeadRecipe, transferPanel);
                CurrentRecipe_TabControl.Items.Add(transferTab);

                // 4. FlipperTapeDetachRecipe tab
                var flipperTab = new TabItem { Header = "Rotator SpongeDetach" , Width = double.NaN };
                var flipperPanel = new StackPanel();
                flipperTab.Content = new ScrollViewer { Content = flipperPanel };
                LoadRecipe(dataContext.CurrentRecipe.FlipperTapeDetachRecipe, flipperPanel);
                CurrentRecipe_TabControl.Items.Add(flipperTab);

                // 5. FilmDetachHeadRecipe tab
                var filmTab = new TabItem { Header = "Vinyl Detach" , Width = double.NaN };
                var filmPanel = new StackPanel();
                filmTab.Content = new ScrollViewer { Content = filmPanel };
                LoadRecipe(dataContext.CurrentRecipe.FilmDetachHeadRecipe, filmPanel);
                CurrentRecipe_TabControl.Items.Add(filmTab);

                // 6. CameraHeadRecipe tab
                var cameraTab = new TabItem { Header = "Camera Head" , Width = double.NaN };
                var cameraPanel = new StackPanel();
                cameraTab.Content = new ScrollViewer { Content = cameraPanel };
                LoadRecipe(dataContext.CurrentRecipe.CameraHeadRecipe, cameraPanel);
                CurrentRecipe_TabControl.Items.Add(cameraTab);

                // 7. SetCVRecipe tab
                //var conveyorTab = new TabItem { Header = "Set CV" };
                //var conveyorPanel = new StackPanel();
                //conveyorTab.Content = new ScrollViewer { Content = conveyorPanel };
                //LoadRecipe(dataContext.CurrentRecipe.SetConveyorRecipe, conveyorPanel);
                //CurrentRecipe_TabControl.Items.Add(conveyorTab);

                LoadRecipe(dataContext.CurrentRecipe.SetConveyorRecipe, CVDelayTimeStackPanel);

                // chọn tab đầu tiên
                CurrentRecipe_TabControl.SelectedIndex = 0;
                SelectedBackground();
                dataContext.CameraTypeSelectViewModel.UpdateCameraType();
            }
        }

        //private void LoadAllRecipe()
        //{
        //    if (this.DataContext is DataViewModel dataContext)
        //    {
        //        index = 0;
        //        CurrentRecipe_StackPanel.Children.Clear();
        //        //CurrentRecipe_StackPanel.Children.Add(new SingleRecipe(null, null) { IsHeader = true });

        //        LoadRecipe(dataContext.CurrentRecipe.GlobalRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.TraySuplierRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.TransferHeadRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.FlipperTapeDetachRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.FilmDetachHeadRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.CameraHeadRecipe, CurrentRecipe_StackPanel);
        //        LoadRecipe(dataContext.CurrentRecipe.SetCVRecipe, CurrentRecipe_StackPanel);
        //        SelectedBackground();
        //    }
        //}

        private void ReloadButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_ReloadAllData"]) == true)
            {
                if (this.DataContext is DataViewModel dataContext)
                {
                    dataContext.RecipeSelector.Load();
                    LoadAllRecipe();
                }
            }
        }

        private void ChangeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_ChangeModel"]) == true)
            {
                if (this.DataContext is DataViewModel dataContext)
                {
                    dataContext.RecipeSelector.SetCurrentModel(dataContext.SelectedRecipe);
                    LoadAllRecipe();
                }
            }
        }

        private ListBoxItem _previousSelectedItem;
        private void SelectedBackground()
        {
            if (this.DataContext is DataViewModel dataContext)
            {
                var selectedRecipe = dataContext.RecipeSelector.RecipeSetting.CurrentRecipe;
                var selectedItem = (ListBoxItem)_listBoxModel.ItemContainerGenerator.ContainerFromItem(selectedRecipe);
                if (selectedItem != null)
                {
                    if (_previousSelectedItem != null && _previousSelectedItem != selectedItem)
                    {
                        _previousSelectedItem.Background = Brushes.Transparent;
                    }
                    selectedItem.Background = Brushes.LightSkyBlue;
                    _previousSelectedItem = selectedItem;
                }

                _listBoxModel.UnselectAll();
            }
        }
    }
}
