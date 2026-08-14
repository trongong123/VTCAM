namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_SpongeRemoveStep_OriginalVer
    {
        Start,
        ConditionCheck,

        PreaAlignCentering,
        PreaAlignCentering_Check,

        SpongeRemoverMoveUp,
        SpongeRemoverUp_Check,

        SafetyConditionCheck,

        SpongeRemoverMoveIn,
        SpongeRemoverMoveIn_Check,

        SpongeRemoverGripOffBeforeDown,
        SpongeRemoverGripOffBeforeDown_Wait,

        SpongeRemoverMoveDown,
        SpongeRemoverDown_Check,
        SpongeRemoverDownDone_Wait,

        SpongeRemoverVacOn,
        SpongeRemoverVacOn_Check,

        SpongeRemoverGripOn,
        SpongeRemoverGripOn_Check,
        SpongeRemoverGripOnDone_Wait,
        SpongeRemoverGripOffToRetry,
        SpongeRemoverGripOffToRetry_Check,

        SpongeRemoverMoveUpAgain,
        SpongeRemoverMoveUpAgain_Check,

        SpongeRemoverMoveOut,
        SpongeRemoverMoveOut_Check,

        SpongeRemoverDownAgain,
        SpongeRemoverDownAgain_Check,

        SpongeHoldVacCheck,

        SpongeRemoverGripOff,
        SpongeRemoverGripOff_Check,

        SpongeRemoverVacOff,
        SpongeRemoverVacOff_Check,

        TrashSuctionDelay,

        End,
    }
}
