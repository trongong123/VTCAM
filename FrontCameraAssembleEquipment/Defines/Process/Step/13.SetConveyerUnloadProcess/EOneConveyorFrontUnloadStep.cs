namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOneConveyorFrontUnloadStep
    {
        Start,
        WaitEndSensor,
        StopConveyor,

        StopperUp,
        StopperUpCheck,

        MoverUpAndStopperDown,
        MoverUpAndStopperDownCheck,

        VacuumOn,
        VacuumOnCheck,

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

        ReturnWithoutUnit,
        ReturnWithoutUnitCheck,

        MoverDownWithoutUnit,
        MoverDownWithoutUnitCheck,

        StopperDownForReady,
        StopperDownForReadyCheck,

        End
    }
}
