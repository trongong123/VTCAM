using EQX.Core.Common;
using EQX.Core.Motion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.UI.MVVM
{
    public class MotionsStatusViewModel : ViewModelBase
    {
        public MotionsStatusViewModel()
        {
            
        }

        public List<IMotion> AllMotions { get; set; }
    }
}
