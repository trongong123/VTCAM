using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Motion
{
    public class MotionStatus : ObservableObject, IMotionStatus
    {
        public bool IsAlarm
        {
            get => isAlarm;
            set
            {
                isAlarm = value;
                OnPropertyChanged();
            }
        }
        public bool IsMotionOn
        {
            get => isMotionOn;
            set
            {
                isMotionOn = value;
                OnPropertyChanged();
            }
        }
        public bool IsHomeDone
        {
            get => isHomeDone;
            set
            {
                isHomeDone = value;
                OnPropertyChanged();
            }
        }
        public bool IsMotioning
        {
            get => isMotioning;
            set
            {
                isMotioning = value;
                OnPropertyChanged();
            }
        }

        public bool HwPosLimitDetect
        {
            get => hwPosLimitDetect;
            set
            {
                hwPosLimitDetect = value;
                OnPropertyChanged();
            }
        }
        public bool HwNegLimitDetect
        {
            get => hwNegLimitDetect;
            set
            {
                hwNegLimitDetect = value;
                OnPropertyChanged();
            }
        }

        public double CommandPosition
        {
            get => commandPosition;
            set
            {
                commandPosition = value;
                OnPropertyChanged();
            }
        }
        public double ActualPosition
        {
            get => actualPosition;
            set
            {
                actualPosition = value;
                OnPropertyChanged();
            }
        }
        public double PositionError
        {
            get => positionError;
            set
            {
                positionError = value;
                OnPropertyChanged();
            }
        }
        public double ActualVelocity
        {
            get => actualVelocity;
            set
            {
                actualVelocity = value;
                OnPropertyChanged();
            }
        }
        public bool IsConnected
        {

            get => isConnected;
            set
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
