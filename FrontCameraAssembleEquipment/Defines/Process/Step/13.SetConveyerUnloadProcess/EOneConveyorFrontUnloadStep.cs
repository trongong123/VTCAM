namespace FrontCameraAssembleEquipment.Defines
{
    public enum EOneConveyorFrontUnloadStep
    {
        Start,
        StopperUp,
        StopperUpCheck,

        //WaitEndSensor,
        //StopConveyor,

        MoverUpAndStopperDown,
        MoverUpAndStopperDownCheck,

        VacuumOn,
        VacuumOnCheck,

        Turn,
        TurnCheck,

        VacuumOff,

        MoverDown,
        MoverDownCheck,
        WaitDownstreamLoadEnableBeforeStopperDown,
        StopperDownAfterDownstreamEnable,
        StopperDownAfterDownstreamEnableCheck,

        VacuumOffCheck,

        ConveyorRun,
        WaitEndSensorOff,
        WaitAfterEndSensorOff,
        StopperUpAfterUnload,
        StopperUpAfterUnloadCheck,

        TurnReturn,
        TurnReturnCheck,

        RecoverVacuumWhileMoverUp,
        RecoverVacuumWhileMoverUpCheck,
        StopperDownBeforeTurn,
        StopperDownBeforeTurnCheck,
        StopperDownForReady,
        StopperDownForReadyCheck,

        MoverDownForReady,
        MoverDownForReadyCheck,
        CheckVacuumWhileMoverUp,
        CheckVacuumWhileMoverUpCheck,
        TurnReturnBeforeReady,
        TurnReturnBeforeReadyCheck,
        VacuumOffBeforeMoverDown,

        End
    }
}
