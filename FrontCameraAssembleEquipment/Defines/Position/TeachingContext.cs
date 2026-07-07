using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class TeachingContext
    {
        public ETeachingState State = ETeachingState.Idle;
        public ESearchZPos Status = ESearchZPos.Idle;
        public PositionGroup PositionGroup;
        public IEnumerator<Position> Enumerator;
        public string Error;
    }
}
