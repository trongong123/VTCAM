using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class SetConveyorRecipe : RecipeBase
    {
        [SingleRecipeDescription(Description = "End Sensor Detach CV Stop Wait", Detail = "Delay time after check End Sensor Exist", Unit = Unit.MilliSecond)]
        public int EndDetachCvStopWait
        {
            get => endDetachCvStopWait;
            set
            {
                if (endDetachCvStopWait == value) return;
                OnRecipeChanged(endDetachCvStopWait, value);
                endDetachCvStopWait = value;
            }
        }

        [SingleRecipeDescription(Description = "End Sensor Assemble CV Stop Wait", Detail = "Delay time after check End Sensor Exist", Unit = Unit.MilliSecond)]
        public int EndAssembleCvStopWait
        {
            get => endAssembleCvStopWait;
            set
            {
                if (endAssembleCvStopWait == value) return;
                OnRecipeChanged(endAssembleCvStopWait, value);
                endAssembleCvStopWait = value;
            }
        }

        private int endDetachCvStopWait;

        [SingleRecipeDescription(Description = "End Sensor Check Set Out Work area", Detail = "Delay time after Set Out Work Area", Unit = Unit.MilliSecond)]
        public int SetOutWorkAreaWait
        {
            get => setOutWorkAreaWait;
            set
            {
                if (setOutWorkAreaWait == value) return;
                OnRecipeChanged(setOutWorkAreaWait, value);
                setOutWorkAreaWait = value;
            }
        }
        [SingleRecipeDescription(Description = "Set CV Detect Timeout", Detail = "Set CV Detect Timeout", Unit = Unit.MilliSecond)]
        public double SetConveyorDetectTimeout
        {
            get => setCVDetectTimeout;
            set
            {
                if (setCVDetectTimeout == value) return;
                OnRecipeChanged(setCVDetectTimeout, value);
                setCVDetectTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "End Sensor Out CV Stop Wait", Detail = "Delay time after check End Sensor Exist", Unit = Unit.MilliSecond)]
        public int OutSetConveyorStopWait
        {
            get { return outSetConveyorStopWait; }
            set { outSetConveyorStopWait = value; }
        }

        private int outSetConveyorStopWait = 500;
        private int setOutWorkAreaWait;

        private int endAssembleCvStopWait;
        private double setCVDetectTimeout;

    }
}
