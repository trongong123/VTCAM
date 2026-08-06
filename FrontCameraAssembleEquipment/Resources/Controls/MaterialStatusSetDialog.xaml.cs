using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for MaterialStatusSetDialog.xaml
    /// </summary>
    public partial class MaterialStatusSetDialog : Window
    {
        public MaterialStatusSetDialog()
        {
            InitializeComponent();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).DragMove();
        }

        public bool? Result { get; private set; } = null;

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = true;
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!DialogResult.HasValue)
                Result = null;
        }

        private void Button_CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
