namespace FrontCameraAssembleEquipment.Defines
{
    public enum EPrealign_SpongeRemoveStep
    {
        Start,
        CheckCamera,
        CenteringOpen,
        CenteringOpenCheck,
        RequestRotatorLoad,
        WaitRotatorLoadDone,
        ClearRotatorLoadRequest,
        RequestTapeRemove,
        WaitTapeRemoveDone,
        RequestRotatorRemoveSponge,
        WaitRotatorRemoveSpongeDone,
        RequestTapeRelease,
        WaitTapeSafeDone,
        ClearRemoveRequests,
        End,
    }
}
