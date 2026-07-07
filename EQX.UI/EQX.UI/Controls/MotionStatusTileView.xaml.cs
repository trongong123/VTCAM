using EQX.Core.Motion;
using System.Windows.Controls;

namespace EQX.UI.Controls
{
    /// <summary>
    ///     Simple status tile for one motion axis.
    /// </summary>
    public partial class MotionStatusTileView : UserControl
    {
        public MotionStatusTileView()
        {
            InitializeComponent();
        }

        private bool IsValidMotion(out IMotion motion)
        {
            motion = DataContext as IMotion;
            return motion != null;
        }

        private void ServoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsValidMotion(out var motion)) return;

            if (motion.Status.IsMotionOn)
            {
                motion.MotionOff();
            }
            else
            {
                motion.MotionOn();
            }
        }

        private void OriginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsValidMotion(out var motion)) return;

            motion.SearchOrigin();
        }

        private void AlarmResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsValidMotion(out var motion)) return;

            motion.AlarmReset();
        }
    }
}

