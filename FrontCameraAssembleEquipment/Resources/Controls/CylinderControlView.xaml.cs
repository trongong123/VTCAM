using EQX.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for CylinderControlView.xaml
    /// </summary>
    public partial class CylinderControlView : UserControl
    {
        public CylinderControlView()
        {
            InitializeComponent();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CylinderBase cylinder == false) return;

            if (cylinder.CylinderType == EQX.Core.InOut.ECylinderType.ForwardBackwardReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.UpDownReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.OpenCloseReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.RightLeftReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.LockUnlockReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.GripUngripReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.FlipUnflipReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.ClampUnclampReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.AlignUnalignReverse)
            {
                cylinder.Backward();
                return;
            }

            cylinder.Forward();
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CylinderBase cylinder == false) return;

            if (cylinder.CylinderType == EQX.Core.InOut.ECylinderType.ForwardBackwardReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.UpDownReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.OpenCloseReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.RightLeftReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.LockUnlockReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.GripUngripReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.FlipUnflipReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.ClampUnclampReverse ||
               cylinder.CylinderType == EQX.Core.InOut.ECylinderType.AlignUnalignReverse)
            {
                cylinder.Forward();
                return;
            }

            cylinder.Backward();
        }
    }
}
