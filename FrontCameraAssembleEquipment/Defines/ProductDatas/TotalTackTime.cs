using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class TotalTackTime : ObservableObject
    {
        public StopWatch TotalStopWatch { get; set; }
        public StopWatch FrontStopWatch { get; set; }
        public StopWatch RearStopWatch { get; set; }

        public StopWatch FrontDetachStopwatch { get; set; }
        public StopWatch RearDetachStopWatch { get; set; }
        public StopWatch FrontAssembleStopwatch { get; set; }
        public StopWatch RearAssembleStopwatch { get; set; }

        public StopWatch EDMLogStopWatch { get; set; }

        public StopWatch FrontCycleStopWatch { get; set; }
        public StopWatch RearCycleStopWatch { get; set; }

        public TotalTackTime() 
        {
            TotalStopWatch = new StopWatch();
            FrontStopWatch = new StopWatch();
            RearStopWatch = new StopWatch();

            FrontDetachStopwatch = new StopWatch();
            RearDetachStopWatch = new StopWatch();
            FrontAssembleStopwatch = new StopWatch();
            RearAssembleStopwatch = new StopWatch();

            EDMLogStopWatch = new StopWatch();

            RearCycleStopWatch = new StopWatch();
            FrontCycleStopWatch = new StopWatch();
        }


        public void StopAllStopWatch()
        {
            TotalStopWatch.StopTiming();
            FrontStopWatch.StopTiming();
            RearStopWatch.StopTiming();
            FrontDetachStopwatch.StopTiming();
            FrontAssembleStopwatch.StopTiming();
            //RearDetachStopwatch.StopTiming();
            RearAssembleStopwatch.StopTiming();

            FrontCycleStopWatch.StopTiming();
            RearCycleStopWatch.StopTiming();
        }

        //private void UpdateTackTime(object? sender, EventArgs e)
        //{
        //    OnPropertyChanged(nameof(TackTime));
        //}

    }
}
