namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOutput2CV
    {
        #region BOARD 1
        FRONT_START_LAMP = 0,
        FRONT_STOP_LAMP = 1,
        FRONT_RESET_LAMP = 2,
        REAR_START_LAMP = 3,
        REAR_STOP_LAMP = 4,
        TOWER_LAMP_RED = 5,
        TOWER_LAMP_YELLOW = 6,
        TOWER_LAMP_GREEN = 7,
        BUZZER_1 = 8,
        MUTING_LAMP = 9,
        AREA_SENSOR_BY_PASS_ON = 10,
        SPARE_OUTPUT_11 = 11,
        TRAY_IN_CV1_STOPPER_UP = 12,
        MANUAL_MODE = 13,
        TRASH_SUCTION_ON = 14,
        SPARE_OUTPUT_15 = 15,
        #endregion

        #region BOARD 2
        TRAY_CENTERING_ON = 16,
        SPONGE_HOLD_VAC_OFF = 17,
        SPARE_OUTPUT_18 = 18,

        UNLOAD_AGV_READY = 19,
        UNLOAD_AGV_READY_INPUT = 20,
        UNLOAD_AGV_TRANFER_DONE = 21,

        LOAD_AGV_READY = 22,
        LOAD_AGV_READY_INPUT = 23,
        LOAD_AGV_TRANFER_DONE = 24,

        SPARE_OUTPUT_25 = 25,
        SPARE_OUTPUT_26 = 26,
        TRAY_OUT_EXT_CV_RUN = 27,
        TRAY_IN_EXT_CV_RUN = 28,
        SPARE_OUTPUT_29 = 29,
        SPARE_OUTPUT_30 = 30,
        SPARE_OUTPUT_31 = 31,
        #endregion

        #region BOARD 3
        TRAY_PICKER_DW = 32,
        TRAY_PICKER_VAC_ON = 33,
        TRAY_PICKER_VAC_OFF = 34,
        VTCAM_SUPPLY_PP_VAC_ON = 35,
        VTCAM_SUPPLY_PP_VAC_OFF = 36,
        VTCAM_PREALIGN_VAC_ON = 37,
        VTCAM_CENTERING_ON = 38,
        VTCAM_CENTERING_OFF = 39,
        VTCAM_PREALIGN_VAC_OFF = 40,
        SPONGE_PICKUP_FW = 41,
        SPONGE_PICKUP_UP = 42,
        SPONGE_PICKUP_DW = 43,
        SPONGE_HOLD_GRIP_ON = 44,
        SPONGE_HOLD_GRIP_OFF = 45,
        SPONGE_HOLD_VAC_ON = 46,
        FILM_DETACH_IONIZER_ON = 47,
        #endregion

        #region BOARD 4
        VTCAM_ROTATOR_FW = 48,
        FILM_DETACH_SUCTION_VAC_OFF = 49,
        VTCAM_ROTATOR_UP = 50,
        VTCAM_ROTATOR_DW = 51,
        VTCAM_ROTATOR_GRIP = 52,
        VTCAM_ROTATOR_UNGRIP = 53,
        VTCAM_ROTATOR_180 = 54,
        VTCAM_ROTATOR_0 = 55,
        FILM_DETACH_DW = 56,
        SPARE_OUTPUT_57 = 57,
        FILM_DETACH_GRIP = 58,
        FILM_DETACH_UNGRIP = 59,
        VTCAM_PREALIGN_FPCB_VAC_ON = 60,
        FILM_DETACH_SUCTION_VAC_ON = 61,
        VTCAM_ASSEMBLE_PP_VAC_ON = 62,
        VTCAM_ASSEMBLE_PP_PURGE_ON = 63,
        #endregion
    
        #region BOARD 5
        FRONT_LOAD_CV_ON = 64,
        FRONT_DETACH_CV_ON = 65,
        FRONT_ASSEMBLE_CV_ON = 66,
        FRONT_UNLOAD_CV_ON = 67,
        REAR_LOAD_CV_ON = 68,
        REAR_DETACH_CV_ON = 69,
        REAR_ASSEMBLE_CV_ON = 70,
        REAR_UNLOAD_CV_ON = 71,
        FRONT_DETACH_CV_STOPPER_UP = 72,
        FRONT_DETACH_CV_CENTERING_ON = 73,
        FRONT_ASSEMBLE_CV_STOPPER_UP = 74,
        FRONT_ASSEMBLE_CV_CENTERING_ON = 75,
        REAR_DETACH_CV_STOPPER_UP = 76,
        REAR_DETACH_CV_CENTERING_ON = 77,
        REAR_ASSEMBLE_CV_STOPPER_UP = 78,
        REAR_ASSEMBLE_CV_CENTERING_ON = 79,
        #endregion

        #region BOARD 6
        UPSTREAM_FRONT_LOAD_ENABLE = 80,
        UPSTREAM_REAR_LOAD_ENABLE = 81,
        DOWNSTREAM_FRONT_LOAD_ENABLE = 82,
        DOWNSTREAM_REAR_LOAD_ENABLE = 83,
        PRE_FRONT_CONVEYOR_RUN = 84,
        PRE_REAR_CONVEYOR_RUN = 85,
        FRONT_UNLOAD_CV_SLOW = 86,
        REAR_UNLOAD_CV_SLOW = 87,
        TRAY_IN_CV_ROLLER_RUN = 88,
        TRAY_IN_LIFT_ROLLER_RUN = 89,
        TRAY_OUT_CV_ROLLER_RUN = 90,
        TRAY_OUT_LIFT_ROLLER_RUN = 91,
        FRONT_UNLOAD_POS_UP = 92,
        SPARE_OUTPUT_93 = 93,
        REAR_UNLOAD_POS_UP = 94,
        SPARE_OUTPUT_95 = 95,
        #endregion
    }

    public enum EOutput1CV
    {
        #region BOARD 1
        FRONT_START_LAMP = 0,
        FRONT_STOP_LAMP = 1,
        FRONT_RESET_LAMP = 2,
        REAR_START_LAMP = 3,
        REAR_STOP_LAMP = 4,
        TOWER_LAMP_RED = 5,
        TOWER_LAMP_YELLOW = 6,
        TOWER_LAMP_GREEN = 7,
        BUZZER_1 = 8,
        MUTING_LAMP = 9,
        AREA_SENSOR_BY_PASS_ON = 10,
        SPARE_OUTPUT_11 = 11,
        TRAY_IN_CV1_STOPPER_UP = 12,
        MANUAL_MODE = 13,
        TRASH_SUCTION_ON = 14,
        SPARE_OUTPUT_15 = 15,
        #endregion

        #region BOARD 2
        TRAY_CENTERING_ON = 16,
        SPONGE_HOLD_VAC_OFF = 17,
        SPARE_OUTPUT_18 = 18,

        UNLOAD_AGV_READY = 19,
        UNLOAD_AGV_READY_INPUT = 20,
        UNLOAD_AGV_TRANFER_DONE = 21,

        LOAD_AGV_READY = 22,
        LOAD_AGV_READY_INPUT = 23,
        LOAD_AGV_TRANFER_DONE = 24,

        SPARE_OUTPUT_25 = 25,
        SPARE_OUTPUT_26 = 26,
        TRAY_OUT_EXT_CV_RUN = 27,
        TRAY_IN_EXT_CV_RUN = 28,
        SPARE_OUTPUT_29 = 29,
        FRONT_UNLOAD_CV_VAC_ON = 30,
        FRONT_UNLOAD_CV_VAC_OFF = 31,
        #endregion

        #region BOARD 3
        TRAY_PICKER_DW = 32,
        TRAY_PICKER_VAC_ON = 33,
        TRAY_PICKER_VAC_OFF = 34,
        VTCAM_SUPPLY_PP_VAC_ON = 35,
        VTCAM_SUPPLY_PP_VAC_OFF = 36,
        VTCAM_PREALIGN_VAC_ON = 37,
        VTCAM_CENTERING_ON = 38,
        VTCAM_CENTERING_OFF = 39,
        VTCAM_PREALIGN_VAC_OFF = 40,
        SPONGE_PICKUP_FW = 41,
        SPONGE_PICKUP_UP = 42,
        SPONGE_PICKUP_DW = 43,
        SPONGE_HOLD_GRIP_ON = 44,
        SPONGE_HOLD_GRIP_OFF = 45,
        SPONGE_HOLD_VAC_ON = 46,
        FILM_DETACH_IONIZER_ON = 47,
        #endregion

        #region BOARD 4
        VTCAM_ROTATOR_FW = 48,
        FILM_DETACH_SUCTION_VAC_OFF = 49,
        VTCAM_ROTATOR_UP = 50,
        VTCAM_ROTATOR_DW = 51,
        VTCAM_ROTATOR_GRIP = 52,
        VTCAM_ROTATOR_UNGRIP = 53,
        VTCAM_ROTATOR_180 = 54,
        VTCAM_ROTATOR_0 = 55,
        FILM_DETACH_DW = 56,
        SPARE_OUTPUT_57 = 57,
        FILM_DETACH_GRIP = 58,
        FILM_DETACH_UNGRIP = 59,
        VTCAM_PREALIGN_FPCB_VAC_ON = 60,
        FILM_DETACH_SUCTION_VAC_ON = 61,
        VTCAM_ASSEMBLE_PP_VAC_ON = 62,
        VTCAM_ASSEMBLE_PP_PURGE_ON = 63,
        #endregion

        #region BOARD 5
        FRONT_LOAD_CV_ON = 64,
        FRONT_DETACH_CV_ON = 65,
        FRONT_ASSEMBLE_CV_ON = 66,
        FRONT_UNLOAD_CV_ON = 67,
        SPARE_OUTPUT_68 = 68,
        SPARE_OUTPUT_69 = 69,
        SPARE_OUTPUT_70 = 70,
        SPARE_OUTPUT_71 = 71,
        FRONT_DETACH_CV_STOPPER_UP = 72,
        FRONT_DETACH_CV_CENTERING_ON = 73,
        FRONT_ASSEMBLE_CV_STOPPER_UP = 74,
        FRONT_ASSEMBLE_CV_CENTERING_ON = 75,
        SPARE_OUTPUT_76 = 76,
        SPARE_OUTPUT_77 = 77,
        SPARE_OUTPUT_78 = 78,
        SPARE_OUTPUT_79 = 79,
        #endregion

        #region BOARD 6
        UPSTREAM_FRONT_LOAD_ENABLE = 80,
        UPSTREAM_REAR_LOAD_ENABLE = 81,
        DOWNSTREAM_FRONT_LOAD_ENABLE = 82,
        DOWNSTREAM_REAR_LOAD_ENABLE = 83,
        PRE_FRONT_CONVEYOR_RUN = 84,
        PRE_REAR_CONVEYOR_RUN = 85,
        FRONT_UNLOAD_CV_SLOW = 86,
        SPARE_OUTPUT_87 = 87,
        TRAY_IN_CV_ROLLER_RUN = 88,
        TRAY_IN_LIFT_ROLLER_RUN = 89,
        TRAY_OUT_CV_ROLLER_RUN = 90,
        TRAY_OUT_LIFT_ROLLER_RUN = 91,
        FRONT_UNLOAD_POS_UP = 92,
        FRONT_UNLOAD_CV_TURN = 93,
        SPARE_OUTPUT_94 = 94,
        FRONT_UNLOAD_CV_STOPPER_UP = 95,
        #endregion
    }

    public enum EOutput
    {
        #region BOARD 1
        FRONT_START_LAMP = 0,
        FRONT_STOP_LAMP = 1,
        FRONT_RESET_LAMP = 2,
        REAR_START_LAMP = 3,
        REAR_STOP_LAMP = 4,
        TOWER_LAMP_RED = 5,
        TOWER_LAMP_YELLOW = 6,
        TOWER_LAMP_GREEN = 7,
        BUZZER_1 = 8,
        MUTING_LAMP = 9,
        AREA_SENSOR_BY_PASS_ON = 10,
        SPARE_OUTPUT_11 = 11,
        TRAY_IN_CV1_STOPPER_UP = 12,
        MANUAL_MODE = 13,
        TRASH_SUCTION_ON = 14,
        SPARE_OUTPUT_15 = 15,
        #endregion

        #region BOARD 2
        TRAY_CENTERING_ON = 16,
        SPONGE_HOLD_VAC_OFF = 17,
        SPARE_OUTPUT_18 = 18,

        UNLOAD_AGV_READY = 19,
        UNLOAD_AGV_READY_INPUT = 20,
        UNLOAD_AGV_TRANFER_DONE = 21,

        LOAD_AGV_READY = 22,
        LOAD_AGV_READY_INPUT = 23,
        LOAD_AGV_TRANFER_DONE = 24,

        SPARE_OUTPUT_25 = 25,
        SPARE_OUTPUT_26 = 26,
        TRAY_OUT_EXT_CV_RUN = 27,
        TRAY_IN_EXT_CV_RUN = 28,
        SPARE_OUTPUT_29 = 29,
        SPARE_OUTPUT_30 = 30,
        SPARE_OUTPUT_31 = 31,

        #region One Conveyor
        FRONT_UNLOAD_CV_VAC_ON = 30,
        FRONT_UNLOAD_CV_VAC_OFF = 31,
        #endregion
        #endregion

        #region BOARD 3
        TRAY_PICKER_DW = 32,
        TRAY_PICKER_VAC_ON = 33,
        TRAY_PICKER_VAC_OFF = 34,
        VTCAM_SUPPLY_PP_VAC_ON = 35,
        VTCAM_SUPPLY_PP_VAC_OFF = 36,
        VTCAM_PREALIGN_VAC_ON = 37,
        VTCAM_CENTERING_ON = 38,
        VTCAM_CENTERING_OFF = 39,
        VTCAM_PREALIGN_VAC_OFF = 40,
        SPONGE_PICKUP_FW = 41,
        SPONGE_PICKUP_UP = 42,
        SPONGE_PICKUP_DW = 43,
        SPONGE_HOLD_GRIP_ON = 44,
        SPONGE_HOLD_GRIP_OFF = 45,
        SPONGE_HOLD_VAC_ON = 46,
        FILM_DETACH_IONIZER_ON = 47,
        #endregion

        #region BOARD 4
        VTCAM_ROTATOR_FW = 48,
        FILM_DETACH_SUCTION_VAC_OFF = 49,
        VTCAM_ROTATOR_UP = 50,
        VTCAM_ROTATOR_DW = 51,
        VTCAM_ROTATOR_GRIP = 52,
        VTCAM_ROTATOR_UNGRIP = 53,
        VTCAM_ROTATOR_180 = 54,
        VTCAM_ROTATOR_0 = 55,
        FILM_DETACH_DW = 56,
        SPARE_OUTPUT_57 = 57,
        FILM_DETACH_GRIP = 58,
        FILM_DETACH_UNGRIP = 59,
        VTCAM_PREALIGN_FPCB_VAC_ON = 60,
        FILM_DETACH_SUCTION_VAC_ON = 61,
        VTCAM_ASSEMBLE_PP_VAC_ON = 62,
        VTCAM_ASSEMBLE_PP_PURGE_ON = 63,
        #endregion

        #region BOARD 5
        FRONT_LOAD_CV_ON = 64,
        FRONT_DETACH_CV_ON = 65,
        FRONT_ASSEMBLE_CV_ON = 66,
        FRONT_UNLOAD_CV_ON = 67,
        REAR_LOAD_CV_ON = 68,
        REAR_DETACH_CV_ON = 69,
        REAR_ASSEMBLE_CV_ON = 70,
        REAR_UNLOAD_CV_ON = 71,
        FRONT_DETACH_CV_STOPPER_UP = 72,
        FRONT_DETACH_CV_CENTERING_ON = 73,
        FRONT_ASSEMBLE_CV_STOPPER_UP = 74,
        FRONT_ASSEMBLE_CV_CENTERING_ON = 75,
        REAR_DETACH_CV_STOPPER_UP = 76,
        REAR_DETACH_CV_CENTERING_ON = 77,
        REAR_ASSEMBLE_CV_STOPPER_UP = 78,
        REAR_ASSEMBLE_CV_CENTERING_ON = 79,

        #region One Conveyor
        SPARE_OUTPUT_68 = 68,
        SPARE_OUTPUT_69 = 69,
        SPARE_OUTPUT_70 = 70,
        SPARE_OUTPUT_71 = 71,
        SPARE_OUTPUT_76 = 76,
        SPARE_OUTPUT_77 = 77,
        SPARE_OUTPUT_78 = 78,
        SPARE_OUTPUT_79 = 79,
        #endregion
        #endregion

        #region BOARD 6
        UPSTREAM_FRONT_LOAD_ENABLE = 80,
        UPSTREAM_REAR_LOAD_ENABLE = 81,
        DOWNSTREAM_FRONT_LOAD_ENABLE = 82,
        DOWNSTREAM_REAR_LOAD_ENABLE = 83,
        PRE_FRONT_CONVEYOR_RUN = 84,
        PRE_REAR_CONVEYOR_RUN = 85,
        FRONT_UNLOAD_CV_SLOW = 86,
        REAR_UNLOAD_CV_SLOW = 87,
        TRAY_IN_CV_ROLLER_RUN = 88,
        TRAY_IN_LIFT_ROLLER_RUN = 89,
        TRAY_OUT_CV_ROLLER_RUN = 90,
        TRAY_OUT_LIFT_ROLLER_RUN = 91,
        FRONT_UNLOAD_POS_UP = 92,
        SPARE_OUTPUT_93 = 93,
        REAR_UNLOAD_POS_UP = 94,
        SPARE_OUTPUT_95 = 95,

        #region One Conveyor
        FRONT_UNLOAD_CV_TURN = 93,
        SPARE_OUTPUT_94 = 94,
        FRONT_UNLOAD_CV_STOPPER_UP = 95,
        #endregion
        #endregion
    }
}
