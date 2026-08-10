namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_UnloadStep
    {
        Start,
        CheckPosition,
        MoveUp,
        MoveUpCheck,
        MoveBackwardAndTurn,
        MoveBackwardAndTurnCheck,
        SetLeftPrealignDone,
        WaitUnloadRequestClear,
        SpongeExistCheck,
        CameraExistCheck,
        RequestCamUnload,
        WaitCamAssembleVacuumOn,
        GripperOff,
        GripperOffCheck,
        SetGripOffDone,
        WaitCamUnloadDone,
        ClearHandshake,
        End,
    }
}
