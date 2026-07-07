using EQX.Core.Recipe;
using EQX.Core.Units;
using Microsoft.Xaml.Behaviors.Layout;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class TrayHeadRecipe : RecipeBase
    {
        //[JsonIgnore]
        [SingleRecipeDescription(Description = "Enable Pre Centering ")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UsePreCentering
        {
            get => usePreCentering;
            set
            {
                if (usePreCentering == value) return;
                OnRecipeChanged(usePreCentering, value);
                usePreCentering = value;
            }
        }
        [SingleRecipeDescription(Description = "Enable Scan Only One Cam 1st")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseScanOnlyOneCam
        {
            get => useScanOnlyOneCam;
            set
            {
                if (useScanOnlyOneCam == value) return;
                OnRecipeChanged(useScanOnlyOneCam, value);
                useScanOnlyOneCam = value;
            }
        }
        [SingleRecipeDescription(Description = "Enable Check Metarial In Tray Empty")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseWarningWhenPickTray
        {
            get => useWarningWhenPickTray;
            set
            {
                if (useWarningWhenPickTray == value) return;
                OnRecipeChanged(useWarningWhenPickTray, value);
                useWarningWhenPickTray = value;
            }
        }

        //[SingleRecipeDescription(Description = "Start Cam Pick Offset Y", Unit = Unit.mm)]
        public double TrayType2Offset
        {
            get => trayType2Offset;
            set
            {
                if (trayType2Offset == value) return;
                OnRecipeChanged(trayType2Offset, value);
                trayType2Offset = value;
            }
        }
        //Wait Position
        [SingleRecipeDescription(Description = "X Axis Wait Position ", Unit = Unit.mm)]
        public double XAxisWaitPosition
        {
            get => xAxisWaitPosition;
            set
            {
                if (xAxisWaitPosition == value) return;
                OnRecipeChanged(xAxisWaitPosition, value);
                xAxisWaitPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Wait Position ", Unit = Unit.mm)]
        public double YAxisWaitPosition
        {
            get => yAxisWaitPosition;
            set
            {
                if (yAxisWaitPosition == value) return;
                OnRecipeChanged(yAxisWaitPosition, value);
                yAxisWaitPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Z Axis Wait Position ", Unit = Unit.mm)]
        public double ZAxisReadyPosition
        {
            get => zAxisWaitPosition;
            set
            {
                if (zAxisWaitPosition == value) return;
                OnRecipeChanged(zAxisWaitPosition, value);
                zAxisWaitPosition = value;
            }
        }

        ////Camera Pick Ready Position
        //[SingleRecipeDescription(Description = "X Axis Camera Pick Ready Position ", Unit = Unit.mm)]
        //public double XAxisPickReadyPosition
        //{
        //    get => xAxisPickReadyPosition;
        //    set
        //    {
        //        if (xAxisPickReadyPosition == value) return;
        //        OnRecipeChanged(xAxisPickReadyPosition, value);
        //        xAxisPickReadyPosition = value;
        //    }
        //}

        //[SingleRecipeDescription(Description = "Y Axis Camera Pick Ready Position ", Unit = Unit.mm)]
        //public double YAxisPickReadyPosition
        //{
        //    get => yAxisPickReadyPosition;
        //    set
        //    {
        //        if (yAxisPickReadyPosition == value) return;
        //        OnRecipeChanged(yAxisPickReadyPosition, value);
        //        yAxisPickReadyPosition = value;
        //    }
        //}

        //[SingleRecipeDescription(Description = "Z Axis Camera Pick Ready Position ", Unit = Unit.mm)]
        //public double ZAxisPickReadyPosition
        //{
        //    get => zAxisPickReadyPosition;
        //    set
        //    {
        //        if (zAxisPickReadyPosition == value) return;
        //        OnRecipeChanged(zAxisPickReadyPosition, value);
        //        zAxisPickReadyPosition = value;
        //    }
        //}

        //Camera Pick Position
        [SingleRecipeDescription(Description = "X Axis Camera Pick Position ", Unit = Unit.mm)]
        public double XAxisCamPickPosition
        {
            get => xAxisCamPickPosition;
            set
            {
                if (xAxisCamPickPosition == value) return;
                OnRecipeChanged(xAxisCamPickPosition, value);
                xAxisCamPickPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Camera Pick Position ", Unit = Unit.mm)]
        public double YAxisCamPickPosition
        {
            get => yAxisCamPickPosition;
            set
            {
                if (yAxisCamPickPosition == value) return;
                OnRecipeChanged(yAxisCamPickPosition, value);
                yAxisCamPickPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Z Axis Camera Pick Position ", Unit = Unit.mm)]
        public double ZAxisCamPickPosition
        {
            get => zAxisCamPickPosition;
            set
            {
                if (zAxisCamPickPosition == value) return;
                OnRecipeChanged(zAxisCamPickPosition, value);
                zAxisCamPickPosition = value;
            }
        }

        //[SingleRecipeDescription(Description = "X Axis Camera Place Ready Position ", Unit = Unit.mm)]
        //public double XAxisCamPlaceReadyPosition
        //{
        //    get => xAxisPlaceReadyPosition;
        //    set
        //    {
        //        if (xAxisPlaceReadyPosition == value) return;
        //        OnRecipeChanged(xAxisPlaceReadyPosition, value);
        //        xAxisPlaceReadyPosition = value;
        //    }
        //}

        //[SingleRecipeDescription(Description = "Y Axis Camera Place Ready Position ", Unit = Unit.mm)]
        //public double YAxisCamPlaceReadyPosition
        //{
        //    get => yAxisPlaceReadyPosition;
        //    set
        //    {
        //        if (yAxisPlaceReadyPosition == value) return;
        //        OnRecipeChanged(yAxisPlaceReadyPosition, value);
        //        yAxisPlaceReadyPosition = value;
        //    }
        //}

        //[SingleRecipeDescription(Description = "Z Axis Camera Place Ready Position ", Unit = Unit.mm)]
        //public double ZAxisCamPlaceReadyPosition
        //{
        //    get => zAxisPlaceReadyPosition;
        //    set
        //    {
        //        if (zAxisPlaceReadyPosition == value) return;
        //        OnRecipeChanged(zAxisPlaceReadyPosition, value);
        //        zAxisPlaceReadyPosition = value;
        //    }
        //}

        //Camera Place Position
        [SingleRecipeDescription(Description = "X Axis Camera Place Position ", Unit = Unit.mm)]
        public double XAxisCamPlacePosition
        {
            get => xAxisCamPlacePosition;
            set
            {
                OnRecipeChanged(xAxisCamPlacePosition, value);
                xAxisCamPlacePosition = value;
            }
        }

        public double YAxisCamPlacePosition
        {
            get => yAxisCamPlacePosition;
            set
            {
                OnRecipeChanged(yAxisCamPlacePosition, value);
                yAxisCamPlacePosition = value;
            }
        }
        public double ZAxisCamPlacePosition
        {
            get => zAxisCamPlacePosition;
            set
            {
                OnRecipeChanged(zAxisCamPlacePosition, value);
                zAxisCamPlacePosition = value;
            }
        }

        //Tray Place Position
        [SingleRecipeDescription(Description = "X Axis Tray Place Position ", Unit = Unit.mm)]
        public double XAxisTrayPlacePosition
        {
            get => xAxisTrayPlacePosition;
            set
            {
                if (xAxisTrayPlacePosition == value) return;
                OnRecipeChanged(xAxisTrayPlacePosition, value);
                xAxisTrayPlacePosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Tray Place Position ", Unit = Unit.mm)]
        public double YAxisTrayPlacePosition
        {
            get => yAxisTrayPlacePosition;
            set
            {
                if (yAxisTrayPlacePosition == value) return;
                OnRecipeChanged(yAxisTrayPlacePosition, value);
                yAxisTrayPlacePosition = value;
            }
        }

        //Tray Pick Position
        [SingleRecipeDescription(Description = "X Axis Tray Pick Position ", Unit = Unit.mm)]
        public double XAxisTrayPickPosition
        {
            get => xAxisTrayPickPosition;
            set
            {
                if (xAxisTrayPickPosition == value) return;
                OnRecipeChanged(xAxisTrayPickPosition, value);
                xAxisTrayPickPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Tray Pick Position ", Unit = Unit.mm)]
        public double YAxisTrayPickPosition
        {
            get => yAxisTrayPickPosition;
            set
            {
                if (yAxisTrayPickPosition == value) return;
                OnRecipeChanged(yAxisTrayPickPosition, value);
                yAxisTrayPickPosition = value;
            }
        }

        /// <summary>
        ///  Camera Scan Position
        /// </summary>
        [SingleRecipeDescription(Description = "X Axis Camera Scan Position ", Unit = Unit.mm)]
        public double XAxisCamScanPosition
        {
            get => xAxisCamScanPosition;
            set
            {
                if (xAxisCamScanPosition == value) return;
                OnRecipeChanged(xAxisCamScanPosition, value);
                xAxisCamScanPosition = value;
            }
        }


        [SingleRecipeDescription(Description = "Y Axis Camera Scan Position ", Unit = Unit.mm)]
        public double YAxisCamScanPosition
        {
            get => yAxisCamScanPosition;
            set
            {
                if (yAxisCamScanPosition == value) return;
                OnRecipeChanged(yAxisCamScanPosition, value);
                yAxisCamScanPosition = value;
            }
        }


        [SingleRecipeDescription(Description = "Z Axis Camera Scan Position ", Unit = Unit.mm)]
        public double ZAxisCamScanPosition
        {
            get => zAxisCamScanPosition;
            set
            {
                if (zAxisCamScanPosition == value) return;
                OnRecipeChanged(zAxisCamScanPosition, value);
                zAxisCamScanPosition = value;
            }
        }
        #region Privates
        private double xAxisWaitPosition;
        private double yAxisWaitPosition;
        private double zAxisWaitPosition;

        //private double xAxisPickReadyPosition;
        //private double yAxisPickReadyPosition;
        //private double zAxisPickReadyPosition;

        private double xAxisCamPickPosition;
        private double yAxisCamPickPosition;
        private double zAxisCamPickPosition;

        private double xAxisCamScanPosition;
        private double yAxisCamScanPosition;
        private double zAxisCamScanPosition;

        //private double xAxisPlaceReadyPosition;
        //private double yAxisPlaceReadyPosition;
        //private double zAxisPlaceReadyPosition;

        private double xAxisCamPlacePosition;
        private double yAxisCamPlacePosition;
        private double zAxisCamPlacePosition;

        private double xAxisTrayPlacePosition;
        private double yAxisTrayPlacePosition;
        private double xAxisTrayPickPosition;
        private double yAxisTrayPickPosition;

        private double xyAxisPickSpeed;
        private double xyAxisPlaceSpeed;

        private double zAxisPickSpeed;
        private double zAxisPlaceSpeed;

        private double trayType2Offset;

        private int trayType;

        private int usePreCentering;
        private int useScanOnlyOneCam;
        private int useWarningWhenPickTray;
        #endregion
    }
}