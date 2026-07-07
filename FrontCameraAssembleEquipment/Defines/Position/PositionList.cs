using EQX.Core.Motion;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Defines.Units;
using FrontCameraAssembleEquipment.Helpers;
using System.Collections.ObjectModel;

namespace FrontCameraAssembleEquipment.Defines
{
    public class PositionList
    {
        // === Tray Supplier Positions ===
        public PositionGroup TrayInElevator_LimitUpPos { get; set; }
        public PositionGroup TrayInElevator_InputTrayPos { get; set; }
        public PositionGroup TrayOutElevator_ReadyPlacePos { get; set; }
        public PositionGroup TrayOutElevator_OutputTrayPos { get; set; }
        public PositionGroup TrayOutElevator_LimitDownTraySearchPos { get; set; }

        // === Tray Head Positions ===
        public PositionGroup TrayHead_WaitPos { get; set; }
        public PositionGroup TrayHead_CamPickPos { get; set; }
        public PositionGroup TrayHead_CamReadyPickPos { get; set; }
        public PositionGroup TrayHead_CamScanPos { get; set; }
        public PositionGroup TrayHead_CamPlacePos { get; set; }
        public PositionGroup TrayHead_CamReadyPlacePos { get; set; }
        public PositionGroup TrayHead_TrayPickPos { get; set; }
        public PositionGroup TrayHead_TrayPlacePos { get; set; }

        // === Film Detach Head Positions ===
        public PositionGroup FilmDetachHead_ReadyPos { get; set; }
        public PositionGroup FilmDetachHead_FrontDetachPos { get; set; }
        public PositionGroup FilmDetachHead_RearDetachPos { get; set; }
        public PositionGroup FilmDetachHead_FrontSuctionPos { get; set; }
        public PositionGroup FilmDetachHead_RearSuctionPos { get; set; }
        public PositionGroup FilmDetachHead_FrontCleanPos { get; set; }
        public PositionGroup FilmDetachHead_RearCleanPos { get; set; }

        // === Camera Head Positions ===
        public PositionGroup CamHead_ReadyPickPos { get; set; }
        public PositionGroup CamHead_PickPos { get; set; }

        public PositionGroup CamHead_FrontReadyPlacePos { get; set; }
        public PositionGroup CamHead_RearReadyPlacePos { get; set; }
        public PositionGroup CamHead_FrontPlace1stPos { get; set; }
        public PositionGroup CamHead_RearPlace1stPos { get; set; }
        public PositionGroup CamHead_FrontPlace2ndPos { get; set; }
        public PositionGroup CamHead_RearPlace2ndPos { get; set; }
        //push 
        public PositionGroup CamHead_FrontPrePushInPlacePos { get; set; }
        public PositionGroup CamHead_RearPrePushInPlacePos { get; set; }


        public PositionList(RecipeSelector recipeSelector, RecipeList recipeList, Devices devices, AxisUnitList axisUnitList)
        {
            _recipeSelector = recipeSelector;
            _recipeList = recipeList;
            _devices = devices;
            _axisUnitList = axisUnitList;

            // === Tray Supplier Positions ===
            TrayInElevator_LimitUpPos = new PositionGroup
            {
                Name = EPosition.TrayInElevator_LimitUpPos.ToString(),
                Description = EPosition.TrayInElevator_LimitUpPos.GetDescription(),
                AxisUnit = _axisUnitList.TraySupplier,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayInputZ,  RecipePropertyPath = "TraySuplierRecipe.ZAxisLimitUpPosition" },
                }
            };

            TrayInElevator_InputTrayPos = new PositionGroup
            {
                Name = EPosition.TrayInElevator_InputTrayPos.ToString(),
                Description = EPosition.TrayInElevator_InputTrayPos.GetDescription(),
                AxisUnit = _axisUnitList.TraySupplier,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayInputZ,  RecipePropertyPath = "TraySuplierRecipe.ZAxisInputTrayPosition" },
                }
            };

            TrayOutElevator_ReadyPlacePos = new PositionGroup
            {
                Name = EPosition.TrayOutElevator_ReadyPlacePos.ToString(),
                Description = EPosition.TrayOutElevator_ReadyPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.TraySupplier,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayOutputZ,  RecipePropertyPath = "TraySuplierRecipe.ZAxisReadyPlaceTrayPosition" },
                }
            };

            TrayOutElevator_OutputTrayPos = new PositionGroup
            {
                Name = EPosition.TrayOutElevator_OutputTrayPos.ToString(),
                Description = EPosition.TrayOutElevator_OutputTrayPos.GetDescription(),
                AxisUnit = _axisUnitList.TraySupplier,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayOutputZ,  RecipePropertyPath = "TraySuplierRecipe.ZAxisReadyOutputTrayPosition" },
                }
            };

            TrayOutElevator_LimitDownTraySearchPos = new PositionGroup
            {
                Name = EPosition.TrayOutElevator_LimitDownTraySearchPos.ToString(),
                Description = EPosition.TrayOutElevator_LimitDownTraySearchPos.GetDescription(),
                AxisUnit = _axisUnitList.TraySupplier,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayOutputZ,  RecipePropertyPath = "TraySuplierRecipe.ZAxisLimitDownTraySearchPosition" },
                }
            };


            // === Tray Head Positions ===
            TrayHead_WaitPos = new PositionGroup
            {
                Name = EPosition.TrayHead_ReadyPos.ToString(),
                Description = EPosition.TrayHead_ReadyPos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis,  RecipePropertyPath = "TrayHeadRecipe.XAxisWaitPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis,  RecipePropertyPath = "TrayHeadRecipe.YAxisWaitPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadZAxis,  RecipePropertyPath = "TrayHeadRecipe.ZAxisReadyPosition" },
                }
            };

            TrayHead_CamPickPos = new PositionGroup
            {
                Name = EPosition.TrayHead_CamPickPos.ToString(),
                Description = EPosition.TrayHead_CamPickPos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisCamPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisCamPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadZAxis, RecipePropertyPath = "TrayHeadRecipe.ZAxisCamPickPosition" },
                }
            };

            TrayHead_CamScanPos = new PositionGroup
            {
                Name = EPosition.TrayHead_CamScanPos.ToString(),
                Description = EPosition.TrayHead_CamScanPos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisCamScanPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisCamScanPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadZAxis, RecipePropertyPath = "TrayHeadRecipe.ZAxisCamScanPosition" },
                }
            };

            TrayHead_CamPlacePos = new PositionGroup
            {
                Name = EPosition.TrayHead_CamPlacePos.ToString(),
                Description = EPosition.TrayHead_CamPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisCamPlacePosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisCamPlacePosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadZAxis, RecipePropertyPath = "TrayHeadRecipe.ZAxisCamPlacePosition" },
                }
            };

            TrayHead_CamReadyPlacePos = new PositionGroup
            {
                Name = EPosition.TrayHead_CamReadyPlacePos.ToString(),
                Description = EPosition.TrayHead_CamReadyPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisCamPlaceReadyPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisCamPlaceReadyPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadZAxis, RecipePropertyPath = "TrayHeadRecipe.ZAxisCamPlaceReadyPosition" },
                }
            };

            TrayHead_TrayPickPos = new PositionGroup
            {
                Name = EPosition.TrayHead_TrayPickPos.ToString(),
                Description = EPosition.TrayHead_TrayPickPos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisTrayPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisTrayPickPosition" },
                }
            };

            TrayHead_TrayPlacePos = new PositionGroup
            {
                Name = EPosition.TrayHead_TrayPlacePos.ToString(),
                Description = EPosition.TrayHead_TrayPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.TrayHead,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadXAxis, RecipePropertyPath = "TrayHeadRecipe.XAxisTrayPlacePosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.TrayHeadYAxis, RecipePropertyPath = "TrayHeadRecipe.YAxisTrayPlacePosition" },
                }
            };


            // === Film Detach Head Positions ===
            FilmDetachHead_ReadyPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_ReadyPos.ToString(),
                Description = EPosition.FilmDetachHead_ReadyPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisReadyPosition" },
                }
            };

            FilmDetachHead_FrontDetachPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_FrontDetachPos.ToString(),
                Description = EPosition.FilmDetachHead_FrontDetachPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisFrontFilmDetachPosition" },
                }
            };

            FilmDetachHead_RearDetachPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_RearDetachPos.ToString(),
                Description = EPosition.FilmDetachHead_RearDetachPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisRearFilmDetachPosition" },
                }
            };

            FilmDetachHead_FrontSuctionPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_FrontSuctionPos.ToString(),
                Description = EPosition.FilmDetachHead_FrontSuctionPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisFrontFilmSuctionPosition" },
                }
            };

            FilmDetachHead_RearSuctionPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_RearSuctionPos.ToString(),
                Description = EPosition.FilmDetachHead_RearSuctionPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisRearFilmSuctionPosition" },
                }
            };

            FilmDetachHead_FrontCleanPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_FrontCleanPos.ToString(),
                Description = EPosition.FilmDetachHead_FrontCleanPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisFrontCleanFilmPosition" },
                }
            };

            FilmDetachHead_RearCleanPos = new PositionGroup
            {
                Name = EPosition.FilmDetachHead_RearCleanPos.ToString(),
                Description = EPosition.FilmDetachHead_RearCleanPos.GetDescription(),
                AxisUnit = _axisUnitList.FilmDetach,
                MoveType = EMoveType.Normal,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.FilmDetachY, RecipePropertyPath = "FilmDetachHeadRecipe.YAxisRearCleanFilmPosition" },
                }
            };


            // === Camera Head Positions ===
            CamHead_ReadyPickPos = new PositionGroup
            {
                Name = EPosition.CamHead_ReadyPickPos.ToString(),
                Description = EPosition.CamHead_ReadyPickPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisReadyPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisReadyPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxisReadyPosition" },
                }
            };

            CamHead_PickPos = new PositionGroup
            {
                Name = EPosition.CamHead_PickPos.ToString(),
                Description = EPosition.CamHead_PickPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxisPickPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxisPickPosition" },
                }
            };

            CamHead_FrontReadyPlacePos = new PositionGroup
            {
                Name = EPosition.CamHead_FrontReadyPlacePos.ToString(),
                Description = EPosition.CamHead_FrontReadyPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisReadyPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceFrontPosition" },
                }
            };

            CamHead_RearReadyPlacePos = new PositionGroup
            {
                Name = EPosition.CamHead_RearReadyPlacePos.ToString(),
                Description = EPosition.CamHead_RearReadyPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.Continous,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisReadyPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceRearPosition" },

                }
            };

            CamHead_FrontPlace1stPos = new PositionGroup
            {
                Name = EPosition.CamHead_FrontPlace1stPos.ToString(),
                Description = EPosition.CamHead_FrontPlace1stPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxis1stPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxis1stPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxis1stPlaceFrontPosition" },
                }
            };

            CamHead_FrontPlace2ndPos = new PositionGroup
            {
                Name = EPosition.CamHead_FrontPlace2ndPos.ToString(),
                Description = EPosition.CamHead_FrontPlace2ndPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxis2ndPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxis2ndPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxis2ndPlaceFrontPosition" },
                }
            };

            CamHead_RearPlace1stPos = new PositionGroup
            {
                Name = EPosition.CamHead_RearPlace1stPos.ToString(),
                Description = EPosition.CamHead_RearPlace1stPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxis1stPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxis1stPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxis1stPlaceRearPosition" },
                }
            };

            CamHead_RearPlace2ndPos = new PositionGroup
            {
                Name = EPosition.CamHead_RearPlace2ndPos.ToString(),
                Description = EPosition.CamHead_RearPlace2ndPos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxis2ndPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxis2ndPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxis2ndPlaceRearPosition" },
                }
            };

            CamHead_FrontPrePushInPlacePos = new PositionGroup
            {
                Name = EPosition.CamHead_FrontPrePushInPlacePos.ToString(),
                Description = EPosition.CamHead_FrontPrePushInPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisPrePushInPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxisPushInPlaceFrontPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxisPushInPlaceFrontPosition" },
                }
            };

            CamHead_RearPrePushInPlacePos = new PositionGroup
            {
                Name = EPosition.CamHead_RearPrePushInPlacePos.ToString(),
                Description = EPosition.CamHead_RearPrePushInPlacePos.GetDescription(),
                AxisUnit = _axisUnitList.CameraAssemble,
                MoveType = EMoveType.ContinousAll,
                Positions = new ObservableCollection<Position>()
                {
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceX, RecipePropertyPath = "CameraHeadRecipe.XAxisPrePushInPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceY, RecipePropertyPath = "CameraHeadRecipe.YAxisPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceZ, RecipePropertyPath = "CameraHeadRecipe.ZAxisPushInPlaceRearPosition" },
                    new Position(_recipeSelector) { Axis = _devices.Motions.AssemblePickPlaceRX, RecipePropertyPath = "CameraHeadRecipe.RXAxisPushInPlaceRearPosition" },
                }
            };

        }

        private readonly RecipeSelector _recipeSelector;
        private readonly RecipeList _recipeList;
        private readonly Devices _devices;
        private readonly AxisUnitList _axisUnitList;
    }
}
