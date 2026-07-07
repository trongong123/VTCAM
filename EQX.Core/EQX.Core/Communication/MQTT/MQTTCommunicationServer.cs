using EQX.Core.Common;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Communication.MQTT
{
    public class MQTTCommunicationServer : IHandleConnection
    {
        private MqttServer _server;
        private IMqttClient _client;
        private readonly int _port;

        public MQTTCommunicationServer(int port)
        {
            _port = port;
            StartBroker();
        }
        public bool IsConnected => _client != null && _client.IsConnected;

        private void StartBroker()
        {
            var factory = new MqttFactory();
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(_port)
                .Build();
            _server = factory.CreateMqttServer(options);
            _server.StartAsync();
        }
        public bool Connect()
        {
            var factory = new MqttFactory();
            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", _port)
                .Build();
            _client = factory.CreateMqttClient();
            _client.ConnectAsync(clientOptions);

            return _client.IsConnected;
        }

        public bool Disconnect()
        {
            _client.DisconnectAsync();
            return _client.IsConnected == false;
        }

        private void PublishMessage(string topic, string payload)
        {
            if (IsConnected == false)
            {
                Connect();
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();
            _client.PublishAsync(message);
        }
    }
}
