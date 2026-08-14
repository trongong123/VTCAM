namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVOut_UnLoadStep
    {
        Start,
        WaitEndCvDetect,
        WaitLoadEnableSignal,
        Cyl_UnloadUp,
        Cyl_UnloadUp_Check,
        In_OutCvSetUpDetectCheck,
        DownstreamSetUnload_Set,

        DownStream_WorkEnd_Wait,
        Cyl_UnloadDown,
        Cyl_UnloadDown_Check,

        End
    }
}
