namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayOutputElevator_LoadStep
    {
        Start, 
        Check_Status_Tray_Exis,
        Move_To_Ready_Place,
        Move_To_Ready_Place_Check,
        Set_Flag_TrayOutElevatorReadyPlace,
        Wait_TrayOutElevatorPlaceDone,
        End,
    }
}
