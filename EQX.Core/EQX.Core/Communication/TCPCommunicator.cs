using System.Net.Sockets;
using System.Net;
using System.Text;
using log4net;
using EQX.Core.Common;

namespace EQX.Core.Communication
{
    public class TCPBasicCommunicator : IHandleConnection, IIdentifier
    {
        #region Constructor(s)
        public TCPBasicCommunicator(int index, string name, IPAddress iPAddress, int port)
        {
            Id = index;
            Name = name;
            IPAddress = iPAddress;
            Port = port;

            tcpClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _log = LogManager.GetLogger(Name);
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
                return tcpClient.Connected;
            }
        }
        public IPAddress IPAddress { get; protected set; }
        public int Port { get; protected set; }
        #endregion

        #region Method(s)
        public bool Connect()
        {
            if (tcpClient.Connected)
            {
                CloseSocket();
            }
            
            IAsyncResult result = tcpClient.BeginConnect(IPAddress, Port, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(2000, true);

            if (tcpClient.Connected)
            {
                tcpClient.EndConnect(result);
                return true;
            }
            else
            {
                CloseSocket();

                _log.Error($"Failed to connect device {Name}.");
                return false;
            }
        }

        private void CloseSocket()
        {
            // Close current connection, then initialize object for next try
            tcpClient.Close();
            tcpClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Disconnect()
        {
            CloseSocket();

            return !tcpClient.Connected;
        }

        public bool SendData(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            return SendData(buffer, buffer.Length);
        }

        public bool SendData(byte[] buffer, int size)
        {
            if (tcpClient.Connected == false) return false;

            int sendByte = tcpClient.Send(buffer, size, SocketFlags.None);

            return sendByte == size;
        }

        public string ReadTo(string endOfData, int timeoutMs = 5000)
        {
            if (tcpClient.Connected == false) return string.Empty;

            int startMs = Environment.TickCount;

            string data = string.Empty;
            byte[] bytes;

            while (true)
            {
                bytes = new byte[1024];
                if (tcpClient.Available > 0)
                {
                    int bytesRec = tcpClient.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf(endOfData) > -1)
                    {
                        return data;
                    }

                    if (Environment.TickCount - startMs > timeoutMs)
                    {
                        return string.Empty;
                    }

                    Thread.Sleep(2);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int ReadData(ref byte[] buffer)
        {
            if (tcpClient.Connected == false) return -1;

            return tcpClient.Receive(buffer);
        }
        #endregion

        #region Private(s)
        protected Socket tcpClient;
        private readonly ILog _log;
        #endregion
    }
}
