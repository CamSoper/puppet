using System;
using Newtonsoft.Json;

namespace Puppet.Common.Models
{
    public class SunriseAndSunset
    {
        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }
        
        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }
    }
}