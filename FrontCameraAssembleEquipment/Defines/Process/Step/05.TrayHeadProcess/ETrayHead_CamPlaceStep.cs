namespace FrontCameraAssembleEquipment.Defines.Process.Step._05.TransferHeadProcess
{
    public enum ETrayHead_CamPlaceStep
    {
        Start,
        Check_Status_Tray_Head_Is_Placing,
        ZAxis_Move_Ready_Position,
        ZAxis_Move_Ready_Position_Check_1st,
        XYAxis_Move_ReadyPlacePosition_Check_Status,
        Barcode_Use_Check,
        No_Camera_Check,
        XYAxis_Move_ScanPosition,
        XYAxis_Move_ScanPosition_Check,      
        Scan_Barcode_Request,
        Wait_Result_Barcode,
        Check_Reseult_Barcode,
        
        ZAxis_Move_ReadyPlacePosition,
        ZAxis_Move_ReadyPlacePosition_Check,
        XYAxis_Move_ReadyPlacePosition,
        XYAxis_Move_ReadyPlacePosition_Check,
        Wait_FlagSpongeDetachCamInRequest,
        XYAxis_Move_PlacePosition,
        XYAxis_Move_PlacePosition_Check,
        Wait_FlagSpongeDetachCamInRequestBeforePlace,
        ZAxis_Move_PlacePosition,
        ZAxis_Move_PlacePosition_Check,

        PreCentering_Option_Check,

        CamPreCenteringOn,
        CamPreCenteringOn_Check,

        VacPreAlignOn,

        CamPreCenteringOff,
        CamPreCenteringOff_Check,

        VacPreAlign_Check,

        VacuumOff,
        VacuumOff_Wait,
        ZAxis_MoveBack_ReadyPlacePosition,
        ZAxis_MoveBack_ReadyPlacePosition_Check,
        XYAxis_MoveBack_ReadyPlacePosition,
        XYAxis_MoveBack_ReadyPlacePosition_Check,
        Set_FlagSpongeDetachCamInDone,

        XYAxis_Move_ReturnPosition,
        XYAxis_Move_ReturnPosition_Wait,

        ZAxis_Move_Return_Position,
        ZAxis_Move_Return_Position_Wait,

        Vacuum_Return_Off,

        Set_Tray_Status,

        ZAxis_Move_Return_ReadyPisition,
        ZAxis_Move_Return_ReadyPisition_Wait,

        End,
    }
}
