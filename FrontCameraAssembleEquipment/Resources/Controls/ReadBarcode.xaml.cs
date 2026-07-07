using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Vision;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public partial class ReadBarcode : UserControl
    {
        private CognexTrigger _reader;
        public GlobalRecipe _globalRecipe;

        public ReadBarcode()
        {
            InitializeComponent();
            InitBarcodeReader();

            // Khi control bị unload → đóng COM
            this.Unloaded += ReadBarcode_Unloaded;
        }

        private void InitBarcodeReader()
        {
            try
            {
                _reader = new CognexTrigger();

                // Mở COM
                //_reader.OpenPort();

                // Đăng ký sự kiện nhận dữ liệu
                _reader.DataReceivedEvent += Reader_DataReceived;
            }
            catch (Exception ex)
            {
                txtbox_Barcode.Text = "Init ERR: " + ex.Message;
            }
        }

        private void Reader_DataReceived(string data)
        {
            Dispatcher.Invoke(() =>
            {
                txtbox_Barcode.Text = data.Trim();
            });
        }

        private void RunClick(object sender, RoutedEventArgs e)
        {
            Vision_Result ResultData = new Vision_Result();
            if (sender is Button btn && btn.DataContext is VisionProcess vision)
            {
                ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.BARCODE);
            }
        }

        private void ReadBarcode_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reader != null)
                {
                    _reader.DataReceivedEvent -= Reader_DataReceived;
                    _reader.Close();
                }
            }
            catch { }
        }
    }
}
