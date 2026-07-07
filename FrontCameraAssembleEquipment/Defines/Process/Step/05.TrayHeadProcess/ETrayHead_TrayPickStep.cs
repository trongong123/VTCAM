namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHead_TrayPickStep
    {
        Start,
        Wait_TrayInElevatorUnloadTrayRequest,
        ZAxis_MoveWait,
        ZAxis_MoveWait_Check,
        XYAxis_MovePickPosition,
        XYAxis_MovePickPosition_Check,
        TrayPick_Warning_Check_Metarial_In_Tray_Empty,
        Cyl_TrayPicker_Down,
        Cyl_TrayPicker_Down_Check,
        Cyl_TrayPicker_VacOn,
        Cyl_TrayPicker_VacOn_Check,
        SetFlag_TrayPickerVacOnDone,
        Wait_TrayInElevatorUnAlignDone,
        Cyl_TrayPicker_Up,
        Cyl_TrayPicker_Up_Wait,
        SetFlag_TrayInElevatorUnloadTrayDone,
        Wait_TrayInElevatorUnloadTrayReveived,
        End,
    }
}
