using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class SystemConfig 
    {
		public event EventHandler DevModeStateChange;

		public string LoginPassword
        {
			get { return loginPassword; }
			set { loginPassword = value; }
		}


		public bool RestrictedMode
		{
			get { return restrictedMode; }
			set { restrictedMode = value; }
		}

		[JsonIgnore]
		public bool DevMode
		{
			get { return devMode; }
			set { devMode = value; OnStateChange(); }
		}

        [JsonIgnore]
        public bool VIPRunMode
		{
			get { return vipRunMode; }
			set { vipRunMode = value;  }
		}

		public void OnStateChange()
		{
			DevModeStateChange?.Invoke(this, EventArgs.Empty);
		}

        private bool restrictedMode = false;
		private bool devMode = false;
		private bool vipRunMode = false;
        private string loginPassword = "0000";
    }
}
