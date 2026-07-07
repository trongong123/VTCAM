using EQX.Core.Motion;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for MovingView.xaml
    /// </summary>
    public partial class MovingView : UserControl
    {
        private List<string> jogSpeedList = new List<string>
        {
            (string)Application.Current.Resources["str_SuperSlow"],
            (string)Application.Current.Resources["str_Slow"],
            (string)Application.Current.Resources["str_Medium"],
            (string)Application.Current.Resources["str_High"],
        };

        private List<double> jogSpeedRates = new List<double>
        {
            .005,
            .02,
            .10,
            .20,
        };

        private List<double> absDistanceList = new List<double>
        {
            0.001,
            0.010,
            0.1,
            1,
            10,
        };

        public MovingView()
        {
            InitializeComponent();
            cbBoxStepInc.ItemsSource = jogSpeedList;
        }

        private void MoveDec_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsValid() == false) return;

            if (JogMode.IsChecked == true)
            {
                ((IMotion)DataContext).MoveJog(
                    ((IMotion)DataContext).Parameter.MaxVelocity * jogSpeedRates[cbBoxStepInc.SelectedIndex],
                    false);
            }
            else
            {
                // Move INC by 10% max speed
                ((IMotion)DataContext).MoveInc((double)cbBoxStepInc.SelectedItem * -1,
                   ((IMotion)DataContext).Parameter.MaxVelocity * .10);
            }
        }

        private void MoveButton_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsValid() == false) return;

            if (JogMode.IsChecked == true)
            {
                ((IMotion)DataContext).Stop();
            }
        }

        private void MoveInc_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsValid() == false) return;

            if (JogMode.IsChecked == true)
            {
                ((IMotion)DataContext).MoveJog(
                    ((IMotion)DataContext).Parameter.MaxVelocity * jogSpeedRates[cbBoxStepInc.SelectedIndex],
                    true);
            }
            else
            {
                // Move INC by 10% max speed
                ((IMotion)DataContext).MoveInc((double)cbBoxStepInc.SelectedItem,
                    ((IMotion)DataContext).Parameter.MaxVelocity * .10);
            }
        }

        private bool IsValid()
        {
            if (DataContext is IMotion motion == false) return false;

            return true;
        }

        private void JogAbsMode_Checked(object sender, RoutedEventArgs e)
        {
            cbBoxStepInc.ItemsSource = JogMode.IsChecked == true ? jogSpeedList : absDistanceList;
            cbBoxStepInc.SelectedIndex = 0;
        }

        private void cbBoxStepInc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JogMode.IsChecked == false) return;
            if (cbBoxStepInc.SelectedIndex < 0) cbBoxStepInc.SelectedIndex = 0;
        }
    }
}
