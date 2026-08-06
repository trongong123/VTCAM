namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVOut_LoadStep
    {
        Start,
        Stopper_Up,
        Stopper_Up_Wait,

        CVUnload_RequestLoad_Set,
        CV_StartDetect_Wait,
        CV_StartDetect_Check,
        CVUnload_RequestLoad_Clear,

        CV_StartNotDetect_Wait,
        CV_StartNotDetect_Check,
        CV_EndDetect_Wait,

        End,
    }
}
