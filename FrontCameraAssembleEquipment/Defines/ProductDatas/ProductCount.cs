using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FrontCameraAssembleEquipment.Defines.ProductDatas
{
    public class ProductCount : ObservableObject
    {
        private uint _output;
        private uint _input;
        private uint _filmDetachFail;
        private uint _assembleFail;

        public ProductCount() { }
        /// <summary>
        /// Count number of Input at Detach CV
        /// </summary>
        public uint Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FailRate));
                OnPropertyChanged(nameof(FilmFailRate));
                OnPropertyChanged(nameof(AssembleFailRate));
            }
        }
        public uint Output
        {
            get
            {
                return _output;
            }
            set
            {
                _output = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Pass));
            }
        }

        public uint FilmDetachFail
        {
            get
            {
                return _filmDetachFail;
            }
            set
            {
                _filmDetachFail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Fail));
            }
        }
        public uint AssembleFail
        {
            get
            {
                return _assembleFail;
            }
            set
            {
                _assembleFail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Fail));
            }
        }

        public uint Pass
        {
            get
            {
                if (Fail > Output)
                {
                    return 0;
                }
                return Output - Fail;
            }
        }

        public uint FilmPass
        {
            get
            {
                if (Fail > Output)
                {
                    return 0;
                }
                return Output - FilmDetachFail;
            }
        }

        public uint AssemblePass
        {
            get
            {
                if (Fail > Output)
                {
                    return 0;
                }
                return Output - AssembleFail;
            }
        }

        public uint Fail => FilmDetachFail + AssembleFail;

        public double FailRate
        {
            get
            {
                //Debug.WriteLine($"Fail Rate = {1.0 * Fail / Input}");
                if (Input != 0)
                    return 100.0 * Fail / Input;
                else
                    return 0;
            }
        }
        public double FilmFailRate
        {
            get
            {
                if (Input != 0)
                    return 100.0 * FilmDetachFail / Input;
                else
                    return 0;
            }
        }
        public double AssembleFailRate
        {
            get
            {
                if (Input != 0)
                    return 100.0 * AssembleFail / Input;
                else
                    return 0;
            }
        }

        public static ProductCount operator +(ProductCount a, ProductCount b)
        {
            return new ProductCount()
            {
                Input = a.Input + b.Input,
                Output = a.Output + b.Output,
                FilmDetachFail = a.FilmDetachFail + b.FilmDetachFail,
                AssembleFail = a.AssembleFail + b.AssembleFail
            };
        }
    }
}
