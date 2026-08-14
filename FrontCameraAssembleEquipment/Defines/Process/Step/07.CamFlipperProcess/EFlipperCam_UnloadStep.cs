namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_UnloadStep
    {
        Start,

        FlipperConditionCheck,

        MoveFlipperUp,
        MoveFlipperUp_Check,

        MoveFlipperToUnloadAndPosRotate,
        MoveFlipperToUnloadPosAndRotate_Check,

        SpongeExisCheck,

        CamExistCheck,

        Wait_RemoveSpongeDoneClear,

        RequestCamUnload,
        CheckCamUnload,
        CamGripperOff,
        CamGripperOff_Check,
        CheckCamUnloadComplete,
        End,
    }
}
