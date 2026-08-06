namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_ReadyStep
    {
        Start, 

        InternalInOutSignal_Reset,

        WaitSpongeRemoveOut,
        WaitSpongeRemoveOut_Check,
        Check_Status_Gripper,

        GripperOff,
        GripperOff_Check,

        FlipperUp,
        FlipperUp_Check,

        FlipperMoveToReady,
        FlipperMoveToReady_Check,

        FlipperTurn,
        FlipperTurn_Check,

        DelayToCheckCamExist,
        CheckCamExist,
        End,
    }
}
