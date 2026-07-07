using EQX.Core.Common;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private readonly INavigationService _navigationService;
        private readonly IViewModelFactory _viewModelFactory;
        public MainWindowView(INavigationService navigationService, IViewModelFactory viewModelFactory)
        {
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;

            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<InitDeinitViewModel>();

            _viewModelFactory.Create<InitDeinitViewModel>().Initialization();

            WindowStateHelper.RegisterTaskbarClick(this, OnTaskbarClick);
        }

        private void OnTaskbarClick()
        {
            WindowStateHelper.HideTaskbar();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Environment.ExitCode != 100)
            {
                e.Cancel = true;

                _viewModelFactory.Create<HeaderViewModel>().ApplicationCloseCommand.Execute(null);
            }
        }

    }
}