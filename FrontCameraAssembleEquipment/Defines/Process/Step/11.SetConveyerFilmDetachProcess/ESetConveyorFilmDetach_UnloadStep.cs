namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVFilmDetach_UnloadStep
    {
        Start,

        CVAssemble_Request_Wait,


        CV_SetUnloadStart,


        CVAssemble_SetReceive_Wait,
        CVAssemble_SetReceive_Check,
        CV_UnloadStop,

        End,
    }
}
