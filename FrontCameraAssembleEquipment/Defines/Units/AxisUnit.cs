using EQX.Core.Motion;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class AxisUnit
    {
        public string Name { get; set; }
        public List<IMotion> AxisList { get; set; }
        public bool HasZAxis => AxisList.Any(a => a.Name.Contains("_Z") && (a.Id != (int)EMotion.TRAY_INPUT_Z && a.Id != (int)EMotion.TRAY_OUTPUT_Z));
    }
}