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
using EQX.Device.SpeedController;
using EQX.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for RollerControl.xaml
    /// </summary>
    public partial class RollerControl : UserControl
    {
        public static readonly DependencyProperty IsRunStatusProperty =
            DependencyProperty.Register(nameof(IsRunStatus), typeof(bool), typeof(RollerControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsStopStatusProperty =
            DependencyProperty.Register(nameof(IsStopStatus), typeof(bool), typeof(RollerControl), new PropertyMetadata(true));

        public bool IsRunStatus
        {
            get => (bool)GetValue(IsRunStatusProperty);
            set => SetValue(IsRunStatusProperty, value);
        }

        public bool IsStopStatus
        {
            get => (bool)GetValue(IsStopStatusProperty);
            set => SetValue(IsStopStatusProperty, value);
        }
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
                SetTrayRollerParameter(CV);
                SetExternalTrayConveyorRunOutput(devices, CV, true);
                CV.Run();
                IsRunStatus = true;
                IsStopStatus = false;
            }
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BD201SRollerController CV)
            {
                Devices devices = App.AppHost!.Services.GetRequiredService<Devices>();

                SetExternalTrayConveyorRunOutput(devices, CV, false);
                CV.Stop();
                IsRunStatus = false;
                IsStopStatus = true;
            }
        }

        private static void SetTrayRollerParameter(BD201SRollerController roller)
        {
            RecipeList recipeList = App.AppHost!.Services.GetRequiredService<RecipeList>();
            TraySuplierRecipe traySuplierRecipe = recipeList.TraySuplierRecipe;

            roller.SetDirection(true);
            roller.SetSpeed(GetTrayRollerSpeed(roller, traySuplierRecipe));
            roller.SetAcceleration((int)traySuplierRecipe.Acceleration);
            roller.SetDeceleration((int)traySuplierRecipe.Deceleration);
        }

        private static int GetTrayRollerSpeed(BD201SRollerController roller, TraySuplierRecipe traySuplierRecipe)
        {
            return (ESpeedController)roller.Id switch
            {
                ESpeedController.TRAYIN_CV_ROLLER => (int)traySuplierRecipe.CVTrayInSpeed,
                ESpeedController.TRAYIN_ELEVATOR_ROLLER => (int)traySuplierRecipe.CVTrayInElevatorSpeed,
                ESpeedController.TRAYOUT_CV_ROLLER => (int)traySuplierRecipe.CVTrayOutSpeed,
                ESpeedController.TRAYOUT_ELEVATOR_ROLLER => (int)traySuplierRecipe.CVTrayOutElevatorSpeed,
                _ => 0,
            };
        }

        private static void SetExternalTrayConveyorRunOutput(Devices devices, BD201SRollerController roller, bool isRun)
        {
            switch ((ESpeedController)roller.Id)
            {
                case ESpeedController.TRAYOUT_CV_ROLLER:
                    devices.Outputs.TrayOutCVRollerRun.Value = isRun;
                    break;
                case ESpeedController.TRAYIN_CV_ROLLER:
                    devices.Outputs.TrayInCVRollerRun.Value = isRun;
                    break;
                case ESpeedController.TRAYIN_ELEVATOR_ROLLER:
                    devices.Outputs.TrayInLiftRollerRun.Value = isRun;
                    break;
                case ESpeedController.TRAYOUT_ELEVATOR_ROLLER:
                    devices.Outputs.TrayOutLiftRollerRun.Value = isRun;
                    break;
            }
        }
    }
}
