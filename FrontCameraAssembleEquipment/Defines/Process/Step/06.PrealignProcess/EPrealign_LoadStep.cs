namespace FrontCameraAssembleEquipment.Defines
{
    public enum EPrealign_LoadStep
    {
        Start,
        WaitRotatorSafe,
        CheckPrealignEmpty,
        CenteringOpen,
        CenteringOpenCheck,
        VacuumOn,
        RequestCameraFromTrayHead,
        WaitCameraFromTrayHead,
        VacuumCheck,
        WaitTrayHeadSafe,
        ClearRequest,
        End,
    }
}
