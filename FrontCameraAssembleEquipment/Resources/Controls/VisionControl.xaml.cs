using EQX.InOut;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Vision;
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
    /// Interaction logic for VisionControl.xaml
    /// </summary>
    public partial class VisionControl : UserControl
    {
        public VisionControl()
        {
            InitializeComponent();
            
        }
        private string selectedInspection;

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selected = sender as RadioButton;
            if (selected != null)
            {
                selectedInspection = selected.Content.ToString();
            }
        }

        private void Inspection_Click(object sender, RoutedEventArgs e)
        {
            Vision_Result ResultData = new Vision_Result();
            if (sender is Button btn && btn.DataContext is VisionProcess vision)
            {
                switch(selectedInspection)
                {
                    case "Rear FILM":
                        ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.REARFILM);
                        break;
                    case "Front FILM":
                        ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.FRONTFILM);
                        break;
                    case "Rear CAM":
                        ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.REARASSY);
                        break;
                    case "Front CAM":
                        ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.FRONTASSY);
                        break;
                    case "BCR":
                        ResultData = vision.Vision.SendRequestInspection(INSPECTION_NAME.BARCODE);
                        break;
                }
                string strResultData;
                if (ResultData.eResult == INSPECTION_RESULT.SUCCESS)
                {
                    strResultData = string.Format($"Inspection Success! [{ResultData.eResult.ToString()}], [{ResultData.strBarcode}]");

                }
                else
                {
                    strResultData = string.Format("Inspection Fail! [{0}]", ResultData.eResult.ToString());
                }

                MessageBoxEx.ShowDialog(strResultData);
            }
        }
    }
}
