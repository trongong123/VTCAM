namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVIn_UnloadStep
    {
        Start,

        CVDetach_Request_Wait,

        CVIn_SetUnloadStart,

        CVDetach_SetReceive_Wait,

        CVIn_Stop_Delay,

        CVIn_Stop,

        Wait_Set_OutDone,


        End
    }
}
