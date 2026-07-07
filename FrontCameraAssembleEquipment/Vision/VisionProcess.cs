using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using EQX.Core.Vision;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Vision;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FrontCameraAssembleEquipment.Vision.CVision_FrontCamera;
using static System.Formats.Asn1.AsnWriter;

namespace FrontCameraAssembleEquipment.Vision
{
    public class VisionProcess : ObservableObject
    {
        public string BarcodeId
        {
            get => barcodeId;
            set
            {
                barcodeId = value;
                OnPropertyChanged(nameof(BarcodeId));
            }
        }

        public bool IsBarcodeIdMatched
        {
            get
            {
                switch (_recipeSelector.CurrentRecipe.GlobalRecipe.CameraType)
                {
                    case ECameraType.NorM:
                        return BarcodeId.StartsWith('N') || BarcodeId.StartsWith('M');
                    case ECameraType.CorH:
                        return BarcodeId.StartsWith('C') || BarcodeId.StartsWith('H');
                    default:
                        return false;
                }
            }
        }
        #region Flags
        private bool FlagOut_VisionInspectionRun
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.VISION_INSPECTION_RUN] = value; }
        }
        private bool FlagOut_ScanRun
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.SCAN_BARCODE_RUN] = value; }
        }

        private bool FlagOut_VisionFrontFilmError
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.VISION_FRONT_FILM_INSPECTION_ERROR] = value; }
        }

        private bool FlagOut_VisionRearFilmError
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.VISION_REAR_FILM_INSPECTION_ERROR] = value; }
        }

        private bool FlagOut_VisionFrontAssembleError
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.VISION_FRONT_ASSEMBLE_INSPECTION_ERROR] = value; }
        }
        private bool FlagOut_SCanBarCodeError
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.SCAN_BARCODE_ERROR] = value; }
        }

        private bool FlagOut_VisionRearAssembleError
        {
            set { _visionProcessOutput[(int)EVisionProcessOutput.VISION_REAR_ASSEMBLE_INSPECTION_ERROR] = value; }
        }
        #endregion

        private static object obj = new object();
        public bool Vision_job(EVisionCmd cmdID, bool bThreadOn = true)
        {
            if (bThreadOn)
            {
                Thread threadRobotJobOn = new Thread(() => _vision_job_thread(cmdID));
                threadRobotJobOn.IsBackground = true;
                threadRobotJobOn.Start();
            }
            else
            {
                if (_vision_job_thread(cmdID) == INSPECTION_RESULT.SUCCESS)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private INSPECTION_RESULT _vision_job_thread(EVisionCmd cmdID)
        {
            lock (obj)
            {
                Vision_Result tempResult = new();
                FlagOut_VisionInspectionRun = true;
                switch (cmdID)
                {
                    case EVisionCmd.CMD_FRONT_DETACH_SEARCH:
                        LogFrontFilm.Info("Send CMD_FRONT_FILM_CHECK");
                        tempResult = Vision.SendRequestInspection(INSPECTION_NAME.FRONTFILM);
                        if (tempResult.eResult != INSPECTION_RESULT.SUCCESS)
                        {
                            Thread.Sleep(100);
                            LogFrontFilm.Info("Send CMD_FRONT_FILM_CHECK");
                            tempResult = Vision.SendRequestInspection(INSPECTION_NAME.FRONTFILM);
                        }
                        setResultToArr(tempResult, ResultData[(int)cmdID - 1]);
                        FlagOut_VisionFrontFilmError = (ResultData[(int)cmdID - 1].eResult == INSPECTION_RESULT.SUCCESS) ? false : true;
                        LastFrontResult = ResultData[(int)cmdID - 1];
                        LogFrontFilm.Info($"Result: {ResultData[(int)cmdID - 1].eResult}");
                        LogFrontFilm.Info($"Result: X_{LastFrontResult.dPosX.ToString("0.###")} - Y_{LastFrontResult.dPosY.ToString("0.###")}");
                        break;
                    case EVisionCmd.CMD_FRONT_ASSEMBLE_SEARCH:
                        LogFrontAssemble.Info("Send CMD_FRONT_ASSEMBLE_CHECK");
                        tempResult = Vision.SendRequestInspection(INSPECTION_NAME.FRONTASSY);
                        if (tempResult.eResult != INSPECTION_RESULT.SUCCESS)
                        {
                            Thread.Sleep(100);
                            LogFrontAssemble.Info("Send CMD_FRONT_ASSEMBLE_CHECK");
                            tempResult = Vision.SendRequestInspection(INSPECTION_NAME.FRONTASSY);
                        }
                        setResultToArr(tempResult, ResultData[(int)cmdID - 1]);
                        FlagOut_VisionFrontAssembleError = (ResultData[(int)cmdID - 1].eResult == INSPECTION_RESULT.SUCCESS) ? false : true;
                        LogFrontAssemble.Info($"Result: {ResultData[(int)cmdID - 1].eResult}");
                        break;
                    case EVisionCmd.CMD_REAR_DETACH_SEARCH:
                        LogRearFilm.Info("Send CMD_REAR_FILM_CHECK");
                        tempResult = Vision.SendRequestInspection(INSPECTION_NAME.REARFILM);
                        if (tempResult.eResult != INSPECTION_RESULT.SUCCESS)
                        {
                            Thread.Sleep(100);
                            LogRearFilm.Info("Send CMD_REAR_FILM_CHECK");
                            tempResult = Vision.SendRequestInspection(INSPECTION_NAME.REARFILM);
                        }
                        setResultToArr(tempResult, ResultData[(int)cmdID - 1]);
                        FlagOut_VisionRearFilmError = (ResultData[(int)cmdID - 1].eResult == INSPECTION_RESULT.SUCCESS) ? false : true;
                        LastRearResult = ResultData[(int)cmdID - 1];
                        LogRearFilm.Info($"Result: X_{LastRearResult.dPosX.ToString("0.###")} - Y_{LastRearResult.dPosY.ToString("0.###")}");
                        break;
                    case EVisionCmd.CMD_REAR_ASSEMBLE_SEARCH:
                        LogRearAssemble.Info("Send CMD_REAR_ASSEMBLE_CHECK");
                        tempResult = Vision.SendRequestInspection(INSPECTION_NAME.REARASSY);
                        if (tempResult.eResult != INSPECTION_RESULT.SUCCESS)
                        {
                            Thread.Sleep(100);
                            LogRearAssemble.Info("Send CMD_REAR_ASSEMBLE_CHECK");
                            tempResult = Vision.SendRequestInspection(INSPECTION_NAME.REARASSY);
                        }
                        setResultToArr(tempResult, ResultData[(int)cmdID - 1]);
                        FlagOut_VisionRearAssembleError = (ResultData[(int)cmdID - 1].eResult == INSPECTION_RESULT.SUCCESS) ? false : true;
                        LogFrontAssemble.Info($"Result: {ResultData[(int)cmdID - 1].eResult}");
                        break;
                    case EVisionCmd.CMD_BARCODE_SEARCH:
                        LogBarcode.Info("Send CMD_BARCODE_CHECK");
                        FlagOut_ScanRun = true;
                        tempResult = Vision.SendRequestInspection(INSPECTION_NAME.BARCODE);
                        if (tempResult.eResult != INSPECTION_RESULT.SUCCESS)
                        {
                            Thread.Sleep(100);
                            LogBarcode.Info("Send CMD_BARCODE_CHECK");
                            tempResult = Vision.SendRequestInspection(INSPECTION_NAME.BARCODE);
                        }
                        setResultToArr(tempResult, ResultData[(int)cmdID - 1]);
                        BarcodeId = ResultData[(int)cmdID - 1].strBarcode;
                        FlagOut_SCanBarCodeError = (ResultData[(int)cmdID - 1].eResult != INSPECTION_RESULT.SUCCESS) ? true : false;
                        FlagOut_ScanRun = false;
                        LogBarcode.Info($"Result : {BarcodeId}");
                        break;
                }
                //Log.Info($"{ResultData[(int)cmdID - 1].eInspectionName.ToString()}, Result: {ResultData[(int)cmdID - 1].eResult}");
                //Log.Info($"Front Result: X_{LastFrontResult.dPosX.ToString("0.###")} - Y_{LastFrontResult.dPosY.ToString("0.###")}");
                //Log.Info($"Rear Result: X_{LastRearResult.dPosX.ToString("0.###")} - Y_{LastRearResult.dPosY.ToString("0.###")}");
#if SIMULATION
                FlagOut_VisionFrontFilmError = false;
                FlagOut_VisionFrontAssembleError = false;
                FlagOut_VisionRearAssembleError = false;
                FlagOut_VisionRearFilmError = false;
#endif

                FlagOut_VisionInspectionRun = false;
                return ResultData[(int)cmdID-1].eResult;
            }
        }

        private void setResultToArr(Vision_Result sourceResult, VisionResultData targetResult)
        {
            targetResult.dPosX = sourceResult.dPosX;
            targetResult.dPosY = sourceResult.dPosY;
            targetResult.dPosTH = sourceResult.dPosTH;
            targetResult.dWidth = sourceResult.dWidth;
            targetResult.dHeight = sourceResult.dHeight;
            targetResult.dScore = sourceResult.dScore;
            targetResult.strBarcode = sourceResult.strBarcode;
            targetResult.eResult = sourceResult.eResult;
            targetResult.eInspectionName = sourceResult.eInspectionName;
        }


        public VisionProcess(CVision_FrontCamera vision,
            VisionResultList visionResultList,
            RecipeSelector recipeSelector,
            [FromKeyedServices("VisionProcessInput")] IDInputDevice<EVisionProcessInput> visionProcessInput,
            [FromKeyedServices("VisionProcessOutput")] IDOutputDevice<EVisionProcessOutput> visionProcessOutput)
        {
            Vision = vision;
            _visionResultList = visionResultList;
            _recipeSelector = recipeSelector;
            _visionProcessInput = visionProcessInput;
            _visionProcessOutput = visionProcessOutput;
        }

        public CVision_FrontCamera Vision { get; set; }
        private readonly IDInputDevice _visionProcessInput;
        private readonly IDOutputDevice _visionProcessOutput;
        private VisionResultList _visionResultList;
        private readonly RecipeSelector _recipeSelector;

        public ILog Log => LogManager.GetLogger("VISION");
        public ILog LogFrontFilm => LogManager.GetLogger("FRONT FILM");
        public ILog LogRearFilm => LogManager.GetLogger("REAR FILM");
        public ILog LogFrontAssemble => LogManager.GetLogger("FRONT ASSY");
        public ILog LogRearAssemble => LogManager.GetLogger("REAR ASSY");
        public ILog LogBarcode => LogManager.GetLogger("BARCODE");
        VisionResultData[] ResultData = new VisionResultData[5] { new VisionResultData(), new VisionResultData(), new VisionResultData(), new VisionResultData(), new VisionResultData() }; 

        public VisionResultData LastFrontResult = new();
        public VisionResultData LastRearResult = new ();

        private string barcodeId;
    }
}
