using System.Windows;
using EQX.Core.Common;
using FrontCameraAssembleEquipment.Helpers;
using FrontCameraAssembleEquipment.MVVM.ViewModels;

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