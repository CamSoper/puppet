using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Events
{
    public class HubEvent
    {
        public string deviceId { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string descriptionText { get; set; }
        public string source { get; set; }
    }
}
