using FrontCameraAssembleEquipment.Defines.Recipes;
using log4net;
using log4net.Util;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Defines
{
    public class EDMLogger
    {
        private static object lockObjectEDMLog = new object();

        private readonly GlobalRecipe _globalRecipe;
        private readonly Information _information;
        public static object CSystemInfo { get; private set; }
        public EDMLogger(GlobalRecipe globalRecipe, Information information)
        {
            _globalRecipe = globalRecipe;
            _information = information;
        }

        public void EDMLogWrite(string sEventCode, string sJigStatus, params object[] sMessage)
        {
            FileStream fWriteData = null;
            StreamWriter swFile = null;

            lock (lockObjectEDMLog)
            {
                try
                {
                    string sFileName = "", sTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    string sEQName = _information.MachineName;
                    string sEQVersion = string.Format("{0}_{1}.{2}.{3}", sEQName, _information.sMajorVersion, _information.sRevisionDate, _information.sMinorVersion);
                    string sDirPath = "C:\\FA\\LOG\\";    
                    string sFilePath = "";

                    if (!Directory.Exists(sDirPath))
                        Directory.CreateDirectory(sDirPath);

                    sFileName = string.Format(",{0},{1},{2},{3},", sTime, sEventCode, sJigStatus, sEQVersion);
                    if (sEventCode == "9020")
                    {
                        sFileName = string.Format(",{0},{1},{2},", sTime, sEventCode, sJigStatus);
                    }
                    for (int i = 0; i < sMessage.Count(); i++)
                    {
                        sFileName += sMessage[i];
                    }
                    sFileName += ".txt";
                    sFilePath = sDirPath + sFileName;

                    swFile = new StreamWriter(sFilePath);

                    if (!File.Exists(sFilePath))
                    {
                        swFile = new StreamWriter(sFilePath);
                    }

                    swFile.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception");
                }
                finally
                {
                    if (swFile != null) swFile.Close();
                }
            }
        }
        public void AddEDMLog(string sEventCode, string sJigStatus = "00000002", params object[] sLog)
        {

            bool bUseEDM = false;
                bUseEDM = _globalRecipe.UseEDMLog;

            if (bUseEDM)
                EDMLogWrite(sEventCode, sJigStatus, sLog);
        }
    }
}
