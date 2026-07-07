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
using System.Windows.Shapes;

namespace EQX.UI.Controls
{
    /// <summary>
    /// Interaction logic for VirtualKeyboard.xaml
    /// </summary>
    public partial class VirtualKeyboard : Window
    {
        public string InputText { get; set; } = string.Empty;
        public bool IsPasswordMode { get; }

        private bool _touchHandled = false;

        private string CurrentInput
        {
            get => IsPasswordMode ? passwordBox.Password : inputTextBox.Text;
            set
            {
                if (IsPasswordMode)
                {
                    passwordBox.Password = value;
                }
                else
                {
                    inputTextBox.Text = value;
                }
            }
        }

        public VirtualKeyboard(bool isPasswordMode = true)
        {
            IsPasswordMode = isPasswordMode;
            InitializeComponent();
            ApplyInputMode();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            CurrentInput = InputText;

            if (IsPasswordMode)
            {
                passwordBox.Focus();
            }
            else
            {
                inputTextBox.Focus();
                inputTextBox.CaretIndex = inputTextBox.Text.Length;
            }
        }

        private void ApplyInputMode()
        {
            if (IsPasswordMode)
            {
                passwordBox.Visibility = Visibility.Visible;
                inputTextBox.Visibility = Visibility.Collapsed;
                return;
            }

            passwordBox.Visibility = Visibility.Collapsed;
            inputTextBox.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn == false) return;

            if (_touchHandled)
            {
                _touchHandled = false;
                e.Handled = true;
                return; 
            }

            CurrentInput += btn.Content.ToString();
            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_touchHandled)
            {
                _touchHandled = false;
                e.Handled = true;
                return;
            }

            DialogResult = false;
            e.Handled = true;
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_touchHandled)
            {
                _touchHandled = false;
                e.Handled = true;
                return;
            }

            DialogResult = true;
            InputText = CurrentInput;
            e.Handled = true;
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_touchHandled)
            {
                _touchHandled = false;
                e.Handled = true;
                return;
            }

            if (CurrentInput.Length <= 0 ) return;
            CurrentInput = CurrentInput[..^1];
            e.Handled = true;
        }

        private void Button_TouchUp(object sender, TouchEventArgs e)
        {
            if (sender is Button btn == false) return;
            _touchHandled = true;

            CurrentInput += btn.Content.ToString();
            e.Handled = true;
        }

        private void CloseButton_TouchUp(object sender, TouchEventArgs e)
        {
            _touchHandled = true;

            DialogResult = false;
            e.Handled = true;
        }

        private void EnterButton_TouchUp(object sender, TouchEventArgs e)
        {
            _touchHandled = true;

            DialogResult = true;
            InputText = CurrentInput;
            e.Handled = true;
        }

        private void BackspaceButtonTouchUp(object sender, TouchEventArgs e)
        {
            _touchHandled = true;

            if (CurrentInput.Length <= 0) return;
            CurrentInput = CurrentInput[..^1];
            e.Handled = true;
        }
    }
}
