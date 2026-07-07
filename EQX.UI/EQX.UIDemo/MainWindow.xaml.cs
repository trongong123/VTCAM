using EQX.Core.Common;
using EQX.UI.Controls;
using System.Windows;

namespace EQX.UIDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAlarmService alarmService;

        public MainWindow(IAlarmService alarmService)
        {
            InitializeComponent();
            this.alarmService = alarmService;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AlarmNotifyView alarmNotifyView = new AlarmNotifyView();
            //AlarmModel alarmModel = new AlarmModel();
            //alarmModel = alarmService.GetById(1);

            AlarmNotifyView.ShowDialog(new AlarmModel());
        }

    }
}