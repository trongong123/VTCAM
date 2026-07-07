using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{ 
   public enum ESetConveyorIn_IFStep
    {
        Start,

        Wait_IF_On,
        CV_Stop,
        Wait_IF_Off,
        CV_Run,

        End,
    }
}
