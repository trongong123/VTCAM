using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Services.WindowServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(IWindowService windowService, IConfiguration configuration, SystemConfig systemConfig)
        {
            _windowService = windowService;
            _configuration = configuration;
            _systemConfig = systemConfig;
        }

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand<string>((password) =>
                {
                    if (password != _systemConfig.LoginPassword)
                    {
                        WindowResult = false;
                        MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_WrongPassword"]);
                        return;
                    }
                    WindowResult = true;
                });
            }
        }

        public ICommand CloseLoginViewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _windowService.Close<LoginViewModel>();
                });
            }
        }

        public bool WindowResult { get; set; }

        private IWindowService _windowService;
        private IConfiguration _configuration;
        private SystemConfig _systemConfig;
        private string SystemConfigFile => _configuration["Files:SystemConfigFile"] ?? "";
    }
}
