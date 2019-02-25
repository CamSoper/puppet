using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Puppet.Common.Models
{
    public class HubitatDevice
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
    }
}
