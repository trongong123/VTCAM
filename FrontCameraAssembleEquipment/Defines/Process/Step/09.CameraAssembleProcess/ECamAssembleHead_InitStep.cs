namespace FrontCameraAssembleEquipment.Defines
{ 
    public enum ECamAssembleHead_InitStep
    {
        Start,
        CamHeadStateCheck,

        MoveBackTo1stPos,
        MoveBackTo1stPos_Check,

        ZAxisUp,
        ZAxisUp_Check,
        MoveXY_Ready_Pos,
        MoveXY_Ready_Pos_Check,
        End,
    }
}
