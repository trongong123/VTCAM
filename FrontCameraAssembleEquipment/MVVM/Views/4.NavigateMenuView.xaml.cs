using EQX.Core.Common;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
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
