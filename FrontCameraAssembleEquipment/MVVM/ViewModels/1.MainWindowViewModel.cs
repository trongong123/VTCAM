using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties
        public ViewModelBase HeaderVM { get; }
        public ViewModelBase FooterVM { get; }

        public ViewModelBase CurrentFrameVM => _navigationStore.CurrentViewModel;

        public Information Information { get; }
        #endregion

        public MainWindowViewModel(NavigationStore navigationStore,
                                   IViewModelFactory viewModelFactory,
                                   Information information)
        {
            _navigationStore = navigationStore;
            Information = information;
            HeaderVM = viewModelFactory.Create<HeaderViewModel>();
            FooterVM = viewModelFactory.Create<FooterViewModel>();

            _navigationStore.CurrentViewModelChanged += FrameNavigationStore_CurrentViewModelChanged;
        }

        #region Privates methods
        private void FrameNavigationStore_CurrentViewModelChanged()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                OnPropertyChanged(nameof(CurrentFrameVM));
            }), DispatcherPriority.DataBind);
        }
        #endregion

        #region Private fields
        private readonly NavigationStore _navigationStore;
        #endregion
    }
}
