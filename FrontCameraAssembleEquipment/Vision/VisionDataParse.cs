using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Vision
{
    #region RESPONSE_DATA_PARSING
    public class ResponseVisionData_Base
    {
        public string sVisionName;
        public VISION_ACTION visionAction;
        public MESSAGE_TYPE messageType;

        public bool parsingValueBase(string[] sValue)
        {
            VISION_ACTION inputVisionAction;
            MESSAGE_TYPE inputMessageType;
            try
            {
                this.sVisionName = sValue[0];
                Enum.TryParse(sValue[1], out inputVisionAction);
                this.visionAction = inputVisionAction;
                Enum.TryParse(sValue[2], out inputMessageType);
                this.messageType = inputMessageType;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }


    public class ResponseVisionData_Alive : ResponseVisionData_Base
    {
        public int iAliveCheckCount;
        public bool parsingValue(string[] sValue)
        {
            try
            {
                base.parsingValueBase(sValue);
                sValue[3] = sValue[3].Trim(new char[] { '[', ']' });
                sValue[3] = sValue[3].Replace("COUNT:", string.Empty);
                this.iAliveCheckCount = int.Parse(sValue[3]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ResponseVisionData_Error : ResponseVisionData_Base
    {
        public int iErrorCode;
        public bool parsingValue(string[] sValue)
        {
            try
            {
                base.parsingValueBase(sValue);
                sValue[3] = sValue[3].Trim(new char[] { '[', ']' });
                this.iErrorCode = int.Parse(sValue[3]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ResponseVisionData_ModelChange : ResponseVisionData_Base
    {
        public string sChangedModelName;
        public bool parsingValue(string[] sValue)
        {
            try
            {
                base.parsingValueBase(sValue);
                sValue[3] = sValue[3].Trim(new char[] { '[', ']' });
                this.sChangedModelName = sValue[3].Replace("MODELNAME:", "");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ResponseVisionData_Inspection : ResponseVisionData_Base
    {
        public string sBodyData;
        public string sChangedModelName;
        public INSPECTION_NAME inspectionName;
        public INSPECTION_RESULT inspectionResult;
        public INSPECTION_TYPE inspectionType;
        public int iResultCount;
        public string sInspectionData;
        public string[,] sRawInspectionDataMatrix;
        public ResultDataMatrixConverter[] convertedInspectionDataMatrix;

        public bool parsingValue(string[] sValue)
        {
            try
            {
                base.parsingValueBase(sValue);

                //BODY
                this.sBodyData = sValue[3];

                // PARSING BODY TO 6 FIELD
                if (!parsingBodyData()) return false;
                // PARSING INSPECTION RESULT DATA TO STRING MATRIX
                if (!parsingResultDataMatrix()) return false;

                // COVERT RESULT DATA STRING MATRIX TO FLOAT INSTANCE 
                if (!convertResultDataMatrix()) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool parsingBodyData()
        {
            INSPECTION_NAME inputInspectionName;
            try
            {
                string[] sSplitBodyData = sBodyData.Split(new string[] { "][" }, StringSplitOptions.None);
                for (int i = 0; i < sSplitBodyData.Length; i++)
                {
                    sSplitBodyData[i] = sSplitBodyData[i].Trim(new char[] { '[', ']' });
                }
                if (sSplitBodyData.Length != 3) return false;

                // parsing start
                sChangedModelName = sSplitBodyData[0].Replace("MODELNAME:", "");
                sSplitBodyData[1] = sSplitBodyData[1].Replace("INSPECTNAME:", "");
                Enum.TryParse(sSplitBodyData[1], out inputInspectionName);
                inspectionName = inputInspectionName;
                sInspectionData = sSplitBodyData[2];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool parsingResultDataMatrix()
        {
            INSPECTION_RESULT inputInspectionResult;
            INSPECTION_TYPE inputInspectionType;
            try
            {
                string[] sSplitInspectionData = sInspectionData.Split(new string[] { "}{" }, StringSplitOptions.None);
                for (int i = 0; i < sSplitInspectionData.Length; i++)
                {
                    sSplitInspectionData[i] = sSplitInspectionData[i].Trim(new char[] { '{', '}' });
                }

                sSplitInspectionData[0] = sSplitInspectionData[0].Replace("RESULT:", "");
                Enum.TryParse(sSplitInspectionData[0], out inputInspectionResult);
                inspectionResult = inputInspectionResult;
                if (inspectionResult != INSPECTION_RESULT.SUCCESS) return false;
                sSplitInspectionData[1] = sSplitInspectionData[1].Replace("INSPECTIONTYPE:", "");
                Enum.TryParse(sSplitInspectionData[1], out inputInspectionType);
                inspectionType = inputInspectionType;
                iResultCount = int.Parse(sSplitInspectionData[2].Replace("RESULTCOUNT:", ""));

                string[] sSplitAlignValue = sSplitInspectionData[3].Split(new string[] { ")(" }, StringSplitOptions.None);
                for (int i = 0; i < sSplitAlignValue.Length; i++)
                {
                    sSplitAlignValue[i] = sSplitAlignValue[i].Trim(new char[] { '(', ')' });
                }

                string[] sElementOfDataInLine = sSplitAlignValue[0].Split(new string[] { "," }, StringSplitOptions.None);
                sRawInspectionDataMatrix = new string[sElementOfDataInLine.Length, iResultCount];

                for (int x = 0; x < iResultCount; x++)
                {
                    string[] sElementOfData = sSplitAlignValue[x].Split(new string[] { "," }, StringSplitOptions.None);

                    for (int y = 0; y < sElementOfData.Length; y++)
                    {
                        sRawInspectionDataMatrix[y, x] = sElementOfData[y];
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool convertResultDataMatrix()
        {
            try
            {
                int iNumOfElementInRow;
                switch (inspectionType)
                {
                    case INSPECTION_TYPE.PM:
                        iNumOfElementInRow = ResultDataMatrixConverter_PM.DATA_MAX;
                        convertedInspectionDataMatrix = new ResultDataMatrixConverter_PM[iResultCount];
                        for (int i = 0; i <iResultCount; i++)
                        {
                            convertedInspectionDataMatrix[i] = new ResultDataMatrixConverter_PM();
                        }
                        break;
                    case INSPECTION_TYPE.BLOB:
                        iNumOfElementInRow = ResultDataMatrixConverter_BLOB.DATA_MAX;
                        convertedInspectionDataMatrix = new ResultDataMatrixConverter_BLOB[iResultCount];
                        for(int i = 0; i < iResultCount; i++)
                        {
                            convertedInspectionDataMatrix[i] = new ResultDataMatrixConverter_BLOB();
                        }
                        break;
                    case INSPECTION_TYPE.JUDGE:
                        iNumOfElementInRow = ResultDataMatrixConverter_JUDGE.DATA_MAX;
                        convertedInspectionDataMatrix = new ResultDataMatrixConverter_JUDGE[iResultCount];
                        for(int i = 0; i < iResultCount; i++)
                        {
                            convertedInspectionDataMatrix[i] = new ResultDataMatrixConverter_JUDGE();
                        }
                        break;
                    case INSPECTION_TYPE.ID:
                        iNumOfElementInRow = ResultDataMatrixConverter_ID.DATA_MAX;
                        convertedInspectionDataMatrix = new ResultDataMatrixConverter_ID[iResultCount];
                        for(int i = 0; i < iResultCount; i++)
                        {
                            convertedInspectionDataMatrix[i] = new ResultDataMatrixConverter_ID();
                        }
                        break;
                    case INSPECTION_TYPE.OCR:
                        iNumOfElementInRow = ResultDataMatrixConverter_OCR.DATA_MAX;
                        convertedInspectionDataMatrix = new ResultDataMatrixConverter_OCR[iResultCount];
                        for(int i = 0; i < iResultCount; i++)
                        {
                            convertedInspectionDataMatrix[i] = new ResultDataMatrixConverter_OCR();
                        }
                        break;

                    default: return false;
                }


                for (int iConvertedMatrixNo = 0; iConvertedMatrixNo < iResultCount; iConvertedMatrixNo++)
                {
                    string[] sAllElementOfRow = new string[iNumOfElementInRow];
                    for (int iRawMatrixRowNo = 0; iRawMatrixRowNo < iNumOfElementInRow; iRawMatrixRowNo++)
                    {
                        sAllElementOfRow[iRawMatrixRowNo] = sRawInspectionDataMatrix[iRawMatrixRowNo, iConvertedMatrixNo];
                    }
                    if (convertedInspectionDataMatrix[iConvertedMatrixNo].Convert(sAllElementOfRow) != true) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    #endregion

    public class ResponseVisionData_Calibration : ResponseVisionData_Base
    {
        public string sBodyData;
        public CALIBRATION_METHOD calMethod;
        public CALIBRATION_TARGET calTarget;
        public CALIBRATION_RESULT calResult;
        public string sCalData;
        public CalibrationResultDataMatrix[] calResultMatrix;
        public bool parsingValue(string[] sValue)
        {
            try
            {
                calResultMatrix = new CalibrationResultDataMatrix[(int)CAL_POINT_NO.POINT_MAX];
                for (int i = 0; i < (int)CAL_POINT_NO.POINT_MAX; i++)
                {
                    calResultMatrix[i] = new CalibrationResultDataMatrix();
                }
                base.parsingValueBase(sValue);

                //BODY
                this.sBodyData = sValue[3];

                if (!parsingBodyData()) return false;
                if (!parsingResultDataMatrix()) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool parsingBodyData()
        {
            CALIBRATION_METHOD outCalMethod;
            CALIBRATION_TARGET outCalTarget;
            try
            {
                string[] sSplitBodyData = sBodyData.Split(new string[] { "][" }, StringSplitOptions.None);
                for (int i = 0; i < sSplitBodyData.Length; i++)
                {
                    sSplitBodyData[i] = sSplitBodyData[i].Trim(new char[] { '[', ']' });
                }
                if (sSplitBodyData.Length != 3) return false;

                // parsing start
                sSplitBodyData[0] = sSplitBodyData[0].Replace("METHOD:", string.Empty);
                Enum.TryParse(sSplitBodyData[0], out outCalMethod);
                calMethod = outCalMethod;
                sSplitBodyData[1] = sSplitBodyData[1].Replace("TARGET:", string.Empty);
                Enum.TryParse(sSplitBodyData[1], out outCalTarget);
                calTarget = outCalTarget;
                sCalData = sSplitBodyData[2];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool parsingResultDataMatrix()
        {
            double dPointNo = double.NaN;
            double dPosX = double.NaN;
            double dPosY = double.NaN;
            {
                CALIBRATION_RESULT outCalResult;
                try
                {
                    string[] sCalDataSplitted = sCalData.Split(new string[] { "}{" }, StringSplitOptions.None);
                    for (int i = 0; i < sCalDataSplitted.Length; i++)
                    {
                        sCalDataSplitted[i] = sCalDataSplitted[i].Trim(new char[] { '{', '}' });
                    }
                    sCalDataSplitted[0] = sCalDataSplitted[0].Replace("RESULT:", string.Empty);
                    Enum.TryParse(sCalDataSplitted[0], out outCalResult);
                    calResult = outCalResult;

                    string[] sElementOfData = sCalDataSplitted[1].Split(new string[] { ")(" }, StringSplitOptions.None);
                    if (sElementOfData.Length == 5)
                    {
                        for (int i = 0; i < sElementOfData.Length; i++)
                        {
                            sElementOfData[i] = sElementOfData[i].Trim(new char[] { '(', ')' });
                            string[] sPosData = sElementOfData[i].Split(new string[] { "," }, StringSplitOptions.None);
                            sPosData[0] = sPosData[0].Replace("POINT:", string.Empty);
                            dPointNo = double.Parse(sPosData[0]);
                            sPosData[1] = sPosData[1].Replace("POSX:", string.Empty);
                            dPosX = double.Parse(sPosData[1]);
                            sPosData[2] = sPosData[2].Replace("POSY:", string.Empty);
                            dPosY = double.Parse(sPosData[2]);
                            double[] doubleArray = { dPointNo, dPosX, dPosY };
                            calResultMatrix[i].SetDataValue(doubleArray);
                        }
                    }
                    else
                    {
                        sElementOfData[0] = sElementOfData[0].Trim(new char[] { '(', ')' });
                        string[] sPosData = sElementOfData[0].Split(new string[] { "," }, StringSplitOptions.None);
                        sPosData[0] = sPosData[0].Replace("POINT:", string.Empty);
                        dPointNo = double.Parse(sPosData[0]);
                        sPosData[1] = sPosData[1].Replace("POSX:", string.Empty);
                        dPosX = double.Parse(sPosData[1]);
                        sPosData[2] = sPosData[2].Replace("POSY:", string.Empty);
                        dPosY = double.Parse(sPosData[2]);
                        double[] doubleArray = { dPointNo, dPosX, dPosY };
                        calResultMatrix[(int)dPointNo - 1].SetDataValue(doubleArray);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }

    public class CalibrationResultDataMatrix
    {
        double dPointNo = double.NaN;
        double dPosX = double.NaN;
        double dPosY = double.NaN;

        public void SetDataValue(double[] dValueArray)
        {
            dPointNo = dValueArray[0];
            dPosX = dValueArray[1];
            dPosY = dValueArray[2];
        }

        public double[] GetDataValue()
        {
            double[] doubleArray = {dPointNo, dPosX, dPosY};
            return doubleArray;
        }
    }


    #region CONVERT_STRING_RESULT_DATA_TO_FLOAT
    public abstract class ResultDataMatrixConverter
    {
        public abstract bool Convert(string[] sRowOfMatrix);
        public abstract object[] GetValue();
        protected float ConvertToFloat(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return 0;
            }
            else
            {
                return float.Parse(s);
            }
        }
    }

    public class ResultDataMatrixConverter_PM : ResultDataMatrixConverter
    {
        public static int DATA_RESULT = 0;
        public static int DATA_POSX = 1;
        public static int DATA_POSY = 2;
        public static int DATA_ANGLE = 3;
        public static int DATA_SCORE = 4;
        public static int DATA_MAX = 5;

        float fPosX;
        float fPosY;
        float fAngle;
        float fScore;
        string strResult;

        public override bool Convert(string[] sRowOfMatrix)
        {
            try
            {
                if (sRowOfMatrix.Length != 5) { return false; }
                strResult = sRowOfMatrix[DATA_RESULT].Replace("Result:", string.Empty);
                sRowOfMatrix[DATA_POSX] = sRowOfMatrix[DATA_POSX].Replace("X:", string.Empty);
                fPosX = base.ConvertToFloat(sRowOfMatrix[DATA_POSX]);
                sRowOfMatrix[DATA_POSY] = sRowOfMatrix[DATA_POSY].Replace("Y:", string.Empty);
                fPosY = base.ConvertToFloat(sRowOfMatrix[DATA_POSY]);
                sRowOfMatrix[DATA_ANGLE] = sRowOfMatrix[DATA_ANGLE].Replace("R:", string.Empty);
                fAngle = base.ConvertToFloat(sRowOfMatrix[DATA_ANGLE]);
                sRowOfMatrix[DATA_SCORE] = sRowOfMatrix[DATA_SCORE].Replace("SCORE:", string.Empty);
                fScore = base.ConvertToFloat(sRowOfMatrix[DATA_SCORE]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object[] GetValue()
        {
            object[] floatArray = { strResult, fPosX, fPosY, fAngle, fScore };
            return floatArray;
        }

    }

    public class ResultDataMatrixConverter_BLOB : ResultDataMatrixConverter
    {
        public static int DATA_RESULT = 0;
        public static int DATA_CENX = 1;
        public static int DATA_CENY = 2;
        public static int DATA_WIDTH = 3;
        public static int DATA_HEIGHT = 4;
        public static int DATA_AREA = 5;
        public static int DATA_MAX = 6;

        string strResult;
        float fCenX;
        float fCenY;
        float fWidth;
        float fHeight;
        float fArea;

        public override bool Convert(string[] sRowOfMatrix)
        {
            try
            {
                if (sRowOfMatrix.Length != DATA_MAX) { return false; }
                strResult = sRowOfMatrix[DATA_RESULT].Replace("RESULT:", string.Empty);
                sRowOfMatrix[DATA_CENX] = sRowOfMatrix[DATA_CENX].Replace("CENX:", string.Empty);
                fCenX = base.ConvertToFloat(sRowOfMatrix[DATA_CENX]);
                sRowOfMatrix[DATA_CENY] = sRowOfMatrix[DATA_CENY].Replace("CENY:", string.Empty);
                fCenY = base.ConvertToFloat(sRowOfMatrix[DATA_CENY]);
                sRowOfMatrix[DATA_WIDTH] = sRowOfMatrix[DATA_WIDTH].Replace("WIDTH:", string.Empty);
                fWidth = base.ConvertToFloat(sRowOfMatrix[DATA_WIDTH]);
                sRowOfMatrix[DATA_HEIGHT] = sRowOfMatrix[DATA_HEIGHT].Replace("HEIGHT:", string.Empty);
                fHeight = base.ConvertToFloat(sRowOfMatrix[DATA_HEIGHT]);
                sRowOfMatrix[DATA_AREA] = sRowOfMatrix[DATA_AREA].Replace("AREA:", string.Empty);
                fArea = base.ConvertToFloat(sRowOfMatrix[DATA_AREA]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object[] GetValue()
        {
            object[] floatArray = { strResult, fCenX, fCenY, fWidth, fHeight, fArea };
            return floatArray;
        }
    }

    public class ResultDataMatrixConverter_JUDGE : ResultDataMatrixConverter
    {
        public static int DATA_RATING = 0;
        public static int DATA_MAX = 1;
        string sRating;
        public override bool Convert(string[] sRowOfMatrix)
        {
            if (sRowOfMatrix.Length != DATA_MAX) { return false; }
            sRowOfMatrix[DATA_RATING] = sRowOfMatrix[DATA_RATING].Replace("RATING:", string.Empty);
            sRating = sRowOfMatrix[DATA_RATING];
            return true;
        }
        public override object[] GetValue()
        {
            object[] floatArray = { sRating };
            return floatArray;
        }
    }

    public class ResultDataMatrixConverter_ID : ResultDataMatrixConverter
    {
        public static int DATA_RESULT = 0;
        public static int DATA_BARCODEID = 1;
        public static int DATA_MAX = 2;

        string strResult;
        string sBarcode;
        public override bool Convert(string[] sRowOfMatrix)
        {
            try
            {
                if (sRowOfMatrix.Length != DATA_MAX) { return false; }
                sRowOfMatrix[DATA_RESULT] = sRowOfMatrix[DATA_RESULT].Replace("RESULT:", string.Empty);
                strResult = sRowOfMatrix[DATA_RESULT];
                sRowOfMatrix[DATA_BARCODEID] = sRowOfMatrix[DATA_BARCODEID].Replace("TEXT:", string.Empty);
                sBarcode = sRowOfMatrix[DATA_BARCODEID];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object[] GetValue()
        {
            object[] stringArray = { strResult, sBarcode };
            return stringArray;
        }
    }

    public class ResultDataMatrixConverter_OCR : ResultDataMatrixConverter
    {
        public static int DATA_RESULT = 0;
        public static int DATA_BARCODEID = 1;
        public static int DATA_MAX = 2;

        string strResult;
        string sBarcode;
        public override bool Convert(string[] sRowOfMatrix)
        {
            try
            {
                if (sRowOfMatrix.Length != DATA_MAX) { return false; }
                sRowOfMatrix[DATA_RESULT] = sRowOfMatrix[DATA_RESULT].Replace("Result:", string.Empty);
                strResult = sRowOfMatrix[DATA_RESULT];
                sRowOfMatrix[DATA_BARCODEID] = sRowOfMatrix[DATA_BARCODEID].Replace("Data:", string.Empty);
                sBarcode = sRowOfMatrix[DATA_BARCODEID];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object[] GetValue()
        {
            object[] stringArray = { strResult, sBarcode };
            return stringArray;
        }
    }
    #endregion
}
