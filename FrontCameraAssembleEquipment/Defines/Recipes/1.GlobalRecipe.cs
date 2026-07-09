using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class GlobalRecipe : RecipeBase
    {
        #region Properties
        public bool UseInputAuto
        {
            get => useInputAuto;
            set
            {
                if (useInputAuto == value) return;
                OnRecipeChanged(useInputAuto, value);
                useInputAuto = value;
                OnPropertyChanged(nameof(useInputAuto));
            }
        }
        public bool UseEDMLog
        {
            get => useEDMLog;
            set
            {
                if (useEDMLog == value) return;

                OnRecipeChanged(useEDMLog, value);
                useEDMLog = value;
                OnPropertyChanged(nameof(useEDMLog));
            }
        }

        public bool UseScaner
        {
            get => useScaner;
            set
            {
                if (useScaner == value) return;
                OnRecipeChanged(useScaner, value);
                useScaner = value;
                OnPropertyChanged(nameof(useScaner));
            }
        }
        public bool ScanByVision
        {
            get => scanByVision;

            set
            {
                if (scanByVision == value) return;
                OnRecipeChanged(scanByVision, value);
                scanByVision = value;
                OnPropertyChanged(nameof(scanByVision));
            }
        }
        public string Comport
        {
            get => comPort;
            set
            {
                if (comPort == value) return;
                comPort = value;
                OnPropertyChanged(nameof(Comport));
                OnRecipeChanged(Comport, value);
            }
        }

        public ECameraType CameraType
        {
            get { return cameraType; }
            set
            {
                if (cameraType == value) return;
                cameraType = value;
                OnPropertyChanged(nameof(CameraType));
                OnRecipeChanged(CameraType, value);
            }
        }

        //[SingleRecipeDescription(Description = "Cylinder Move Timeout ", Unit = Unit.Second)]
        //[SingleRecipeMinMax(Min = 1, Max = 10000)]
        //public double CylinderMoveTimeout
        //{
        //    get => cylinderMoveTimeout;
        //    set
        //    {
        //        if (cylinderMoveTimeout == value) return;
        //        OnRecipeChanged(cylinderMoveTimeout, value);
        //        cylinderMoveTimeout = value;
        //    }
        //}

        [SingleRecipeDescription(Description = "Motion Move Timeout ", Unit = Unit.Second)]
        [SingleRecipeMinMax(Min = 1, Max = 10000)]
        public double MotionMoveTimeout
        {
            get => motionMoveTimeout;
            set
            {
                if (motionMoveTimeout == value) return;
                OnRecipeChanged(motionMoveTimeout, value);
                motionMoveTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "CV Move Timeout ", Unit = Unit.Second)]
        [SingleRecipeMinMax(Min = 1, Max = 10000)]
        public double CVMoveTimeout
        {
            get => motionMoveTimeout;
            set
            {
                if (motionMoveTimeout == value) return;
                OnRecipeChanged(motionMoveTimeout, value);
                motionMoveTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "Motion Origin Timeout ", Unit = Unit.Second)]
        [SingleRecipeMinMax(Min = 1, Max = 10000)]
        public double MotionOriginTimeout
        {
            get => motionOriginTimeout;
            set
            {
                if (motionOriginTimeout == value) return;
                OnRecipeChanged(motionOriginTimeout, value);
                motionOriginTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "Vac Check wait time ", Unit = Unit.MilliSecond)]
        public int VacCheckWaitTime
        {
            get => vacCheckWaitTime;
            set
            {
                if (vacCheckWaitTime == value) return;
                OnRecipeChanged(vacCheckWaitTime, value);
                vacCheckWaitTime = value;
            }
        }

        [SingleRecipeDescription(Description = "Vacuum Delay", Unit = Unit.MilliSecond)]
        public int VacuumDelay
        {
            get => vacDelay;
            set
            {
                if (vacDelay == value) return;
                OnRecipeChanged(vacDelay, value);
                vacDelay = value;
            }
        }

        [SingleRecipeDescription(Description = "Trash Suction Sponge On Time", Detail = "Trash Suction Sponge On Time", Unit = Unit.Second)]
        public double TrashSuctionOnTime
        {
            get => trashSuctionOnTime;
            set
            {
                if (trashSuctionOnTime == value) return;
                OnRecipeChanged(trashSuctionOnTime, value);
                trashSuctionOnTime = value;
            }
        }

        [SingleRecipeDescription(Description = "Log Save Day")]
        [SingleRecipeMinMax(Max = 180, Min = 5)]
        public int LogSaveDay
        {
            get { return logSaveDay; }
            set 
            {
                logSaveDay = value; 
            }
        }
        #endregion

        #region Privates
        private string comPort;
        private bool useEDMLog;
        private bool useInputAuto;
        private bool useScaner;
        private bool scanByVision;
        private double cylinderMoveTimeout;
        private double motionMoveTimeout;
        private double motionOriginTimeout;
        private int vacCheckWaitTime;
        private int vacDelay;
        private ECameraType cameraType;
        private double trashSuctionOnTime = 1;
        private int logSaveDay = 30;
        #endregion
    }
}
