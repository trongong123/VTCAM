using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class FlipperTapeDetachRecipe : RecipeBase
    {
        [SingleRecipeDescription(Description = "Sponge Remove Down wait ", Detail ="Delay time after sponge remover down", Unit = Unit.MilliSecond)]
        public int SpongeRemoverDownWait
        {
            get => tapeRemoverDownWait;
            set
            {
                if (tapeRemoverDownWait == value) return;
                OnRecipeChanged(tapeRemoverDownWait, value);
                tapeRemoverDownWait = value;
            }
        }

        [SingleRecipeDescription(Description = "Sponge Remove Up wait ", Detail ="Delay time after sponge remover Up", Unit = Unit.MilliSecond)]
        public int SpongeRemoverUpWait
        {
            get => tapeRemoverUpWait;
            set
            {
                if (tapeRemoverUpWait == value) return;
                OnRecipeChanged(tapeRemoverUpWait, value);
                tapeRemoverUpWait = value;
            }
        }

        [SingleRecipeDescription(Description = "Sponge Gripper On Wait ", Detail ="Delay time after sponge gripper on", Unit = Unit.MilliSecond)]
        public int SpongeGripperOnWait
        {
            get => spongeGripperOnWait;
            set
            {
                if (spongeGripperOnWait == value) return;
                OnRecipeChanged(spongeGripperOnWait, value);
                spongeGripperOnWait = value;
            }
        }

        [SingleRecipeDescription(Description = "Flipper Down Wait ", Detail ="Delay time after Flipper Down to pick", Unit = Unit.MilliSecond)]
        public int FlipperDownWait
        {
            get => flipperDownWait;
            set
            {
                if (flipperDownWait == value) return;
                OnRecipeChanged(flipperDownWait, value);
                flipperDownWait = value;
            }
        }

        [SingleRecipeDescription(Description = "Flipper Grip On Wait ", Detail ="Delay time after Flipper Grip On", Unit = Unit.MilliSecond)]
        public int CamGripOnWait
        {
            get => camGripOnWait;
            set
            {
                if (camGripOnWait == value) return;
                OnRecipeChanged(camGripOnWait, value);
                camGripOnWait = value;
            }
        }

        [SingleRecipeDescription(Description = "Prealign Retry Centering")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        [OptionRecipe()]
        public int RetryCentering
        {
            get => retryCentering;
            set
            {
                if (retryCentering == value) return;
                OnRecipeChanged(retryCentering, value);
                retryCentering = value;
            }
        }

        [SingleRecipeDescription(Description = "Sponge Gripper Grip Count", Detail = "Sponge Gripper Grip Count")]
        [SingleRecipeMinMax(Min = 1, Max = 5)]
        public uint SpongeGripperGripCount
        {
            get => spongeGripperGripCount;
            set
            {
                if (spongeGripperGripCount == value) return;
                OnRecipeChanged(spongeGripperGripCount, value);
                spongeGripperGripCount = value;
            }
        }

        [SingleRecipeDescription(Description = "Gripper Remove Sponge Retry Count", Detail = "Gripper To Remove Retry Count")]
        [SingleRecipeMinMax(Min = -1, Max = 10)]
        public int SpongeRemoveGripRetryCount
        {
            get => spongeRemoveGripRetryCount;
            set
            {
                if (spongeRemoveGripRetryCount == value) return;
                OnRecipeChanged(spongeRemoveGripRetryCount, value);
                spongeRemoveGripRetryCount = value;
            }
        }

        [SingleRecipeDescription(Description = "Rotator Gripper Grip Count", Detail = "Rotator Gripper Grip Count")]
        [SingleRecipeMinMax(Min = 0, Max = 5)]
        public int FlipperGripperGripCount
        {
            get { return flipperGripperGripCount; }
            set 
            {
                if (flipperGripperGripCount == value) return;
                OnRecipeChanged(flipperGripperGripCount, value);
                flipperGripperGripCount = value; 
            }
        }


        [SingleRecipeDescription(Description = "Sponge Detect", Detail ="0: Disable, 1:Enable")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        public int SpongeDetect
        {
            get => spongeDetect;
            set
            {
                if (spongeDetect == value) return;
                OnRecipeChanged(spongeDetect, value);
                spongeDetect = value;
            }
        }

        [SingleRecipeDescription(Description = "Sponge Head Function", Detail ="0: Sponge Check, 1: Sponge Vaccum")]
        [SingleRecipeMinMax(Min = 0, Max = 1)]
        public int SpongeHeadFunction
        {
            get => spongeHeadFunction;
            set
            {
                if (spongeHeadFunction == value) return;
                OnRecipeChanged(spongeHeadFunction, value);
                spongeHeadFunction = value;
            }
        }

        

        private int flipperGripperGripCount = 1;
        private int vacCheckWaitTime;
        private int tapeRemoverDownWait;
        private int tapeRemoverUpWait;
        private int spongeGripperOnWait;
        private int flipperDownWait;
        private int camGripOnWait;
        private int retryCentering;
        private uint spongeGripperGripCount = 5;
        private int spongeRemoveGripRetryCount;
        private int spongeDetect;
        private int spongeHeadFunction;
    }
}
