using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.InOut;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Services.WindowServices;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class InterfaceViewModel : ViewModelBase
    {
        public IDInput In_UpstreamFrontLoadEnable => _devices.Inputs.UpStreamFrontLoadEnable;
        public IDInput In_UpstreamRearLoadEnable => _devices.Inputs.UpStreamRearLoadEnable;
        public IDInput In_DownstreamFrontLoadEnable => _devices.Inputs.DownstreamFrontLoadEnable;
        public IDInput In_DownstreamRearLoadEnable => _devices.Inputs.DownstreamRearLoadEnable;

        public IDOutput Out_UpstreamFrontLoadEnable => _devices.Outputs.UpstreamFrontLoadEnable;
        public IDOutput Out_UpstreamRearLoadEnable => _devices.Outputs.UpstreamRearLoadEnable;
        public IDOutput Out_DownstreamFrontLoadEnable => _devices.Outputs.DownstreamFrontLoadEnable;
        public IDOutput Out_DownstreamRearLoadEnable => _devices.Outputs.DownstreamRearLoadEnable;
        public bool InUpstreamFrontLoadEnableValue => In_UpstreamFrontLoadEnable.Value;
        public bool InUpstreamRearLoadEnableValue => In_UpstreamRearLoadEnable.Value;
        public bool InDownstreamFrontLoadEnableValue => In_DownstreamFrontLoadEnable.Value;
        public bool InDownstreamRearLoadEnableValue => In_DownstreamRearLoadEnable.Value;
        public bool OutUpstreamFrontLoadEnableValue => Out_UpstreamFrontLoadEnable.Value;
        public bool OutUpstreamRearLoadEnableValue => Out_UpstreamRearLoadEnable.Value;
        public bool OutDownstreamFrontLoadEnableValue => Out_DownstreamFrontLoadEnable.Value;
        public bool OutDownstreamRearLoadEnableValue => Out_DownstreamRearLoadEnable.Value;

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
                InterfaceSignalUpdated(output, EventArgs.Empty);
            }
        }

        public InterfaceViewModel(Devices devices, IWindowService windowService)
        {
            _devices = devices;
            _windowService = windowService;

            In_UpstreamFrontLoadEnable.ValueUpdated += InterfaceSignalUpdated;
            In_UpstreamRearLoadEnable.ValueUpdated += InterfaceSignalUpdated;
            In_DownstreamFrontLoadEnable.ValueUpdated += InterfaceSignalUpdated;
            In_DownstreamRearLoadEnable.ValueUpdated += InterfaceSignalUpdated;
            SubscribeOutputValueChanged(Out_UpstreamFrontLoadEnable);
            SubscribeOutputValueChanged(Out_UpstreamRearLoadEnable);
            SubscribeOutputValueChanged(Out_DownstreamFrontLoadEnable);
            SubscribeOutputValueChanged(Out_DownstreamRearLoadEnable);
        }

        private void InterfaceSignalUpdated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(InUpstreamFrontLoadEnableValue));
            OnPropertyChanged(nameof(InUpstreamRearLoadEnableValue));
            OnPropertyChanged(nameof(InDownstreamFrontLoadEnableValue));
            OnPropertyChanged(nameof(InDownstreamRearLoadEnableValue));
            OnPropertyChanged(nameof(OutUpstreamFrontLoadEnableValue));
            OnPropertyChanged(nameof(OutUpstreamRearLoadEnableValue));
            OnPropertyChanged(nameof(OutDownstreamFrontLoadEnableValue));
            OnPropertyChanged(nameof(OutDownstreamRearLoadEnableValue));
        }

        private void SubscribeOutputValueChanged(IDOutput output)
        {
            if (output is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(IDOutput.Value))
                    {
                        InterfaceSignalUpdated(s, EventArgs.Empty);
                    }
                };
            }
        }
        private readonly Devices _devices;
        private readonly IWindowService _windowService;
    }
}
