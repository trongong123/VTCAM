using EQX.Core.Recipe;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for SinglePositionTeaching.xaml
    /// </summary>
    public partial class SinglePositionTeaching : UserControl, INotifyPropertyChanged
    {
        public SinglePositionTeaching(SingleRecipeDescriptionAttribute singleRecipeDescription,
           SinglePositionTeachingAttribute singlePosition)
        {
            InitializeComponent();

            SingleRecipeDescription = singleRecipeDescription;
            SinglePosition = singlePosition;
            this.DataContext = this;
        }

        public bool IsChangeEnable { get; set; } = true;

        public bool IsSaveEnable { get; set; } = true;

        public bool IsHeader
        {
            get { return (bool)GetValue(IsHeaderProperty); }
            set { SetValue(IsHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHeaderProperty =
            DependencyProperty.Register("IsHeader", typeof(bool), typeof(SinglePositionTeaching), new PropertyMetadata(false));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SinglePositionTeaching), new PropertyMetadata(0.0));

        public ICommand MovePositionTeachingCommand
        {
            get { return (ICommand)GetValue(MovePositionTeachingCommandProperty); }
            set { SetValue(MovePositionTeachingCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MovePositionTeachingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MovePositionTeachingCommandProperty =
            DependencyProperty.Register("MovePositionTeachingCommand", typeof(ICommand), typeof(SinglePositionTeaching), new PropertyMetadata(null));

        public ICommand GetCurrentPositionCommand
        {
            get { return (ICommand)GetValue(GetCurrentPositionCommandProperty); }
            set { SetValue(GetCurrentPositionCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for GetCurrentPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GetCurrentPositionCommandProperty =
            DependencyProperty.Register("GetCurrentPositionCommand", typeof(ICommand), typeof(SinglePositionTeaching), new PropertyMetadata(null));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(SinglePositionTeaching), new PropertyMetadata(null));

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SingleRecipeDescriptionAttribute SingleRecipeDescription { get; set; }
        public SinglePositionTeachingAttribute SinglePosition { get; set; }

        private void Value_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsChangeEnable == false) return;

            DataEditor dataEditor = new DataEditor(Value, null!);
            if (dataEditor.ShowDialog() == true)
            {
                Value = dataEditor.NewValue;
            }
        }

        private bool isOnPosition;

        public bool IsOnPosition
        {
            get { return isOnPosition; }
            set { isOnPosition = value; OnPropertyChanged(); }
        }

    }
}
