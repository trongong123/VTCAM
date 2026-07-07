using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.UI.Controls;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace EQX.UI.MVVM
{
    /// <summary>
    /// Interaction logic for TeachPositionUnitView.xaml
    /// </summary>
    public partial class TeachPositionUnitView : UserControl
    {
        private NonOverlappingTimer positionUpdateTimer;

        public TeachPositionUnitView()
        {
            InitializeComponent();

            positionUpdateTimer = new NonOverlappingTimer(100);
            positionUpdateTimer.Elapsed += positionUpdateTimerElapsed;
        }

        private void positionUpdateTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Application.Current == null) return;

            if (Application.Current.Dispatcher == null) return;

            if (Application.Current?.Dispatcher?.HasShutdownStarted == true)
                return;

            try
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (this.DataContext is TeachPositionUnitViewModelBase unitTeachingViewModel == false) return;
                    foreach (var positionTeaching in PositionTeaching_StackPanel.Children)
                    {
                        if (positionTeaching is SinglePositionTeaching spt == false) continue;
                        if (spt.SinglePosition == null) continue;

                        var motion = unitTeachingViewModel.Motions
                           .FirstOrDefault(m => m.Name.Contains(spt.SinglePosition.Motion));
                        if (motion == null) continue;

                        spt.IsOnPosition = motion.IsOnPosition(spt.Value);
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

        private void TeachPositionUnitView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is TeachPositionUnitViewModelBase teachViewModel)
            {
                teachViewModel.SelectedUnitChanged += (s, e) =>
                {
                    LoadPositionTeaching();
                };
            }

            LoadPositionTeaching();
            positionUpdateTimer.Start();
        }

        private void TeachPositionUnitView_Unloaded(object sender, RoutedEventArgs e)
        {
            positionUpdateTimer.Stop();
        }

        private void LoadPositionTeaching()
        {
            if (this.DataContext is TeachPositionUnitViewModelBase teachViewModel == false) return;
            PositionTeaching_StackPanel.Children.Clear();
            PositionTeaching_StackPanel.Children.Add(new SinglePositionTeaching(null, null) { IsHeader = true, IsSaveEnable = false });

            int index = 0;

            foreach (var recipe in teachViewModel.Recipes)
            {
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
                        PositionTeaching_StackPanel.Children.Add(new SinglePositionTeaching(dataAttr, null));
                        continue;
                    }

                    // 5. Extract SingleRecipePositionAttribute
                    SinglePositionTeachingAttribute positionAttribute = null;
                    if (attrs.FirstOrDefault(att => att is SinglePositionTeachingAttribute) != null)
                    {
                        positionAttribute = (SinglePositionTeachingAttribute)attrs.FirstOrDefault(att => att is SinglePositionTeachingAttribute);
                    }

                    if (positionAttribute == null) continue;

                    // 6. Add recipe DataView to the view
                    if (prop.PropertyType.Name == nameof(Double)
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

                        SinglePositionTeaching singlePositionTeaching = new SinglePositionTeaching(dataAttr, positionAttribute);
                        singlePositionTeaching.SetBinding(SinglePositionTeaching.ValueProperty, binding);
                        singlePositionTeaching.IsSaveEnable = false;
                        singlePositionTeaching.FontSize = 12;

                        singlePositionTeaching.MovePositionTeachingCommand = teachViewModel.MovePositionTeachingCommand;
                        singlePositionTeaching.GetCurrentPositionCommand = teachViewModel.GetCurrentPositionCommand;
                        PositionTeaching_StackPanel.Children.Add(singlePositionTeaching);
                    }
                }
            }
        }

        private void MoveButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is TeachPositionUnitViewModelBase teachViewModel == false) return;
            if (sender is Button button == false) return;
            if (button.DataContext is IMotion motion == false) return;
            if (button.Tag.ToString() is string buttonTag == false) return;
            if (motion == null) return;

            if (teachViewModel.IsMoveJogMode)
            {
                motion.Stop();
            }
        }

        private void MoveButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is TeachPositionUnitViewModelBase teachViewModel == false) return;
            if (sender is Button button == false) return;
            if (button.DataContext is IMotion motion == false) return;
            if (button.Tag.ToString() is string buttonTag == false) return;
            if (motion == null) return;

            if (teachViewModel.IsMoveJogMode)
            {
                if (buttonTag == "Minus")
                {
                    motion.MoveJog(jogSpeedRates[teachViewModel.JogSpeedIndexSelected], false);
                }
                else
                {
                    motion.MoveJog(jogSpeedRates[teachViewModel.JogSpeedIndexSelected], true);
                }
            }
            else
            {
                if (buttonTag == "Minus")
                {
                    motion.MoveInc(teachViewModel.IncStepSelected * -1.0);
                }
                else
                {
                    motion.MoveInc(teachViewModel.IncStepSelected);
                }
            }
        }

        private List<double> jogSpeedRates = new List<double>
        {
            1.0,
            5.0,
            30.0,
            70.0,
        };
    }
}
