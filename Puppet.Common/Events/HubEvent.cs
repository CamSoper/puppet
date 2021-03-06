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
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        public bool IsOpenEvent => Value == "open";
        public bool IsClosedEvent => Value == "closed";
        public bool IsLockedEvent => Value == "locked";
        public bool IsUnLockedEvent => Value == "unlocked";
        public bool IsOnEvent => Value == "on";
        public bool IsOffEvent => Value == "off";
        public bool IsButtonPushedEvent => Name == "pushed";
        public bool IsButtonHeldEvent => Name == "held";
        public bool IsButtonDoubleTappedEvent => Name == "doubleTapped";
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
                    {"Data", Data },
                    {"Unit", Unit },
                    {"Type", Type },
                    {"Value", Value },
                };
        }
    }
}
