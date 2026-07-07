namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHead_TrayPlaceStep
    {
        Start,
        Cyl_TrayPicker_MoveUp_1st,
        Cyl_TrayPicker_MoveUp_1St_Wait,
        ZAxis_MoveWaitPosition,
        ZAxis_MoveWaitPosition_Check,

        Wait_TrayOutElevator_ReadyPlace,

        XYAxis_MoveReadyPlacePosition,
        XYAxis_MoveReadyPlacePosition_Check,

        TrayPicker_Vacuum_Check,

        Cyl_TrayPicker_MoveDown,
        Cyl_TrayPicker_MoveDown_Wait,
        Vac_TrayPicker_Off,
        Vac_TrayPicker_Off_Wait,
        Cyl_TrayPicker_MoveUp,
        Cyl_TrayPicker_MoveUp_Wait,
        Set_Flag_TrayOutElevatorPlaceDone,
        Wait_TrayOutElevatorPlaceDoneReceived,
        End,
    }
}
