using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Vision
{
    public enum MACHINE_NAME
    {
        BONDING_VTCAM_AUTO_LOADER
    }
    public enum CAMERA_LIST
    {
        TRAY_VISION,
        UNDER_VISION,
        SETALIGN_VISION
    }
    public enum VISION_ACTION
    {
        NONE,
        ALIVE,
        ERROR,
        RESET,
        CHANGE,
        INSPECTION,
        CALIBRATION,
        SHOW
    };

    public enum MESSAGE_TYPE
    {
        REQUEST,
        RESPONSE
    }

    public enum VISION_ERROR_CODE
    {
        NONE,
    }

    public enum INSPECTION_RESULT
    {
        SUCCESS,
        FAILED,
        INVALID_CAMERA,
        INVAILD_TYPE,
        INVALID_PARAM,
        INVALID_RECIPE,
        INVALID_MANUAL,
        NO_RESPONSE
    }


    public enum INSPECTION_NAME
    {
        FRONTFILM,
        FRONTASSY,
        REARFILM,
        REARASSY,
        BARCODE,
        SHOW,
        ID,
    }

    public enum INSPECTION_TYPE
    {
        NONE,
        PM,
        BLOB,
        JUDGE,
        OCR,
        ID,
        FILM_ALIGN
    }

    public enum CALIBRATION_RESULT
    {
        NONE,
        SUCCESS,
        FAILED
    }

    public enum CALIBRATION_METHOD
    {
        NONE,
        THREEPOINTCAL,
    }

    public enum CALIBRATION_TARGET
    {
        NONE,
        TRAY,
        UNDER,
        SETALIGN,
    }

    public enum CAL_POINT_NO
    {
        NONE = -1,
        NO_1,
        NO_2,
        NO_3,
        NO_4,
        NO_5,
        NO_6,
        POINT_MAX
    }

    public enum CALIBRATION_DIS_VALUE_TYPE
    {
        FPD,
        V_SCALE_DIS,
        FINAL_OFFSET,

        CIRCLE_CENTER,
        VISION_CENTER
    }
    

    public class VisionAlignValue
    {
        public float fValueX = float.NaN;
        public float fValueY = float.NaN;
        public float fValueT = float.NaN;
        public float fScore = float.NaN;
        public VisionAlignValue(float fValueX, float fValueY, float fValueT, float fScore)
        {
            this.fValueX = fValueX;
            this.fValueY = fValueY;
            this.fValueT = fValueT;
            this.fScore = fScore;
        }
        public VisionAlignValue() { }
        public void Reset()
        {
            this.fValueX = float.NaN;
            this.fValueY = float.NaN;
            this.fValueT = float.NaN;
            this.fScore = float.NaN;
        }

        public VisionAlignValue Clone() => new VisionAlignValue(this.fValueX, this.fValueY, this.fValueT, this.fScore);
        public bool IsEqual(VisionAlignValue target)
        {
            if (float.IsNaN(this.fScore) && float.IsNaN(target.fScore)) return true;
            if (this.fValueX == target.fValueX
                && this.fValueY == target.fValueY
                 && this.fValueT == target.fValueT
                  && this.fScore == target.fScore)
            {
                return true;
            }
            else return false;
        }
    }

    public class CalibrationPointCoord
    {
        private float fXCoord = float.NaN;
        private float fYCoord = float.NaN;
        private float fTCoord = float.NaN;
        public CalibrationPointCoord(float fInputX, float fInputY)
        {
            this.fXCoord = fInputX;
            this.fYCoord = fInputY;
        }
        public CalibrationPointCoord()
        {
        }
        public CalibrationPointCoord(float fInputX, float fInputY, float fInputT)
        {
            this.fXCoord = fInputX;
            this.fYCoord = fInputY;
            this.fTCoord = fInputT;
        }
        public void SetCoord(CalibrationPointCoord inputPoint)
        {
            this.fXCoord = inputPoint.GetX;
            this.fYCoord = inputPoint.GetY;
            this.fTCoord = inputPoint.GetT;
        }

        public void Reset()
        {
            fXCoord = float.NaN;
            fYCoord = float.NaN;
            fTCoord = float.NaN;
        }

        public float GetX => this.fXCoord;
        public float GetY => this.fYCoord;
        public float GetT => this.fTCoord;
    }
}
