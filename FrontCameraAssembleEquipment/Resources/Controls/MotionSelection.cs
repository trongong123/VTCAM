using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Motion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public static class MotionSelection
    {
        private static Dictionary<IMotion, bool> Map = new();

        public static void MapReset()
        {
            Map = new();
        }

        public static bool GetIsSelected(DependencyObject obj)
            => (bool)obj.GetValue(IsSelectedProperty);

        public static void SetIsSelected(DependencyObject obj, bool value)
            => obj.SetValue(IsSelectedProperty, value);

        public static void SetSelection(IMotion motion, bool value)
        {
            Map[motion] = value;
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsSelected",
                typeof(bool),
                typeof(MotionSelection),
                new FrameworkPropertyMetadata(false, OnChanged));

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                if (fe.DataContext is IMotion motionFromDC)
                {
                    Map[motionFromDC] = (bool)e.NewValue;
                    return;
                }

                if (fe is ContentControl cc && cc.Content is IMotion motionFromContent)
                {
                    Map[motionFromContent] = (bool)e.NewValue;
                    return;
                }
            }
        }

        public static bool IsSelected(IMotion motion)
            => Map.TryGetValue(motion, out bool v) && v;
    }
}
