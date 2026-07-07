using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Motion;

namespace EQX.Motion
{
    public class MotionStatus : ObservableObject, IMotionStatus
    {
        public bool IsAlarm
        {
            get => isAlarm;
            internal set
            {
                isAlarm = value;
                OnPropertyChanged();
            }
        }
        public bool IsMotionOn
        {
            get => isMotionOn;
            internal set
            {
                isMotionOn = value;
                OnPropertyChanged();
            }
        }
        public bool IsHomeDone
        {
            get => isHomeDone;
            internal set
            {
                if (isHomeDone == value) return;

                isHomeDone = value;
                //if (value == true) ActualPosition = 0;
                OnPropertyChanged();
            }
        }
        public bool IsMotioning
        {
            get => isMotioning;
            internal set
            {
                isMotioning = value;
                OnPropertyChanged();
            }
        }

        public bool HwPosLimitDetect
        {
            get => hwPosLimitDetect;
            internal set
            {
                hwPosLimitDetect = value;
                OnPropertyChanged();
            }
        }
        public bool HwNegLimitDetect
        {
            get => hwNegLimitDetect;
            internal set
            {
                hwNegLimitDetect = value;
                OnPropertyChanged();
            }
        }

        public double CommandPosition
        {
            get => commandPosition;
            internal set
            {
                commandPosition = value;
                OnPropertyChanged();
            }
        }
        public double ActualPosition
        {
            get => actualPosition;
            internal set
            {
                actualPosition = value;
                OnPropertyChanged();
            }
        }
        public double PositionError
        {
            get => positionError;
            internal set
            {
                positionError = value;
                OnPropertyChanged();
            }
        }
        public double ActualVelocity
        {
            get => actualVelocity;
            internal set
            {
                actualVelocity = value;
                OnPropertyChanged();
            }
        }
        public bool IsConnected
        {

            get => isConnected;
            internal set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }
        #region Privates
        private bool isConnected;
        private bool isAlarm;
        private bool isMotionOn;
        private bool isHomeDone;
        private bool isMotioning;
        private bool hwPosLimitDetect;
        private bool hwNegLimitDetect;
        private double commandPosition;
        private double actualPosition;
        private double positionError;
        private double actualVelocity;
        #endregion
    }
}
