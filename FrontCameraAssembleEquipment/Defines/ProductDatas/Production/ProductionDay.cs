using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class ProductionDay : ObservableObject
    {
        public DateTime Date { get; set; }
        public ObservableCollection<ProductionHour> ProductionHours { get; set; }

        public void SubscribeCountChanged()
        {
            foreach (var productionHour in ProductionHours)
            {
                productionHour.FrontOutputCountChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalFrontOutput));
                    OnPropertyChanged(nameof(TotalFrontOutputShiftDay));
                    OnPropertyChanged(nameof(TotalFrontOutputShiftNight));
                    OnPropertyChanged(nameof(TotalOutput));
                    OnPropertyChanged(nameof(TotalOutputShiftDay));
                    OnPropertyChanged(nameof(TotalOutputShiftNight));
                    OnPropertyChanged(nameof(TotalFrontPass));
                    OnPropertyChanged(nameof(TotalFrontPassShiftDay));
                    OnPropertyChanged(nameof(TotalFrontPassShiftNight));
                    OnPropertyChanged(nameof(TotalPass));
                    OnPropertyChanged(nameof(TotalPassShiftDay));
                    OnPropertyChanged(nameof(TotalPassShiftNight));
                };

                productionHour.RearOutputCountChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalRearOutput));
                    OnPropertyChanged(nameof(TotalRearOutputShiftDay));
                    OnPropertyChanged(nameof(TotalRearOutputShiftNight));
                    OnPropertyChanged(nameof(TotalOutput));
                    OnPropertyChanged(nameof(TotalOutputShiftDay));
                    OnPropertyChanged(nameof(TotalOutputShiftNight));
                    OnPropertyChanged(nameof(TotalRearPass));
                    OnPropertyChanged(nameof(TotalRearPassShiftDay));
                    OnPropertyChanged(nameof(TotalRearPassShiftNight));
                    OnPropertyChanged(nameof(TotalPass));
                    OnPropertyChanged(nameof(TotalPassShiftDay));
                    OnPropertyChanged(nameof(TotalPassShiftNight));
                };

                productionHour.FrontFilmDetachFailChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalFail));
                    OnPropertyChanged(nameof(TotalFailShiftDay));
                    OnPropertyChanged(nameof(TotalFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFilmDetachFail));
                    OnPropertyChanged(nameof(TotalFrontFilmDetachFailShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFilmDetachFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFail));
                    OnPropertyChanged(nameof(TotalFrontFailShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFailRate));
                    OnPropertyChanged(nameof(TotalFrontFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFailRateShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFilmFailRate));
                    OnPropertyChanged(nameof(TotalFrontFilmFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFilmFailRateShiftNight));
                };

                productionHour.FrontAssembleFailChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalFail));
                    OnPropertyChanged(nameof(TotalFailShiftDay));
                    OnPropertyChanged(nameof(TotalFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontAssembleFail));
                    OnPropertyChanged(nameof(TotalFrontAssembleFailShiftDay));
                    OnPropertyChanged(nameof(TotalFrontAssembleFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFail));
                    OnPropertyChanged(nameof(TotalFrontFailShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFailShiftNight));
                    OnPropertyChanged(nameof(TotalFrontFailRate));
                    OnPropertyChanged(nameof(TotalFrontFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalFrontFailRateShiftNight));
                    OnPropertyChanged(nameof(TotalFrontAssembleFailRate));
                    OnPropertyChanged(nameof(TotalFrontAssembleFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalFrontAssembleFailRateShiftNight));
                };

                productionHour.RearFilmDetachFailChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalFail));
                    OnPropertyChanged(nameof(TotalFailShiftDay));
                    OnPropertyChanged(nameof(TotalFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearFilmDetachFail));
                    OnPropertyChanged(nameof(TotalRearFilmDetachFailShiftDay));
                    OnPropertyChanged(nameof(TotalRearFilmDetachFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearFail));
                    OnPropertyChanged(nameof(TotalRearFailShiftDay));
                    OnPropertyChanged(nameof(TotalRearFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearFailRate));
                    OnPropertyChanged(nameof(TotalRearFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalRearFailRateShiftNight));
                    OnPropertyChanged(nameof(TotalRearFilmFailRate));
                    OnPropertyChanged(nameof(TotalRearFilmFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalRearFilmFailRateShiftNight));

                };

                productionHour.RearAssembleFailChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(TotalFail));
                    OnPropertyChanged(nameof(TotalFailShiftDay));
                    OnPropertyChanged(nameof(TotalFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearAssembleFail));
                    OnPropertyChanged(nameof(TotalRearAssembleFailShiftDay));
                    OnPropertyChanged(nameof(TotalRearAssembleFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearFail));
                    OnPropertyChanged(nameof(TotalRearFailShiftDay));
                    OnPropertyChanged(nameof(TotalRearFailShiftNight));
                    OnPropertyChanged(nameof(TotalRearFailRate));
                    OnPropertyChanged(nameof(TotalRearFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalRearFailRateShiftNight));
                    OnPropertyChanged(nameof(TotalRearAssembleFailRate));
                    OnPropertyChanged(nameof(TotalRearAssembleFailRateShiftDay));
                    OnPropertyChanged(nameof(TotalRearAssembleFailRateShiftNight));
                };
            }
        }
        public ProductionDay()
        {
            Date = DateTime.Now;
            ProductionHours = new ObservableCollection<ProductionHour>();
            for (int i = 8; i <= 23; i++)
            {
                ProductionHour productionHour = new ProductionHour(i);
                ProductionHours.Add(productionHour);
            }

            for (int i = 0; i <= 7; i++)
            {
                ProductionHour productionHour = new ProductionHour(i);
                ProductionHours.Add(productionHour);
            }

            SubscribeCountChanged();
        }

        [JsonConstructor]
        public ProductionDay(DateTime date, ObservableCollection<ProductionHour> productionHours)
        {
            Date = date;
            ProductionHours = productionHours;
        }

        [JsonIgnore]
        public int TotalOutput
        {
            get
            {
                return TotalFrontOutput + TotalRearOutput;
            }
        }

        [JsonIgnore]
        public int TotalPass
        {
            get
            {
                return TotalFrontPass + TotalRearPass;
            }
        }

        [JsonIgnore]
        public int TotalPassShiftDay
        {
            get
            {
                return TotalFrontPassShiftDay + TotalRearPassShiftDay;
            }
        }

        [JsonIgnore]
        public int TotalPassShiftNight
        {
            get
            {
                return TotalFrontPassShiftNight + TotalRearPassShiftNight;
            }
        }

        [JsonIgnore]
        public int TotalFrontOutput
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.FrontOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontOutputShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.FrontOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontOutputShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.FrontOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontPass
        {
            get
            {
                int total = 0;
                if(TotalFrontOutput > TotalFrontFail)
                {
                    total = TotalFrontOutput - TotalFrontFail;
                }
                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontPassShiftDay
        {
            get
            {
                int total = 0;
                if (TotalFrontOutputShiftDay > TotalFrontFailShiftDay)
                {
                    total = TotalFrontOutputShiftDay - TotalFrontFailShiftDay;
                }
                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontPassShiftNight
        {
            get
            {
                int total = 0;
                if (TotalFrontOutputShiftNight > TotalFrontFailShiftNight)
                {
                    total = TotalFrontOutputShiftNight - TotalFrontFailShiftNight;
                }
                return total;
            }
        }


        [JsonIgnore]
        public int TotalRearOutput
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.RearOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearOutputShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.RearOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearOutputShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.RearOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearPass
        {
            get
            {
                int total = 0;
                if (TotalRearOutput > TotalRearFail)
                {
                    total = TotalRearOutput - TotalRearFail;
                }
                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearPassShiftDay
        {
            get
            {
                int total = 0;
                if (TotalRearOutputShiftDay > TotalRearFailShiftDay)
                {
                    total = TotalRearOutputShiftDay - TotalRearFailShiftDay;
                }
                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearPassShiftNight
        {
            get
            {
                int total = 0;
                if (TotalRearOutputShiftNight > TotalRearFailShiftNight)
                {
                    total = TotalRearOutputShiftNight - TotalRearFailShiftNight;
                }
                return total;
            }
        }


        [JsonIgnore]
        public int TotalFrontFilmDetachFail
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.FrontFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontFilmDetachFailShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.FrontFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontFilmDetachFailShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.FrontFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontAssembleFail
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.FrontAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontAssembleFailShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.FrontAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFrontAssembleFailShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.FrontAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalFail => TotalFrontFail + TotalRearFail;

        [JsonIgnore]
        public int TotalFailShiftDay => TotalFrontFailShiftDay + TotalRearFailShiftDay;
        [JsonIgnore]
        public int TotalFailShiftNight => TotalFrontFailShiftNight + TotalRearFailShiftNight;

        [JsonIgnore]
        public double TotalFailRate => TotalFrontFailRate + TotalRearFailRate;
        [JsonIgnore]
        public double TotalFailRateShiftDay => TotalFrontFailRateShiftDay + TotalRearFailRateShiftDay;
        [JsonIgnore]
        public double TotalFailRateShiftNight => TotalFrontFailRateShiftNight + TotalRearFailRateShiftNight;

        [JsonIgnore]
        public int TotalFrontFail => TotalFrontFilmDetachFail + TotalFrontAssembleFail;
        [JsonIgnore]
        public int TotalFrontFailShiftDay => TotalFrontFilmDetachFailShiftDay + TotalFrontAssembleFailShiftDay;
        [JsonIgnore]
        public int TotalFrontFailShiftNight => TotalFrontFilmDetachFailShiftNight + TotalFrontAssembleFailShiftNight;

        [JsonIgnore]
        public double TotalFrontFailRate
        {
            get
            {
                if(TotalFrontOutput != 0)
                {
                    return 100.0 * TotalFrontFail / TotalFrontOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontFailRateShiftDay
        {
            get
            {
                if (TotalFrontOutputShiftDay != 0)
                {
                    return 100.0 * TotalFrontFailShiftDay / TotalFrontOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontFailRateShiftNight
        {
            get
            {
                if (TotalFrontOutputShiftNight != 0)
                {
                    return 100.0 * TotalFrontFailShiftNight / TotalFrontOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontFilmFailRate
        {
            get
            {
                if (TotalFrontOutput != 0)
                {
                    return 100.0 * TotalFrontFilmDetachFail / TotalFrontOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontFilmFailRateShiftDay
        {
            get
            {
                if (TotalFrontOutputShiftDay != 0)
                {
                    return 100.0 * TotalFrontFilmDetachFailShiftDay / TotalFrontOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontFilmFailRateShiftNight
        {
            get
            {
                if (TotalFrontOutputShiftNight != 0)
                {
                    return 100.0 * TotalFrontFilmDetachFailShiftNight / TotalFrontOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontAssembleFailRate
        {
            get
            {
                if (TotalFrontOutput != 0)
                {
                    return 100.0 * TotalFrontAssembleFail / TotalFrontOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontAssembleFailRateShiftDay
        {
            get
            {
                if (TotalFrontOutputShiftDay != 0)
                {
                    return 100.0 * TotalFrontAssembleFailShiftDay / TotalFrontOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalFrontAssembleFailRateShiftNight
        {
            get
            {
                if (TotalFrontOutputShiftNight != 0)
                {
                    return 100.0 * TotalFrontAssembleFailShiftNight / TotalFrontOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public int TotalRearFilmDetachFail
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.RearFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearFilmDetachFailShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.RearFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearFilmDetachFailShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.RearFilmDetachFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearAssembleFail
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ProductionHours)
                {
                    total += productionHour.RearAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearAssembleFailShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.RearAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearAssembleFailShiftNight
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftNight)
                {
                    total += productionHour.RearAssembleFail;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalRearFail => TotalRearFilmDetachFail + TotalRearAssembleFail;
        [JsonIgnore]
        public int TotalRearFailShiftDay => TotalRearFilmDetachFailShiftDay + TotalRearAssembleFailShiftDay;
        [JsonIgnore]
        public int TotalRearFailShiftNight => TotalRearFilmDetachFailShiftNight + TotalRearAssembleFailShiftNight;

        [JsonIgnore]
        public double TotalRearFailRate
        {
            get
            {
                if (TotalRearOutput != 0)
                {
                    return 100.0 * TotalRearFail / TotalRearOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearFailRateShiftDay
        {
            get
            {
                if (TotalRearOutputShiftDay != 0)
                {
                    return 100.0 * TotalRearFailShiftDay / TotalRearOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearFailRateShiftNight
        {
            get
            {
                if (TotalRearOutputShiftNight != 0)
                {
                    return 100.0 * TotalRearFailShiftNight / TotalRearOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearFilmFailRate
        {
            get
            {
                if (TotalRearOutput != 0)
                {
                    return 100.0 * TotalRearFilmDetachFail / TotalRearOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearFilmFailRateShiftDay
        {
            get
            {
                if (TotalRearOutputShiftDay != 0)
                {
                    return 100.0 * TotalRearFilmDetachFailShiftDay / TotalRearOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearFilmFailRateShiftNight
        {
            get
            {
                if (TotalRearOutputShiftNight != 0)
                {
                    return 100.0 * TotalRearFilmDetachFailShiftNight / TotalRearOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearAssembleFailRate
        {
            get
            {
                if (TotalRearOutput != 0)
                {
                    return 100.0 * TotalRearAssembleFail / TotalRearOutput;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearAssembleFailRateShiftDay
        {
            get
            {
                if (TotalRearOutputShiftDay != 0)
                {
                    return 100.0 * TotalRearAssembleFailShiftDay / TotalRearOutputShiftDay;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double TotalRearAssembleFailRateShiftNight
        {
            get
            {
                if (TotalRearOutputShiftNight != 0)
                {
                    return 100.0 * TotalRearAssembleFailShiftNight / TotalRearOutputShiftNight;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public int TotalInputShiftDay
        {
            get
            {
                int total = 0;
                foreach (var productionHour in ShiftDay)
                {
                    total += productionHour.FrontOutputCount;
                }

                return total;
            }
        }

        [JsonIgnore]
        public int TotalOutputShiftDay
        {
            get
            {
                return TotalRearOutputShiftDay + TotalFrontOutputShiftDay;
            }
        }

        [JsonIgnore]
        public int TotalInputShiftNight
        {
            get
            {
                int totalInput = 0;
                foreach (var productionHour in ShiftNight)
                {
                    totalInput += productionHour.FrontOutputCount;
                }

                return totalInput;
            }
        }

        [JsonIgnore]
        public int TotalOutputShiftNight
        {
            get
            {
                return TotalRearOutputShiftNight + TotalFrontOutputShiftNight;
            }
        }
        [JsonIgnore]
        public ObservableCollection<ProductionHour> ShiftDay
        {
            get
            {
                return new ObservableCollection<ProductionHour>(ProductionHours.SkipLast(12));
            }
        }

        [JsonIgnore]
        public ObservableCollection<ProductionHour> ShiftNight
        {
            get
            {
                return new ObservableCollection<ProductionHour>(ProductionHours.Skip(12));
            }
        }

        //[JsonIgnore]
        //public ObservableCollection<ProductionHour> ProductionHours;

        //public void SetFilterShift(bool isDayshift)
        //{
        //    ProductionHours = isDayshift ? ShiftDay : ShiftNight;
        //}
    }
}
