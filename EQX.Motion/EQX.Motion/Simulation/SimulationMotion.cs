using EQX.Core.Motion;

namespace EQX.Motion
{
    public class SimulationMotion : MotionBase
    {
        public SimulationMotion(int id, string name, IMotionParameter parameter)
            : base(id, name, parameter)
        {
            random = new Random();
            flag_StopSimulationMove = false;
        }

        protected override bool ActualConnect()
        {
            IsConnected = true;
            return true;
        }

        protected override bool ActualDisconnect()
        {
            IsConnected = false;
            return true;
        }

        protected override bool ActualMotionOff()
        {
            ((MotionStatus)Status).IsMotionOn = false;
            return ((MotionStatus)Status).IsMotionOn;
        }

        protected override bool ActualMotionOn()
        {
            ((MotionStatus)Status).IsMotionOn = true;
            return ((MotionStatus)Status).IsMotionOn;
        }

        protected override bool ActualMoveAbs(double position, double speed)
        {
            if (((MotionStatus)Status).IsMotionOn == false) return false;

            Simuation_MoveTo(position, speed);
            return true;
        }

        protected override bool ActualMoveInc(double position, double speed)
        {
            if (((MotionStatus)Status).IsMotionOn == false) return false;

            Simuation_MoveTo(((MotionStatus)Status).ActualPosition + position, speed);

            return true;
        }

        protected override void ActualMoveJog(double speed, bool isForward)
        {
            if (((MotionStatus)Status).IsMotionOn == false) return;

            int direct = isForward ? 1 : -1;

            ((MotionStatus)Status).ActualPosition += direct * random.Next((int)speed) / 60.0;
        }

        protected override bool ActualSearchOrigin()
        {
            if (((MotionStatus)Status).IsMotionOn == false) return false;

            Simuation_MoveTo(0, 100, true);

            return true;
        }

        public override bool Stop(bool forceStop = true)
        {
            flag_StopSimulationMove = true;
            return true;
        }

        public override bool AlarmReset()
        {
            ((MotionStatus)Status).IsAlarm = false;
            return true;
        }

        public override bool ClearPosition()
        {
            ((MotionStatus)Status).ActualPosition = 0.0;
            return true;
        }

        private void Simuation_MoveTo(double dTargetPos, bool IsHomeSearch = false)
        {
            Simuation_MoveTo(dTargetPos, Parameter.Velocity, IsHomeSearch);
        }

        private void Simuation_MoveTo(double dTargetPos, double dVelocity, bool IsHomeSearch = false)
        {
            if (dVelocity <= 0) dVelocity = 1;

            flag_StopSimulationMove = false;
            Task task = new Task(async () =>
            {
                const int updateFrequency = 10;  // Milisecond

                double distance = dTargetPos - Status.ActualPosition;
                double timeCost = Math.Abs(distance) / dVelocity;

                int numberOfSteps = (int)Math.Floor(timeCost * 1000 / updateFrequency);

                double moveStep = Math.Truncate((distance / numberOfSteps) * 1000) / 1000;

                for (int i = 0; i < numberOfSteps; i++)
                {
                    if (flag_StopSimulationMove == true)
                    {
                        return;
                    }

                    await Task.Delay(updateFrequency);
                    ((MotionStatus)Status).ActualPosition += moveStep;
                }

                if (IsHomeSearch)
                {
                    ((MotionStatus)Status).IsHomeDone = true;
                }

                await Task.Delay(Math.Max((int)(timeCost - updateFrequency * numberOfSteps), 0));  // Sleep remain time
                ((MotionStatus)Status).ActualPosition = dTargetPos;
            });

            task.Start();
        }

        #region Privates
        private Random random;
        private bool flag_StopSimulationMove;
        #endregion
    }
}
