using System.Net;
using System.Text;
using log4net;
using EQX.Core.Common;
using SuperSimpleTcp;
using EQX.Core.Robot;

namespace EQX.Core.Communication
{
    public class TCPEventCommunicator : IHandleConnection, IIdentifier
    {
        public event MessageResponseHandler? OnMessageResponsed;

        #region Constructor(s)
        public TCPEventCommunicator(int index, string name, IPAddress iPAddress, int port)
        {
            Id = index;
            Name = name;
            IPAddress = iPAddress;
            Port = port;

            _log = LogManager.GetLogger(Name);
            ClientInit();
        }
        #endregion

        #region Properties
        public int Id { get; init; }
        public string Name { get; init; }
        public bool IsConnected
        {
            get
            {
                if (tcpClient == null) return false;
                return tcpClient.IsConnected;
            }
        }
        public IPAddress IPAddress { get; protected set; }
        public int Port { get; protected set; }
        #endregion


        public bool Connect()
        {
            if (tcpClient.IsConnected)
            {
                CloseSocket();
                ClientInit();
            }

            try
            {
                tcpClient.ConnectWithRetries(5000);
                _log.Info($"Connect to device {Name} success.");
                return true;
            }
            catch (TimeoutException)
            {
                _log.Error($"Failed to connect device {Name}.");
                return false;
            }
        }

        private void CloseSocket()
        {
            // Close current connection
            TCPClientHelpers.DisposeClient(IPAddress, (uint)Port);
        }

        private void ClientInit()
        {
            if (TCPClientHelpers.GetClient(IPAddress, (uint)Port) == null)
                TCPClientHelpers.AddClient(IPAddress, (uint)Port);

            tcpClient = TCPClientHelpers.GetClient(IPAddress, (uint)Port);

            tcpClient!.Events.DataSent -= Events_DataSent;
            tcpClient!.Events.Connected -= Events_Connected;
            tcpClient!.Events.Disconnected -= Events_Disconnected;
            tcpClient!.Events.DataReceived -= Events_DataReceived;

            tcpClient!.Events.DataSent += Events_DataSent;
            tcpClient!.Events.Connected += Events_Connected;
            tcpClient!.Events.Disconnected += Events_Disconnected;
            tcpClient!.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            if (e == null) return;
            if (e.Data.Array == null) return;

            string data = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
            Interlocked.Exchange(ref receivedData, data);

            OnMessageResponsed?.Invoke(this, data);
            _log.Info($"Data received from {Name}: \"{data.Replace("\r\n", "\\r\\n")}\"");
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            _log.Error($"Disconnected.");
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            _log.Info($"Connected.");
        }

        private void Events_DataSent(object? sender, DataSentEventArgs e)
        {
            _log.Info($"Total {e.BytesSent} bytes send to {e.IpPort}");
        }

        public bool Disconnect()
        {
            CloseSocket();

            return !tcpClient.IsConnected;
        }

        public bool SendData(string data)
        {
            if (tcpClient.IsConnected == false) return false;

            // Clear receivedData from server before send new Data
            Interlocked.Exchange(ref receivedData, string.Empty);

            try
            {
                _log.Info($"Sending {data.Replace("\r\n", "\\r\\n")}");
                tcpClient.Send(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendData(byte[] buffer, int size)
        {
            string data = Encoding.ASCII.GetString(buffer, 0, size);
            return SendData(data);
        }

        public string ReadTo(string endOfData, int timeoutMs = 5000)
        {
            if (tcpClient.IsConnected == false)
            {
                throw new Exception("tcpClient.IsConnected == false");
                //return string.Empty;
            }
            int startMs = Environment.TickCount;

            // Clear receivedData from server reading new Data
            Interlocked.Exchange(ref receivedData, string.Empty);
            
            while (true)
            {
                if (Environment.TickCount - startMs > timeoutMs)
                {
                    _log.Error($"ReadTo {endOfData.Replace("\r\n", "\"\\r\\n\"")} timeout {Environment.TickCount - startMs}ms");
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(receivedData))
                {
                    Thread.Sleep(2);
                    continue;
                }

                if (receivedData.EndsWith(endOfData) == false) return string.Empty;
                else return receivedData;
            }
        }

        public int ReadData(ref byte[] buffer)
        {
            int startMs = Environment.TickCount;

            while (true)
            {
                if (Environment.TickCount - startMs > 100)
                {
                    buffer = new byte[] { };
                }

                if (string.IsNullOrEmpty(receivedData) == false)
                {
                    buffer = Encoding.ASCII.GetBytes(receivedData);
                }
                else
                {
                    Thread.Sleep(2);
                    continue;
                }

            }
        }

        protected SimpleTcpClient tcpClient;

        private readonly ILog _log;
        private string receivedData = string.Empty;
    }
}
