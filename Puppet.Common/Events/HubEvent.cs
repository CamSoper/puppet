using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Puppet.Common.Events
{
    public class HubEvent
    {
        [JsonProperty(PropertyName = "deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "descriptionText")]
        public string DescriptionText { get; set; }
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        public bool IsOpenEvent()
        {
            if (Value == "open") return true;
            return false;
        }

        public bool IsClosedEvent()
        {
            if (Value == "closed") return true;
            return false;
        }

        public bool IsLockedEvent()
        {
            if (Value == "locked") return true;
            return false;
        }

        public bool IsUnLockedEvent()
        {
            if (Value == "unlocked") return true;
            return false;
        }
    }
}
