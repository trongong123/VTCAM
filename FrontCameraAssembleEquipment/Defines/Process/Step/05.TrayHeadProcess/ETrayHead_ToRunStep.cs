namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayHead_ToRunStep
    {
        Start,

        InternalInOutSignal_Reset,

        MaterialDataMatching_VacOn,
        MaterialDataMatching_Check,
        CheckZReadyForTraySearch,
        ZAxisMovetoReady,
        ZAxisMovetoReady_Check,
        Cylinder_Up,
        Cylinder_Up_Check,

        End,
    }
}
