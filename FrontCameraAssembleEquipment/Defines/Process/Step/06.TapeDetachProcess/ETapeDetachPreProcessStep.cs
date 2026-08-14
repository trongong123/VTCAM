namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETapeDetachPreProcessStep
    {
        Start,
        Material_Set,

        Gripper_OnOffEnable_Check,
        Gripper_On,
        Gripper_On_Wait,
        Gripper_Off,
        Gripper_Off_Wait,
        RemoveSponge_Up,
        RemoveSponge_Up_Check,
        End
    }
}
