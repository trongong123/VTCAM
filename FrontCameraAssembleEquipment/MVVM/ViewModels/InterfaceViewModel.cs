using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Services.WindowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class InterfaceViewModel : ViewModelBase
    {
        public IDInput In_DownstreamFrontLoadEnable => _devices.Inputs.DownstreamFrontLoadEnable;
        public IDInput In_DownstreamRearLoadEnable => _devices.Inputs.DownstreamRearLoadEnable;

        public IDOutput Out_UpstreamFrontLoadEnable => _devices.Outputs.UpstreamFrontLoadEnable;
        public IDOutput Out_UpstreamRearLoadEnable => _devices.Outputs.UpstreamRearLoadEnable;
        public IDOutput Out_DownstreamFrontLoadEnable => _devices.Outputs.DownstreamFrontLoadEnable;
        public IDOutput Out_DownstreamRearLoadEnable => _devices.Outputs.DownstreamRearLoadEnable;

        public ICommand CloseIntefaceViewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                   _windowService.Close<InterfaceViewModel>(); 
                });
            }
        }

        public ICommand OutputValueSetCommand => new RelayCommand<IDOutput>(OutputValueSet);

        private void OutputValueSet(object parameter)
        {
            IDOutput output = parameter as IDOutput;
            if (output != null)
            {
                output.Value = !output.Value;
            }
        }

        public InterfaceViewModel(Devices devices, IWindowService windowService)
        {
            _devices = devices;
            _windowService = windowService;
        }
        private readonly Devices _devices;
        private readonly IWindowService _windowService;
    }
}
