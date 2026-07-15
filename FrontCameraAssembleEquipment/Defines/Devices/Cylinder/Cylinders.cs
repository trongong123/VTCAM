using EQX.Core.InOut;
using EQX.InOut;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Cylinders
    {
        // Unit: CameraTray Supplier
        public ICylinder TraySupplier_TrayInStopper { get; }
        public ICylinder TraySupplier_TrayCentering1 { get; }
        public ICylinder TraySupplier_TrayCentering2 { get; }

        //Unit: Picker Head
        public ICylinder TrayPicker { get; }

        //Unit: Flipper Sponge Detach
        public ICylinder FlipperSpongeDetach_VtCamCentering { get; }
        public ICylinder FlipperSpongeDetach_SpongePickupMoverFwBw { get; }
        public ICylinder FlipperSpongeDetach_SpongePickupMoverUpDn { get; }
        public ICylinder FlipperSpongeDetach_SpongeHoldGripper { get; }
        public ICylinder FlipperSpongeDetach_VtCamRotatorMoverFwBw { get; }
        public ICylinder FlipperSpongeDetach_VtCamRotatorMoverUpDn { get; }
        public ICylinder FlipperSpongeDetach_VtCamRotatorGripper { get; }
        public ICylinder FlipperSpongeDetach_VtCamRotatorFlipper { get; }

        //Unit: Film Detach Head
        public ICylinder FilmDetach_MoverUpDn { get; }
        public ICylinder FilmDetach_GripperOnOff { get; }

        //Unit: Cam Head
        ////public ICylinder CamHead_MoverUpDn { get; }

        //Unit: Set CV
        public ICylinder SetCV_FrontDetachStopperUpDn { get; }
        public ICylinder SetCV_FrontDetachCentering { get; }
        public ICylinder SetCV_FrontAssembleStopperUpDn { get; }
        public ICylinder SetCV_FrontAssembleCentering { get; }
        public ICylinder SetCV_RearDetachStopperUpDn { get; }
        public ICylinder SetCV_RearDetachCentering { get; }
        public ICylinder SetCV_RearAssembleStopperUpDn { get; }
        public ICylinder SetCV_RearAssembleCentering { get; }
        public ICylinder SetCV_FrontUnloadMoverUpDn { get; }
        public ICylinder SetCV_RearUnloadMoverUpDn { get; }
        public ICylinder SetCV_FrontUnloadStopperUpDn { get; }
        public ICylinder SetCV_FrontUnloadTurnReturn { get; }

        public Cylinders(ICylinderFactory cylinderFactory, Inputs inputs, Outputs outputs, Motions motions, RecipeList recipeList, ProcessConfig processConfig)
        {
            _cylinderFactory = cylinderFactory;
            _inputs = inputs;
            _outputs = outputs;
            _motions = motions;
            _recipeList = recipeList;
            _processConfig = processConfig;
            // Unit: CameraTray Supplier
            TraySupplier_TrayInStopper = _cylinderFactory
                .Create(_inputs.TrayInCV1StopperUP, _inputs.TrayInCV1StopperDN, _outputs.TrayInCv1StopperUp, null)
                .SetIdentity((int)ECylinder.TraySupplier_TrayInStopper, ECylinder.TraySupplier_TrayInStopper.GetDescription());
            TraySupplier_TrayInStopper.CylinderType = ECylinderType.UpDown;

            TraySupplier_TrayCentering1 = _cylinderFactory
                .Create(_inputs.TrayCentering1On, _inputs.TrayCentering1Off, _outputs.TrayCenteringOn, null)
                .SetIdentity((int)ECylinder.TraySupplier_TrayCentering1, ECylinder.TraySupplier_TrayCentering1.GetDescription());
            TraySupplier_TrayCentering1.CylinderType = ECylinderType.AlignUnalign;

            TraySupplier_TrayCentering2 = _cylinderFactory
                .Create(_inputs.TrayCentering2On, _inputs.TrayCentering2Off, _outputs.TrayCenteringOn, null)
                .SetIdentity((int)ECylinder.TraySupplier_TrayCentering2, ECylinder.TraySupplier_TrayCentering2.GetDescription());
            TraySupplier_TrayCentering2.CylinderType = ECylinderType.AlignUnalign;

            // Unit: Picker Head
            TrayPicker = _cylinderFactory
                .Create(_inputs.TrayPickerDown, _inputs.TrayPickerUp, _outputs.TrayPickerDw, null)
                .SetIdentity((int)ECylinder.TrayPicker_UpDown, ECylinder.TrayPicker_UpDown.GetDescription());
            TrayPicker.CylinderType = ECylinderType.UpDownReverse;

            // Unit: Flipper Sponge Detach
            FlipperSpongeDetach_VtCamCentering = _cylinderFactory
                .Create(_inputs.VtCamCenteringOn, _inputs.VtCamCenteringOff, _outputs.VtCamCenteringOn, _outputs.VtCamCenteringOff)
                .SetIdentity((int)ECylinder.SpongeDetach_VtCamCentering, ECylinder.SpongeDetach_VtCamCentering.GetDescription());
            FlipperSpongeDetach_VtCamCentering.CylinderType = ECylinderType.GripUngrip;

            FlipperSpongeDetach_SpongePickupMoverFwBw = _cylinderFactory
                .Create(_inputs.SpongePickupFw, _inputs.SpongePickupBw, _outputs.SpongePickupFw, null)
                .SetIdentity((int)ECylinder.SpongeDetach_SpongePickupMoverFwBw, ECylinder.SpongeDetach_SpongePickupMoverFwBw.GetDescription());
            FlipperSpongeDetach_SpongePickupMoverFwBw.CylinderType = ECylinderType.ForwardBackward;

            FlipperSpongeDetach_SpongePickupMoverUpDn = _cylinderFactory
                .Create(_inputs.SpongePickupUp, _inputs.SpongePickupDown, _outputs.SpongePickupUp, _outputs.SpongePickupDown)
                .SetIdentity((int)ECylinder.SpongeDetach_SpongePickupMoverUpDn, ECylinder.SpongeDetach_SpongePickupMoverUpDn.GetDescription());
            FlipperSpongeDetach_SpongePickupMoverUpDn.CylinderType = ECylinderType.UpDown;

            FlipperSpongeDetach_SpongeHoldGripper = _cylinderFactory
                .Create(_inputs.SpongeHoldGripOn, _inputs.SpongeHoldGripOff, _outputs.SpongeHoldGripOn, _outputs.SpongeHoldGripOff)
                .SetIdentity((int)ECylinder.SpongeDetach_SpongeHoldGripper, ECylinder.SpongeDetach_SpongeHoldGripper.GetDescription());
            FlipperSpongeDetach_SpongeHoldGripper.CylinderType = ECylinderType.GripUngrip;

            FlipperSpongeDetach_VtCamRotatorMoverFwBw = _cylinderFactory
                .Create(_inputs.VtCamRotatorFw, _inputs.VtCamRotatorBw, _outputs.VtCamRotatorFw, null)
                .SetIdentity((int)ECylinder.Flipper_VtCamRotatorMoverFwBw, ECylinder.Flipper_VtCamRotatorMoverFwBw.GetDescription());
            FlipperSpongeDetach_VtCamRotatorMoverFwBw.CylinderType = ECylinderType.ForwardBackward;

            FlipperSpongeDetach_VtCamRotatorMoverUpDn = _cylinderFactory
                .Create(_inputs.VtCamRotatorUp, _inputs.VtCamRotatorDown, _outputs.VtCamRotatorUp, _outputs.VtCamRotatorDown)
                .SetIdentity((int)ECylinder.Flipper_VtCamRotatorMoverUpDn, ECylinder.Flipper_VtCamRotatorMoverUpDn.GetDescription());
            FlipperSpongeDetach_VtCamRotatorMoverUpDn.CylinderType = ECylinderType.UpDown;

            FlipperSpongeDetach_VtCamRotatorGripper = _cylinderFactory
                .Create(_inputs.VtCamRotatorGrip, null, _outputs.VtCamRotatorGrip, _outputs.VtCamRotatorUnGrip)
                .SetIdentity((int)ECylinder.Flipper_VtCamRotatorGripper, ECylinder.Flipper_VtCamRotatorGripper.GetDescription());
            FlipperSpongeDetach_VtCamRotatorGripper.CylinderType = ECylinderType.GripUngrip;

            FlipperSpongeDetach_VtCamRotatorFlipper = _cylinderFactory
                .Create(_inputs.VtCamRotator180, _inputs.VtCamRotator0, _outputs.VtCamRotator180, _outputs.VtCamRotator0    )
                .SetIdentity((int)ECylinder.Flipper_VtCamRotatorFlipper, ECylinder.Flipper_VtCamRotatorFlipper.GetDescription());
            FlipperSpongeDetach_VtCamRotatorFlipper.CylinderType = ECylinderType.FlipUnflip;

            // Unit: Film Detach Head
            FilmDetach_MoverUpDn = _cylinderFactory
                .Create(_inputs.FilmDetachDown, _inputs.FilmDetachUp, _outputs.FilmDetachDw, null)
                .SetIdentity((int)ECylinder.FilmDetach_MoverUpDn, ECylinder.FilmDetach_MoverUpDn.GetDescription());
            FilmDetach_MoverUpDn.CylinderType = ECylinderType.UpDownReverse;

            FilmDetach_GripperOnOff = _cylinderFactory
                .Create(_inputs.FilmDetachGrip, _inputs.FilmDetachUnGrip, _outputs.FilmDetachGrip, _outputs.FilmDetachUnGrip)
                .SetIdentity((int)ECylinder.FilmDetach_GripperOnOff, ECylinder.FilmDetach_GripperOnOff.GetDescription());
            FilmDetach_GripperOnOff.CylinderType = ECylinderType.GripUngrip;

            // Unit: Cam Head
            //CamHead_MoverUpDn = _cylinderFactory
            //    .Create(_inputs.VtCamAssemblePnPDown, _inputs.VtCamAssemblePnPUp, _outputs.VtCamAssemblePnPDown, null)
            //    .SetIdentity((int)ECylinder.CamHead_MoverUpDn, ECylinder.CamHead_MoverUpDn.ToString());
            //CamHead_MoverUpDn.CylinderType = ECylinderType.UpDownReverse;

            // Unit: Set CV
            SetCV_FrontDetachStopperUpDn = _cylinderFactory
                .Create(_inputs.FrontDetachCvStopperUp, null, _outputs.FrontDetachCvStopperUp, null)
                .SetIdentity((int)ECylinder.SetCV_FrontDetachStopperUpDn, ECylinder.SetCV_FrontDetachStopperUpDn.GetDescription());
            SetCV_FrontDetachStopperUpDn.CylinderType = ECylinderType.UpDown;

            SetCV_FrontDetachCentering = _cylinderFactory
                .Create(_inputs.FrontDetachCvCenteringNG, _inputs.FrontDetachCvCenteringOff, _outputs.FrontDetachCvCenteringOn, null)
                .SetIdentity((int)ECylinder.SetCV_FrontDetachCentering, ECylinder.SetCV_FrontDetachCentering.GetDescription());
            SetCV_FrontDetachCentering.CylinderType = ECylinderType.AlignUnalign;

            SetCV_FrontAssembleStopperUpDn = _cylinderFactory
                .Create(_inputs.FrontAssembleCvStopperUp, null, _outputs.FrontAssembleCvStopperUp, null)
                .SetIdentity((int)ECylinder.SetCV_FrontAssembleStopperUpDn, ECylinder.SetCV_FrontAssembleStopperUpDn.GetDescription());
            SetCV_FrontAssembleStopperUpDn.CylinderType = ECylinderType.UpDown;

            SetCV_FrontAssembleCentering = _cylinderFactory
                .Create(_inputs.FrontAssembleCvCenteringNG, _inputs.FrontAssembleCvCenteringOff, _outputs.FrontAssembleCvCenteringOn, null)
                .SetIdentity((int)ECylinder.SetCV_FrontAssembleCentering, ECylinder.SetCV_FrontAssembleCentering.GetDescription());
            SetCV_FrontAssembleCentering.CylinderType = ECylinderType.AlignUnalign;

            SetCV_RearDetachStopperUpDn = _cylinderFactory
                .Create(_inputs.RearDetachCvStopperUp, null, _outputs.RearDetachCvStopperUp, null)
                .SetIdentity((int)ECylinder.SetCV_RearDetachStopperUpDn, ECylinder.SetCV_RearDetachStopperUpDn.GetDescription());
            SetCV_RearDetachStopperUpDn.CylinderType = ECylinderType.UpDown;

            SetCV_RearDetachCentering = _cylinderFactory
                .Create(_inputs.RearDetachCvCenteringNG, _inputs.RearDetachCvCenteringOff, _outputs.RearDetachCvCenteringOn, null)
                .SetIdentity((int)ECylinder.SetCV_RearDetachCentering, ECylinder.SetCV_RearDetachCentering.GetDescription());
            SetCV_RearDetachCentering.CylinderType = ECylinderType.AlignUnalign;

            SetCV_RearAssembleStopperUpDn = _cylinderFactory
                .Create(_inputs.RearAssembleCvStopperUp, null, _outputs.RearAssembleCvStopperUp, null)
                .SetIdentity((int)ECylinder.SetCV_RearAssembleStopperUpDn, ECylinder.SetCV_RearAssembleStopperUpDn.GetDescription());
            SetCV_RearAssembleStopperUpDn.CylinderType = ECylinderType.UpDown;

            SetCV_RearAssembleCentering = _cylinderFactory
                .Create(_inputs.RearAssembleCvCenteringNG, _inputs.RearAssembleCvCenteringOff, _outputs.RearAssembleCvCenteringOn, null)
                .SetIdentity((int)ECylinder.SetCV_RearAssembleCentering, ECylinder.SetCV_RearAssembleCentering.GetDescription());
            SetCV_RearAssembleCentering.CylinderType = ECylinderType.AlignUnalign;

            SetCV_RearAssembleCentering = _cylinderFactory
                .Create(_inputs.RearAssembleCvCenteringNG, _inputs.RearAssembleCvCenteringOff, _outputs.RearAssembleCvCenteringOn, null)
                .SetIdentity((int)ECylinder.SetCV_RearAssembleCentering, ECylinder.SetCV_RearAssembleCentering.GetDescription());
            SetCV_RearAssembleCentering.CylinderType = ECylinderType.AlignUnalign;

            SetCV_FrontUnloadMoverUpDn = _cylinderFactory
                .Create(_inputs.FrontUnloadPosUp, _inputs.FrontUnloadPosDown, _outputs.FrontUnloadPosUp, null)
                .SetIdentity((int)ECylinder.SetCV_FrontUnloadMoverUpDn, ECylinder.SetCV_FrontUnloadMoverUpDn.GetDescription());
            SetCV_FrontUnloadMoverUpDn.CylinderType = ECylinderType.UpDown;

            SetCV_RearUnloadMoverUpDn = _cylinderFactory
                .Create(_inputs.RearUnloadPosUp, _inputs.RearUnloadPosDown, _outputs.RearUnloadPosUp, null)
                .SetIdentity((int)ECylinder.SetCV_RearUnloadMoverUpDn, ECylinder.SetCV_RearUnloadMoverUpDn.GetDescription());
            SetCV_RearUnloadMoverUpDn.CylinderType = ECylinderType.UpDown;

            if(!_processConfig.IsTwoConveyor)
            {
                SetCV_FrontUnloadStopperUpDn = _cylinderFactory
                    .Create(_inputs.FrontUnloadCvStopperUp, null, _outputs.FrontUnloadCvStopperUp, null)
                    .SetIdentity((int)ECylinder.SetCV_FrontUnloadStopperUpDn, ECylinder.SetCV_FrontUnloadStopperUpDn.GetDescription());
                SetCV_FrontUnloadStopperUpDn.CylinderType = ECylinderType.UpDown;

                SetCV_FrontUnloadTurnReturn = _cylinderFactory
                    .Create(_inputs.FrontUnloadCvTurn, _inputs.FrontUnloadCvReturn, _outputs.FrontUnloadCvTurn, null)
                    .SetIdentity((int)ECylinder.SetCV_FrontUnloadTurnReturn, ECylinder.SetCV_FrontUnloadTurnReturn.GetDescription());
                SetCV_FrontUnloadTurnReturn.CylinderType = ECylinderType.FlipUnflip;
            }

        }

        private readonly ProcessConfig _processConfig;
        private readonly ICylinderFactory _cylinderFactory;
        private readonly Inputs _inputs;
        private readonly Outputs _outputs;
        private readonly Motions _motions;
        private readonly RecipeList _recipeList;
    }
}
