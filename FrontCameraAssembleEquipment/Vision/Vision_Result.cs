namespace FrontCameraAssembleEquipment.Vision
{
    public class Vision_Result
    {
        public double dPosX;
        public double dPosY;
        public double dPosTH;
        public double dWidth;
        public double dHeight;
        public double dScore;
        public double dAngle;
        public double dCount;
        public string strBarcode;
        public INSPECTION_RESULT eResult;
        public INSPECTION_NAME eInspectionName;

        public Vision_Result()
        {
            dPosX = 0.0;
            dPosY = 0.0;
            dPosTH = 0.0;
            dWidth = 0.0;
            dHeight = 0.0;
            dScore = 0;
            strBarcode = string.Empty;
            eResult = INSPECTION_RESULT.NO_RESPONSE;
            eInspectionName = INSPECTION_NAME.ID;
        }
        public void ResetData()
        {
            dPosX = 0.0;
            dPosY = 0.0;
            dPosTH = 0.0;
            dWidth = 0.0;
            dHeight = 0.0;
            dScore = 0;
            strBarcode = string.Empty;
            eResult = INSPECTION_RESULT.NO_RESPONSE;
            eInspectionName = INSPECTION_NAME.ID;
        }
    }
}
