namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESpongeDetach_CamUnloadStep_OriginalVer
    {
        Start,
        CamCenteringOff,
        CamCenteringOff_Check,
        
        CamCemteringRetryOn,
        CamCemteringRetryOn_Check,
        CamCemteringRetryOff,
        CamCemteringRetryOff_Check,

        PrealignVacCheck,

        RequestCameraOut,
        CamPrealignVacOff,
        CamPrealignVacOff_Check,
        CheckCameraOutDone,
        End
    }
}
