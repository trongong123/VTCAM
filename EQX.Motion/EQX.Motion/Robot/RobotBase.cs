using EQX.Core.Robot;

namespace EQX.Motion.Robot
{
    public class RobotBase : IRobot
    {
        public event MessageResponseHandler? OnRobotResponsed;
        public string ResponseMessage { get; protected set; }

        public int Id { get; init; }
        public string Name { get; init; }

        public virtual bool IsConnected => isConnected;

        public RobotBase(int id, string name)
        {
            Id = id;
            Name = name;

            isConnected = false;
        }

        protected void OnRobotResponseHandler(string message)
        {
            OnRobotResponsed?.Invoke(this, message);
        }

        public virtual bool Connect()
        {
            isConnected = true;

            return true;
        }

        public virtual bool Disconnect()
        {
            isConnected = false;

            return true;
        }

        public virtual bool ReadResponse(string expectedResponse)
        {
            return true;
        }

        public virtual string ReadResponse()
        {
            return "";
        }

        public virtual string ReadResponse(int timeoutMs)
        {
            return "";
        }

        public virtual async Task<string> ReadResponseAsync(int timeoutMs)
        {
            return await Task.Run(() => { return ""; });
        }

        public virtual bool SendCommand(string command)
        {
            return true;
        }

        public virtual bool ReadResponse(int timeoutMs, string expectedResponse)
        {
            string response = ReadResponse(timeoutMs);

            if (response != expectedResponse)
            {
                return false;
            }

            return true;
        }

        #region Privates
        protected bool isConnected;
        #endregion
    }
}
