using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class FilmDetachHeadRecipe : RecipeBase
    {
        [SingleRecipeDescription(Description = "Use Detect Set Reverse ")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseDetectSetReverse
        {
            get => useDetectSetReverse;
            set
            {
                if (useDetectSetReverse == value) return;
                OnRecipeChanged(useDetectSetReverse, value);
                useDetectSetReverse = value;
            }
        }
        //[SingleRecipeDescription(Description = "Vinyl Detach Suction On time", Detail = "Vinyl Detach Suction On time", Unit = Unit.Second)]
        public double FilmSuctionOnTime
        {
            get => filmSuctionOnTime;
            set
            {
                if (filmSuctionOnTime == value) return;
                OnRecipeChanged(filmSuctionOnTime, value);
                filmSuctionOnTime = value;
            }
        }

        [SingleRecipeDescription(Description = "Vinyl Blow On time", Detail = "Vinyl Blow On time", Unit = Unit.Second)]
        public int FilmBlowOnTime
        {
            get => filmBlowOnTime;
            set
            {
                if (filmBlowOnTime == value) return;
                OnRecipeChanged(filmBlowOnTime, value);
                filmBlowOnTime = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Ready Position", Unit = Unit.mm)]
        public double YAxisReadyPosition
        {
            get => yAxisReadyPosition;
            set
            {
                if (yAxisReadyPosition == value) return;
                OnRecipeChanged(yAxisReadyPosition, value);
                yAxisReadyPosition = value;
            }
        }


        [SingleRecipeDescription(Description = "Y Axis Front CV Film Detach Position", Unit = Unit.mm)]
        public double YAxisFrontFilmDetachPosition
        {
            get => yAxisFrontFilmDetachPosition;
            set
            {
                if (yAxisFrontFilmDetachPosition == value) return;
                OnRecipeChanged(yAxisFrontFilmDetachPosition, value);
                yAxisFrontFilmDetachPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Rear CV Film Detach Position", Unit = Unit.mm)]
        public double YAxisRearFilmDetachPosition
        {
            get => yAxisRearFilmDetachPosition;
            set
            {
                if (yAxisRearFilmDetachPosition == value) return;
                OnRecipeChanged(yAxisRearFilmDetachPosition, value);
                yAxisRearFilmDetachPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Front CV Vinyl Detach Check Position", Unit = Unit.mm)]
        public double YAxisFrontFilmSuctionPosition
        {
            get => yAxisFrontFilmSuctionPosition;
            set
            {
                if (yAxisFrontFilmSuctionPosition == value) return;
                OnRecipeChanged(yAxisFrontFilmSuctionPosition, value);
                yAxisFrontFilmSuctionPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Rear CV Vinyl Detach Check Position", Unit = Unit.mm)]
        public double YAxisRearFilmSuctionPosition
        {
            get => yAxisRearFilmSuctionPosition;
            set
            {
                if (yAxisRearFilmSuctionPosition == value) return;
                OnRecipeChanged(yAxisRearFilmSuctionPosition, value);
                yAxisRearFilmSuctionPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Front CV Clean Position", Unit = Unit.mm)]
        public double YAxisFrontCleanFilmPosition
        {
            get => yAxisFrontCleanFilmPosition;
            set
            {
                if (yAxisFrontCleanFilmPosition == value) return;
                OnRecipeChanged(yAxisFrontCleanFilmPosition, value);
                yAxisFrontCleanFilmPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Rear CV Clean Position", Unit = Unit.mm)]
        public double YAxisRearCleanFilmPosition
        {
            get => yAxisRearCleanFilmPosition;
            set
            {
                if (yAxisRearCleanFilmPosition == value) return;
                OnRecipeChanged(yAxisRearCleanFilmPosition, value);
                yAxisRearCleanFilmPosition = value;
            }
        }

        [SingleRecipeDescription(Description = "Shift Detach Gap", Unit = Unit.mm)]
        public double ShiftDetachMoveGap
        {
            get => shiftDetachMoveGap;
            set
            {
                if (shiftDetachMoveGap == value) return;
                OnRecipeChanged(shiftDetachMoveGap, value);
                shiftDetachMoveGap = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Offset Peeling Position", Unit = Unit.mm)]
        [SingleRecipeMinMax(Min = -50, Max = 50)]
        public double YAxisOffsetPeel
        {
            get => _yAxisOffsetPeel;
            set
            {
                if (_yAxisOffsetPeel == value) return;
                OnRecipeChanged(_yAxisOffsetPeel, value);
                _yAxisOffsetPeel = value;
            }
        }

        [SingleRecipeDescription(Description = "Y Axis Offset Peeling Velocity", Unit = Unit.mmPerSecond)]
        [SingleRecipeMinMax(Min = 1, Max = 9999)]
        public double YAxisOffsetPeelVelocity
        {
            get => _yAxisOffsetPeelVelocity;
            set
            {
                if (_yAxisOffsetPeelVelocity == value) return;
                OnRecipeChanged(_yAxisOffsetPeelVelocity, value);
                _yAxisOffsetPeelVelocity = value;
            }
        }

        [SingleRecipeDescription(Description = "Use Retry Detach Film")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseRetryDetachFilm
        {
            get => useRetryDetachFilm;
            set
            {
                if (useRetryDetachFilm == value) return;
                OnRecipeChanged(useRetryDetachFilm, value);
                useRetryDetachFilm = value;
            }
        }
        [SingleRecipeDescription(Description = "Use Ionizer")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseIonizer
        {
            get => useIonizer;
            set
            {
                if (useIonizer == value) return;
                OnRecipeChanged(useIonizer, value);
                useIonizer = value;
            }
        }

        [SingleRecipeDescription(Description = "Use Sequence Film Detach")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int UseSequenceFilmDetach
        {
            get => useSequenceFilmDetach;
            set
            {
                if (useSequenceFilmDetach == value) return;
                OnRecipeChanged(useSequenceFilmDetach, value);
                useSequenceFilmDetach = value;
            }
        }

        private int useDetectSetReverse;
        private double filmSuctionOnTime;
        private int filmBlowOnTime;
        private int trashSuctionOnTime;
        private double yAxisReadyPosition;
        private double yAxisReadySpeed;
        private double yAxisFrontFilmDetachPosition;
        private double yAxisRearFilmDetachPosition;
        private double yAxisFrontFilmSuctionPosition;
        private double yAxisRearFilmSuctionPosition;
        private double yAxisFrontCleanFilmPosition;
        private double yAxisRearCleanFilmPosition;
        private double shiftDetachMoveGap;
        private double yAxisFilmDetachSpeed;
        private int useIonizer;
        private int useRetryDetachFilm;
        private int useSequenceFilmDetach = 1;
        private double _yAxisOffsetPeel;
        private double _yAxisOffsetPeelVelocity;
    }
}
