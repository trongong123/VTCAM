namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_ReadyStep
    {
        Start,
        InternalInOutSignal_Reset,
        SpongeDetachMoveUp,
        SpongeDetachMoveUp_Check,
        SpongeDetachMoveOut,
        SpongeDetachMoveOut_Check,
        SpongeDetachUngrip,
        SpongeDetachUngrip_Check,
        CamCenteringOff,
        CamCenteringOff_Check,
        SpongeDetachMoveUpAgain,
        SpongeDetachMoveUpAgain_Check,
        End,
    }
}
