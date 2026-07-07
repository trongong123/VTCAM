using CommunityToolkit.Mvvm.ComponentModel;

namespace EQX.Device.SyringePump
{
	public enum EPSD4SyringPumpError
	{
		None = 0,
		INITIALIZE_ERROR = 1,
        INVALID_COMMAND = 2,
        INVALID_PARAMETER = 3,
		INVALID_COMMAND_SEQUENCE = 4,
        EEPROM_ERROR = 6,
		SYRINGE_NOT_INITIALIZE = 7,
		SYRINGE_OVERLOAD = 9,
		VALVE_OVERLOAD = 10,
		SYRINGE_MOVE_NOT_ALLOW = 11,
        COMMAND_BUFFER_FULL = 15,

    }
    public class PSD4SyringePumpStatus : ObservableObject
    {
		private bool isReady;

		public bool IsReady
		{
			get { return isReady; }
			set { isReady = value; OnPropertyChanged(); }
		}

		private EPSD4SyringPumpError errorType;

		public EPSD4SyringPumpError ErrorType
		{
			get { return errorType; }
			set { errorType = value; }
		}

	}
}
