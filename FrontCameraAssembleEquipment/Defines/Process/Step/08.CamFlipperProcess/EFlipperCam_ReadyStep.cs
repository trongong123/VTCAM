namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_ReadyStep
    {
        Start,
        ClearHandshake,
        WaitPrealignAndTapeReady,
        CheckInterruptedState,
        NormalizeLoadedDown,
        NormalizeLoadedDownCheck,
        GripperOffIfEmpty,
        GripperOffIfEmptyCheck,
        MoveUp,
        MoveUpCheck,
        MoveBackwardAndTurn,
        MoveBackwardAndTurnCheck,
        SyncCameraStatus,
        End,
    }
}
