using EQX.Core.Motion;

namespace FrontCameraAssembleEquipment.Defines
{
    public class AxisUnit
    {
        public string Name { get; set; }
        public List<IMotion> AxisList { get; set; }
        public bool HasZAxis => AxisList.Any(a => a.Name.Contains("_Z") && (a.Id != (int)EMotion.TRAY_INPUT_Z && a.Id != (int)EMotion.TRAY_OUTPUT_Z));
    }
}