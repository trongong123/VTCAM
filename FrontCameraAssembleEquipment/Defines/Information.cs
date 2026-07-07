using EQX.Core.InOut;
using log4net.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Information
    {
        public string Customer => "Automation Engineering Group(MX)";
        public string MachineName => "FRONTCAMASSEMBLY-C-V1";
        public string SoftwareVersion => $"{MachineName}_{sMajorVersion}.{sRevisionDate}.{sMinorVersion}";

        public Information()
        {
        }

        public static string[] sCopyright =
        {
                "******************************************************************************",
                "* Copyright 2025 by Top Engineering Co.,Ltd.",

                "* This software is the confidential and proprietary information of Top Engineering Co.,Ltd.",
                "*",
                "******************************************************************************", };

        public int sMajorVersion = 1;  //컴파일 날짜
        public int sMinorVersion = 1;  //컴파일 날짜
        public string sRevisionDate
        {
            get
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly(); 
                var creationTime = new FileInfo(execAssembly.Location).LastWriteTime;

                return creationTime.ToString("yyMMdd");
            }
        }
    }
}
