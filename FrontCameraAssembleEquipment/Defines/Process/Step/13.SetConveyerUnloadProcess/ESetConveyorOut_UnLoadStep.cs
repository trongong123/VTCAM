namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVOut_UnLoadStep
    {
        Start,
        WaitEndCvDetect,
        WaitLoadEnableSignal,
        Cyl_UnloadUp,
        Cyl_UnloadUp_Check,
        DownstreamSetUnload_Set,

        DownStream_WorkEnd_Wait,

        End
    }
}
