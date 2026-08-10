namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_PickStep
    {
        Start,
        CheckAlreadyLoaded,
        MoveUp,
        MoveUpCheck,
        GripperOff,
        GripperOffCheck,
        RotateToPick,
        RotateToPickCheck,
        MoveForward,
        MoveForwardCheck,
        WaitCenteringOpen,
        MoveDown,
        MoveDownCheck,
        DownDelay,
        GripOn,
        GripOnCheck,
        SetLoadDone,
        WaitLoadRequestClear,
        End,
    }
}
