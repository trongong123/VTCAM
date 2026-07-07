using EQX.Core.Motion;
using EQX.Core.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.Resources.Controls;
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

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for OriginView.xaml
    /// </summary>
    public partial class OriginView : Window
    {
        private Dictionary<IMotion, Action> _axisActionMap = new();
        public OriginView()
        {
            InitializeComponent();

            Loaded += (o, b) => MotionSelection.MapReset();
            Loaded += OriginView_Loaded;
        }

        private void OriginView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is OriginViewModel originVM == false) return;

            _axisActionMap.Clear();

            _axisActionMap.Add(originVM.Motions.TrayInputZ, () => originVM.Processes.TrayInElevatorProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.TrayOutputZ, () => originVM.Processes.TrayOutElevatorProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.TrayHeadXAxis, () => originVM.Processes.TransferHeadProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.TrayHeadYAxis, () => originVM.Processes.TransferHeadProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.TrayHeadZAxis, () => originVM.Processes.TransferHeadProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.FilmDetachY, () => originVM.Processes.FilmDetachProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.AssemblePickPlaceX, () => originVM.Processes.CameraAssembleProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.AssemblePickPlaceY, () => originVM.Processes.CameraAssembleProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.AssemblePickPlaceZ, () => originVM.Processes.CameraAssembleProcess.IsOriginOrInitSelected = true);
            _axisActionMap.Add(originVM.Motions.AssemblePickPlaceRX, () => originVM.Processes.CameraAssembleProcess.IsOriginOrInitSelected = true);
        }

        private void OnAlarmResetClicked(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedMotions().ToList();
            foreach(var motion in selected)
            {
                motion.AlarmReset();
            }
        }

        private void OnServoOnClicked(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedMotions().ToList();
            foreach (var motion in GetSelectedMotions())
            {
                motion.MotionOn();
            }
        }

        private void OnServoOffClicked(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedMotions().ToList();
            foreach (var motion in GetSelectedMotions())
            {
                motion.MotionOff();
            }
        }

        private void OnAxisClicked(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is OriginViewModel originVM == false) return;
            if (sender is ContentControl cc)
            {
                bool cur = MotionSelection.GetIsSelected(cc);
                MotionSelection.SetIsSelected(cc, !cur);
            }

            originVM.Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
            CheckAxisToProcessOrigin();
        }

        private void OnSelectAllClicked(object sender, RoutedEventArgs e)
        {
            SelectAll();

            if (DataContext is OriginViewModel originVM == false) return;
            originVM.Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = true);
        }

        private void OnDeselectAllClicked(object sender, RoutedEventArgs e)
        {
            UnSelectAll();

            if (DataContext is OriginViewModel originVM == false) return;
            originVM.Processes.RootProcess.Childs!.ToList().ForEach(p => p.IsOriginOrInitSelected = false);
        }

        public IEnumerable<IMotion> GetSelectedMotions()
        {
            foreach (var cc in FindVisualChildren<ContentControl>(AxisCanvas))
            {
                if (cc.Content is IMotion motion &&
                    MotionSelection.GetIsSelected(cc))
                {
                    yield return motion;
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null) yield break;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    yield return t;

                foreach (var sub in FindVisualChildren<T>(child))
                    yield return sub;
            }
        }

        private void SelectAll()
        {
            foreach (var cc in FindVisualChildren<ContentControl>(AxisCanvas))
                MotionSelection.SetIsSelected(cc, true);
        }

        private void UnSelectAll()
        {
            foreach (var cc in FindVisualChildren<ContentControl>(AxisCanvas))
                MotionSelection.SetIsSelected(cc, false);
        }

        private void CheckAxisToProcessOrigin()
        {
            if (DataContext is OriginViewModel originVM == false) return;

            foreach (var action in _axisActionMap)
            {
                if(MotionSelection.IsSelected(action.Key) == true)
                {
                    action.Value.Invoke();
                }
            }
        }
    }
}
