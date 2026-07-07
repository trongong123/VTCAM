using FrontCameraAssembleEquipment.Defines.LogHistory;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ErrorLogView.xaml
    /// </summary>
    public partial class ErrorLogView : UserControl
    {
        private List<ErrorLogEntry> currentLogEntries;

        public ErrorLogView()
        {
            InitializeComponent();
        }

        private void ErrorListHistoryDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ErrorLogViewModel errorLogVM == false) return;

            //await errorLogVM.ReloadLogData();
            if (errorLogVM.ClearLogSet)
            {
                errorLogVM.LoadLogSetTime();
                return;
            }
            if(errorLogVM.IsAllShift)
            {
                await errorLogVM.LoadLogData(errorLogVM.LogFilePaths);
            }
            else if(errorLogVM.IsDayShift)
            {
                await errorLogVM.LoadLogData(errorLogVM.LogFilePathsShiftDay);
            }
            else if (errorLogVM.IsNightShift)
            {
                await errorLogVM.LoadLogData(errorLogVM.LogFilePathsShiftNight);
            }
        }
    }
}
