using EQX.Core.InOut;
using EQX.Core.Process;
using EQX.Core.Units;
using FrontCameraAssembleEquipment.Define;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Process;
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
using static OpenCvSharp.Stitcher;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    /// <summary>
    /// Interaction logic for MaterialStatus.xaml
    /// </summary>
    public partial class MaterialStatusView : UserControl
    {

        public MaterialStatusView()
        {
            InitializeComponent();
        }

        private void OpenControlWindow()
        {
            if (this.DataContext is MaterialStatus materialStatus == false) return;

            MachineStatus machineStatus = App.AppHost!.Services.GetRequiredService<MachineStatus>();

            if (machineStatus.IsStandByProcessMode == false) return;

            IProcess<ESequence> process = null;
            Processes processes = App.AppHost!.Services.GetRequiredService<Processes>();

            switch (materialStatus.Name)
            {
                case "Tray IN":
                    process = processes.TrayInElevatorProcess;
                    break;
                case "Tray OUT":
                    process = processes.TrayOutElevatorProcess;
                    break;
                case "CAM Loader":
                    process = processes.TransferHeadProcess;
                    break;
                case "CAM Detach":
                    process = processes.SpongeDetachProcess;
                    break;
                case "Rotator":
                    process = processes.CameraFlipperProcess;
                    break;
                case "CAM Assy'":
                    process = processes.CameraAssembleProcess;
                    break;
                case "Detach":
                    if(materialStatus.CVLine== Defines.Process.ECVLine.Front)
                    {
                        process = processes.FrontCVSetFilmDetachProcess;
                    }
                    else
                    {
                        process = processes.RearCVSetFilmDetachProcess;
                    }
                    break;
                case "Assemble":
                    if (materialStatus.CVLine == Defines.Process.ECVLine.Front)
                    {
                        process = processes.FrontCVSetCamAssembleProcess;
                    }
                    else
                    {
                        process = processes.RearCVSetCamAssembleProcess;
                    }
                    break;
                default:
                    return;
            }

            UnitManualControlViewModel unitManualControlViewModel = App.AppHost!.Services.GetRequiredService<UnitManualControlViewModel>();

            //var vm = _serviceProvider.GetRequiredService<UnitManualControlViewModel>();
            unitManualControlViewModel.ControlUnitLoad(process);
            var window = App.AppHost!.Services.GetRequiredService<UnitManualControl>();
            window.DataContext = unitManualControlViewModel;
            bool? result = (window.ShowDialog() == true ? window.Result : null);

            if (result == true)
            {
                ((MaterialStatus)this.DataContext).Set();
                ((MaterialStatus)this.DataContext).OnStateChangedEvent(true);
            }
            else if (result == false)
            {
                ((MaterialStatus)this.DataContext).Clear();
                ((MaterialStatus)this.DataContext).OnStateChangedEvent(false);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext == null) return;
            if (this.DataContext.GetType() != typeof(MaterialStatus)) return;

            OpenControlWindow();
            //var dlg = new MaterialStatusSetDialog();
            //bool? result = (dlg.ShowDialog() == true ? dlg.Result : null);
            //if (result == true)
            //{
            //    ((MaterialStatus)this.DataContext).Set();
            //    ((MaterialStatus)this.DataContext).OnStateChangedEvent(true);
            //}
            //else if (result == false)
            //{
            //    ((MaterialStatus)this.DataContext).Clear();
            //    ((MaterialStatus)this.DataContext).OnStateChangedEvent(false);
            //}
        }
    }
}
