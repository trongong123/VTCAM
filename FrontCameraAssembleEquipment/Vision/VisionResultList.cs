using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCameraAssembleEquipment.Vision;

namespace FrontCameraAssembleEquipment.Vision
{
    public class VisionResultList
    {
        public VisionResultData FrontFilm_VisionResult { get; set; }
        public VisionResultData FrontAssy_VisionResult { get; set; }
        public VisionResultData RearFilm_VisionResult { get; set;}
        public VisionResultData RearAssy_VisionResult { get; set; }

        public VisionResultList()
        {
            FrontFilm_VisionResult = new VisionResultData();
            FrontAssy_VisionResult = new VisionResultData();
            RearFilm_VisionResult = new VisionResultData();
            RearAssy_VisionResult = new VisionResultData();
        }
    }
}
