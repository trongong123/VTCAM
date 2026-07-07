using CommunityToolkit.Mvvm.ComponentModel;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class CCountData : ObservableObject
    {
        #region Properties

        public CCountData() 
        {
            FrontCountData = new ProductCount();
            RearCountData = new ProductCount();

            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCountData));
            OnPropertyChanged(nameof(TotalCountData.Output));
            OnPropertyChanged(nameof(TotalCountData.Pass));
            OnPropertyChanged(nameof(TotalCountData.Fail));
        }

        public ProductCount FrontCountData
        {
            get { return _frontCountData; }
            set
            {
                _frontCountData = value;
                OnPropertyChanged(nameof(FrontCountData));
            }
        }

        public ProductCount RearCountData
        {
            get { return _rearCountData; }
            set
            {
                _rearCountData = value;
                OnPropertyChanged(nameof(RearCountData));
            }
        }

        public ProductCount TotalCountData => FrontCountData + RearCountData;

        #endregion

        public void Reset()
        {
            FrontCountData = new ProductCount();
            RearCountData = new ProductCount();
        }

        #region Privates

        private ProductCount _frontCountData;
        private ProductCount _rearCountData;
        #endregion
    }
}
