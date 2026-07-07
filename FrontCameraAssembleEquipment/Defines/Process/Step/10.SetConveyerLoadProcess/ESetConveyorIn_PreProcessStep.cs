using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{ 
    public enum  ESetConveyorIn_PreProcessStep
    {
        Start,

        Check_Sensor_Detect,
        UpStreamSignal_Off,

        Wait_Sensor_No_Detect_Enough,
        UpStreamSignal_On,

        End,
    }
}
