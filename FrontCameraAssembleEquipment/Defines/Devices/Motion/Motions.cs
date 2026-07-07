using EQX.Core.Motion;
using EQX.Motion;
using EQX.Motion.ByVendor.Inovance;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Defines
{
    public partial class Motions
    {
        public Motions([FromKeyedServices("AjinMaster#1")] IMotionMaster ajinMaster,
            [FromKeyedServices("AjinMotionFactory")] IMotionFactory<IMotion> ajinFactory
            , List<IMotionParameter> parameterList)
        {
            AjinMaster = ajinMaster;

            AjinMotions = new MotionList<EMotion>(ajinFactory, parameterList);
        }

        #region Publics
        public readonly MotionList<EMotion> AjinMotions;

        public List<IMotion> All => AjinMotions.All;

        public IMotionMaster AjinMaster { get; }
        #endregion

        public IMotion TrayInputZ => AjinMotions.All.First(m => m.Id == (int)EMotion.TRAY_INPUT_Z);
        public IMotion TrayOutputZ => AjinMotions.All.First(m => m.Id == (int)EMotion.TRAY_OUTPUT_Z);
        public IMotion TrayHeadXAxis => AjinMotions.All.First(m => m.Id == (int)EMotion.TRAY_HEAD_X);
        public IMotion TrayHeadYAxis => AjinMotions.All.First(m => m.Id == (int)EMotion.TRAY_HEAD_Y);
        public IMotion TrayHeadZAxis => AjinMotions.All.First(m => m.Id == (int)EMotion.TRAY_HEAD_Z);
        public IMotion FilmDetachY => AjinMotions.All.First(m => m.Id == (int)EMotion.FILM_DETACH_Y);
        public IMotion AssemblePickPlaceX => AjinMotions.All.First(m => m.Id == (int)EMotion.CAM_HEAD_X);
        public IMotion AssemblePickPlaceY => AjinMotions.All.First(m => m.Id == (int)EMotion.CAM_HEAD_Y);
        public IMotion AssemblePickPlaceZ => AjinMotions.All.First(m => m.Id == (int)EMotion.CAM_HEAD_Z);
        public IMotion AssemblePickPlaceRX => AjinMotions.All.First(m => m.Id == (int)EMotion.CAM_HEAD_RX);

        public void MoveContiMotion(ECoordinate coordinate, double[] positionArr, double vel = 200, double acc = 0.25, double dec = 0.25)
        {
            AXM.AxmContiWriteClear((int)coordinate);
            switch (coordinate)
            {
                case ECoordinate.Assemble_XZ:
                    AXM.AxmContiSetAxisMap((int)coordinate, 2, new int[] { (int)EMotion.CAM_HEAD_X, (int)EMotion.CAM_HEAD_Z });
                    break;
                case ECoordinate.Assemble_XY:
                    AXM.AxmContiSetAxisMap((int)coordinate, 2, new int[] { (int)EMotion.CAM_HEAD_X, (int)EMotion.CAM_HEAD_Y });
                    break;
                case ECoordinate.TrayHead_XY:
                    AXM.AxmContiSetAxisMap((int)coordinate, 2, new int[] { (int)EMotion.TRAY_HEAD_X, (int)EMotion.TRAY_HEAD_Y });
                    break;
                case ECoordinate.Assemble_XZRX:
                    AXM.AxmContiSetAxisMap((int)coordinate, 3, new int[] { (int)EMotion.CAM_HEAD_X, (int)EMotion.CAM_HEAD_Z, (int)EMotion.CAM_HEAD_RX });
                    break;
            }

            AXM.AxmContiSetAbsRelMode((int)coordinate, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);

            AXM.AxmContiBeginNode((int)coordinate);

            AXM.AxmLineMove((int)coordinate, positionArr, vel, acc, dec);

            AXM.AxmContiEndNode((int)coordinate);
            AXM.AxmContiStart((int)coordinate, 0, 0);
            //xtd 20251113 Got a Massage ticket!!!
            //while(true)
            //{
            //    uint inMotion = 0;
            //    uint ret = AXM.AxmContiIsMotion((int)coordinate, ref inMotion);
            //    if (inMotion == 0) break;
            //    Thread.Sleep(10);
            //}
        }

        public bool IsContiMotioning(ECoordinate coordinate)
        {
            uint uInMotion = 1;
            AXM.AxmContiIsMotion((int)coordinate, ref uInMotion);

            return uInMotion == 1;
        }

        public EMotionStatus MoveMotion(IMotion axis, double position)
        {
            EMotionStatus bRet = EMotionStatus.None;

            if (axis == null) return EMotionStatus.None;
            if (axis.Status.IsMotionOn == false) return EMotionStatus.PowerOff;

            axis.MoveAbs(position);
            var start = DateTime.Now;

            while (true)
            {
                if ((DateTime.Now - start).TotalSeconds > 60)
                {
                    bRet = EMotionStatus.MoveTimeOut;
                }
                if (axis.IsOnPosition(0))
                {
                    bRet = EMotionStatus.MotionDone;
                    break;
                }
                Thread.Sleep(10);
            }

            return bRet;
        }
    }
}
