namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_OriginStep
    {
        Start,
        WaitTrayHeadHomeZAxisDoneFlag,
        Set_TrayHeadHomeZAxisDoneReceived,
        WaitCamAssembleHeadHomeZAxisDoneFlag,
        CamFlipperCurrentStateCheck,
        CamFlipperMoverMoveUp,
        CamFlipperMoverMoveUp_Check,
        CamFlipperMoverMoveReady,
        CamFlipperMoverMoveReady_Check,
        CamFLipperRotateReady,
        CamFLipperRotateReady_Check,
        CamFlipperMoverMoveDown,
        CamFlipperMoverMoveDown_Check,
        End,
    }
}
