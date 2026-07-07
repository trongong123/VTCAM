namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayInCV_UnloadStep
    {
        Start,

        TrayInElevator_TrayRequest_Wait,
        Stopper_MoveDown,
        Stopper_Down_Check,

        CV_Run,
        Tray_Unload_Done_Check,

        CV_Stop,

        Stopper_MoveUp,
        Stopper_Up_Check,

        End,
    }
}
 