using EQX.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Vision
{
    public class VisionResultData
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

        public VisionResultData()
        {

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
