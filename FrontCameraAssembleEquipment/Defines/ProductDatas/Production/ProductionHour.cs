using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class ProductionHour : ObservableObject
    {
		private int frontOutputCount;
        private int rearOutputCount;
        private int frontFilmDetachFail;
        private int frontAssembleFail;
        private int rearFilmDetachFail;
        private int rearAssembleFail;

        [JsonIgnore]
        public EventHandler FrontOutputCountChanged;
        [JsonIgnore]
        public EventHandler RearOutputCountChanged;
        [JsonIgnore]
        public EventHandler FrontFilmDetachFailChanged;
        [JsonIgnore]
        public EventHandler FrontAssembleFailChanged;
        [JsonIgnore]
        public EventHandler RearFilmDetachFailChanged;
        [JsonIgnore]
        public EventHandler RearAssembleFailChanged;

        public int Hour { get; set; }

        public int FrontOutputCount
		{
			get { return frontOutputCount; }
			set 
			{
				frontOutputCount = value;
                FrontOutputCountChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(); 
			}
		}

		public int RearOutputCount
		{
			get { return rearOutputCount; }
			set 
			{
				rearOutputCount = value;
                RearOutputCountChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(); 
			}
		}

		public int FrontFilmDetachFail
		{
			get { return frontFilmDetachFail; }
			set 
			{
				frontFilmDetachFail = value;
				FrontFilmDetachFailChanged?.Invoke(this, EventArgs.Empty);
				OnPropertyChanged();
            }
		}

		public int FrontAssembleFail
		{
			get { return frontAssembleFail; }
			set 
			{
				frontAssembleFail = value;
                FrontAssembleFailChanged?.Invoke(this, EventArgs.Empty);
				OnPropertyChanged();
            }
        }

        public int RearFilmDetachFail
		{
			get { return rearFilmDetachFail; }
			set 
			{
				rearFilmDetachFail = value;
                RearFilmDetachFailChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
			}
		}

		public int RearAssembleFail
		{
			get { return rearAssembleFail; }
			set 
			{
				rearAssembleFail = value;
                RearAssembleFailChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
			}
		}

		public ProductionHour(int hour)
        {
            Hour = hour;
        }
    }
}
