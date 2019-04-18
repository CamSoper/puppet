using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Puppet.Common.Configuration;
using System;
using System.Threading.Tasks;

namespace Puppet.Executive.Mqtt
{
    public class MqttClientFactory
    {
        public static async Task<IManagedMqttClient> GetClient(MqttOptions mqttOptions)
        {


            var clientConnectionOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(mqttOptions.ClientId)
                .WithTcpServer(mqttOptions.BrokerHostNameOrIp, mqttOptions.Port);

            if (!String.IsNullOrEmpty(mqttOptions.UserName) &&
                !String.IsNullOrEmpty(mqttOptions.Password))
            {
                clientConnectionOptionsBuilder = clientConnectionOptionsBuilder
                    .WithCredentials(mqttOptions.UserName, mqttOptions.Password);
            }

            if (mqttOptions.EnableTls)
            {
                clientConnectionOptionsBuilder = clientConnectionOptionsBuilder
                    .WithTls();
            }

            var managedClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(clientConnectionOptionsBuilder.Build())
            .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.StartAsync(managedClientOptions);

            return mqttClient;
        }
    }

}
