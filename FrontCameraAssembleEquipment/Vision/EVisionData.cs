namespace FrontCameraAssembleEquipment.Vision
{
    public enum EVisionCmd
    {
        CMD_NONE = 0,
        CMD_FRONT_DETACH_SEARCH,
        CMD_FRONT_ASSEMBLE_SEARCH,
        CMD_REAR_DETACH_SEARCH,
        CMD_REAR_ASSEMBLE_SEARCH,
        CMD_BARCODE_SEARCH,       
    }
    
    public enum EVisoinResult
    {
        RESULT_OK = 0,
        RESULT_NG,
    }
}
