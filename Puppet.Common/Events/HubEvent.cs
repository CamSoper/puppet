using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Events
{
    public class HubEvent
    {
        public string deviceId { get; set; }
        public string value { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }
        public string source { get; set; }
    }
}
