using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Inputs : ObservableObject
    {
        private readonly IDInputDevice _dInputDevice;

        public Inputs([FromKeyedServices("InputDevice#1")]IDInputDevice dInputDevice)
        {
            _dInputDevice = dInputDevice;

            Initialize();

            System.Timers.Timer inputUpdateTimer = new System.Timers.Timer(200);
            inputUpdateTimer.Elapsed += InputUpdateTimer_Elapsed;
            inputUpdateTimer.AutoReset = true;
            inputUpdateTimer.Enabled = true;
        }

        public bool Initialize()
        {
            return _dInputDevice.Initialize();
        }

        public bool Connect()
        {
            return _dInputDevice.Connect();
        }

        public bool Disconnect()
        {
            return _dInputDevice.Disconnect();
        }

        private void InputUpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var input in _dInputDevice.Inputs)
            {
                input.RaiseValueUpdated();
            }
        }

        public List<IDInput> All => _dInputDevice.Inputs;

        #region BOARD 1
        public IDInput FrontStartSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_START_SW);
        public IDInput FrontStopSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_STOP_SW);
        public IDInput FrontResetSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_RESET_SW);
        public IDInput FrontPowerOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_POWER_ON);
        public IDInput FrontEmergencyStop => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_EMERGENCY_STOP);
        public IDInput RearStartSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_START_SW);
        public IDInput RearStopSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_STOP_SW);
        public IDInput RearEmergencyStop => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_EMERGENCY_STOP);
        public IDInput FrontDoor => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DOOR);
        public IDInput RightDoor => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.RIGHT_DOOR);
        public IDInput RearDoor => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DOOR);
        public IDInput LightCurtainMutingSW => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.LIGHT_CURTAIN_MUTING_SW);
        public IDInput TrayInCV1StopperUP => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV1_STOPPER_UP);
        public IDInput TrayInCV1StopperDN => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV1_STOPPER_DOWN);
        public IDInput TrayInCV2Level => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV2_LEVEL);
        public IDInput TrayOutCV2Level => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV2_LEVEL);
        #endregion
        #region BOARD 2
        public IDInput TrayCentering1On => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_CENTERING1_ON);
        public IDInput TrayCentering1Off => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_CENTERING1_OFF);
        public IDInput TrayCentering2On => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_CENTERING2_ON);
        public IDInput TrayCentering2Off => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_CENTERING2_OFF);
        public IDInput TrayInCv1DetectStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV1_DETECT_START);
        public IDInput TrayInCv1DetectEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV1_DETECT_END);
        public IDInput TrayInCv1DetectExist => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV1_DETECT_EXIST);
        public IDInput TrayOutCv2DetectStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV2_DETECT_START);
        public IDInput TrayOutCv2DetectEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV2_DETECT_END);
        public IDInput TrayOutCv2DetectExist => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV2_DETECT_EXIST);
        public IDInput TrayOutCv1DetectStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV1_DETECT_START);
        public IDInput TrayOutCv1DetectEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_OUT_CV1_DETECT_END);
        public IDInput TrayInCv2DetectStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV2_DETECT_START);
        public IDInput TrayInCv2DetectEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV2_DETECT_END);
        public IDInput TrayInCv2DetectExist => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_IN_CV2_DETECT_EXIST);
        public IDInput AreaSensorDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.AREA_SENSOR_DETECT);
        #endregion
        #region BOARD 3
        public IDInput TrayPickerUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_PICKER_UP);
        public IDInput TrayPickerDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_PICKER_DOWN);
        public IDInput TrayPickerVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.TRAY_PICKER_VAC_ON);
        public IDInput VtCamSupplyPnPOverload => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_SUPPLY_PP_OVERLOAD);
        public IDInput VtCamSupplyPnPVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_SUPPLY_PP_VAC_ON);
        public IDInput VtCamPrealignVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_PREALIGN_VAC_ON);
        public IDInput VtCamCenteringOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_CENTERING_ON);
        public IDInput VtCamCenteringOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_CENTERING_OFF);
        public IDInput SpongePickupFw => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_PICKUP_FW);
        public IDInput SpongePickupBw => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_PICKUP_BW);
        public IDInput SpongePickupUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_PICKUP_UP);
        public IDInput SpongePickupDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_PICKUP_DW);
        public IDInput SpongeHoldGripOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_HOLD_GRIP_ON);
        public IDInput SpongeHoldGripOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_HOLD_GRIP_OFF);
        public IDInput VtCamAssemblePnPUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ASSEMBLE_PP_UP);
        public IDInput VtCamAssemblePnPDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ASSEMBLE_PP_DOWN);
        #endregion
        #region BOARD 4
        public IDInput VtCamRotatorFw => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_FW);
        public IDInput VtCamRotatorBw => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_BW);
        public IDInput VtCamRotatorUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_UP);
        public IDInput VtCamRotatorDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_DOWN);
        public IDInput VtCamRotatorGrip => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_GRIP);
        public IDInput VtCamRotatorDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_DETECT);
        public IDInput VtCamRotator180 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_180);
        public IDInput VtCamRotator0 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_0);
        public IDInput FilmDetachUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_UP);
        public IDInput FilmDetachDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_DOWN);
        public IDInput FilmDetachGrip => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_GRIP);
        public IDInput FilmDetachUnGrip => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_UNGRIP);
        public IDInput FilmDetachIonizerOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_IONIZER_ON);
        public IDInput FilmDetachSuctionVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FILM_DETACH_SUCTION_VAC_ON);
        public IDInput VtCamAssemblePnPVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ASSEMBLE_PP_VAC_ON);
        public IDInput VtCamAssemblePnPOverload => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ASSEMBLE_PP_OVERLOAD);
        #endregion
        #region BOARD 5
        public IDInput FrontLoadCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_LOAD_CV_START);
        public IDInput FrontLoadCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_LOAD_CV_END);
        public IDInput FrontDetachCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DETACH_CV_START);
        public IDInput FrontDetachCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DETACH_CV_END);
        public IDInput FrontAssembleCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_ASSEMBLE_CV_START);
        public IDInput SpongeHoldVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_HOLD_VAC_ON);
        public IDInput SpongeHoldDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SPONGE_HOLD_DETECT);
        public IDInput FrontAssembleCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_ASSEMBLE_CV_END);
        public IDInput FrontDetachCvStopperUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DETACH_CV_STOPPER_UP);
        public IDInput VtCamRotatorSpongeDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VTCAM_ROTATOR_SPONGE_DETECT);
        public IDInput FrontDetachCvCenteringNG => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DETACH_CV_CENTERING_NG);
        public IDInput FrontDetachCvCenteringOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_DETACH_CV_CENTERING_OFF);
        public IDInput FrontAssembleCvStopperUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_ASSEMBLE_CV_STOPPER_UP);
        public IDInput FrontOutCvSetUpDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_OUT_CV_SET_UP_DETECT);
        public IDInput FrontAssembleCvCenteringNG => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_ASSEMBLE_CV_CENTERING_NG);
        public IDInput FrontAssembleCvCenteringOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_ASSEMBLE_CV_CENTERING_OFF);
        #endregion
        #region BOARD 6
        public IDInput RearLoadCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_LOAD_CV_START);
        public IDInput RearLoadCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_LOAD_CV_END);
        public IDInput RearDetachCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DETACH_CV_START);
        public IDInput RearDetachCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DETACH_CV_END);
        public IDInput RearAssembleCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_ASSEMBLE_CV_START);
        public IDInput RearAssembleCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_ASSEMBLE_CV_END);
        public IDInput UpStreamFrontLoadEnable => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.UPSTREAM_FRONT_LOAD_ENABLE);
        public IDInput UpStreamRearLoadEnable => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.UPSTREAM_REAR_LOAD_ENABLE);
        public IDInput RearDetachCvStopperUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DETACH_CV_STOPPER_UP);
        public IDInput SetReverseDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.SET_REVERSE_DETECT);
        public IDInput RearDetachCvCenteringNG => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DETACH_CV_CENTERING_NG);
        public IDInput RearDetachCvCenteringOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_DETACH_CV_CENTERING_OFF);
        public IDInput RearAssembleCvStopperUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_ASSEMBLE_CV_STOPPER_UP);
        public IDInput RearOutCvSetUpDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_OUT_CV_SET_UP_DETECT);
        public IDInput RearAssembleCvCenteringNG => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_ASSEMBLE_CV_CENTERING_NG);
        public IDInput RearAssembleCvCenteringOff => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_ASSEMBLE_CV_CENTERING_OFF);
        #endregion
        #region BOARD 7
        public IDInput FrontUnloadCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_START);
        public IDInput FrontUnloadCvMid1 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_MID1);
        public IDInput FrontUnloadCvMid2 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_MID2);
        public IDInput FrontUnloadCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_END);
        public IDInput FrontUnloadPosUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_POS_UP);
        public IDInput FrontUnloadPosDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_POS_DOWN);
        public IDInput RearUnloadCvStart => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_CV_START);
        public IDInput RearUnloadCvMid1 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_CV_MID1);
        public IDInput RearUnloadCvMid2 => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_CV_MID2);
        public IDInput RearUnloadCvEnd => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_CV_END);
        public IDInput RearUnloadPosUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_POS_UP);
        public IDInput RearUnloadPosDown => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_UNLOAD_POS_DOWN);
        public IDInput PreFrontCvDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.PRE_FRONT_CV_DETECT);
        public IDInput PreRearCvDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.PRE_REAR_CV_DETECT);
        public IDInput VinylDetect => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.VINYL_DETECT);
        public IDInput FrontUnloadCvVacOn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_VAC_ON);

        #endregion
        #region BOARD 8
        public IDInput DownstreamFrontLoadEnable => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.DOWNSTREAM_FRONT_LOAD_ENABLE);
        public IDInput DownstreamRearLoadEnable => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.DOWNSTREAM_REAR_LOAD_ENABLE);
        public IDInput PreFrontCvSwitch => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_CONVEYOR_SWITCH);
        public IDInput PreRearCvSwitch => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.REAR_CONVEYOR_SWITCH);
        public IDInput InAGV_InWorkPosition => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.LOAD_AGV_ARRIVAL);
        public IDInput InAGV_StartWork => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.LOAD_AGV_CV_RUN);
        public IDInput InAGV_GoConfirm => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.LOAD_AGV_TRANFER_DONE);
        //public IDInput LoadAGVStop => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.LOAD_AGV_STOP);
        public IDInput UnloadAGVArrival => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.UNLOAD_AGV_ARRIVAL);
        public IDInput UnloadAGVCvRun => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.UNLOAD_AGV_CV_RUN);
        public IDInput UnloadAGVComplete => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.UNLOAD_AGV_COMPLETE);
        public IDInput FrontUnloadCvTurn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_TURN);
        public IDInput FrontUnloadCvReturn => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_RETURN);
        public IDInput FrontUnloadCvStopperUp => _dInputDevice.Inputs.First(i => i.Id == (int)EInput.FRONT_UNLOAD_CV_STOPPER_UP);
        #endregion
    }
}
