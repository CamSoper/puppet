﻿using Newtonsoft.Json;
using Puppet.Common.Devices;
using System.Collections.Generic;

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

        public bool IsOpenEvent => Value == "open";
        public bool IsClosedEvent => Value == "closed";
        public bool IsLockedEvent => Value == "locked";
        public bool IsUnLockedEvent => Value == "unlocked";
        public bool IsOnEvent => Value == "on";
        public bool IsOffEvent => Value == "off";
        public bool IsButtonPushedEvent => Name == "pushed";
        public bool IsButtonHeldEvent => Name == "held";
        public bool IsActiveEvent => Value == "active";
        public bool IsInactiveEvent => Value == "inactive";

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>()
                {
                    {"Device ID", DeviceId },
                    {"Description", DescriptionText },
                    {"Display Name", DisplayName },
                    {"Name", Name },
                    {"Source", Source },
                    {"Value", Value },
                };
        }
    }
}
