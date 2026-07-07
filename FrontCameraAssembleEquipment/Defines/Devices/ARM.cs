using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class ARM
    {
        private readonly IConfiguration _configuration;

        private string LogARMFolder => _configuration["Folders:LogARMFolder"] ?? "";
        public ARM(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void RequestARMInput(bool isReq)
        {
            string currentFile = Path.Combine(LogARMFolder, "InputLog", DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt");

            if (Directory.Exists(Path.Combine(LogARMFolder, "InputLog")) == false) Directory.CreateDirectory(Path.Combine(LogARMFolder, "InputLog"));
            File.WriteAllText(currentFile, isReq ? "1" :"");
        }

        public void RequestARMOutput(bool isReq)
        {
            string currentFile = Path.Combine(LogARMFolder, "OutputLog", DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt");
            if (Directory.Exists(Path.Combine(LogARMFolder, "OutputLog")) == false) Directory.CreateDirectory(Path.Combine(LogARMFolder, "OutputLog"));

            File.WriteAllText(currentFile, isReq ? "1" : "");
        }
    }
}
