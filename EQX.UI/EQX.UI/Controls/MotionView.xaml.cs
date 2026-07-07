using EQX.Core.Motion;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for MotionView.xaml
    /// </summary>
    public partial class MotionView : UserControl
    {
        public double MaxJogSpeed
        {
            get { return (double)GetValue(MaxJogSpeedProperty); }
            set { SetValue(MaxJogSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxJogSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxJogSpeedProperty =
            DependencyProperty.Register("MaxJogSpeed", typeof(double), typeof(MotionView), new PropertyMetadata(100.0));

        private List<string> jogSpeedList = new List<string>
        {
            (string)Application.Current.Resources["str_SuperSlow"],
            (string)Application.Current.Resources["str_Slow"],
            (string)Application.Current.Resources["str_Medium"],
            (string)Application.Current.Resources["str_High"],
        };

        private double jogSpeedRate = 5;

        private List<double> jogSpeedRates = new List<double>
        {
            .05,
            .1,
            .5,
            1,
        };

        private List<double> absDistanceList = new List<double>
        {
            0.001,
            0.010,
            0.1,
            1,
            10,
        };

        public MotionView()
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
                    Math.Min(((IMotion)DataContext).Parameter.MaxVelocity, MaxJogSpeed) * jogSpeedRates[cbBoxStepInc.SelectedIndex],
                    false);
            }
            else
            {
                // Move INC by 10% max speed
                ((IMotion)DataContext).MoveInc((double)cbBoxStepInc.SelectedItem * -1,
                   Math.Min(((IMotion)DataContext).Parameter.MaxVelocity, MaxJogSpeed) * .10);
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
                    Math.Min(((IMotion)DataContext).Parameter.MaxVelocity, MaxJogSpeed) * jogSpeedRates[cbBoxStepInc.SelectedIndex],
                    true);
            }
            else
            {
                // Move INC by 10% max speed
                ((IMotion)DataContext).MoveInc((double)cbBoxStepInc.SelectedItem,
                    Math.Min(((IMotion)DataContext).Parameter.MaxVelocity, MaxJogSpeed) * .10);
            }
        }

        private bool IsValid()
        {
            if (DataContext is  IMotion motion == false) return false;

            return true;
        }

        private void cbBoxStepInc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JogMode.IsChecked == false) return;
            if (cbBoxStepInc.SelectedIndex < 0) cbBoxStepInc.SelectedIndex = 0;

            jogSpeedRate = jogSpeedRates[cbBoxStepInc.SelectedIndex];
        }

        private void ButtonServoOn_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() == false) return;

            if (((IMotion)DataContext).Status.IsMotionOn)
            {
                ((IMotion)DataContext).MotionOff();
            }
            else
            {
                ((IMotion)DataContext).MotionOn();
            }
        }

        private void ButtonOrigin_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() == false) return;

            ((IMotion)DataContext).SearchOrigin();
        }
        private void ButtonResetAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() == false) return;

            ((IMotion)DataContext).AlarmReset();
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() == false) return;

            ((IMotion)DataContext).Stop();
        }
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() == false) return;
            if (((IMotion)DataContext).IsConnected)
            {
                ((IMotion)DataContext).Disconnect();
            }
            else
            {
                ((IMotion)DataContext).Connect();
            }
        }

        private void JogMode_Click(object sender, RoutedEventArgs e)
        {
            cbBoxStepInc.ItemsSource = JogMode.IsChecked == true ? jogSpeedList : absDistanceList;
            cbBoxStepInc.SelectedIndex = 0;
        }
    }
}
