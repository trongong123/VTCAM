namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOneConveyorFrontUnloadStep
    {
        Start,
        WaitEndSensor,
        StopConveyor,

        StopperUp,
        StopperUpCheck,

        VacuumOn,
        VacuumOnCheck,

        MoverUpAndStopperDown,
        MoverUpAndStopperDownCheck,

        Turn,
        TurnCheck,

        MoverDown,
        MoverDownCheck,

        VacuumOff,
        VacuumOffCheck,

        ConveyorRun,
        WaitEndSensorOff,

        TurnReturn,
        TurnReturnCheck,

        RecoverVacuumWhileMoverUp,
        RecoverVacuumWhileMoverUpCheck,
        StopperDownBeforeTurn,
        StopperDownBeforeTurnCheck,
        StopperDownForReady,
        StopperDownForReadyCheck,

        End
    }
}
