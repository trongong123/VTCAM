namespace FrontCameraAssembleEquipment.Defines
{ 
    public enum EPrealignOutput
    {
        CAMERA_DETECTED,
        CENTERING_OPEN_DONE,
        CENTERING_CLOSE_DONE,
        VACUUM_ON_DONE,
        VACUUM_OFF_DONE,
        FPCB_VACUUM_ON_DONE,
        FPCB_VACUUM_OFF_DONE,
        MATERIAL_CLEAR_DONE,
        READY_DONE,

        TRAYHEAD_CAM_IN_REQUEST,

        TAPE_REMOVE_REQUEST,
        TAPE_RELEASE_REQUEST,

        ROTATOR_LOAD_REQUEST,
        ROTATOR_REMOVE_SPONGE_REQUEST,
        ROTATOR_UNLOAD_REQUEST,
    }
}
