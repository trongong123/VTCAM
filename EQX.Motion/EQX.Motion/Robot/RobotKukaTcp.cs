using EQX.Core.Communication;
using log4net;
using System.Net;

namespace EQX.Motion.Robot
{
    public class RobotKukaTcp : RobotBase
    {
        public override bool IsConnected => communicator.IsConnected;

        #region Constructor(s)
        public RobotKukaTcp(int id, string name, string ipAddress = "192.168.1.100")
            : base(id, name)
        {
            communicator = new TCPEventCommunicator(Id, Name, IPAddress.Parse(ipAddress), 54600);
            _log = LogManager.GetLogger(Name);
            _ipAddress = ipAddress;
            messageBuffer = new Queue<string>();

            communicator.OnMessageResponsed += Communicator_OnMessageResponsed;
        }
        #endregion

        #region Public methods
        public override bool Connect()
        {
            return communicator.Connect();
        }

        public override bool Disconnect()
        {
            return communicator.Disconnect();
        }

        public override bool SendCommand(string command)
        {
            clearResponseData();

            return communicator.SendData(command);
        }

        public override string ReadResponse(int timeoutMs = 5000)
        {
            string response = communicator.ReadTo("\r\n", timeoutMs);
            _log.Debug($"{Name} ReadResponse: {response.Replace("\r\n", "\\r\\n")}");
            return response;
        }

        public override bool ReadResponse(string expectedResponse)
        {
            if (messageBuffer.Count <= 0) return false;

            ResponseMessage = messageBuffer.Dequeue();

            return ResponseMessage == expectedResponse;
        }

        public override string ReadResponse()
        {
            if (messageBuffer.Count <= 0) return string.Empty;

            ResponseMessage = messageBuffer.Dequeue();

            return ResponseMessage;
        }
        #endregion

        #region Privates
        private readonly TCPEventCommunicator communicator;
        private readonly ILog _log;
        private string _ipAddress;

        private void Communicator_OnMessageResponsed(object? sender, string response)
        {
            currentMessage += response;
            if (currentMessage.EndsWith("\r\n"))
            {
                messageBuffer.Enqueue(currentMessage);
                currentMessage = string.Empty;
            }

            //OnRobotResponseHandler(response);
        }

        private void clearResponseData()
        {
            messageBuffer.Clear();
            ResponseMessage = string.Empty;
        }

        private Queue<string> messageBuffer;
        private string currentMessage = string.Empty;
        #endregion
    }
}
