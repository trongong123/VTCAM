using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
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
using System.Windows.Shapes;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ProductionInfoView.xaml
    /// </summary>
    public partial class ProductionInfoView : Window
    {
        public ProductionInfoView()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).DragMove();
        }
        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho nhập số
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            TextBox tb = sender as TextBox;

            // Tự thêm dấu : sau 2 số đầu
            if (tb.Text.Length == 1)
            {
                tb.Text += e.Text + ":";
                tb.CaretIndex = tb.Text.Length;
                e.Handled = true; // Đã xử lý
            }
        }
        private void TimeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Cấm xóa dấu :
            if ((e.Key == Key.Back || e.Key == Key.Delete) && tb.SelectionStart == 3)
                e.Handled = true;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //this.Close(); // Đóng cửa sổ hiện tại
        }
    }
}
