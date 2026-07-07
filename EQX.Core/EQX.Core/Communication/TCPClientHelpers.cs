using System.Net;
using SuperSimpleTcp;

namespace EQX.Core.Communication
{
    public static class TCPClientHelpers
    {
        /// <summary>
        /// key : endPoint format "xxx.xxx.xxx.xxx:port"
        /// </summary>

        #region Method(s)
        public static void AddClient(IPAddress iPAddress, uint port)
        {
            lock (_lock)
            {
                if (clientDictionary == null) clientDictionary = new Dictionary<string, SimpleTcpClient>();
                if (iPAddress == null || port == 0) throw new Exception("Parameter format not match");

                string endPoint = string.Format($"{iPAddress}:{port}");

                if (clientDictionary.ContainsKey(endPoint)) return;

                clientDictionary.Add(endPoint, new SimpleTcpClient(new IPEndPoint(iPAddress, (int)port)));
            }
        }

        public static SimpleTcpClient? GetClient(IPAddress iPAddress, uint port)
        {
            if (clientDictionary == null) return null;
            if (iPAddress == null || port == 0) throw new Exception("Parameter format not match");

            string endPoint = string.Format($"{iPAddress}:{port}");

            if (clientDictionary.ContainsKey(endPoint) == false) return null;

            clientDictionary.TryGetValue(endPoint, out SimpleTcpClient? client);

            return client;
        }

        public static void DisposeClient(IPAddress iPAddress, uint port)
        {
            lock (_lock)
            {
                if (clientDictionary == null) return;
                if (iPAddress == null || port == 0) return;

                string endPoint = string.Format($"{iPAddress}:{port}");

                if (clientDictionary.ContainsKey(endPoint) == false) return;

                clientDictionary.TryGetValue(endPoint, out SimpleTcpClient? client);
                client?.Disconnect();

                clientDictionary.Remove(endPoint);
            }
        }
        #endregion

        #region Private(s)
        private static Dictionary<string, SimpleTcpClient> clientDictionary;
        private static object _lock = new object();
        #endregion
    }
}
