namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVOut_AutoRunStep
    {
        Start,

        CheckUpSetExistToDownCylinder,

        StopperUnloadDown,
        StopperUnloadDown_Check,
        CV_Run,
        CV_Stop,
        CheckConditionToRun,

        End
    }
}
