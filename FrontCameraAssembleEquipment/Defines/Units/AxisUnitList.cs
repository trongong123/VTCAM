using EQX.Core.Motion;
using FrontCameraAssembleEquipment.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Units
{
    public class AxisUnitList
    {
        public AxisUnit TraySupplier { get; set; }
        public AxisUnit TrayHead { get; set; }
        public AxisUnit FilmDetach { get; set; }
        public AxisUnit CameraAssemble { get; set; }

        public AxisUnitList(Devices devices)
        {
            _devices = devices;

            TraySupplier = new AxisUnit()
            {
                Name = EUnit.TraySupplier.ToString(),
                AxisList = new List<IMotion>()
                {
                    _devices.Motions.TrayInputZ,
                    _devices.Motions.TrayOutputZ
                }
            };

            TrayHead = new AxisUnit()
            {
                Name = EUnit.TrayHead.ToString(),
                AxisList = new List<IMotion>()
                {
                    _devices.Motions.TrayHeadXAxis,
                    _devices.Motions.TrayHeadYAxis,
                    _devices.Motions.TrayHeadZAxis
                }
            };

            FilmDetach = new AxisUnit()
            {
                Name = EUnit.FilmDetach.ToString(),
                AxisList = new List<IMotion>()
                {
                    _devices.Motions.FilmDetachY
                }
            };

            CameraAssemble = new AxisUnit()
            {
                Name = EUnit.CameraAssemble.ToString(),
                AxisList = new List<IMotion>()
                {
                    _devices.Motions.AssemblePickPlaceX,
                    _devices.Motions.AssemblePickPlaceY,
                    _devices.Motions.AssemblePickPlaceZ,
                    _devices.Motions.AssemblePickPlaceRX
                }
            };
        }

        private readonly Devices _devices;
    }
}
