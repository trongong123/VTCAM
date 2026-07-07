using EQX.Core.Recipe;
using EQX.Core.Units;
using Newtonsoft.Json;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class TraySuplierRecipe : RecipeBase
    {
        [SingleRecipeDescription(Description = "Tray Pitch X", Unit = Unit.mm)]
        public double TrayPitchX
        {
            get => trayPitchX;
            set
            {
                if (trayPitchX == value) return;
                OnRecipeChanged(trayPitchX, value);
                trayPitchX = value;
            }
        }

        [SingleRecipeDescription(Description = "Tray Pitch Y", Unit = Unit.mm)]
        public double TrayPitchY
        {
            get => trayPitchY;
            set
            {
                if (trayPitchY == value) return;
                OnRecipeChanged(trayPitchY, value);
                trayPitchY = value;
            }
        }

        [JsonIgnore]
        public EventHandler TraySizeChanged;

        [SingleRecipeDescription(Description = "Camera X Count", Unit = Unit.EA)]
        [SingleRecipeMinMax(Min = 1, Max = 30)]
        public int Columns
        {
            get => columns;
            set
            {
                if (columns == value) return;

                OnRecipeChanged(columns, value, $"{Name}_Columns");
                columns = value;

                TraySizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SingleRecipeDescription(Description = "Camera Y Count", Unit = Unit.EA)]
        [SingleRecipeMinMax(Min = 1, Max = 30)]
        public int Rows
        {
            get => rows;
            set
            {
                if (rows == value) return;

                OnRecipeChanged(rows, value, $"{Name}_Rows");
                rows = value;

                TraySizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        //[SingleRecipeDescription(Description = "JigOrientation", Unit = Unit.ETC)]
        //[SingleRecipeMinMax(Min = 0, Max = 3)]
        public int JigOrientationNumber
        {
            get => jigOrientationNumber;
            set
            {
                if (jigOrientationNumber == value) return;

                OnRecipeChanged(jigOrientationNumber, value, $"{Name}_JigOrientation");
                jigOrientationNumber = value;

                TraySizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ETrayOrientation JigOrientation => (ETrayOrientation)jigOrientationNumber;
        /// <summary>
        /// Tray In Elevator
        /// </summary>     
  
        [SingleRecipeDescription(Description = "Z Axis Move Up Position", Unit = Unit.mm)]
        public double ZAxisLimitUpPosition
        {
            get => zAxisLimitUpPosition;
            set
            {
                if (zAxisLimitUpPosition == value) return;
                OnRecipeChanged(zAxisLimitUpPosition, value);
                zAxisLimitUpPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Z Axis Warning Empty Material Position", Unit = Unit.mm)]
        public double ZAxixWarningEmptyMaterialPos
        {
            get => zAxixWarningEmptyMaterialPosition;
            set
            {
                if (zAxixWarningEmptyMaterialPosition == value) return;
                OnRecipeChanged(zAxixWarningEmptyMaterialPosition, value);
                zAxixWarningEmptyMaterialPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Wait Times Z Axis Move Up Position", Unit = Unit.mm)]
        public double WaitTimesZAxisLimitUpPosition
        {
            get => waitTimesZAxisLimitUpPosition;
            set
            {
                if (waitTimesZAxisLimitUpPosition == value) return;
                OnRecipeChanged(waitTimesZAxisLimitUpPosition, value);
                waitTimesZAxisLimitUpPosition = value;
            }
        }
        private double waitTimesZAxisLimitUpPosition;

        [SingleRecipeDescription(Description = "Z Axis Move Input Tray Position", Unit = Unit.mm)]
        public double ZAxisInputTrayPosition
        {
            get => zAxisInputTrayPosition;
            set
            {
                if (zAxisInputTrayPosition == value) return;
                OnRecipeChanged(zAxisInputTrayPosition, value);
                zAxisInputTrayPosition = value;
            }
        }

        /// <summary>
        /// Tray Out Elevator
        /// </summary>
        [SingleRecipeDescription(Description = "Z Axis Ready Place Tray Position", Unit = Unit.mm)]
        public double ZAxisReadyOutputTrayPosition
        {
            get => zAxisReadyOutputTrayPosition;
            set
            {
                if (zAxisReadyOutputTrayPosition == value) return;
                OnRecipeChanged(zAxisReadyOutputTrayPosition, value);
                zAxisReadyOutputTrayPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Z Axis Full Tray Position", Unit = Unit.mm)]
        public double ZAxisFullTrayPosition
        {
            get => zAxisFullTrayPosition;
            set
            {
                if (zAxisFullTrayPosition == value) return;
                OnRecipeChanged(zAxisFullTrayPosition, value);
                zAxisFullTrayPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Move Ready Out Tray Position", Unit = Unit.mm)]
        public double ZAxisReadyPlaceTrayPosition
        {
            get => zAxisReadyPlaceTrayPosition;
            set
            {
                if (zAxisReadyPlaceTrayPosition == value) return;
                OnRecipeChanged(zAxisReadyPlaceTrayPosition, value);
                zAxisReadyPlaceTrayPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Z Axis Move Output Tray Position", Unit = Unit.mm)]
        public double ZAxisLimitDownTraySearchPosition
        {
            get => zAxisLimitDownTraySearchPos;
            set
            {
                if (zAxisLimitDownTraySearchPos == value) return;
                OnRecipeChanged(zAxisLimitDownTraySearchPos, value);
                zAxisLimitDownTraySearchPos = value;
            }
        }
        /// <summary>
        /// Speed Move
        /// </summary>

        //[SingleRecipeDescription(Description = "Z Axis Elevator Move Up Speed", Detail ="1st Speed", Unit = Unit.mmPerSecond)]
        //public double ZAxisElevatorSpeed1St
        //{
        //    get => zAxisElevatorSpeed1St;
        //    set
        //    {
        //        if (zAxisElevatorSpeed1St == value) return;
        //        OnRecipeChanged(zAxisElevatorSpeed1St, value);
        //        zAxisElevatorSpeed1St = value;
        //    }
        //}
        [SingleRecipeDescription(Description = "Z Axis Elevator TraySearch Speed", Detail = "2nd Speed", Unit = Unit.mmPerSecond)]
        public double ZAxisElevatorSpeed2nd
        {
            get => zAxisElevatorSpeed2nd;
            set
            {
                if (zAxisElevatorSpeed2nd == value) return;
                OnRecipeChanged(zAxisElevatorSpeed2nd, value);
                zAxisElevatorSpeed2nd = value;
            }
        }
        [SingleRecipeDescription(Description = "Tray In CV Speed", Unit = Unit.RevolutionsPerMinute)]
        public double CVTrayInSpeed
        {
            get => conveyorTrayInSpeed;
            set
            {
                if (conveyorTrayInSpeed == value) return;
                OnRecipeChanged(conveyorTrayInSpeed, value);
                conveyorTrayInSpeed = value;
            }
        }
        [SingleRecipeDescription(Description = "Tray In Elevator CV Speed", Unit = Unit.RevolutionsPerMinute)]
        public double CVTrayInElevatorSpeed
        {
            get => conveyorTrayInElevatorSpeed;
            set
            {
                if (conveyorTrayInElevatorSpeed == value) return;
                OnRecipeChanged(conveyorTrayInElevatorSpeed, value);
                conveyorTrayInElevatorSpeed = value;
            }
        }
        [SingleRecipeDescription(Description = "Tray Out Elevator CV Speed", Unit = Unit.RevolutionsPerMinute)]
        public double CVTrayOutElevatorSpeed
        {
            get => conveyorTrayOutElevatorSpeed;
            set
            {
                if (conveyorTrayOutElevatorSpeed == value) return;
                OnRecipeChanged(conveyorTrayOutElevatorSpeed, value);
                conveyorTrayOutElevatorSpeed = value;
            }
        }
        [SingleRecipeDescription(Description = "Tray Out CV Speed", Unit = Unit.RevolutionsPerMinute)]
        public double CVTrayOutSpeed
        {
            get => conveyorTrayOutSpeed;
            set
            {
                if (conveyorTrayOutSpeed == value) return;
                OnRecipeChanged(conveyorTrayOutSpeed, value);
                conveyorTrayOutSpeed = value;
            }
        }

        [SingleRecipeDescription(Description = "CV Acceleration", Unit = Unit.RevolutionsPerMinutePerSecond)]
        public double Acceleration
        {
            get => acceleration;
            set
            {
                if (acceleration == value) return;
                OnRecipeChanged(acceleration, value);
                acceleration = value;
            }
        }

        [SingleRecipeDescription(Description = "CV Deceleration", Unit = Unit.RevolutionsPerMinutePerSecond)]
        public double Deceleration
        {
            get => deceleration;
            set
            {
                if (deceleration == value) return;
                OnRecipeChanged(deceleration, value);
                deceleration = value;
            }
        }
        #region Privates
        
        /// <summary>
        /// Tray In CV
        /// </summary>
        private double conveyorTrayInSpeed;
        /// <summary>
        /// Tray In Elevator
        /// </summary>
        private double zAxisInputTrayPosition;
        private double zAxisLimitUpPosition;
        private double zAxixWarningEmptyMaterialPosition;
        private double conveyorTrayInElevatorSpeed;
        /// <summary>
        /// Tray Out Elevator
        /// </summary>
        private double zAxisLimitDownTraySearchPos;
        private double zAxisReadyPlaceTrayPosition;
        private double zAxisReadyOutputTrayPosition;
        private double zAxisInElevatorSpeed;
        private double zAxisFullTrayPosition;
        private double conveyorTrayOutElevatorSpeed;

        /// <summary>
        /// Tray Out CV
        /// </summary>
        private double conveyorTrayOutSpeed;

        /// <summary>
        /// Config
        /// </summary>
        private double acceleration;
        private double deceleration;
        private double zAxisElevatorSpeed1St;
        private double zAxisElevatorSpeed2nd;
        private int rows;
        private int columns;
        private int jigOrientationNumber;
        private double trayPitchX;
        private double trayPitchY;
        #endregion
    }
}
