namespace FrontCameraAssembleEquipment.Defines
{
    /// <summary>
    /// Sequences of the machine
    /// </summary>
    public enum ESequence
    {
        None,
        Stop,

        AutoRun,
        Ready,

        Change,

        /// <summary>
        /// Input tray from OP / AGV to Tray In CV
        /// </summary>
        TrayInCV_Load,

        /// <summary>
        /// Move tray from Tray In CV to Tray In Elevator
        /// </summary>
        TrayInElevator_Load,

        /// <summary>
        /// First tray search (by Height), include Tray In Elevator & Tray Out Elevator
        /// </summary>
        TraySearch,

        /// <summary>
        /// Unload tray from Tray Out Elevator to Tray Out CV
        /// </summary>
        TrayOutElevator_Unload,
        /// <summary>
        /// Unload tray from Tray Out CV to OP / AGV
        /// </summary>
        TrayOutCV_Unload,

        /// <summary>
        /// Pick empty tray from Tray In Elevator (to move to Tray Out Elevator)
        /// </summary>
        TrayHead_Tray_Pick,
        /// <summary>
        /// Move empty tray to Tray Out Elevator
        /// </summary>
        TrayHead_Tray_Place,

        /// <summary>
        /// Pick camera from Tray in Tray In Elevator
        /// </summary>
        TrayHead_Cam_Pick,
        /// <summary>
        /// Place camera to Sponge Detach unit
        /// </summary>
        TrayHead_Cam_Place,

        /// <summary>
        /// Sponge detach sequence
        /// </summary>
        SpongeDetach_RemoveSponge,

        Flipper_Pick,

        CamHead_Pick,
        CamHead_Place,

        Detach_FilmDetach,

        CVIn_Load,
        CVDetach_Load,
        CVAssemble_Load,
        CVOut_Load,
        CVOut_Unload,
        
    }

    public enum ESemiSequence
    {
        None,

        /// <summary>
        /// Input tray from OP / AGV to Tray In CV
        /// </summary>
        TrayInCV_Load,

        /// <summary>
        /// Move tray from Tray In CV to Tray In Elevator
        /// </summary>
        TrayInElevator_Load,

        /// <summary>
        /// First tray search (by Height), include Tray In Elevator & Tray Out Elevator
        /// </summary>
        TraySearch,

        /// <summary>
        /// Unload tray from Tray Out Elevator to Tray Out CV
        /// </summary>
        TrayOutElevator_Unload,
        /// <summary>
        /// Unload tray from Tray Out CV to OP / AGV
        /// </summary>
        TrayOutCV_Unload,

        /// <summary>
        /// Pick empty tray from Tray In Elevator (to move to Tray Out Elevator)
        /// </summary>
        TrayHead_Tray_Pick,
        /// <summary>
        /// Move empty tray to Tray Out Elevator
        /// </summary>
        TrayHead_Tray_Place,

        /// <summary>
        /// Pick camera from Tray in Tray In Elevator
        /// </summary>
        TrayHead_Cam_Pick,
        /// <summary>
        /// Place camera to Sponge Detach unit
        /// </summary>
        TrayHead_Cam_Place,

        /// <summary>
        /// Sponge detach sequence
        /// </summary>
        SpongeDetach_RemoveSponge,

        Flipper_Pick,

        CamHead_Pick,
        CamHead_Place,

        Detach_FilmDetach,

        CVIn_Load,
        CVDetach_Load,
        CVAssemble_Load,
        CVOut_Load,
        CVOut_Unload,

    }
}
