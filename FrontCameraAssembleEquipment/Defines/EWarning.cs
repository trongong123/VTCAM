using System.ComponentModel;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EWarning
    {
        // Global warning start with index 0

        [Description("Door Open")]
        DoorOpen = 10000,
        //DoorNotSafetyLock = 10001,

        [Description("Light Curtain Left Detected")]
        LightCurtainDetected = 10001,

        [Description("Material Data Not Matching")]
        MaterialDataNotMatching = 10002,

        [Description("AGV Network Communication Fail")]
        AGV_NetworkCommunication_Fail = 11003, // New Warning
        [Description("AGV IO Communication Fail")]
        AGV_PIOCommunication_Fail = 11004, // New Warning

        // Tray in start with index 10000
        [Description("[Tray In Buffer] Stopper Up Timeout")]
        TrayINBuffer_StopperUp_Timeout = 10003,
        [Description("[Tray In Buffer] Stopper Down Timeout")]
        TrayINBuffer_StopperDown_Timeout = 10004,
        [Description("[Tray In Buffer] Detect Start Timeout")]
        TrayINBuffer_Detect_Start_Timeout = 10005,
        [Description("[Tray In Buffer] Detect End Timeout")]
        TrayINBuffer_Detect_End_Timeout = 10006,


        // Tray in Elevator start with index 10100
        [Description("[Tray In Lift] Input Position Move Timeout")]
        TrayINLift_Input_Position_Move_Timeout = 10007,
        [Description("[Tray In Lift] Up Height Position Move Timeout")]
        TrayINLift_Up_Height_Position_Move_Timeout = 10008,
        [Description("[Tray In Lift] Up Height Position Move To Z Up Limit")]
        TrayINLift_Up_Height_Position_Move_To_ZUpLimit = 10009,
        [Description("[Tray In Lift] Sensor Check Height Not Detect")]
        TrayINLift_SenSor_Check_Height_Not_Detect = 10010,
        [Description("[Tray In Lift] Move To Tray Input Position Timeout")]
        TrayINLift_Move_To_TrayInputPosition_Timeout = 10011,
        [Description("[Tray In Lift] Detect End Timeout")]
        TrayINLift_Detect_End_Timeout = 10012,


        [Description("[Tray In Lift] Align Timeout")]
        TrayINLift_Align_Timeout = 11015,
        [Description("[Tray In Lift] UnAlign Timeout")]
        TrayINLift_UnAlign_Timeout = 10013,
        [Description("[Tray In Lift] Z Axis Home Search Fail")]
        TrayINLift_ZAxis_Home_Search_Fail = 10014,
        [Description("[Tray In Lift] Check Material In Tray Empty Timeout")]
        Check_Material_In_Tray_Empty_TimeOut = 11014, //New Warning

        // Tray out start with index 10200
        [Description("[Tray Out Lift] Z Axis Home Search Fail")]
        TrayOUTLift_ZAxis_Home_Search_Timeout = 10015,
        [Description("[Tray Out Lift] Input Position Move Timeout")]
        TrayOUTLift_Input_Position_Move_Timeout = 10016,
        [Description("[Tray Out Lift] Up Height Position Move Timeout")]
        TrayOUTLift_Up_Height_Position_Move_Timeout = 10017,
        [Description("[Tray Out Lift] Tray Search Move Up 2nd Fail")]
        TrayOUTLift_TraySearch_MoveUp2nd_Fail = 10018,
        [Description("[Tray Out Lift] Tray Search Fail")]
        TrayOUTLift_TraySearchFail = 10019,
        [Description("[Tray Out Lift] Out Sensor End Detect")]
        TrayOUTLift_Out_SenSor_EndDetect = 10020,
        [Description("[Tray Out Lift] Stop Fail")]
        TrayOUTLift_StopFail = 11020,


        // Tray out CV start with index 10300
        [Description("[Tray Out Buffer] Detect Start Timeout")]
        TrayOUTBuffer_Detect_Start_Timeout = 10021,
        [Description("[Tray Out Buffer] Detect End Timeout")]
        TrayOUTBuffer_Detect_End_Timeout = 10022,

        //Tray Head start with index 10400
        [Description("[Tray CAM Loader] Tray Picker Up Fail")]
        TrayCAMLoader_TrayPicker_Up_Fail = 10023,
        [Description("[Tray CAM Loader] Tray Picker Down Fail")]
        TrayCAMLoader_TrayPicker_Down_Fail = 10024,
        [Description("[Tray CAM Loader] Tray Picker Vacuum On Fail")]
        TrayCAMLoader_TrayPicker_VacuumOn_Fail = 10025,
        [Description("[Tray CAM Loader] Tray Picker Vacuum Off Fail")]
        TrayCAMLoader_TrayPicker_VacuumOff_Fail = 10026,
        [Description("[Tray CAM Loader] VtCam Supply Vacuum On Fail")]
        TrayCAMLoader_VtCamSupplyPnP_VacOn_Fail = 10027,
        [Description("[Tray CAM Loader] VtCam Supply Vacuum Off Fail")]
        TrayCAMLoader_VtCamSupplyPnP_VacOff_Fail = 10028,
        [Description("[Tray CAM Loader] Scan Barcode Fail")]
        TrayCAMLoader_ScanBarCode_Fail = 10029,
        [Description("[Tray CAM Loader] Move ZScan Position Fail")]
        TrayCAMLoader_MoveZScanPos_Fail = 11030, //New Warning
        [Description("[Tray CAM Loader] Barcode Not Matched")]
        TrayCAMLoader_Barcode_NotMatched = 11031, //New Warning
        [Description("[Tray CAM Loader] Camera Pick Fail")]
        TrayCAMLoader_Camera_PickFail = 11032, //New Warning
        [Description("[Tray CAM Loader] Stop Fail")]
        TrayCAMLoader_StopFail = 11033,


        // Sponge Detach start with index 10500
        [Description("[Cam Sponge Detach] Camera Not Exist")]
        CamSpongeDetach_CameraNotExist = 10030,
        [Description("[Cam Sponge Detach] Camera Exist")]
        CamSpongeDetach_CameraExist = 10031,
        [Description("[Cam Sponge Detach] PreAlign Vacuum On Fail")]
        CamSpongeDetach_PrealignVacOn_Fail = 10032,
        [Description("[Cam Sponge Detach] PreAlign Vacuum Check Fail")]
        CamSpongeDetach_PreAlignVacCheck_Fail = 10033,
        [Description("[Cam Sponge Detach] PreAlign Vacuum Off Fail")]
        CamSpongeDetach_PreAlignVacOff_Fail = 10034,
        [Description("[Cam Sponge Detach] Centering On Fail")]
        CamSpongeDetach_CenteringOn_Fail = 10035,
        [Description("[Cam Sponge Detach] Centering Off Fail")]
        CamSpongeDetach_CenteringOff_Fail = 10036,
        [Description("[Cam Sponge Detach] Move Up Fail")]
        CamSpongeDetach_MoveUp_Fail = 10037,
        [Description("[Cam Sponge Detach] Move Down Fail")]
        CamSpongeDetach_MoveDown_Fail = 10038,
        [Description("[Cam Sponge Detach] Move Forward Fail")]
        CamSpongeDetach_MoveFw_Fail = 10039,
        [Description("[Cam Sponge Detach] Move Backward Fail")]
        CamSpongeDetach_MoveBw_Fail = 10040,
        [Description("[Cam Sponge Detach] Grip On Fail")]
        CamSpongeDetach_GripOn_Fail = 10041,
        [Description("[Cam Sponge Detach] Grip Off Fail")]
        CamSpongeDetach_GripOff_Fail = 10042,
        [Description("[Cam Sponge Detach] Sponge Vacuum On Fail")]
        CamSpongeDetach_SpongeVacOn_Fail = 10043,
        [Description("[Cam Sponge Detach] Sponge Vacuum Off Fail")]
        CamSpongeDetach_SpongeVacOff_Fail = 10044,
        [Description("[Cam Sponge Detach] Sponge Remove Fail")]
        CamSpongeDetach_SpongeRemoveFail = 11044,


        // Cam Flipper start with index 10600
        [Description("[Cam Rotator] Move Unload Position Fail")]
        CAMRotator_MoveUnloadPos_Fail = 10045,
        [Description("[Cam Rotator] Move Unload Position and Rotate Fail")]
        CAMRotator_MoveUnloadPosAndRotate_Fail = 10046,
        [Description("[Cam Rotator] Move and Flip Ready Fail")]
        CAMRotator_MoveAndFlipReady_Fail = 10047,
        [Description("[Cam Rotator] Move Pick Fail")]
        CAMRotator_MovePick_Fail = 10048,
        [Description("[Cam Rotator] Move Down Fail")]
        CAMRotator_MoveDown_Fail = 10049,
        [Description("[Cam Rotator] Move Up Fail")]
        CAMRotator_MoveUp_Fail = 10050,
        [Description("[Cam Rotator] Grip On Fail")]
        CAMRotator_GripOn_Fail = 10051,
        [Description("[Cam Rotator] Grip Off Fail")]
        CAMRotator_GripOff_Fail = 10052,
        [Description("[Cam Rotator] Grip Off and Rotate Ready Fail")]
        CAMRotator_GripOffAndRotateReady_Fail = 10053,
        [Description("[Cam Rotator] Rotate Ready Fail")]
        CAMRotator_RotateReady_Fail = 10054,
        [Description("[Cam Rotator] Rotate Fail")]
        CAMRotator_Rotate_Fail = 10055,
        [Description("[Cam Rotator] Camera Not Exist")]
        CAMRotator_Camera_Not_Exist = 10056,
        [Description("[Cam Rotator] Sponge Exist")]
        CAMRotator_Sponge_Exist = 11057,/// New Warning


        // FilmDetach Head start with index 10700
        [Description("[Vinyl Detach] Move Up Fail")]
        VinylDetach_MoveUp_Fail = 10057,
        [Description("[Vinyl Detach] Move Down Fail")]
        VinylDetach_MoveDown_Fail = 10058,
        [Description("[Vinyl Detach] Vac On Fail")]
        VinylDetach_VacOn_Fail = 10059,
        [Description("[Vinyl Detach] Vac Off Fail")]
        VinylDetach_VacOff_Fail = 10060,
        [Description("[Vinyl Detach] Grip On Fail")]
        VinylDetach_GripOn_Fail = 10061,
        [Description("[Vinyl Detach] Grip Off Fail")]
        VinylDetach_GripOff_Fail = 10062,
        [Description("[Vinyl Detach] Set Reverse Detect")]
        VinylDetach_SetDetectReverse_Fail = 11062, // New Warning
        [Description("[Vinyl Detach] Cylinder Suction Up Fail")]
        VinylDetach_CylinderSuctionUp_Fail = 11063,
        [Description("[Vinyl Detach] Cylinder Suction Down Fail")]
        VinylDetach_CylinderSuctionDown_Fail = 11064,
        [Description("[Vinyl Detach] Front Vinyl Detach Fail")]
        VinylDetach_FrontVinylDetach_Fail = 11065,
        [Description("[Vinyl Detach] Rear Vinyl Detach Fail")]
        VinylDetach_RearVinylDetach_Fail = 11066,

        // [Cam Assemble] Head start with index 10800
        [Description("[Cam Assemble] Head Vac On Fail")]
        CAMAssemble_PickUpVacOn_Fail = 10063,
        [Description("[Cam Assemble] Head Vac Off Fail")]
        CAMAssemble_PickUpVacOff_Fail = 10064,
        [Description("[Cam Assemble] Head Cylinder Up Fail")]
        CAMAssemble_CylinderUp_Fail = 10065,
        [Description("[Cam Assemble] Head Cylinder Down Fail")]
        CAMAssemble_CylinderDown_Fail = 10066,

        // Set In CV start with index 11000
        [Description("[Front IN CV] Set Load Timeout")]
        FrontINCV_SetLoad_Timeout = 10067,


        // Detach Film CV start with index 11100,
        [Description("[Front Detach CV] Stopper Up Fail")]
        FrontDetachCV_StopperUp_Fail = 10068,
        [Description("[Front Detach CV] Stopper Down Fail")]
        FrontDetachCV_StopperDown_Fail = 10069,
        [Description("[Front Detach CV] Align On Fail")]
        FrontDetachCV_AlignOn_Fail = 10070,
        [Description("[Front Detach CV] Align Off Fail")]
        FrontDetachCV_AlignOff_Fail = 10071,
        [Description("[Front Detach CV] Set In Stopper Warning")]
        FrontDetachCV_SetInStopperWarning = 10072,
        [Description("[Front Detach CV] Move Timeout")]
        FrontDetachCV_CVMoveTimeOut = 10073,
        [Description("[Front Detach CV] Two Set Exist")]
        FrontDetachCV_TwoOfSetExist = 10074,
        [Description("[Front Detach CV] Set Load Timeout")]
        FrontDetachCV_SetLoad_Timeout = 10075,
        [Description("[Front Detach CV] Set Unload Timeout")]
        FrontDetachCV_SetUnload_Timeout = 10076,

        [Description("[Rear Detach CV] Stopper Up Fail")]
        RearDetachCV_StopperUp_Fail = 11068,
        [Description("[Rear Detach CV] Stopper Down Fail")]
        RearDetachCV_StopperDown_Fail = 11069,
        [Description("[Rear Detach CV] Align On Fail")]
        RearDetachCV_AlignOn_Fail = 11070,
        [Description("[Rear Detach CV] Align Off Fail")]
        RearDetachCV_AlignOff_Fail = 11071,
        [Description("[Rear Detach CV] Set In Stopper Warning")]
        RearDetachCV_SetInStopperWarning = 11072,
        [Description("[Rear Detach CV] Move Timeout")]
        RearDetachCV_CVMoveTimeOut = 11073,
        [Description("[Rear Detach CV] Two Set Exist")]
        RearDetachCV_TwoOfSetExist = 11074,
        [Description("[Rear Detach CV] Set Load Timeout")]
        RearDetachCV_SetLoad_Timeout = 11075,
        [Description("[Rear Detach CV] Set Unload Timeout")]
        RearDetachCV_SetUnload_Timeout = 11076,

        // Assemble CV start with index 11200,
        [Description("[Front Assemble CV] Stopper Up Fail")]
        FrontAssembleCV_StopperUp_Fail = 10077,
        [Description("[Front Assemble CV] Stopper Down Fail")]
        FrontAssembleCV_StopperDown_Fail = 10078,
        [Description("[Front Assemble CV] Align On Fail")]
        FrontAssembleCV_AlignOn_Fail = 10079,
        [Description("[Front Assemble CV] Align Off Fail")]
        FrontAssembleCV_AlignOff_Fail = 10080,
        [Description("[Front Assemble CV] Film Inspection Fail")]
        FrontAssembleCV_FilmInspection_Fail = 10081,
        [Description("[Front Assemble CV] Assemble Inspection Fail")]
        FrontAssembleCV_AssembleInspection_Fail = 10082,
        [Description("[Front Assemble CV] Set Load Timeout")]
        FrontAssembleCV_SetLoad_Timeout = 10083,
        [Description("[Front Assemble CV] Set Unload Timeout")]
        FrontAssembleCV_SetUnload_Timeout = 10084,
        [Description("[Front Assemble CV] Two Set Exist")]
        FrontAssembleCV_TwoOfSetExisting = 11083, //
        [Description("[Front Assemble CV] Set Need Removed")]
        FrontAssembleCV_SetNeedToBeRemoved = 11084, //
        [Description("[Front Assemble CV] Set In Stopper Warning")]
        FrontAssembleCV_SetInStopperWarning = 11185,

        [Description("[Rear Assemble CV] Stopper Up Fail")]
        RearAssembleCV_StopperUp_Fail = 11077,
        [Description("[Rear Assemble CV] Stopper Down Fail")]
        RearAssembleCV_StopperDown_Fail = 11078,
        [Description("[Rear Assemble CV] Align On Fail")]
        RearAssembleCV_AlignOn_Fail = 11079,
        [Description("[Rear Assemble CV] Align Off Fail")]
        RearAssembleCV_AlignOff_Fail = 11080,
        [Description("[Rear Assemble CV] Film Inspection Fail")]
        RearAssembleCV_FilmInspection_Fail = 11081,
        [Description("[Rear Assemble CV] Assemble Inspection Fail")]
        RearAssembleCV_AssembleInspection_Fail = 11082,
        [Description("[Rear Assemble CV] Set Load Timeout")]
        RearAssembleCV_SetLoad_Timeout = 12083,
        [Description("[Rear Assemble CV] Set Unload Timeout")]
        RearAssembleCV_SetUnload_Timeout = 12084,
        [Description("[Rear Assemble CV] Two Set Exist")]
        RearAssembleCV_TwoOfSetExisting = 13083,  // 
        [Description("[Rear Assemble CV] Set Need Removed")]
        RearAssembleCV_SetNeedToBeRemoved = 13084,  //
        [Description("[Rear Assemble CV] Set In Stopper Warning")]
        RearAssembleCV_SetInStopperWarning = 13085,

        // Set Out CV start with index 11300
        [Description("[Front Out CV] Stopper Up Fail")]
        FrontOUTCV_StopperUp_Fail = 10085,
        [Description("[Front Out CV] Stopper Down Fail")]
        FrontOUTCV_StopperDown_Fail = 10086,
        [Description("[Front Out CV] Sensor Mid2 Detect Timeout")]
        FrontOUTCV_SensorMid2Detect_Timeout = 10087,
        [Description("[Front Out CV] Sensor End Detect Timeout")]
        FrontOUTCV_SensorEndDetect_Timeout = 10088,
        [Description("[Front Out CV] Sensor Unload Pos Up Detect Timeout")]
        FrontOUTCV_SensorUnloadPosUpDetect_Timeout = 10089,
        [Description("[Front Out CV] Sensor Status Fail")]
        FrontOUTCV_SensorStatus_Fail = 10090,
        [Description("[Front Out CV] SetLoad Timeout")]
        FrontOUTCV_SetLoad_Timeout = 11090,
        [Description("[Front Out CV] Vacuum On Fail")]
        FrontOUTCV_VacOn_Fail = 11091,
        [Description("[Front Out CV] Turn Fail")]
        FrontOUTCV_Turn_Fail = 11092,
        [Description("[Front Out CV] Cyl Mover Up Fail")]
        FrontOUTCV_MoverCylUp_Fail = 11093,
        [Description("[Front Out CV] Cyl Mover Down Fail")]
        FrontOUTCV_MoverCylDown_Fail = 11094,
        [Description("[Front Out CV] Return Fail")]
        FrontOUTCV_Return_Fail = 11095,
        [Description("[Front Out CV] EndSensor Timeout")]
        FrontOUTCV_EndSensorNotOff = 11096,
        [Description("[Front Out CV] Vacuum NG")]
        FrontOUTCV_VacuumLostWhileMoverUp = 11097,
        //[Description("[Front Out CV] State Mismatch")]
        //FrontOUTCV_StateMismatch = 11098,

        [Description("[Rear Out CV] Stopper Up Fail")]
        RearOUTCV_StopperUp_Fail = 11085,
        [Description("[Rear Out CV] Stopper Down Fail")]
        RearOUTCV_StopperDown_Fail = 11086,
        [Description("[Rear Out CV] Sensor Mid2 Detect Timeout")]
        RearOUTCV_SensorMid2Detect_Timeout = 11087,
        [Description("[Rear Out CV] Sensor End Detect Timeout")]
        RearOUTCV_SensorEndDetect_Timeout = 11088,
        [Description("[Rear Out CV] Sensor Unload Pos Up Detect Timeout")]
        RearOUTCV_SensorUnloadPosUpDetect_Timeout = 11089,
        [Description("[Rear Out CV] Sensor Status Fail")]
        RearOUTCV_SensorStatus_Fail = 12090,
        [Description("[Rear Out CV] SetLoad Timeout")]
        RearOUTCV_SetLoad_Timeout = 12089,
        [Description("[Rear Out CV] Cyl Mover Up Fail")]
        RearOUTCV_MoverCylUp_Fail = 12091,
        [Description("[Rear Out CV] Cyl Mover Down Fail")]
        RearOUTCV_MoverCylDown_Fail = 12092,
        None = -1,
        //MainAirNotSupplied = 50,
        //MainPowerDown = 51,
        //MotionAlarmDetected = 52,
        //EmergencyStopPressed = 10090,
        //PowerMcOff = 10091,
        //ServoPowerOff = 55,
        // Tray In CV start with index 

        // Tray In Elevator start with index 10150
        [Description("[Tray In Lift] Z Axis Origin Fail")]
        TrayINLift_ZAxis_OriginFail = 10092,
        [Description("[Tray In Lift] Tray Search Fail")]
        TrayINLift_TraySearchFail = 10093,
        [Description("[Tray In Lift] Stop Fail")]
        TrayINLift_StopFail = 10094,

        // Tray Out CV start with index 

        // Tray Out Elevator start with index 
        [Description("[Tray Out Lift] Z Axis Origin Fail")]
        TrayOUTLift_ZAxis_OriginFail = 10095,
        //TrayOutElevator_TraySearchFail = 10096,
        [Description("[Tray Out Lift] Move Ready Place Pos Fail")]
        TrayOUTLift_Move_To_Ready_Place_Fail = 10097,
        [Description("[Tray Out Lift] Move Up Height Pos Fail")]
        TrayOUTLift_OutElevator_Up_Height_Position_Move_Timeout = 10098,


        //Tray Head start with index 
        [Description("[Tray CAM Loader] Z Axis Origin Fail")]
        TrayCAMLoader_ZAxis_OriginFail = 10099,
        [Description("[Tray CAM Loader] XY Axis Origin Fail")]
        TrayCAMLoader_XYAxis_OriginFail = 10100,
        [Description("[Tray CAM Loader] Z Axis Move Wait Pos Fail")]
        TrayCAMLoader_ZAxis_MoveWaitPosition_Fail = 10101,
        [Description("[Tray CAM Loader] XY Axis Move Tray Pick Pos Fail")]
        TrayCAMLoader_XYAxis_MovePickTrayPosition_Fail = 10102,
        [Description("[Tray CAM Loader] XY Axis Move Tray Place Pos Fail")]
        TrayCAMLoader_XYAxis_MovePlaceTrayPosition_Fail = 10103,
        [Description("[Tray CAM Loader] Z Axis Move Cam Ready Pick Pos Fail")]
        TrayCAMLoader_ZAxis_MoveReadyPickCameraPosition_Fail = 10104,
        [Description("[Tray CAM Loader] XY Axis Move Cam Ready Pick Pos Fail")]
        TrayCAMLoader_XYAxis_MoveReadyPickCameraPosition_Fail = 10105,
        [Description("[Tray CAM Loader] XY Axis Move Cam Pick Pos Fail")]
        TrayCAMLoader_XYAxis_MovePickCameraPosition_Fail = 10106,
        [Description("[Tray CAM Loader] XY Axis Move Wait Pos Fail")]
        TrayCAMLoader_XYAxis_MoveWaitPosition_Fail = 10107,
        [Description("[Tray CAM Loader] Z Axis Move Cam Pick Pos Fail")]
        TrayCAMLoader_ZAxis_MovePickCameraPosition_Fail = 10108,
        [Description("[Tray CAM Loader] Z Axis Move Cam Ready Place Pos Fail")]
        TrayCAMLoader_ZAxis_MoveReadyPlaceCameraPosition_Fail = 10109,
        [Description("[Tray CAM Loader] XY Axis Move Cam Ready Place Pos Fail")]
        TrayCAMLoader_XYAxis_MoveReadyPlaceCameraPosition_Fail = 10110,
        [Description("[Tray CAM Loader] XY Axis Move Cam Place Pos Fail")]
        TrayCAMLoader_XYAxis_MovePlaceCameraPosition_Fail = 10111,
        [Description("[Tray CAM Loader] Z Axis Move Cam Place Pos Fail")]
        TrayCAMLoader_ZAxis_MovePlaceCameraPosition_Fail = 10112,
        [Description("[Tray CAM Loader] Z Axis Move Cam Scanner Pos Fail")]
        TrayCAMLoader_ZAxis_MoveScanCameraPosition_Fail = 10113,
        [Description("[Tray CAM Loader] XY Axis Move Cam Scanner Pos Fail")]
        TrayCAMLoader_XYAxis_MoveScanCameraPosition_Fail = 10114,

        // Film Detach start with index 
        [Description("[Vinyl Detach] Y Axis Origin Fail")]
        VinylDetach_OriginFail = 10115,
        [Description("[Vinyl Detach] Y Axis Move Ready Pos Fail")]
        VinylDetach_MoveReady_Fail = 10116,
        [Description("[Vinyl Detach] Y Axis Move Front Detach Pos Fail")]
        VinylDetach_MoveFrontDetachPos_Fail = 10117,
        [Description("[Vinyl Detach] Y Axis Move Rear Detach Pos Fail")]
        VinylDetach_MoveRearDetachPos_Fail = 10118,
        [Description("[Vinyl Detach] Y Axis Move Front Suction Pos Fail")]
        VinylDetach_MoveFrontSuctionPos_Fail = 11117,   // New
        [Description("[Vinyl Detach] Y Axis Move Rear Suction Pos Fail")]
        VinylDetach_MoveRearSuctionPos_Fail = 11118,    // New
        [Description("[Vinyl Detach] Y Axis Move Front Clean Pos Fail")]
        VinylDetach_MoveFrontCleanPos_Fail = 11119,    // New
        [Description("[Vinyl Detach] Y Axis Move Rear Clean Pos Fail")]
        VinylDetach_MoveRearCleanPos_Fail = 11120,    // New

        //Cam Head start with index 
        [Description("[CAM Assemble] Z Axis Origin Fail")]
        CAMAssemble_ZAxis_OriginFail = 10119,
        [Description("[CAM Assemble] RX Axis Origin Fail")]
        CAMAssemble_RXAxis_OriginFail = 10120,
        [Description("[CAM Assemble] XY Axis Origin Fail")]
        CAMAssemble_XYAxis_OriginFail = 10121,
        [Description("[CAM Assemble] Z Axis Move Ready Pick Pos Fail")]
        CAMAssemble_ZAxis_MoveToReadyPick_Fail = 10122,
        [Description("[CAM Assemble] RX Axis Move Pick Pos Fail")]
        CAMAssemble_RXAxis_MoveToPick_Fail = 10123,
        [Description("[CAM Assemble] XY Axis Move Ready Pick Pos Fail")]
        CAMAssemble_XYAxis_MoveToReadyPick_Fail = 10124,
        [Description("[CAM Assemble] XY Axis Move Pick Pos Fail")]
        CAMAssemble_XYAxis_MoveToPick_Fail = 10125,
        [Description("[CAM Assemble] Z Axis Move Pick Pos Fail")]
        CAMAssemble_ZAxis_MoveToPick_Fail = 10126,
        [Description("[CAM Assemble] XY Axis Move Ready Place Pos Fail")]
        CAMAssemble_XYAxis_MoveToReadyPlace_Fail = 10127,
        [Description("[CAM Assemble] Y Axis Move Ready Place Pos Fail")]
        CAMAssemble_YAxis_MoveToReadyPlace_Fail = 10128,
        [Description("[CAM Assemble] Z Axis Move Ready Place Pos Fail")]
        CAMAssemble_ZAxis_MoveToPlaceReady_Fail = 10129,
        [Description("[CAM Assemble] X-Z-RX Axis Move First Place Pos Fail")]
        CAMAssemble_XZRXAxis_MoveToFirstPlace_Fail = 10130,
        [Description("[CAM Assemble] X-Z-RX Axis Move Second Place Pos Fail")]
        CAMAssemble_XZRXAxis_MoveToSecondPlace_Fail = 10131,
        [Description("[CAM Assemble] X-Z Axis Move Place Pos Fail")]
        CAMAssemble_XZAxis_MoveToPlace_Fail = 10132,
        [Description("[CAM Assemble] X-Z-RX Axis Move Ready Pick Pos Fail")]
        CAMAssemble_XZRXAxis_MoveToReadyPick_Fail = 10133,

        [Description("[CAM Assemble] X-Z-RX Axis Move Pre Up Push Pos Fail")]
        CAMAssemble_XZRXAxis_MoveToPreUpPushInPosPlace_Fail = 10134,
        [Description("[CAM Assemble] X Axis Move Pre Push Pos Fail")]
        CAMAssemble_ZAxis_MoveToPrePushInPosPlace_Fail = 10135,
        [Description("[CAM Assemble] X Axis Move Push Pos Fail")]
        CAMAssemble_XAxis_MoveToPushInPosPlace_Fail = 10136,

        [Description("[Rear IN CV] Set Load Timeout")]
        RearINCV_SetLoad_Timeout = 10137,
    }
}
