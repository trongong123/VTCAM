namespace FrontCameraAssembleEquipment.Defines
{
    public enum EFlipperCam_PickStep_OriginalVer
    {
        Start,

        FlipperConditionCheck,

        MoveFlipperUp,
        MoveFlipperUp_Check,

        FlipperUngripAndRotateToPick,
        FlipperUngripAndRotateToPick_Check,

        CheckPrealignCamOutRequest,
        CheckTrayHeadOutOfPlaceArea,

        MoveFlipMoverToPickPos,
        MoveFlipMoverToPickPos_Check,

        WaitPickRequestSignal,

        MovePickupDown,
        MovePickupDown_Check,
        MovePickupDownDone_Wait,

        CamGripperOn,
        CamGripperOn_Check,
        CamGripperOnDone_Wait,

        MovePickupUp,
        MovePickupUp_Check,

        End
    }
}
