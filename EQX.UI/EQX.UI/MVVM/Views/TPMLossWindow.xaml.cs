using EQX.Core.Communication;
using System.Windows;

namespace EQX.UI.MVVM
{
    /// <summary>
    /// Interaction logic for TPMLossWindow.xaml
    /// </summary>
    public partial class TPMLossWindow : Window
    {
        public ETPMLossDesciption? SelectedTPMMode { get; set; }
        public TPMLossWindow()
        {
            DataContextChanged += TPMLossWindow_DataContextChanged;
            InitializeComponent();
            DataContext = new TPMLossViewModel();
        }

        private void TPMLossWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is TPMLossViewModel viewModel && viewModel.CloseAction == null)
            {
                viewModel.CloseAction = (result) =>
                {
                    this.DialogResult = result;
                    SelectedTPMMode = viewModel.SelectedTPMMode;
                };
            }
        }
    }
}