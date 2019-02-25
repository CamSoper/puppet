using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Configuration
{
    public class MqttOptions
    {
        public bool Enabled { get; set; }
        public string ClientId { get; set; }
        public string BrokerHostNameOrIp { get; set; }
        public int Port { get; set; }
        public bool EnableTls { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TopicRoot { get; set; }
    }
}
