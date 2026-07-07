namespace FrontCameraAssembleEquipment.Defines.Process.Step._05.TransferHeadProcess
{
    public enum ETrayHead_CamPickStep
    {
        Start,
        Index_Initiation,
        ZAxis_Move_ReadyPickPosition,
        ZAxis_Move_ReadyPickPosition_Check,
        TrayInElevatorCamUnload_Request_Check,
        XYAxis_Move_WaitPickPosition,
        XYAxis_Move_WaitPickPosition_Check,
        Wait_TrayInElevatorCamUnload_Request,
        XYAxis_Move_PickPosition,
        XYAxis_Move_PickPosition_Check,
        ZAxis_Move_PickPosition,
        ZAxis_Move_PickPosition_Check,
        VacuumOn,
        VacuumOnCheck,
        Wait_Flag_TrayInElevatorCamUnloadDone_Received,
        SetFlag_TrayInElevatorCamUnload_Done,
        ZAxis_MoveBack_ReadyPickPosition,
        ZAxis_MoveBack_ReadyPickPosition_Check,
        End,
    }
}
