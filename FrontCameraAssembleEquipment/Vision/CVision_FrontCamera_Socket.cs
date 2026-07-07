using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Recipe;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Vision
{
    public class CVision_FrontCamera : ObservableObject
    {
        #region BASIC_VARIABLES
        public TopComponent.Comm.TopTcpServer VisionTcpServer;
        private const int InvalidPortNum = -1;
        private int iAliveRequestNumber;
        private bool bIsResponseArrival;
        private bool bIsInpectionResponseArrival;
        private bool bIsAlive;
        private bool bIsConnected;
        private string sModelName;
        private bool bIsModelNameChanged;
        private VISION_ERROR_CODE eErrorCode;
        private object dataSendLockObject;
        private object dataReceiveLockObject;
        private string ipAddr;
        private int portNum;
        private string m_strEndPoint;
        public Vision_Result m_VisionResultData;
        public event Action<bool> ConnectionStateChanged;
        public event Action<bool> OnDataReceived;

        private ConcurrentQueue<string> receiveDataQueue;
        #endregion

        #region DATA_PACKET_INSTANCE
        public ResponseVisionData_Alive responseDataAlive;
        public ResponseVisionData_Error responseDataError;
        public ResponseVisionData_ModelChange reponseDataModelChange;
        private ResponseVisionData_Inspection responseVisionDataInspection_Temp;
        public ResponseVisionData_Inspection responseVisionDataInspection_FrontAssy;
        public ResponseVisionData_Inspection responseVisionDataInspection_FrontFilm;
        public ResponseVisionData_Inspection responseVisionDataInspection_RearAssy;
        public ResponseVisionData_Inspection responseVisionDataInspection_RearFilm;
        public ResponseVisionData_Inspection responseVisionDataInspection_Barcode;

        private ResponseVisionData_Calibration responseVisionDataCalibration_Temp;
        public ResponseVisionData_Calibration responseVisionDataCalibration_Tray;
        public ResponseVisionData_Calibration responseVisionDataCalibration_Under;
        public ResponseVisionData_Calibration responseVisionDataCalibration_SetAlign;
        #endregion


        #region RESULT_DATA_CALLBACK_EVEMT_DEFINE
        public delegate void ReturnVisionAliveResult(ResponseVisionData_Alive dataPacket);
        public event ReturnVisionAliveResult returnVisionAliveResult;
        public delegate void ReturnVisionErrorResult(ResponseVisionData_Error dataPacket);
        public event ReturnVisionErrorResult returnVisionErrorResult;
        public delegate void ReturnVisionModelChangeResult(ResponseVisionData_ModelChange dataPacket);
        public event ReturnVisionModelChangeResult returnVisionModelChangeResult;
        

        public delegate void LogDelegate(string strMessage);
        public event LogDelegate LogEvent;
        #endregion

        private IConfiguration _configuration;
        private readonly RecipeSelector _recipeSelector;

        public CVision_FrontCamera(IConfiguration configuration,RecipeSelector recipeSelector)
        {
            responseDataAlive = new ResponseVisionData_Alive();
            responseDataError = new ResponseVisionData_Error();
            reponseDataModelChange = new ResponseVisionData_ModelChange();
            responseVisionDataInspection_Temp = new ResponseVisionData_Inspection();
            responseVisionDataInspection_FrontFilm = new ResponseVisionData_Inspection();
            responseVisionDataInspection_FrontAssy = new ResponseVisionData_Inspection();
            responseVisionDataInspection_RearAssy = new ResponseVisionData_Inspection();
            responseVisionDataInspection_RearFilm = new ResponseVisionData_Inspection();
            responseVisionDataInspection_Barcode = new ResponseVisionData_Inspection();

            responseVisionDataCalibration_Temp = new ResponseVisionData_Calibration();
            responseVisionDataCalibration_Tray = new ResponseVisionData_Calibration();
            responseVisionDataCalibration_Under = new ResponseVisionData_Calibration();
            responseVisionDataCalibration_SetAlign = new ResponseVisionData_Calibration();

            receiveDataQueue = new ConcurrentQueue<string>();
            dataSendLockObject = new object();
            dataReceiveLockObject = new object();
            m_VisionResultData = new Vision_Result();
            _configuration = configuration;
            _recipeSelector = recipeSelector;
        }

        public void Initialize(string ipAddress = null, int portNum = InvalidPortNum)
        {
            this.ipAddr = ipAddress;
            this.portNum = portNum;
            VisionTcpServer = new TopComponent.Comm.TopTcpServer(ipAddr, portNum);
            VisionTcpServer.StartListening();
            InitVariables();
            InitVisionEvent();
        }

        private void InitVariables()
        {
            this.bIsResponseArrival = false;
            this.bIsInpectionResponseArrival = false;
            this.bIsModelNameChanged = false;
            this.sModelName = GetModelName();
            this.iAliveRequestNumber = 0;
            this.bIsAlive = false;
            this.bIsConnected = false;
        }
        private void InitVisionEvent()
        {
            VisionTcpServer.OnClientAccepted += ConnectCallback;
            VisionTcpServer.OnClientDisconnected += DisconnectCallback;
            VisionTcpServer.OnDataReceived += ReceivedDataCallback;
            returnVisionAliveResult += AliveCallback;
            returnVisionErrorResult += ErrorCallback;
            returnVisionModelChangeResult += ChangeModelNameCallback;
            ConnectionStateChanged += CVision_FrontCamera_ConnectionStateChanged;
            OnDataReceived += CVision_FrontCamera_DataReceived;
            _recipeSelector.RecipeChanged += (s,e) =>
            {
                SendRequestChangeModelName(GetModelName());
            };
        }

        private void CVision_FrontCamera_ConnectionStateChanged(bool obj)
        {
            OnPropertyChanged(nameof(IsVisionConnected));
        }

        private void CVision_FrontCamera_DataReceived(bool obj)
        {
            if(obj == true && responseVisionDataInspection_Barcode.inspectionType == INSPECTION_TYPE.OCR)
            {
                OnPropertyChanged(nameof(BarcodeResult));
            }
        }

        private void ConnectCallback(string strEndPoint)
        {
            if (strEndPoint == null) return;
            if (strEndPoint.Contains(ipAddr))
            {
                bIsConnected = true;
                m_strEndPoint = strEndPoint;
                SendRequestChangeModelName(GetModelName());
                ConnectionStateChanged?.Invoke(bIsConnected);
            }
        }
        private void DisconnectCallback(string strEndPoint)
        {
            if (strEndPoint == null || strEndPoint.Contains(ipAddr))
            {
                bIsConnected = false;
                ConnectionStateChanged?.Invoke(bIsConnected);
            }
        }
        private void ReceivedDataCallback(string strEndPoin, string readData)
        {
            lock (dataReceiveLockObject)
            {
                try
                {
                    receiveDataQueue.Enqueue(readData);
                    Task.Run(() => ParseDataInQueue());
                    OnDataReceived?.Invoke(bIsConnected);
                }
                catch (Exception e)
                {
                }
            }
        }
        private void ParseDataInQueue()
        {
            string strContent;
            VISION_ACTION reponseVisionAction;
            while (receiveDataQueue.TryDequeue(out strContent))
            {
                strContent = strContent.Replace(" ", "");
                string[] splitReadData = strContent.Split(new string[] { "><" }, StringSplitOptions.None);
                for (int i = 0; i < splitReadData.Length; i++)
                {
                    splitReadData[i] = splitReadData[i].Trim(new char[] { '<', '>' });
                }
                if (splitReadData.Length != 4)
                {
                    throw new Exception("Received Data Format Error!");
                }

                Enum.TryParse(splitReadData[1], out reponseVisionAction);

                switch (reponseVisionAction)
                {
                    case VISION_ACTION.ALIVE:
                        {
                            responseDataAlive.parsingValue(splitReadData);
                            returnVisionAliveResult?.Invoke(responseDataAlive);
                        }
                        break;
                    case VISION_ACTION.ERROR:
                        {
                            responseDataError.parsingValue(splitReadData);
                            returnVisionErrorResult?.Invoke(responseDataError);
                        }
                        break;
                    case VISION_ACTION.CHANGE:
                        {
                            reponseDataModelChange.parsingValue(splitReadData);
                            returnVisionModelChangeResult?.Invoke(reponseDataModelChange);
                        }
                        break;
                    case VISION_ACTION.INSPECTION:
                        {
                            responseVisionDataInspection_Temp.parsingValue(splitReadData);
                            switch (responseVisionDataInspection_Temp.inspectionName)
                            {
                                case INSPECTION_NAME.FRONTFILM:
                                    responseVisionDataInspection_FrontFilm = responseVisionDataInspection_Temp;
                                    InspectionResultCallback(responseVisionDataInspection_FrontFilm);
                                    break;
                                case INSPECTION_NAME.FRONTASSY:
                                    responseVisionDataInspection_FrontAssy = responseVisionDataInspection_Temp;
                                    InspectionResultCallback(responseVisionDataInspection_FrontAssy);
                                    break;
                                case INSPECTION_NAME.REARFILM:
                                    responseVisionDataInspection_RearFilm = responseVisionDataInspection_Temp;
                                    InspectionResultCallback(responseVisionDataInspection_RearFilm);
                                    break;
                                case INSPECTION_NAME.REARASSY:
                                    responseVisionDataInspection_RearAssy = responseVisionDataInspection_Temp;
                                    InspectionResultCallback(responseVisionDataInspection_RearAssy);
                                    break;
                                case INSPECTION_NAME.BARCODE:
                                    responseVisionDataInspection_Barcode = responseVisionDataInspection_Temp;
                                    InspectionResultCallback(responseVisionDataInspection_Barcode);
                                    break;
                            }
                        }
                        break;
                    case VISION_ACTION.CALIBRATION:
                        break;
                    default:
                        {
                            //error
                        }
                        break;
                }
                Thread.Sleep(1);
            }
        }


        #region SEND_DATA_PARSING
        public bool SendMessage(VISION_ACTION action, string sPacketField3, string sInspectionName = null, CALIBRATION_TARGET calTarget = CALIBRATION_TARGET.NONE, CAL_POINT_NO calPointNo = CAL_POINT_NO.NONE, CalibrationPointCoord calPointvalue = null)
        {
            lock (dataSendLockObject)
            {
                string sendPacket;
                try
                {
                    sendPacket = SendMessageParsing(action, sPacketField3, sInspectionName, calTarget, calPointNo, calPointvalue);
                    VisionTcpServer.Write(m_strEndPoint, sendPacket);
                    return true;
                }
                catch (Exception ex)
                {
                    //error
                    return false;
                }
            }
        }

        private string SendMessageParsing(VISION_ACTION action, string sPacketField3, string sInspectionName, CALIBRATION_TARGET calTarget = CALIBRATION_TARGET.NONE, CAL_POINT_NO calPointNo = CAL_POINT_NO.NONE, CalibrationPointCoord calPointvalue = null)
        {
            string sSendPacket = "";
            string[] sPacketField = new string[9];
            sPacketField[0] = MACHINE_NAME.BONDING_VTCAM_AUTO_LOADER.ToString();
            sPacketField[1] = action.ToString();
            sPacketField[2] = MESSAGE_TYPE.REQUEST.ToString();
            switch (action)
            {
                case VISION_ACTION.RESET: sPacketField[3] = string.Empty; break;
                case VISION_ACTION.ALIVE: sPacketField[3] = "COUNT:" + sPacketField3; break;
                case VISION_ACTION.CHANGE: sPacketField[3] = "MODELNAME:" + sPacketField3; break;
                case VISION_ACTION.INSPECTION: sPacketField[3] = "MODELNAME:" + sPacketField3; break;
                case VISION_ACTION.CALIBRATION: sPacketField[3] = "METHOD:" + sPacketField3; break;
                default: sPacketField[3] = sPacketField3; break;
            }

            if (sInspectionName != null && action == VISION_ACTION.INSPECTION) { sPacketField[4] = "INSPECTIONNAME:" + sInspectionName; }
            else if (calTarget != CALIBRATION_TARGET.NONE && action == VISION_ACTION.CALIBRATION) { sPacketField[4] = "TARGET:" + calTarget; }
            else sPacketField[4] = string.Empty;
            sPacketField[5] = string.Empty; sPacketField[6] = string.Empty; sPacketField[7] = string.Empty; sPacketField[8] = string.Empty;
            if (action == VISION_ACTION.CALIBRATION && calPointvalue != null && calPointNo != CAL_POINT_NO.NONE)
            {
                int iPointNo = (int)calPointNo + 1;
                sPacketField[5] = "{(" + "POINT:" + iPointNo.ToString() + ",";
                sPacketField[5] += "POSX:" + calPointvalue.GetX.ToString() + ",";
                sPacketField[5] += "POSY:" + calPointvalue.GetY.ToString() + ")}";
            }

            String sBody = "";
            for (int i = 3; i < sPacketField.Length; i++)
            {
                if (sPacketField[i] == string.Empty) continue;
                sBody += "[" + sPacketField[i] + "]";
            }

            sSendPacket = $"<{sPacketField[0]}><{sPacketField[1]}><{sPacketField[2]}><{sBody}>";
            return sSendPacket;
        }
        #endregion

        #region VISION_RESPONSE_CALLBACK
        private void AliveCallback(ResponseVisionData_Alive dataPackage)
        {
            bIsResponseArrival = true;
            if (dataPackage.iAliveCheckCount == (iAliveRequestNumber + 1))
            { this.bIsAlive = true; this.iAliveRequestNumber++; }
            else this.bIsAlive = false;
        }

        private void ErrorCallback(ResponseVisionData_Error dataPackage)
        {
            bIsResponseArrival = true;
            this.eErrorCode = (VISION_ERROR_CODE)dataPackage.iErrorCode;
        }

        private void ChangeModelNameCallback(ResponseVisionData_ModelChange dataPackage)
        {
            bIsResponseArrival = true;
            if (this.sModelName == dataPackage.sChangedModelName)
            { bIsModelNameChanged = true; }
            else bIsModelNameChanged = false;
        }

        private void InspectionResultCallback(ResponseVisionData_Inspection dataPackage)
        {
            m_VisionResultData.ResetData();
            try
            {
                m_VisionResultData.eResult = dataPackage.inspectionResult;
                m_VisionResultData.eInspectionName = dataPackage.inspectionName;

                object[][] oDataArray = new object[dataPackage.iResultCount][];
                for (int i = 0; i < dataPackage.iResultCount; i++)
                {
                    if (dataPackage.inspectionType == INSPECTION_TYPE.ID && i == 1) continue;
                    oDataArray[i] = dataPackage.convertedInspectionDataMatrix[i].GetValue();
                }

                if (dataPackage.inspectionType == INSPECTION_TYPE.PM && dataPackage.iResultCount > 0)
                {
                    m_VisionResultData.dPosX = oDataArray[0][ResultDataMatrixConverter_PM.DATA_POSX] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_PM.DATA_POSX]) : 0.0f;
                    m_VisionResultData.dPosY = oDataArray[0][ResultDataMatrixConverter_PM.DATA_POSY] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_PM.DATA_POSY]) : 0.0f;
                    m_VisionResultData.dAngle = oDataArray[0][ResultDataMatrixConverter_PM.DATA_ANGLE] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_PM.DATA_ANGLE]) : 0.0f;
                    m_VisionResultData.dScore = oDataArray[0][ResultDataMatrixConverter_PM.DATA_SCORE] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_PM.DATA_SCORE]) : 0.0f;
                }
                else if(dataPackage.inspectionType == INSPECTION_TYPE.BLOB && dataPackage.iResultCount > 0)
                {
                    m_VisionResultData.dPosX = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENX] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENX]) : 0.0f;
                    m_VisionResultData.dPosY = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENY] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENY]) : 0.0f;
                    m_VisionResultData.dWidth = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_WIDTH] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_WIDTH]) : 0.0f;
                    m_VisionResultData.dHeight = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_HEIGHT] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_HEIGHT]) : 0.0f;
                    m_VisionResultData.dCount = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_AREA] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_AREA]) : 0.0f;
                }
                else if(dataPackage.inspectionType == INSPECTION_TYPE.FILM_ALIGN && dataPackage.iResultCount > 0)
                {
                    m_VisionResultData.dPosX = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENX] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENX]) : 0.0f;
                    m_VisionResultData.dPosY = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENY] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_CENY]) : 0.0f;
                    m_VisionResultData.dWidth = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_WIDTH] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_WIDTH]) : 0.0f;
                    m_VisionResultData.dHeight = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_HEIGHT] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_HEIGHT]) : 0.0f;
                    m_VisionResultData.dCount = oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_AREA] != null ? Convert.ToSingle(oDataArray[0][ResultDataMatrixConverter_BLOB.DATA_AREA]) : 0.0f;
                }
                else if (dataPackage.inspectionType == INSPECTION_TYPE.OCR && dataPackage.iResultCount > 0)
                {
                    m_VisionResultData.strBarcode = oDataArray[0][ResultDataMatrixConverter_OCR.DATA_BARCODEID] != null ? Convert.ToString(oDataArray[0][ResultDataMatrixConverter_OCR.DATA_BARCODEID]) : string.Empty;

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                bIsInpectionResponseArrival = true;
            }

        }
        #endregion

        #region SPECIFIC_REQUEST_SEND_FUNCTION
        public bool IsVisionConnected => this.bIsConnected;
        public bool IsVisionAlive => this.bIsAlive;
        public bool IsVisionError => (this.eErrorCode != VISION_ERROR_CODE.NONE);
        public bool IsModelNameChanged => this.bIsModelNameChanged;
        public VISION_ERROR_CODE GetErrorCode() => this.eErrorCode;
        public bool IsResponseArrival => this.bIsResponseArrival;
        public bool IsInpectionResponseArrival => this.bIsInpectionResponseArrival; 
        public string BarcodeResult => this.m_VisionResultData.strBarcode;
        public bool SendRequestAlive()
        {
            if (!IsVisionConnected) return false;
            bIsResponseArrival = false;
            bIsAlive = false;
            if (SendMessage(VISION_ACTION.ALIVE, iAliveRequestNumber.ToString())) return true;
            else return false;
        }

        public bool SendRequestReset()
        {
            if (!IsVisionConnected) return false;
            bIsResponseArrival = false;
            if (SendMessage(VISION_ACTION.RESET, string.Empty)) return true;
            else return false;
        }

        public bool SendRequestChangeModelName(string sModelName)
        {
            if (!IsVisionConnected) return false;
            bIsResponseArrival = false;
            this.bIsModelNameChanged = false;
            this.sModelName = sModelName;
            if (SendMessage(VISION_ACTION.CHANGE, this.sModelName)) return true;
            else return false;
        }

        public bool SendRequestShowUI()
        {
            if (!IsVisionConnected) return false;
            bIsResponseArrival = false;
            this.bIsModelNameChanged = false;
            if (SendMessage(VISION_ACTION.SHOW, "AHIHI")) return true;
            else return false;
        }

        public void CheckTurnOnVisionPGM()
        {
            try
            {
                int processVisionCount = System.Diagnostics.Process.GetProcessesByName("SamSungFrontCamVision").Length;
                if (processVisionCount < 1)
                {
                    System.Diagnostics.Process.Start("C:\\FA\\FRONTCAMASSEMBLY_C_V1\\VISION_PGM\\SamSungFrontCamVision.exe");
                }
            }
            catch (Exception e)
            {
                MessageBoxEx.ShowDialog(e.Message);
            }
        }
        public Vision_Result SendRequestInspection(INSPECTION_NAME InspectionID)
        {
            Vision_Result ResultData = new Vision_Result();
            int nResponseTimeOutSpec = 2000;//msec

            if (!IsVisionConnected) return ResultData;

            bIsInpectionResponseArrival = false;
            bool bSendSucces = SendMessage(VISION_ACTION.INSPECTION, this.sModelName, InspectionID.ToString());
            if (bSendSucces == false) return ResultData;

            Stopwatch TimeCheck = Stopwatch.StartNew();

            while (TimeCheck.ElapsedMilliseconds < nResponseTimeOutSpec)
            {
                if (bIsInpectionResponseArrival == true) break;
                Thread.Sleep(1);
            }
            if (bIsInpectionResponseArrival == false) return ResultData; //Time Out
            else
            {
                if (InspectionID == m_VisionResultData.eInspectionName)
                {
                    ResultData = m_VisionResultData;
                }
            }
            return ResultData;
        }
        
        #endregion

        public string GetModelName()
        {
            string strRecipe = "";
            string recipeFolderPath = _configuration["Folders:RecipeFolder"] ?? string.Empty;
            string recipeSettingFilePath = Path.Combine(recipeFolderPath, "RecipeSetting.json");

            if (recipeFolderPath != string.Empty)
            {
                RecipeSetting recipeSetting = JsonConvert.DeserializeObject<RecipeSetting>(File.ReadAllText(recipeSettingFilePath));

                if (recipeSetting != null)
                {
                    strRecipe = recipeSetting.CurrentRecipe;
                }
            }
            

            return strRecipe;
        }
    }
}
