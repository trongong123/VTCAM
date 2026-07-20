namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVFilmDetach_LoadStep
    {
        Start,

        StopperUp,
        StopperUp_Check,

        CV_ConditionCheck,

        CVFilmDetach_LoadRequest_Set,

        CV_StartDetect_Wait,

        CV_Run,
        CV_EndDetect_Wait,
        Cylinder_AlignOn,
        Cylinder_AlignOn_Wait,
        CV_Stop,

        CV_SetDetectCondition_Check,

        

        End,
    }
}
