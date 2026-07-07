using EQX.Core.Motion;
using EQX.Core.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for MotionsSpeedConfigView.xaml
    /// </summary>
    public partial class MotionsSpeedConfigView : UserControl
    {
        public MotionsSpeedConfigView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<IMotion> MotionList
        {
            get { return (ObservableCollection<IMotion>)GetValue(MotionListProperty); }
            set { SetValue(MotionListProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MotionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MotionListProperty =
            DependencyProperty.Register("MotionList", typeof(ObservableCollection<IMotion>), typeof(MotionsSpeedConfigView), new PropertyMetadata(null));

        private void EditSpeed_Click(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            var motion = textBox?.DataContext as IMotion;

            if (motion == null || textBox?.Tag == null) return;

            string parameterType = textBox.Tag.ToString();
            var minMaxAttribute = new SingleRecipeMinMaxAttribute
            {
                Min = 0,
                Max = 2000.0,
            };

            double currentValue = 0;
            string title = "";

            switch (parameterType)
            {
                case "Velocity":
                    currentValue = motion.Parameter.Velocity;
                    title = $"Edit Speed - {motion.Name}";
                    break;
                case "Acceleration":
                    currentValue = motion.Parameter.Acceleration;
                    title = $"Edit Acceleration - {motion.Name}";
                    break;
                case "Deceleration":
                    currentValue = motion.Parameter.Deceleration;
                    title = $"Edit Deceleration - {motion.Name}";
                    break;
                default:
                    return;
            }

            if (currentValue == 0)
            {
                currentValue = 0; // Giá trị mặc định
            }

            var dataEditor = new DataEditor(currentValue, minMaxAttribute);
            dataEditor.Title = title;
            if (dataEditor.ShowDialog() == true)
            {
                switch (parameterType)
                {
                    case "Velocity":
                        motion.Parameter.Velocity = dataEditor.NewValue;
                        break;
                    case "Acceleration":
                        motion.Parameter.Acceleration = dataEditor.NewValue;
                        break;
                    case "Deceleration":
                        motion.Parameter.Deceleration = dataEditor.NewValue;
                        break;
                }
                MotionDataGrid.Items.Refresh();
            }
        }

    }
}
