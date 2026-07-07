namespace EQX.Motion.Robot
{
    public class RobotSimulation : RobotBase
    {
        public RobotSimulation(int id, string name)
            : base(id, name)
        {
        }

        public override bool ReadResponse(int timeoutMs, string expectedResponse)
        {
            return true;
        }

        public override bool ReadResponse(string expectedResponse)
        {
            return true;
        }

        #region Privates
        #endregion
    }
}
