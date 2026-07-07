namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInPutElevator_CamUnloadStep
    {
        Start,
        Tray_WorkCondition_Check,
        Set_FlagTrayInElevatorUnloadCamRequest,
        Wait_TrayHeadPickCameraAndResetStatusCamera,
        SetFlagTrayHeadPickCamera_Received,
        Wait_TrayHeadPickCamera_Received,
        End,
    }
}
