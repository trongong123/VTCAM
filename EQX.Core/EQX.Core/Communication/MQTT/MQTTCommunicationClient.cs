using EQX.Core.Common;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EQX.Core.Communication.MQTT
{
    public class MQTTCommunicationClient : IHandleConnection
    {
        private readonly string _brokerIp;
        private readonly int _port;
        private IMqttClient _client;

        public MQTTCommunicationClient(string brokerIp, int port)
        {
            _brokerIp = brokerIp;
            _port = port;
        }

        public bool IsConnected => _client != null && _client.IsConnected;

        public bool Connect()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _client.ApplicationMessageReceivedAsync += e =>
            {
                var seg = e.ApplicationMessage.PayloadSegment;

                string payload = seg.Array != null
                    ? Encoding.UTF8.GetString(seg.Array, seg.Offset, seg.Count)
                    : string.Empty;

                // TODO: xử lý payload ở đây (UI, log...)
                // Ví dụ:
                // Messages.Insert(0, payload);

                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(_brokerIp, _port)
                    .Build();

            _client.ConnectAsync(options);

            return _client.IsConnected;
        }

        public bool Disconnect()
        {
            _client.DisconnectAsync();
            return _client.IsConnected == false;
        }

        public void Subscribe(string topic)
        {
            if (_client == null || !_client.IsConnected) return;

            _client.SubscribeAsync(topic);
        }
    }
}
