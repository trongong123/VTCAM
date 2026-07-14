using EQX.Core.Common;
using EQX.Core.InOut;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using System.Xml.Linq;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Outputs
    {
        private readonly IDOutputDevice _dOutputDevice;
        protected readonly ActionAssignableTimer blinkTimer;
        public string Name { get; set; }
        protected string ChangeLedBlinkActionKey => $"{Name}changeLedBlinkAction";

        public Outputs([FromKeyedServices("OutputDevice#1")] IDOutputDevice dOutputDevice,
            [FromKeyedServices("BlinkTimer")] ActionAssignableTimer blinkTimer )
        {
            this.blinkTimer = blinkTimer;
            _dOutputDevice = dOutputDevice;

            Initialize();
        }

        public bool Initialize()
        {
            return _dOutputDevice.Initialize();
        }

        public bool Connect()
        {
            return _dOutputDevice.Connect();
        }

        public bool Disconnect()
        {
            return _dOutputDevice.Disconnect();
        }

        public List<IDOutput> All => _dOutputDevice.Outputs;

        #region BOARD 1
        public IDOutput FrontStartLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_START_LAMP);
        public IDOutput FrontStopLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_STOP_LAMP);
        public IDOutput FrontResetLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_RESET_LAMP);
        public IDOutput RearStopLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_STOP_LAMP);
        public IDOutput RearStartLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_START_LAMP);
        public IDOutput TowerLampRed => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TOWER_LAMP_RED);
        public IDOutput TowerLampYellow => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TOWER_LAMP_YELLOW);
        public IDOutput TowerLampGreen => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TOWER_LAMP_GREEN);
        public IDOutput Buzzer1 => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.BUZZER_1);
        public IDOutput MutingLamp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.MUTING_LAMP);
        public IDOutput AreaSensorBypassOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.AREA_SENSOR_BY_PASS_ON);
        public IDOutput TrayInCv1StopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_IN_CV1_STOPPER_UP);
        public IDOutput ManualMode => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.MANUAL_MODE);
        public IDOutput TrashSuctionOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRASH_SUCTION_ON);
        #endregion
        #region BOARD 2
        public IDOutput TrayCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_CENTERING_ON);
        public IDOutput SpongeHoldVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_HOLD_VAC_OFF);
        public IDOutput LoadAGV_MachineReady => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.LOAD_AGV_READY);
        public IDOutput LoadAGV_PermitToWork => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.LOAD_AGV_READY_INPUT);
        public IDOutput LoadAGV_GoPermission => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.LOAD_AGV_TRANFER_DONE);
        //public IDOutput LoadAGVStop => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.LOAD_AGV_STOP);
        public IDOutput UnloadAGVReady => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UNLOAD_AGV_READY);
        public IDOutput UnloadAGVReadyInput => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UNLOAD_AGV_READY_INPUT);
        public IDOutput UnloadAGVTranferDone => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UNLOAD_AGV_TRANFER_DONE);
        //public IDOutput UnloadAGVStop => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UNLOAD_AGV_COMPLETE);
        public IDOutput TrayInExtCvRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_IN_EXT_CV_RUN);
        public IDOutput TrayOutExtCvRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_OUT_EXT_CV_RUN);
        public IDOutput FrontUnloadCvVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_VAC_ON);
        public IDOutput FrontUnloadCvVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_VAC_OFF);
        #endregion
        #region BOARD 3
        public IDOutput TrayPickerDw => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_PICKER_DW);
        public IDOutput TrayPickerVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_PICKER_VAC_ON);
        public IDOutput TrayPickerVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_PICKER_VAC_OFF);
        public IDOutput VtCamSupplyPnPVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_SUPPLY_PP_VAC_ON);
        public IDOutput VtCamSupplyPnPVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_SUPPLY_PP_VAC_OFF);
        public IDOutput VtCamPrealignVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_PREALIGN_VAC_ON);
        public IDOutput VtCamCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_CENTERING_ON);
        public IDOutput VtCamCenteringOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_CENTERING_OFF);
        public IDOutput VtCamPrealignVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_PREALIGN_VAC_OFF);
        public IDOutput SpongePickupFw => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_PICKUP_FW);
        public IDOutput SpongePickupUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_PICKUP_UP);
        public IDOutput SpongePickupDown => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_PICKUP_DW);
        public IDOutput SpongeHoldGripOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_HOLD_GRIP_ON);
        public IDOutput SpongeHoldGripOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_HOLD_GRIP_OFF);
        public IDOutput SpongeHoldVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.SPONGE_HOLD_VAC_ON);
        public IDOutput FilmDetachIonizerOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_IONIZER_ON);
        #endregion
        #region BOARD 4
        public IDOutput VtCamRotatorFw => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_FW);
        public IDOutput FilmDetachSuctionVacOff => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_SUCTION_VAC_OFF);
        public IDOutput VtCamRotatorUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_UP);
        public IDOutput VtCamRotatorDown => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_DW);
        public IDOutput VtCamRotatorGrip => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_GRIP);
        public IDOutput VtCamRotatorUnGrip => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_UNGRIP);
        public IDOutput VtCamRotator180 => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_180);
        public IDOutput VtCamRotator0 => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ROTATOR_0);
        public IDOutput FilmDetachDw => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_DW);
        public IDOutput FilmDetachGrip => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_GRIP);
        public IDOutput FilmDetachUnGrip => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_UNGRIP);
        public IDOutput FilmDetachSuctionVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FILM_DETACH_SUCTION_VAC_ON);
        public IDOutput VtCamAssemblePnPVacOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ASSEMBLE_PP_VAC_ON);
        public IDOutput VtCamAssemblePnPPurgeOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.VTCAM_ASSEMBLE_PP_PURGE_ON);
        #endregion
        #region BOARD 5
        public IDOutput FrontLoadCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_LOAD_CV_ON);
        public IDOutput FrontDetachCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_DETACH_CV_ON);
        public IDOutput FrontAssembleCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_ASSEMBLE_CV_ON);
        public IDOutput FrontUnloadCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_ON);
        public IDOutput RearLoadCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_LOAD_CV_ON);
        public IDOutput RearDetachCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_DETACH_CV_ON);
        public IDOutput RearAssembleCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_ASSEMBLE_CV_ON);
        public IDOutput RearUnloadCvOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_UNLOAD_CV_ON);
        public IDOutput FrontDetachCvStopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_DETACH_CV_STOPPER_UP);
        public IDOutput FrontDetachCvCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_DETACH_CV_CENTERING_ON);
        public IDOutput FrontAssembleCvStopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_ASSEMBLE_CV_STOPPER_UP);
        public IDOutput FrontAssembleCvCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_ASSEMBLE_CV_CENTERING_ON);
        public IDOutput RearDetachCvStopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_DETACH_CV_STOPPER_UP);
        public IDOutput RearDetachCvCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_DETACH_CV_CENTERING_ON);
        public IDOutput RearAssembleCvStopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_ASSEMBLE_CV_STOPPER_UP);
        public IDOutput RearAssembleCvCenteringOn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_ASSEMBLE_CV_CENTERING_ON);
        #endregion
        #region BOARD 6
        public IDOutput UpstreamFrontLoadEnable => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UPSTREAM_FRONT_LOAD_ENABLE);
        public IDOutput UpstreamRearLoadEnable => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.UPSTREAM_REAR_LOAD_ENABLE);
        public IDOutput DownstreamFrontLoadEnable => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.DOWNSTREAM_FRONT_LOAD_ENABLE);
        public IDOutput DownstreamRearLoadEnable => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.DOWNSTREAM_REAR_LOAD_ENABLE);
        public IDOutput PreFrontCVRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.PRE_FRONT_CONVEYOR_RUN);
        public IDOutput PreRearCVRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.PRE_REAR_CONVEYOR_RUN);
        public IDOutput FrontUnloadCvSlow => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_SLOW);
        public IDOutput RearUnloadCvSlow => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_UNLOAD_CV_SLOW);
        public IDOutput TrayInCVRollerRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_IN_CV_ROLLER_RUN);
        public IDOutput TrayInLiftRollerRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_IN_LIFT_ROLLER_RUN);
        public IDOutput TrayOutCVRollerRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_OUT_CV_ROLLER_RUN);
        public IDOutput TrayOutLiftRollerRun => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.TRAY_OUT_LIFT_ROLLER_RUN);

        public IDOutput FrontUnloadPosUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_POS_UP);
        public IDOutput RearUnloadPosUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.REAR_UNLOAD_POS_UP);
        public IDOutput FrontUnloadCvTurn => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_TURN);
        public IDOutput FrontUnloadCvStopperUp => _dOutputDevice.Outputs.First(i => i.Id == (int)EOutput.FRONT_UNLOAD_CV_STOPPER_UP);
        #endregion
        #region Public Methods
        public void TowerLamp_Clear()
        {
            TowerLampRed.Value = false;
            TowerLampYellow.Value = false;
            TowerLampGreen.Value = false;
        }
        public void SwLamp_Clear()
        {
            FrontStartLamp.Value = false;
            FrontStopLamp.Value = false;
            FrontResetLamp.Value = false;
            RearStartLamp.Value =  false;
            RearStopLamp.Value = false;
        }
        public void TowerLamp_Run()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStartLamp.Value = true;
            RearStartLamp.Value = true;
            TowerLampGreen.Value = true;
        }
        public void TowerLamp_Stop()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStopLamp.Value = true;
            RearStopLamp.Value = true;
            TowerLampYellow.Value = true;
        }
        
        public void TowerLampBuzzer_Alarm()
        {
            TowerLamp_Clear();
            SwLamp_Clear();
            RearStopLamp.Value = true;            
            FrontStopLamp.Value = true;            
            Buzzer1.Value = true;
            TowerLampRed.Value = true;
            
        }
        public void ResetTowerLampBuzzer_Alarm()
        {
            TowerLamp_Clear();
            SwLamp_Clear();
            FrontStopLamp.Value=true;
            RearStopLamp.Value = true;
            FrontResetLamp.Value = false;
            Buzzer1.Value = false;
            TowerLampYellow.Value = true;
        }
        public void TowerLampBuzzer_Warning()
        {
            TowerLamp_Clear();
            SwLamp_Clear();
            RearStopLamp.Value = true;
            FrontStopLamp.Value = true;            
            Buzzer1.Value = true;
            Task.Delay(3000).ContinueWith(t => Buzzer1.Value = false);
            TowerLampRed.Value = true;
        }
        public void TowerLampBuzzer_Full_Empty()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStartLamp.Value = true;
            RearStartLamp.Value = true;
            TowerLampGreen.Value = true;
            TowerLampYellow.Value = true;
        }
        public void TowerLamp_InputStop()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStartLamp.Value = true;
            RearStartLamp.Value = true;
            TowerLampYellow.Value = true;
        }
        public void TowerLamp_DoorOpen()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStopLamp.Value = true;
            RearStopLamp.Value = true;
            TowerLampRed.Value = true;
        }
        public void TowerLamp_Origin()
        {
            SwLamp_Clear();
            TowerLamp_Clear();
            FrontStopLamp.Value = true;
            RearStopLamp.Value = true;
            TowerLampRed.Value = true;
        }
        public void TowerLamp_Output_Stop()
        {
            TowerLamp_Clear();
            TowerLampYellow.Value = true;
        }
        public void TowerLamp_EmergencyStop()
        {
            TowerLamp_Clear();
            Buzzer1.Value = true;
            TowerLampRed.Value = true;
        }
       
        #endregion
    }


}

