using System.Windows;
using System.Windows.Controls;
using FrontCameraAssembleEquipment.MVVM.ViewModels;

namespace FrontCameraAssembleEquipment.MVVM.Views
{
    /// <summary>
    /// Interaction logic for NavigateMenuView.xaml
    /// </summary>
    public partial class NavigateMenuView : UserControl
    {
        public NavigateMenuView()
        {
            InitializeComponent();
        }

        private void ViewModelNavigationStore_CurrentViewModelChanged()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (this.DataContext is NavigateMenuViewModel vm)
                {
                    if (vm.NavigationStore.CurrentViewModel.GetType() == typeof(TeachViewModel))
                    {
                        TeachRdBtn.IsChecked = true;
                    }
                    if (vm.NavigationStore.CurrentViewModel.GetType() == typeof(AutoViewModel))
                    {
                        AutoRdBtn.IsChecked = true;
                    }
                    if (vm.NavigationStore.CurrentViewModel.GetType() == typeof(ManualViewModel))
                    {
                        ManualRdBtn.IsChecked = true;
                    }
                    if (vm.NavigationStore.CurrentViewModel.GetType() == typeof(DataViewModel))
                    {
                        DataRdBtn.IsChecked = true;
                    }
                    if (vm.NavigationStore.CurrentViewModel.GetType() == typeof(ErrorLogViewModel))
                    {
                        LogRdBtn.IsChecked = true;
                    }
                }
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is NavigateMenuViewModel vm)
            {
                vm.TabMenuChanged += ViewModelNavigationStore_CurrentViewModelChanged;
                vm.NavigationStore.CurrentViewModelChanged += ViewModelNavigationStore_CurrentViewModelChanged;
            }
        }
    }
}
