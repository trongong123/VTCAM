using FrontCameraAssembleEquipment.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Devices
    {
        public Devices(Inputs inputs, Outputs outputs, Motions motions, Cylinders cylinders, CVs conveyors, RolllerList speedControllerList, CVision_FrontCamera vision, AGV agv)
        {
            Inputs = inputs;
            Outputs = outputs;
            Motions = motions;
            Cylinders = cylinders;
            CVs = conveyors;
            RollerList = speedControllerList;
            Vision = vision;
            AGV = agv;
        }

        public Inputs Inputs { get; set; }
        public Outputs Outputs { get; set; }
        public Motions Motions { get; set; }
        public Cylinders Cylinders { get; set; }
        public CVs CVs { get; set; }
        public RolllerList RollerList { get; set; }
        public CVision_FrontCamera Vision { get; set; }
        public AGV AGV { get; }
    }
}
