using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class CameraHeadRecipe : RecipeBase
    {
        #region Properties

        [SingleRecipeDescription(Description = "Use Align Vision")]
        [SingleRecipeMinMax(Max = 1, Min = 0)]
        [OptionRecipe()]
        public int UseAlignVision
        {
            get => useAlignVision;
            set { OnRecipeChanged(useAlignVision, value); useAlignVision = value; }
        }

        //-----------------------SPEED-----------------------
        //Pick & Ready Speed
        //[SingleRecipeDescription(Description = "X Axis Pick Speed", Unit = Unit.mmPerSecond)]
        //public double XZRXAxisPickSpeed
        //{
        //    get => xzrxAxisPickSpeed;
        //    set { OnRecipeChanged(xzrxAxisPickSpeed, value); xzrxAxisPickSpeed = value; }
        //}

        //Ready Place Speed
        //[SingleRecipeDescription(Description = "X Axis Ready Place Speed", Unit = Unit.mmPerSecond)]
        //public double XAxisReadyPlaceSpeed
        //{
        //    get => xAxisReadyPlaceSpeed;
        //    set { OnRecipeChanged(xAxisReadyPlaceSpeed, value); xAxisReadyPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "Y Axis Ready Place Speed", Unit = Unit.mmPerSecond)]
        //public double YAxisReadyPlaceSpeed
        //{
        //    get => yAxisReadyPlaceSpeed;
        //    set { OnRecipeChanged(yAxisReadyPlaceSpeed, value); yAxisReadyPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "Z Axis Ready Place Speed", Unit = Unit.mmPerSecond)]
        //public double ZAxisReadyPlaceSpeed
        //{
        //    get => zAxisReadyPlaceSpeed;
        //    set { OnRecipeChanged(zAxisReadyPlaceSpeed, value); zAxisReadyPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "RX Axis Ready Place Speed", Unit = Unit.mmPerSecond)]
        //public double RXAxisReadyPlaceSpeed
        //{
        //    get => rxAxisReadyPlaceSpeed;
        //    set { OnRecipeChanged(rxAxisReadyPlaceSpeed, value); rxAxisReadyPlaceSpeed = value; }
        //}

        //Place Speed 
        //[SingleRecipeDescription(Description = "X Axis Push Place Speed", Unit = Unit.mmPerSecond)]
        //public double XAxisPlaceSpeed
        //{
        //    get => xAxisPlaceSpeed;
        //    set { OnRecipeChanged(xAxisPlaceSpeed, value); xAxisPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "Y Axis Place Speed", Unit = Unit.mmPerSecond)]
        //public double YAxisPlaceSpeed
        //{
        //    get => yAxisPlaceSpeed;
        //    set { OnRecipeChanged(yAxisPlaceSpeed, value); yAxisPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "Z Axis Push Place Speed", Unit = Unit.mmPerSecond)]
        //public double ZAxisPlaceSpeed
        //{
        //    get => zAxisPlaceSpeed;
        //    set { OnRecipeChanged(zAxisPlaceSpeed, value); zAxisPlaceSpeed = value; }
        //}
        //[SingleRecipeDescription(Description = "RX Axis Place Speed", Unit = Unit.mmPerSecond)]
        //public double RXAxisPlaceSpeed
        //{
        //    get => rxAxisPlaceSpeed;
        //    set { OnRecipeChanged(rxAxisPlaceSpeed, value); rxAxisPlaceSpeed = value; }
        //}

        //[SingleRecipeDescription(Description = "X-Y Axis Pick Speed", Unit = Unit.mmPerSecond)]
        //public double XYPickSpeed
        //{
        //    get => xyPickSpeed;
        //    set { OnRecipeChanged(xyPickSpeed, value); xyPickSpeed = value; }
        //}

        //[SingleRecipeDescription(Description = "X-Y Axis Place Speed", Unit = Unit.mmPerSecond)]
        //public double XYPlaceSpeed
        //{
        //    get => xyPlaceSpeed;
        //    set { OnRecipeChanged(xyPlaceSpeed, value); xyPlaceSpeed = value; }
        //}

        //-----------------------POSITION-----------------------
        // Ready Pick Position
        [SingleRecipeDescription(Description = "X Axis Ready Pick Position", Unit = Unit.mm)]
        public double XAxisReadyPosition
        {
            get => xAxisReadyPickPosition;
            set { OnRecipeChanged(xAxisReadyPickPosition, value); xAxisReadyPickPosition = value; }
        }

        [SingleRecipeDescription(Description = "Y Axis Ready Pick Position", Unit = Unit.mm)]
        public double YAxisReadyPosition
        {
            get => yAxisReadyPickPosition;
            set { OnRecipeChanged(yAxisReadyPickPosition, value); yAxisReadyPickPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Ready Pick Position", Unit = Unit.mm)]
        public double ZAxisReadyPosition
        {
            get => zAxisReadyPickPosition;
            set { OnRecipeChanged(zAxisReadyPickPosition, value); zAxisReadyPickPosition = value; }
        }

        // Pick position
        [SingleRecipeDescription(Description = "X Axis Pick Position", Unit = Unit.mm)]
        public double XAxisPickPosition
        {
            get => xAxisPickPosition;
            set { OnRecipeChanged(xAxisPickPosition, value); xAxisPickPosition = value; }
        }

        [SingleRecipeDescription(Description = "Y Axis Pick Position", Unit = Unit.mm)]
        public double YAxisPickPosition
        {
            get => yAxisPickPosition;
            set { OnRecipeChanged(yAxisPickPosition, value); yAxisPickPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Pick Position", Unit = Unit.mm)]
        public double ZAxisPickPosition
        {
            get => zAxisPickPosition;
            set { OnRecipeChanged(zAxisPickPosition, value); zAxisPickPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis Pick Position", Unit = Unit.mm)]
        public double RXAxisPickPosition
        {
            get => rxAxisPickPosition;
            set { OnRecipeChanged(rxAxisPickPosition, value); rxAxisPickPosition = value; }
        }

        // Ready Place Pos
        [SingleRecipeDescription(Description = "X Axis Ready Place Front Position", Unit = Unit.mm)]
        public double XAxisReadyPlaceFrontPosition
        {
            get => xAxisReadyPlaceFrontPosition;
            set { OnRecipeChanged(xAxisReadyPlaceFrontPosition, value); xAxisReadyPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Ready Place Front Position", Unit = Unit.mm)]
        public double ZAxisReadyPlaceFrontPosition
        {
            get => zAxisReadyPlaceFrontPosition;
            set { OnRecipeChanged(zAxisReadyPlaceFrontPosition, value); zAxisReadyPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis Ready Place Front Position", Unit = Unit.mm)]
        public double RXAxisReadyPlaceFrontPosition
        {
            get => rxAxisReadyPlaceFrontPosition;
            set { OnRecipeChanged(rxAxisReadyPlaceFrontPosition, value); rxAxisReadyPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "X Axis Ready Place Rear Position", Unit = Unit.mm)]
        public double XAxisReadyPlaceRearPosition
        {
            get => xAxisReadyPlaceRearPosition;
            set { OnRecipeChanged(xAxisReadyPlaceRearPosition, value); xAxisReadyPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Ready Place Rear Position", Unit = Unit.mm)]
        public double ZAxisReadyPlaceRearPosition
        {
            get => zAxisReadyPlaceRearPosition;
            set { OnRecipeChanged(zAxisReadyPlaceRearPosition, value); zAxisReadyPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis Ready Place Rear Position", Unit = Unit.mm)]
        public double RXAxisReadyPlaceRearPosition
        {
            get => rxAxisReadyPlaceRearPosition;
            set { OnRecipeChanged(rxAxisReadyPlaceRearPosition, value); rxAxisReadyPlaceRearPosition = value; }
        }

        // Y Place Position
        [SingleRecipeDescription(Description = "Y Axis Place Front Position", Unit = Unit.mm)]
        public double YAxisPlaceFrontPosition
        {
            get => yAxisPlaceFrontPosition;
            set { OnRecipeChanged(yAxisPlaceFrontPosition, value); yAxisPlaceFrontPosition = value; }
        }
        [SingleRecipeDescription(Description = "Y Axis Place Rear Position", Unit = Unit.mm)]
        public double YAxisPlaceRearPosition
        {
            get => yAxisPlaceRearPosition;
            set { OnRecipeChanged(yAxisPlaceRearPosition, value); yAxisPlaceRearPosition = value; }
        }

        // Place 1st Position
        [SingleRecipeDescription(Description = "X Axis 1st Place Front Position", Unit = Unit.mm)]
        public double XAxis1stPlaceFrontPosition
        {
            get => xAxis1stPlaceFrontPosition;
            set { OnRecipeChanged(xAxis1stPlaceFrontPosition, value); xAxis1stPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis 1st Place Front Position", Unit = Unit.mm)]
        public double ZAxis1stPlaceFrontPosition
        {
            get => zAxis1stPlaceFrontPosition;
            set { OnRecipeChanged(zAxis1stPlaceFrontPosition, value); zAxis1stPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis 1st Place Front Position", Unit = Unit.mm)]
        public double RXAxis1stPlaceFrontPosition
        {
            get => rxAxis1stPlaceFrontPosition;
            set { OnRecipeChanged(rxAxis1stPlaceFrontPosition, value); rxAxis1stPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "X Axis 1st Place Front Position", Unit = Unit.mm)]
        public double XAxis1stPlaceRearPosition
        {
            get => xAxis1stPlaceRearPosition;
            set { OnRecipeChanged(xAxis1stPlaceRearPosition, value); xAxis1stPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis 1st Place Rear Position", Unit = Unit.mm)]
        public double ZAxis1stPlaceRearPosition
        {
            get => zAxis1stPlaceRearPosition;
            set { OnRecipeChanged(zAxis1stPlaceRearPosition, value); zAxis1stPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis 1st Place Rear Position", Unit = Unit.mm)]
        public double RXAxis1stPlaceRearPosition
        {
            get => rxAxis1stPlaceRearPosition;
            set { OnRecipeChanged(rxAxis1stPlaceRearPosition, value); rxAxis1stPlaceRearPosition = value; }
        }

        // Place 2nd Position
        [SingleRecipeDescription(Description = "X Axis 2nd Place Front Position", Unit = Unit.mm)]
        public double XAxis2ndPlaceFrontPosition
        {
            get => xAxis2ndPlaceFrontPosition;
            set { OnRecipeChanged(xAxis2ndPlaceFrontPosition, value); xAxis2ndPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis 2nd Place Front Position", Unit = Unit.mm)]
        public double ZAxis2ndPlaceFrontPosition
        {
            get => zAxis2ndPlaceFrontPosition;
            set { OnRecipeChanged(zAxis2ndPlaceFrontPosition, value); zAxis2ndPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis 2nd Place Front Position", Unit = Unit.mm)]
        public double RXAxis2ndPlaceFrontPosition
        {
            get => rxAxis2ndPlaceFrontPosition;
            set { OnRecipeChanged(rxAxis2ndPlaceFrontPosition, value); rxAxis2ndPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "X Axis 2nd Place Rear Position", Unit = Unit.mm)]
        public double XAxis2ndPlaceRearPosition
        {
            get => xAxis2ndPlaceRearPosition;
            set { OnRecipeChanged(xAxis2ndPlaceRearPosition, value); xAxis2ndPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis 2nd Place Rear Position", Unit = Unit.mm)]
        public double ZAxis2ndPlaceRearPosition
        {
            get => zAxis2ndPlaceRearPosition;
            set { OnRecipeChanged(zAxis2ndPlaceRearPosition, value); zAxis2ndPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis 2nd Place Rear Position", Unit = Unit.mm)]
        public double RXAxis2ndPlaceRearPosition
        {
            get => rxAxis2ndPlaceRearPosition;
            set { OnRecipeChanged(rxAxis2ndPlaceRearPosition, value); rxAxis2ndPlaceRearPosition = value; }
        }

        // Place Pre Push In Pos (X: Pre Push; Y, RX: Push)
        [SingleRecipeDescription(Description = "X Axis Pre Push In Place Front Position", Unit = Unit.mm)]
        public double XAxisPrePushInPlaceFrontPosition
        {
            get => xAxisPrePushInPlaceFrontPosition;
            set { OnRecipeChanged(xAxisPrePushInPlaceFrontPosition, value); xAxisPrePushInPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Push In Place Front Position", Unit = Unit.mm)]
        public double ZAxisPushInPlaceFrontPosition
        {
            get => zAxisPushInPlaceFrontPosition;
            set { OnRecipeChanged(zAxisPushInPlaceFrontPosition, value); zAxisPushInPlaceFrontPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis Push In Place Front Position", Unit = Unit.mm)]
        public double RXAxisPushInPlaceFrontPosition
        {
            get => rxAxisPushInPlaceFrontPosition;
            set { OnRecipeChanged(rxAxisPushInPlaceFrontPosition, value); rxAxisPushInPlaceFrontPosition = value; }
        }


        [SingleRecipeDescription(Description = "X Axis Pre Push In Place Rear Position", Unit = Unit.mm)]
        public double XAxisPrePushInPlaceRearPosition
        {
            get => xAxisPrePushInPlaceRearPosition;
            set { OnRecipeChanged(xAxisPrePushInPlaceRearPosition, value); xAxisPrePushInPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "Z Axis Push In Place Rear Position", Unit = Unit.mm)]
        public double ZAxisPushInPlaceRearPosition
        {
            get => zAxisPushInPlaceRearPosition;
            set { OnRecipeChanged(zAxisPushInPlaceRearPosition, value); zAxisPushInPlaceRearPosition = value; }
        }

        [SingleRecipeDescription(Description = "RX Axis Push In Place Rear Position", Unit = Unit.mm)]
        public double RXAxisPushInPlaceRearPosition
        {
            get => rxAxisPushInPlaceRearPosition;
            set { OnRecipeChanged(rxAxisPushInPlaceRearPosition, value); rxAxisPushInPlaceRearPosition = value; }
        }

        // Offset Push
        [SingleRecipeDescription(Description = "Push Offset X Position", Unit = Unit.mm)]
        public double PushOffsetX
        {
            get => pushOffsetX;
            set { OnRecipeChanged(pushOffsetX, value); pushOffsetX = value; }
        }

        [SingleRecipeDescription(Description = "Push Offset Z Position", Unit = Unit.mm)]
        public double PrePushOffsetZ
        {
            get => prePushOffsetZ;
            set { OnRecipeChanged(prePushOffsetZ, value); prePushOffsetZ = value; }
        }
        #endregion

        #region Privates

        private int useAlignVision = 0;
        //---SPEED---
        //Pick & Ready speed
        private double xAxisPickSpeed;
        private double yAxisPickSpeed;
        private double zAxisPickSpeed;
        private double rxAxisPickSpeed;
        private double xzrxAxisPickSpeed;

        //Ready Place Speed
        private double xAxisReadyPlaceSpeed;
        private double yAxisReadyPlaceSpeed;
        private double zAxisReadyPlaceSpeed;
        private double rxAxisReadyPlaceSpeed;

        //Place Speed
        private double xAxisPlaceSpeed;
        private double yAxisPlaceSpeed;
        private double zAxisPlaceSpeed;
        private double rxAxisPlaceSpeed;

        private double xyPickSpeed;
        private double xyPlaceSpeed;

        //---POSITION---
        //ReadyPick Pos
        private double yAxisReadyPickPosition;
        private double xAxisReadyPickPosition;
        private double zAxisReadyPickPosition;


        //Pick Pos
        private double xAxisPickPosition;
        private double yAxisPickPosition;
        private double zAxisPickPosition;
        private double rxAxisPickPosition;


        //ReadyPlace Pos
        private double xAxisReadyPlaceFrontPosition;
        private double zAxisReadyPlaceFrontPosition;
        private double rxAxisReadyPlaceFrontPosition;

        private double xAxisReadyPlaceRearPosition;
        private double zAxisReadyPlaceRearPosition;
        private double rxAxisReadyPlaceRearPosition;


        // Y Axis Place Pos
        private double yAxisPlaceFrontPosition;
        private double yAxisPlaceRearPosition;


        //Place Pos 1st
        private double xAxis1stPlaceFrontPosition;
        private double zAxis1stPlaceFrontPosition;
        private double rxAxis1stPlaceFrontPosition;

        private double xAxis1stPlaceRearPosition;
        private double zAxis1stPlaceRearPosition;
        private double rxAxis1stPlaceRearPosition;


        //Place Pos 2nd
        private double xAxis2ndPlaceFrontPosition;
        private double zAxis2ndPlaceFrontPosition;
        private double rxAxis2ndPlaceFrontPosition;

        private double xAxis2ndPlaceRearPosition;
        private double zAxis2ndPlaceRearPosition;
        private double rxAxis2ndPlaceRearPosition;


        //Place Push In Cam
        private double xAxisPrePushInPlaceFrontPosition;
        private double zAxisPushInPlaceFrontPosition;
        private double rxAxisPushInPlaceFrontPosition;

        private double xAxisPrePushInPlaceRearPosition;
        private double zAxisPushInPlaceRearPosition;
        private double rxAxisPushInPlaceRearPosition;

        //Push offset
        private double pushOffsetX;
        private double prePushOffsetZ;

        #endregion
    }
}
