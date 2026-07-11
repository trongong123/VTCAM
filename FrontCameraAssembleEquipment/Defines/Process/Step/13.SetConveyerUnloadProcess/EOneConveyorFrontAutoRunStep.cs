namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOneConveyorFrontAutoRunStep
    {
        Start,
        CheckPhysicalState,

        MoverDownForReady,
        MoverDownForReadyCheck,
        RunExistingUnitToEnd,
        RunExistingUnitToEndCheck,
        End
    }
}
