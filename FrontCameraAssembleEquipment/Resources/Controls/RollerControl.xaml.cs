using EQX.Device.SpeedController;
using EQX.InOut;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
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
    /// Interaction logic for RollerControl.xaml
    /// </summary>
    public partial class RollerControl : UserControl
    {
        public RollerControl()
        {
            InitializeComponent();
            //txtbox_Speed.Text = "3000";
            //txtbox_Accel.Text = "10000";
            //txtbox_Decel.Text = "10000";
        }
        private void RunClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BD201SRollerController CV)
            {
                Devices devices = App.AppHost!.Services.GetRequiredService<Devices>();

                //if (int.TryParse(txtbox_Speed.Text, out var speed))
                //{
                //    CV.SetSpeed(speed);
                //}

                //if (int.TryParse(txtbox_Accel.Text, out var accel))
                //{
                //    CV.SetAcceleration(accel);
                //}

                //if (int.TryParse(txtbox_Decel.Text, out var decel))
                //{
                //    CV.SetDeceleration(decel);
                //}
                if (CV.Name == ESpeedController.TRAYOUT_CV_ROLLER.ToString())
                {
                    devices.Outputs.TrayOutExtCvRun.Value = true;
                }
                if(CV.Name == ESpeedController.TRAYIN_CV_ROLLER.ToString())
                {
                    devices.Outputs.TrayInExtCvRun.Value = true;
                }
                CV.Run();
            }
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BD201SRollerController CV)
            {
                Devices devices = App.AppHost!.Services.GetRequiredService<Devices>();

                if (CV.Name == ESpeedController.TRAYOUT_CV_ROLLER.ToString())
                {
                    devices.Outputs.TrayOutExtCvRun.Value = false;
                }
                if (CV.Name == ESpeedController.TRAYIN_CV_ROLLER.ToString())
                {
                    devices.Outputs.TrayInExtCvRun.Value = false;
                }
                CV.Stop();
            }
        }
    }
}
