using EQX.Core.InOut;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EQX.InOut.InputSimulation.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum EInputs
        {
            Start,
            Stop,
            EMO,
            Touch
        }

        public MainWindow()
        {
            InitializeComponent();

            SimulationViewModel = new MMFInputSimulationViewModel<EInputs>(
                new List<string>()
                {
                    EInputs.Start.ToString(),
                    EInputs.Touch.ToString()
                }, new List<string>
                {
                    EInputs.EMO.ToString(),
                    EInputs.Touch.ToString()
                });
            DataContext = this;
        }

        public IInputSimulationViewModel SimulationViewModel { get; }
    }
}