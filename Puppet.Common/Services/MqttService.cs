using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Puppet.Common.Configuration;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Exceptions;

namespace Puppet.Common.Services
{
    public class MqttService : IMqttService
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly HomeAutomationPlatform _hub;
        private readonly string _topicRoot;
        private readonly string _commandTopic;

        public MqttService(IManagedMqttClient mqttClient, MqttOptions mqttOptions, HomeAutomationPlatform hub)
        {
            _mqttClient = mqttClient;
            _hub = hub;
            _topicRoot = mqttOptions.TopicRoot;
            _commandTopic = $"{mqttOptions.TopicRoot}/command";
            _mqttClient.ApplicationMessageReceived += OnMqttMessageReceived;
        }

        private void OnMqttMessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} Received MQTT message. Topic: {e.ApplicationMessage.Topic} Payload: {e.ApplicationMessage.ConvertPayloadToString()}");
            try
            {
                string parm = null;
                string[] tokens = e.ApplicationMessage.Topic.Split('/');
                string payload = e.ApplicationMessage.ConvertPayloadToString();

                if (tokens.Length != 4)
                {
                    throw new InvalidMqttTopicException();
                }

                GenericDevice device = _hub.GetDeviceByLabel<GenericDevice>(tokens[2]) as GenericDevice;
                if (!string.IsNullOrEmpty(payload))
                {
                    parm = payload;
                }

                device.DoAction(tokens[3], parm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} {ex}");
            }
        }

        public async Task SendEventToMqttAsync(HubEvent evt)
        {
            await _mqttClient.PublishAsync($"{_topicRoot}/event/{evt.DisplayName}/{evt.Name}", evt.Value);
        }

        public async Task Start()
        {
            await _mqttClient.SubscribeAsync(
                new TopicFilterBuilder()
                .WithTopic($"{_commandTopic}/#")
                .Build());
        }

    }
}
