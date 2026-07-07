using EQX.Core.Device.SpeedController;
using EQX.Device.SpeedController;
using FrontCameraAssembleEquipment.Defines.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class RolllerList
    {
        public List<BD201SRollerController> All { get; }
        public RolllerList(List<BD201SRollerController> speedControllers,
            RecipeList recipeList,
            DevRecipe devRecipe)
        {
            All = speedControllers;
            _recipeList = recipeList;
            _devRecipe = devRecipe;
        }

        public BD201SRollerController TrayInCVRoller => All.First(s => s.Id == (int)ESpeedController.TRAYIN_CV_ROLLER);
        public BD201SRollerController TrayInElevatorRoller => All.First(s => s.Id == (int)ESpeedController.TRAYIN_ELEVATOR_ROLLER);
        public BD201SRollerController TrayOutCVRoller => All.First(s => s.Id == (int)ESpeedController.TRAYOUT_CV_ROLLER);
        public BD201SRollerController TrayOutElevatorRoller => All.First(s => s.Id == (int)ESpeedController.TRAYOUT_ELEVATOR_ROLLER);
        private TraySuplierRecipe _traySuplierRecipe => _recipeList.TraySuplierRecipe;
        private readonly RecipeList _recipeList;
        private readonly DevRecipe _devRecipe;

        public void SetDirection()
        {
            if (_devRecipe.IsUseRollerIOControl) return;

            TrayInCVRoller.SetDirection(true);
            TrayInElevatorRoller.SetDirection(true);
            TrayOutCVRoller.SetDirection(true);
            TrayOutElevatorRoller.SetDirection(true);
        }
        public void RollerSet()
        {
            if (_devRecipe.IsUseRollerIOControl) return;

            /// Set Speed
            TrayInCVRoller.SetSpeed((int)_traySuplierRecipe.CVTrayInSpeed);
            TrayInElevatorRoller.SetSpeed((int)_traySuplierRecipe.CVTrayInElevatorSpeed);
            TrayOutCVRoller.SetSpeed((int)_traySuplierRecipe.CVTrayOutSpeed);
            TrayOutElevatorRoller.SetSpeed((int)_traySuplierRecipe.CVTrayOutElevatorSpeed);
            /// Set Acc
            TrayInCVRoller.SetAcceleration((int)_traySuplierRecipe.Acceleration);
            TrayInElevatorRoller.SetAcceleration((int)_traySuplierRecipe.Acceleration);
            TrayOutCVRoller.SetAcceleration((int)_traySuplierRecipe.Acceleration);
            TrayOutElevatorRoller.SetAcceleration((int)_traySuplierRecipe.Acceleration);
            /// Set Dec 
            TrayInCVRoller.SetDeceleration((int)_traySuplierRecipe.Deceleration);
            TrayInElevatorRoller.SetDeceleration((int)_traySuplierRecipe.Deceleration);
            TrayOutCVRoller.SetDeceleration((int)_traySuplierRecipe.Deceleration);
            TrayOutElevatorRoller.SetDeceleration((int)_traySuplierRecipe.Deceleration);
        }
    }
}
