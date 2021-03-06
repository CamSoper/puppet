using Newtonsoft.Json;
using Puppet.Common.Events;

namespace Puppet.Api.Models
{
    public class HubitatNotification
    {
        [JsonProperty(PropertyName = "content")]
        public HubEvent Content { get; set; }
    }
}
