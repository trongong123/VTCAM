using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.UI.Controls
{
    public class TactTimeListViewModel : ObservableObject
    {
        public TactTimeListViewModel()
        {
            TactTimes = new ObservableCollection<double>();
            for (int i = 0; i < 30; i++)
            {
                TactTimes.Add(0.0);
            }

            TactTimes.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(AverageTactTime));
            };
        }

        public ObservableCollection<double> TactTimes { get;}

        public double AverageTactTime
        {
            get
            {
                double sum = 0.0;
                foreach (var item in TactTimes)
                {
                    if (item > 0.0) sum += item;
                }

                if (TactTimes.Any(t => t > 0.0) == false) return 0.0;
                return sum / TactTimes.Count(t => t > 0.0);
            }
        }

        public void Reset()
        {
            for (int i = 0; i < TactTimes.Count; i++)
            {
                TactTimes[i] = 0.0;
            }

            OnPropertyChanged(nameof(AverageTactTime));
        }
    }
}
