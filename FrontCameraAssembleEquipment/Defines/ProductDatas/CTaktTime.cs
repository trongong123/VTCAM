using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class CTaktTime : ObservableObject
    {
        public CTaktTime(CCountData countData)
        {
            _countData = countData;
        }
        public CTaktTime()
        {
            
        }
        #region Properties
        public double Total
        {
            get { return _Total; }
            set
            {
                if (_Total == value) return;

                _Total = value;
                OnPropertyChanged();
                //OnPropertyChanged(nameof(Average));
            }
        }

        public double Maximum
        {
            get { return _Maximum; }
            set
            {
                if (_Maximum == value) return;

                _Maximum = value;
                OnPropertyChanged();
            }
        }

        public double CycleCurrent
        {
            get { return _CycleCurrent; }
            set
            {
                if (_CycleCurrent == value) return;

                _CycleCurrent = value;
                OnPropertyChanged();
            }
        }

        //public double Average
        //{
        //    get
        //    {
        //        //if (_countData.Total == 0) return 0;
        //        //return 4.0 * Total / (_countData.Total);
        //    }
        //}

        public int TaktTimeCounter { get; set; }

        public void Reset()
        {
            Total = 0;
            Maximum = 0;
            CycleCurrent = 0;
        }

        public void SetTaktTime()
        {
            CycleCurrent = (Environment.TickCount - TaktTimeCounter) / 1000.0;
            Total += CycleCurrent;
            TaktTimeCounter = Environment.TickCount;

            if (Maximum < CycleCurrent)
            {
                Maximum = CycleCurrent;
            }
        }
        #endregion Properties

        #region Privates
        private double _Total;
        private double _Maximum = 0;
        private double _CycleCurrent = 0;
        private readonly CCountData _countData;
        #endregion
    }
}
