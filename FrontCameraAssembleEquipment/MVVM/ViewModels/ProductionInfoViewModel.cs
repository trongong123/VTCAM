using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.UI.Controls;
using FrontCameraAssembleEquipment.Services.WindowServices;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using System.Windows.Resources;
using System.IO;
using Newtonsoft.Json;
using FrontCameraAssembleEquipment.Defines;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class ProductionInfoViewModel : ViewModelBase
    {
        public ProductionInfoViewModel(IWindowService windowService, IConfiguration configuration, ProductionData productionData, CWorkData workData)
        {
            _windowService = windowService;
            _configuration = configuration;
            ProductionData = productionData;
            _workData = workData;
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _windowService.Close<ProductionInfoViewModel>();
                });
            }
        }

        public ICommand AllShiftCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsAllShift = true;
                    IsDayShift = false;
                    IsNightShift = false;
                });
            }
        }

        public ICommand DayShiftCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsAllShift = false;
                    IsDayShift = true;
                    IsNightShift = false;
                });
            }
        }

        public ICommand NightShiftCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsAllShift = false;
                    IsDayShift = false;
                    IsNightShift = true;
                });
            }
        }


        public ProductionData ProductionData
        {
            get => _productionData;
            set
            {
                _productionData = value;
                OnPropertyChanged(nameof(ProductionData));
            }
        }

        public ProductionDay ProductionDay
        {
            get
            {
                ProductionDay productionDay = new ProductionDay();

                DateTime todayDateProduction = DateTime.Now.Hour >= 8 ? DateTime.Now.Date : DateTime.Now.Date.AddDays(-1);

                if (SearchTime.Date == todayDateProduction)
                {
                    var tempProduction = ProductionData.ProductionDatas.Where(p => p.Date.Date == SearchTime.Date).FirstOrDefault();
                    if (tempProduction != null)
                    {
                        productionDay = tempProduction;
                    }

                    return productionDay;
                }

                if (SearchTime.Date > todayDateProduction)
                {
                    return new ProductionDay();
                }

                try
                {
                    string selectecSearchProductionFilePath = Path.Combine(CountDataFolder, $"{SearchTime.Date:yyyy_MM_dd}.json");

                    if(File.Exists(selectecSearchProductionFilePath) == false)
                    {
                        return new ProductionDay();
                    }

                    ProductionDay? pd = JsonConvert.DeserializeObject<ProductionDay>(File.ReadAllText(selectecSearchProductionFilePath));

                    if (pd == null)
                    {
                        return new ProductionDay();
                    }

                    return pd;
                }
                catch
                {
                    return new ProductionDay();
                }
            }
        }

        public string Today => DateTime.Today.ToString("dd-MM-yyyy");

        private bool isAllShift = true;
        private bool isDayShift;
        private bool isNightShift;

        public bool IsAllShift
        {
            get { return isAllShift; }
            set
            {
                isAllShift = value;
                OnPropertyChanged();
            }
        }

        public bool IsDayShift
        {
            get { return isDayShift; }
            set
            {
                isDayShift = value;
                OnPropertyChanged();
            }
        }

        public bool IsNightShift
        {
            get { return isNightShift; }
            set
            {
                isNightShift = value;
                OnPropertyChanged();
            }
        }
        public DateTime SearchTime
        {
            get => _searchTime;
            set
            {
                if (_searchTime == value) return;
                _searchTime = value;
                OnPropertyChanged(nameof(SearchTime));
                OnPropertyChanged(nameof(ProductionDay));
            }
        }

        public ICommand DataResetCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_ResetWorkData"]) == true)
                    {
                        _productionData.SaveAndClearCurrentProductionDayCounters();
                        OnPropertyChanged(nameof(ProductionDay));
                        _workData.Reset();
                        _workData.CycleTime = 0.0;
                        _workData.CycleTimeTotal = 0.0;
                    }
                });
            }
        }

        public ICommand DataReadCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _productionData.ReloadCurrentProductionDayFromFile();
                    OnPropertyChanged(nameof(ProductionDay));
                });
            }
        }

        public ICommand SetSearchTimeTodayCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SearchTime = DateTime.Today;
                });
            }
        }

        //public CWorkData WorkData
        //{
        //    get => _workData;
        //    set
        //    {
        //        _workData = value;
        //        OnPropertyChanged(nameof(WorkData));
        //    }
        //}

        //public CCountData CountData
        //{
        //    get
        //    {
        //        if (SearchTime == DateTime.Today)
        //        {
        //            return WorkData.CountData;
        //        }
        //        else
        //        {
        //            return GetCountDataWithTimeFilter();
        //        }
        //    }
        //}

        //public List<string> FilterFilePath
        //{
        //    get
        //    {
        //        List<string> filterFilePathList = new();
        //        int days = (SearchEndTime - SearchTime).Days;

        //        for (int i = 0; i <= days; i++)
        //        {
        //            DateTime currentDate = SearchTime.AddDays(i);
        //            string currentCountFolder = Path.Combine(CountDataFolder, currentDate.ToString("yyyy-MM"), currentDate.ToString("yyyy-MM-dd"));
        //            if (Directory.Exists(currentCountFolder) == false) continue;

        //            var lastFile = new DirectoryInfo(currentCountFolder).GetFiles("*.json")
        //                                                                .OrderByDescending(f => f.LastWriteTime)
        //                                                                .FirstOrDefault();

        //            if (lastFile != null)
        //            {
        //                filterFilePathList.Add(lastFile.FullName);
        //            }
        //        }

        //        return filterFilePathList;
        //    }
        //}

        //private CCountData GetCountDataWithTimeFilter()
        //{
        //    CCountData countData = new CCountData();

        //    foreach(var file in FilterFilePath)
        //    {
        //        var tempData = JsonConvert.DeserializeObject<CWorkData>(File.ReadAllText(file));
        //        if (tempData != null)
        //        {
        //            //countData.Front += tempData.CountData.Front;
        //            //countData.Rear += tempData.CountData.Rear;
        //            //countData.Fail += tempData.CountData.Fail;
        //        }
        //    }

        //    return countData;
        //}

        private CWorkData _workData;
        private DateTime _searchTime = DateTime.Today;

        private readonly IWindowService _windowService;
        private readonly IConfiguration _configuration;
        private ProductionData _productionData;

        private string CountDataFolder => _configuration["Folders:CountDataFolder"] ?? "";
    }
}
