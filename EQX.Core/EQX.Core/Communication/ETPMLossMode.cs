namespace EQX.Core.Communication
{
    public enum ETPIndex
    {
        BM,
        PM,
        CM,
        CHANGE_MODEL,
        MATERIAL,
    }

    public enum ETPMLossDesciption
    {
        // BM
        BREAKDOWN_MANUAL                = 03000,
        // PM
        REGULAR_PM                      = 12000,
        IRREGULAR_PM_CHECK_EQUIPMENT    = 15100,
        IRREGULAR_PM_CHECK_QUALITY      = 15200,
        // CM
        IMPPROVE_EQUIPMENT_EE           = 17200,
        IMPPROVE_PROCESSING_EI          = 17300,
        SETUP_NEW_PRODUCT               = 14000,
        // CHANGE_MODEL
        CHANGE_SAME_MODEL               = 41100,
        CHANGE_DIFFERENT_MODEL          = 41200,
        // MATERIAL
        CHANGE_MATERIAL                 = 35000,
        DOWN_MATERIAL                   = 51000,
    }
}
