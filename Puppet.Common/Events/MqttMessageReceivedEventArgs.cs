using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Events
{
    public class MqttMessageReceivedEventArgs
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}
