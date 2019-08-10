using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Puppet.Common.Configuration;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Exceptions;
using Puppet.Common.Telemetry;

namespace Puppet.Common.Services
{
    public class MqttService : IMqttService
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly HomeAutomationPlatform _hub;
        private readonly TelemetryClient _telemetryClient;
        private readonly string _topicRoot;
        private readonly string _commandTopic;

        public MqttService(IManagedMqttClient mqttClient, MqttOptions mqttOptions, HomeAutomationPlatform hub)
        {
            _telemetryClient = hub.TelemetryClient;
            _mqttClient = mqttClient;
            _hub = hub;
            _topicRoot = mqttOptions.TopicRoot;
            _commandTopic = $"{mqttOptions.TopicRoot}/command";
            _mqttClient.ApplicationMessageReceived += OnMqttMessageReceived;
        }

        private async void OnMqttMessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} Received MQTT message. Topic: {e.ApplicationMessage.Topic} Payload: {e.ApplicationMessage.ConvertPayloadToString()}");
            using (var operation =
                _telemetryClient.StartOperation<RequestTelemetry>($"{this.ToString()}: Message Received"))
            {
                _telemetryClient.TrackEvent("MQTT Message Received",
                    new Dictionary<string, string>()
                    {
                        {"Topic", e.ApplicationMessage.Topic },
                        {"Payload", e.ApplicationMessage.ConvertPayloadToString()},
                    });
                try
                {
                    string parm = null;
                    string[] tokens = e.ApplicationMessage.Topic.Split('/');
                    string payload = e.ApplicationMessage.ConvertPayloadToString();

                    if (tokens.Length != 4)
                    {
                        throw new InvalidMqttTopicException();
                    }

                    GenericDevice device = await _hub.GetDeviceByLabel<GenericDevice>(tokens[2]);
                    if (!string.IsNullOrEmpty(payload))
                    {
                        parm = payload;
                    }

                    await device.DoAction(tokens[3], parm);
                }
                catch (Exception ex)
                {
                    operation.Telemetry.Success = false;
                    _telemetryClient.TrackException(ex);
                    Console.WriteLine($"{DateTime.Now} {ex}");
                }
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
