using FrontCameraAssembleEquipment.Defines.Process;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public enum EPropertyProductionData
    {
        Output,
        FilmDetachFail,
        AssembleFail
    }
    public class ProductionData
    {
        private readonly object lockObj = new object();
        private readonly IConfiguration _configuration;
        private string CountDataFolder => _configuration.GetValue<string>("Folders:CountDataFolder") ?? "";

        public EventHandler TodayProductionChanged;
        public ObservableCollection<ProductionDay> ProductionDatas { get; set; }

        /// <summary>Snapshot trước lần Clear gần nhất; sản lượng trong RAM sau Clear là phần cộng thêm. Read / Save ghi file dùng tổng baseline + RAM.</summary>
        private ProductionDay? _baselinePreClear;
        private DateTime? _baselineDate;

        public ProductionData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>Ngày sản xuất theo ca: từ 8h sáng tới trước 8h sáng hôm sau thuộc cùng một ngày (theo lịch của ca ngày).</summary>
        private DateTime GetProductionCalendarDate(DateTime now)
        {
            return now.Hour >= 8 ? now.Date : now.Date.AddDays(-1);
        }

        private static ProductionDay? CloneProductionDay(ProductionDay? source)
        {
            if (source == null) return null;
            return JsonConvert.DeserializeObject<ProductionDay>(JsonConvert.SerializeObject(source));
        }

        /// <summary>Cộng từng counter theo từng giờ: kết quả = baseline + session.</summary>
        private static ProductionDay MergeProductionDays(ProductionDay baseline, ProductionDay session)
        {
            var merged = CloneProductionDay(baseline);
            if (merged == null)
            {
                merged = new ProductionDay { Date = session.Date.Date };
            }
            else
            {
                merged.Date = baseline.Date.Date;
            }

            merged.SubscribeCountChanged();
            foreach (var hm in merged.ProductionHours)
            {
                var s = session.ProductionHours.First(x => x.Hour == hm.Hour);
                hm.FrontOutputCount += s.FrontOutputCount;
                hm.RearOutputCount += s.RearOutputCount;
                hm.FrontFilmDetachFail += s.FrontFilmDetachFail;
                hm.FrontAssembleFail += s.FrontAssembleFail;
                hm.RearFilmDetachFail += s.RearFilmDetachFail;
                hm.RearAssembleFail += s.RearAssembleFail;
            }

            return merged;
        }

        /// <summary>Khi sang ngày sản xuất mới: nếu vẫn đang phiên Clear (chưa Read), gộp baseline + RAM cho ngày cũ, ghi file và xóa trạng thái Clear.</summary>
        private void FinalizeBaselineWhenProductionDayChanged(DateTime productionDate)
        {
            if (!_baselineDate.HasValue || _baselineDate.Value.Date == productionDate.Date)
                return;

            DateTime oldDate = _baselineDate.Value.Date;
            if (_baselinePreClear != null && ProductionDatas != null)
            {
                ProductionDay? current = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == oldDate);
                ProductionDay sessionPart = current ?? new ProductionDay { Date = oldDate };
                ProductionDay full = MergeProductionDays(_baselinePreClear, sessionPart);
                full.Date = oldDate;
                full.SubscribeCountChanged();

                if (current != null)
                {
                    for (int i = 0; i < ProductionDatas.Count; i++)
                    {
                        if (ProductionDatas[i].Date.Date == oldDate)
                        {
                            ProductionDatas[i] = full;
                            break;
                        }
                    }
                }

                WriteProductionDayFileRaw(full);
            }

            _baselinePreClear = null;
            _baselineDate = null;
            TodayProductionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void WriteProductionDayFileRaw(ProductionDay pd)
        {
            string file = GetProductionDayFilePath(pd.Date.Date);
            File.WriteAllText(file, JsonConvert.SerializeObject(pd, Formatting.Indented));
        }

        public void WriteData(ECVLine conveyorLine, EPropertyProductionData propertyProductionData)
        {
            lock(lockObj)
            {
                DateTime now = DateTime.Now;
                int currentHour = now.Hour;
                DateTime productionDate = GetProductionCalendarDate(now);
                FinalizeBaselineWhenProductionDayChanged(productionDate);

                if (ProductionDatas.FirstOrDefault(pd => pd.Date.Date == productionDate) == null)
                {
                    ProductionDay productionNew = new ProductionDay { Date = productionDate };
                    ProductionDatas.Add(productionNew);

                    TodayProductionChanged?.Invoke(this, EventArgs.Empty);

                    var previousProduction = ProductionDatas.FirstOrDefault(pd => (productionDate - pd.Date.Date).Days == 1);
                    if (previousProduction != null)
                    {
                        Save(previousProduction);
                    }
                }

                if (ProductionDatas.Count() > 46)
                {
                    ProductionDatas.RemoveAt(0);
                }

                ProductionDay? currentProductionDay = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == productionDate);

                if (currentProductionDay == null) return;

                if (conveyorLine == ECVLine.Front)
                {
                    if(propertyProductionData == EPropertyProductionData.Output)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).FrontOutputCount++;
                    }

                    if (propertyProductionData == EPropertyProductionData.FilmDetachFail)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).FrontFilmDetachFail++;
                    }

                    if (propertyProductionData == EPropertyProductionData.AssembleFail)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).FrontAssembleFail++;
                    }
                }
                if (conveyorLine == ECVLine.Rear)
                {
                    if (propertyProductionData == EPropertyProductionData.Output)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).RearOutputCount++;
                    }

                    if (propertyProductionData == EPropertyProductionData.FilmDetachFail)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).RearFilmDetachFail++;
                    }

                    if (propertyProductionData == EPropertyProductionData.AssembleFail)
                    {
                        currentProductionDay.ProductionHours.First(ph => ph.Hour == currentHour).RearAssembleFail++;
                    }
                }
            }
        }

        public void Reset()
        {
            Save();

            DateTime prod = GetProductionCalendarDate(DateTime.Now);
            var current = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == prod);
            if (current != null)
            {
                ProductionDatas.Remove(current);
                ProductionDatas.Add(new ProductionDay { Date = prod });
            }

            _baselinePreClear = null;
            _baselineDate = null;
        }

        /// <summary>Lưu tổng đã có ra file, giữ baseline để Read/Save vẫn cộng được cả ngày; sau đó xóa counter trong RAM (chỉ còn phần tích sau Clear).</summary>
        public void SaveAndClearCurrentProductionDayCounters()
        {
            lock (lockObj)
            {
                if (ProductionDatas == null) return;

                DateTime prod = GetProductionCalendarDate(DateTime.Now);
                FinalizeBaselineWhenProductionDayChanged(prod);

                ProductionDay? current = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == prod);
                if (current == null)
                {
                    current = new ProductionDay { Date = prod };
                    ProductionDatas.Add(current);
                }

                if (_baselinePreClear != null && _baselineDate == prod)
                {
                    var folded = MergeProductionDays(_baselinePreClear, current);
                    WriteProductionDayFileRaw(folded);
                    _baselinePreClear = CloneProductionDay(folded);
                }
                else
                {
                    _baselinePreClear = CloneProductionDay(current);
                    WriteProductionDayFileRaw(current);
                }

                _baselineDate = prod;

                foreach (var ph in current.ProductionHours)
                {
                    ph.FrontOutputCount = 0;
                    ph.RearOutputCount = 0;
                    ph.FrontFilmDetachFail = 0;
                    ph.FrontAssembleFail = 0;
                    ph.RearFilmDetachFail = 0;
                    ph.RearAssembleFail = 0;
                }

                TodayProductionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Hiển thị tổng cả ngày: baseline (trước Clear) + sản lượng phiên hiện tại; hoặc đọc file nếu không còn phiên Clear.</summary>
        public void ReloadCurrentProductionDayFromFile()
        {
            lock (lockObj)
            {
                if (ProductionDatas == null) return;

                DateTime prod = GetProductionCalendarDate(DateTime.Now);
                FinalizeBaselineWhenProductionDayChanged(prod);

                ProductionDay? current = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == prod);

                if (_baselinePreClear != null && _baselineDate == prod && current != null)
                {
                    var full = MergeProductionDays(_baselinePreClear, current);
                    full.Date = prod;
                    full.SubscribeCountChanged();

                    for (int i = 0; i < ProductionDatas.Count; i++)
                    {
                        if (ProductionDatas[i].Date.Date == prod)
                        {
                            ProductionDatas[i] = full;
                            break;
                        }
                    }

                    _baselinePreClear = null;
                    _baselineDate = null;
                    WriteProductionDayFileRaw(full);
                    TodayProductionChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }

                string path = GetProductionDayFilePath(prod);
                if (!File.Exists(path)) return;

                ProductionDay? loaded = JsonConvert.DeserializeObject<ProductionDay>(File.ReadAllText(path));
                if (loaded == null) return;

                loaded.Date = prod;
                loaded.SubscribeCountChanged();

                for (int i = 0; i < ProductionDatas.Count; i++)
                {
                    if (ProductionDatas[i].Date.Date == prod)
                    {
                        ProductionDatas[i] = loaded;
                        TodayProductionChanged?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }

                ProductionDatas.Add(loaded);
                TodayProductionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Load()
        {
            lock (lockObj)
            {
                ProductionDatas = new ObservableCollection<ProductionDay>();
                var files = Directory.GetFiles(CountDataFolder);

                files = files.Reverse().ToArray();

                if (files.Count() > 46)
                {
                    files = files.Take(46).ToArray();
                }
                foreach (var file in files)
                {
                    ProductionDay? pd = JsonConvert.DeserializeObject<ProductionDay>(File.ReadAllText(file));

                    if (pd != null)
                    {
                        pd.SubscribeCountChanged();
                        ProductionDatas.Add(pd);
                    }
                }

                DateTime loadProductionDate = GetProductionCalendarDate(DateTime.Now);

                if (ProductionDatas.FirstOrDefault(pd => pd.Date.Date == loadProductionDate.AddDays(-1)) == null)
                {
                    ProductionDay priorProduction = new ProductionDay { Date = loadProductionDate.AddDays(-1) };
                    ProductionDatas.Add(priorProduction);
                }

                if (ProductionDatas.FirstOrDefault(pd => pd.Date.Date == loadProductionDate) == null)
                {
                    ProductionDay currentProduction = new ProductionDay { Date = loadProductionDate };
                    ProductionDatas.Add(currentProduction);
                }

                _baselinePreClear = null;
                _baselineDate = null;
            }
        }

        private string GetProductionDayFilePath(DateTime productionCalendarDate)
        {
            return Path.Combine(CountDataFolder, $"{productionCalendarDate:yyyy_MM_dd}.json");
        }

        public void Save(ProductionDay pd)
        {
            DateTime prodNow = GetProductionCalendarDate(DateTime.Now);
            if (_baselinePreClear != null
                && _baselineDate.HasValue
                && _baselineDate.Value.Date == pd.Date.Date
                && pd.Date.Date == prodNow)
            {
                var full = MergeProductionDays(_baselinePreClear, pd);
                WriteProductionDayFileRaw(full);
            }
            else
            {
                WriteProductionDayFileRaw(pd);
            }
        }

        public void Save()
        {
            DateTime prod = GetProductionCalendarDate(DateTime.Now);
            ProductionDay? currentProduction = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == prod);
            ProductionDay? previousProduction = ProductionDatas.FirstOrDefault(pd => pd.Date.Date == prod.AddDays(-1));
            if (currentProduction != null)
            {
                Save(currentProduction);
            }

            if (previousProduction != null)
            {
                Save(previousProduction);
            }
        }
    }
}
