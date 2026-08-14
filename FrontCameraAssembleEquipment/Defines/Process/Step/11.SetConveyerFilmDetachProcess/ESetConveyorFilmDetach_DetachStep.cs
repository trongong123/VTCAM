namespace FrontCameraAssembleEquipment.Defines
{
    public enum ESetCVFilmDetach_DetachStep
    {
        Start,

        Cylinder_AlignOn,
        Cylinder_AlignOn_Check,
        FilmDetach_Request_Set,
        FilmDetach_Done_Check,

        Cylinder_AlignOff_Stopper_Down,
        Cylinder_AlignOff_Stopper_Down_Wait,

        End,
    }
}
