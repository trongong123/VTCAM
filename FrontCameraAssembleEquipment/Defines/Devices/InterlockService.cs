using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Interlock;
using EQX.Core.Motion;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class InterlockService : ILogable
    {
        private readonly Devices _devices;
        private readonly MachineStatus _machineStatus;
        private readonly RecipeSelector _recipeSelector;
        private Motions _motions => _devices.Motions;
        private RecipeList _recipeList => _recipeSelector.CurrentRecipe;

        public ILog Log => LogManager.GetLogger("Interlock");

        public InterlockService(Devices devices, MachineStatus machineStatus, RecipeSelector recipeSelector)
        {
            _devices = devices;
            _machineStatus = machineStatus;
            _recipeSelector = recipeSelector;
            InterlockMonitor.OnInterlockBlocked += HandleInterlockBlocked;

        }

        public void Config(bool isDisable = false)
        {
            CylindersInterlock(isDisable);
        }

        private void CylindersInterlock(bool isDisable)
        {
            if (isDisable)
            {
                var cylinders = typeof(Cylinders).GetProperties()
                    .Where(p => p.PropertyType == typeof(ICylinder))
                    .Select(p => p.GetValue(_devices.Cylinders) as ICylinder)
                    .Where(c => c is not null);

                foreach (var cylinder in cylinders)
                {
                    cylinder!.ForwardInterlocks = new Dictionary<string, Func<bool>>();
                    cylinder.BackwardInterlocks = new Dictionary<string, Func<bool>>();
                }

                return;
            }

            _devices.Cylinders.TrayPicker.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"XY Axis not in Pick / Place Position" , () =>(_motions.TrayHeadXAxis.IsOnPosition(_recipeList.TrayHeadRecipe.XAxisTrayPickPosition) == true && _motions.TrayHeadYAxis.IsOnPosition(_recipeList.TrayHeadRecipe.YAxisTrayPickPosition) == true)
                                                            || (_motions.TrayHeadXAxis.IsOnPosition(_recipeList.TrayHeadRecipe.XAxisTrayPlacePosition) == true && _motions.TrayHeadYAxis.IsOnPosition(_recipeList.TrayHeadRecipe.YAxisTrayPlacePosition) == true) }
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamCentering.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Rotator not in Safety Position", () =>  (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw!.IsForward
                                                           && _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn!.IsBackward) == false}
            };

            _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Cylinder Sponge UP/DOWN is DOWN Position" , () =>  _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverUpDn!.IsForward },

                {"Rotator FW/BW CYL is FORWARD Position and Rotator UP/DOWN CYL is UP Position"  ,() => (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw!.IsBackward || _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn!.IsBackward)},

                {"Tray CAM Loader in Camera Place Position" , () =>  (_motions.TrayHeadXAxis.IsOnPosition(_recipeList.TrayHeadRecipe.XAxisCamPlacePosition) == false &&
                                                                _motions.TrayHeadYAxis.IsOnPosition(_recipeList.TrayHeadRecipe.YAxisCamPlacePosition) == false) ||
                                                                _motions.TrayHeadZAxis.Status.ActualPosition <= 5}
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {

                {"Sponge FW/BW CYL is FORWARD Position" , () =>  _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw!.IsBackward},

                {"Rotator UP/DOWN CYL is DOWN Position" , () => _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn!.IsForward},

                {"Tray CAM Loader Axis in Camera Place Position" , () =>  (_motions.TrayHeadXAxis.IsOnPosition(_recipeList.TrayHeadRecipe.XAxisCamPlacePosition) == false &&
                                                                _motions.TrayHeadYAxis.IsOnPosition(_recipeList.TrayHeadRecipe.YAxisCamPlacePosition) == false) ||
                                                                _motions.TrayHeadZAxis.Status.ActualPosition <= 5}
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn.BackwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Rotator FW/BW CYL is FORWARD Position and Rotator CYL is TURN Position", () => (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.IsForward
                            && _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper.IsForward) == false }
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Sponge FW/BW CYL is FORWARD Position and Rotator FW/BW CYL is FORWARD Position", () => _devices.Cylinders.FlipperSpongeDetach_SpongePickupMoverFwBw.IsBackward
                            || _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.IsBackward },
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Rotator FW/BW CYL is FORWARD Position and Rotator UP/DOWN CYL is DOWN Position" , () => (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.IsBackward) 
                                                                                                                    || (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn.IsForward) }
            };

            _devices.Cylinders.FlipperSpongeDetach_VtCamRotatorFlipper.BackwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Rotator FW/BW CYL is FORWARD Position and Rotator UP/DOWN CYL is DOWN Position" , () => (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverFwBw.IsBackward) 
                                                                                                || (_devices.Cylinders.FlipperSpongeDetach_VtCamRotatorMoverUpDn.IsForward) }
            };

            _devices.Cylinders.FilmDetach_MoverUpDn.ForwardInterlocks = new Dictionary<string, Func<bool>>
            {
                {"Film Detach Y Axis is Motioning" , () => _motions.FilmDetachY.Status.IsMotioning == false }
            };

        }

        private void HandleInterlockBlocked(object? sender, InterlockEventAgrs e)
        {
            string message = $"{e.Obj.Name} blocked by '{e.Message}' while '{e.Action}'";
            if (_machineStatus.IsStandByProcessMode)
            {
                MessageBoxEx.ShowDialog(message, false, "WARNING");
                return;
            }

            Log.Error(message);
        }

    }
}
