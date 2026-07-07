using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using FrontCameraAssembleEquipment.Helpers;
using System.Net;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class CWorkData : ObservableObject
    {
        private double averageCycleTime;
        private double cycleTime;
        private List<double> cycleTimeRecords;

        private readonly IConfiguration _configuration;
        private string CountDataFolder => _configuration.GetValue<string>("Folders:CountDataFolder") ?? "";

        public double CycleTimeTotal = 0.0;

        public double CycleTime
        {
            get { return cycleTime; }
            set
            {
                cycleTime = value;

                if (cycleTime != 0)
                {
                    //if (cycleTimeRecords.Count >= 50)
                    //{
                    //    cycleTimeRecords.RemoveAt(0);
                    //}
                    cycleTimeRecords.Add(cycleTime);
                }

                if (CycleTimeTotal > double.MaxValue - 100) CycleTimeTotal = 0.0;

                CycleTimeTotal += value;

                OnPropertyChanged(nameof(CycleTime));
                OnPropertyChanged(nameof(AverageCycleTime));
            }
        }

        public double AverageCycleTime
        {
            get
            {
                if (CountData.TotalCountData.Output <= 0) return 0;

                return 1.0 * CycleTimeTotal / CountData.TotalCountData.Output;
            }
        }

        public CWorkData(CCountData countData,
            IConfiguration configuration)
        {
            CountData = countData;
            _configuration = configuration;

            cycleTimeRecords = new List<double>();
        }
        public CWorkData()
        {
            CountData = new CCountData();
            cycleTimeRecords = new List<double>();
        }

        public CCountData CountData { get; }

        public void Reset()
        {
            Save();
            CountData.Reset();
        }

        public void Save()
        {
            string file = DefaultWorkDataFile.Replace(".json", $"_{WorkDataFileIndex + 1}.json");

            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void Load()
        {
            CWorkData loadedWorkData = null;

            if (File.Exists(WorkDataFilePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(WorkDataFilePath);
                    loadedWorkData = JsonConvert.DeserializeObject<CWorkData>(jsonContent);
                    CountData.Copy(loadedWorkData.CountData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading file: {ex.Message}");
                }
            }
            else
            {
                loadedWorkData = new CWorkData();
                using (File.Create(WorkDataFilePath)) { }
                string serializeStr = JsonConvert.SerializeObject(loadedWorkData, Formatting.Indented);
                File.WriteAllText(WorkDataFilePath, serializeStr);
            }

        }
        private string WorkDataFilePath
        {
            get
            {
                string directoryPath = Path.Combine(CountDataFolder, $"{DateTime.Now:yyyy-MM}");
                Directory.CreateDirectory(directoryPath);

                return DefaultWorkDataFile.Replace(".json", $"_{WorkDataFileIndex}.json");
            }
        }

        private string DefaultWorkDataFile
        {
            get
            {
                string folderPath = Path.Combine(CountDataFolder, $"{DateTime.Now:yyyy-MM}", $"{DateTime.Now:yyyy-MM-dd}");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return Path.Combine(folderPath, $"Count_{DateTime.Now:yyyy-MM-dd}.json");
            }
        }

        private int WorkDataFileIndex
        {
            get
            {
                string directoryPath = Path.Combine(CountDataFolder, $"{DateTime.Now:yyyy-MM}", $"{DateTime.Now:yyyy-MM-dd}");
                if (!Directory.Exists(directoryPath)) return 0;

                string[] filePaths = Directory.GetFiles(directoryPath, $"Count_{DateTime.Now:yyyy-MM-dd}*.json");
                if (filePaths.Length == 0) return 0;

                string fileLast = filePaths.Last();
                string fileName = Path.GetFileNameWithoutExtension(fileLast);

                if (int.TryParse(fileName.Split('_').Last(), out int index))
                {
                    return index;
                }

                return 0;
            }
        }
    }
}
