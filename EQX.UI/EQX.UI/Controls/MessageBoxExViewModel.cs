using CommunityToolkit.Mvvm.ComponentModel;

namespace EQX.UI.Controls
{
    internal class MessageBoxExViewModel : ObservableObject
    {
        public EventHandler? ModalChangedEvent { get; set; }

        #region Properties
        public bool ConfirmRequest
        {
            get { return _confirmRequest; }
            set
            {
                _confirmRequest = value;
                OnPropertyChanged();
            }
        }

        public string MessageDetail
		{
			get { return _messageDetail; }
            set
            {
                _messageDetail = value;
                OnPropertyChanged();
            }
		}

		public string Caption
		{
			get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Public Methods
        public void Show(string message, string caption = "Confirm")
        {
            UpdateInformation(message, caption);
        }

        public void ShowDialog(string message, string caption = "Confirm")
        {
            UpdateInformation(message, caption);
        }
        #endregion

        #region Private Methods
        private void UpdateInformation(string message, string caption)
        {
            MessageDetail = message;
            Caption = caption;
        }
        #endregion

        #region Privates
        private bool _confirmRequest;
        private string _messageDetail;
        private string _caption;
        #endregion
    }
}
