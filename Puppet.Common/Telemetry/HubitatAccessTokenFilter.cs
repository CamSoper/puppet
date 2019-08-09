using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Puppet.Common.Telemetry
{
    public class HubitatAccessTokenDependencyFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }
        private const string ACCESSTOKENKEY = "?access_token=";

        // Link processors to each other in a chain.
        public HubitatAccessTokenDependencyFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }
        public void Process(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;
            if(dependency != null && dependency.Data.Contains(ACCESSTOKENKEY))
            {
                var data = dependency.Data;
                dependency.Data = data.Substring(0, data.IndexOf(ACCESSTOKENKEY));
            }
            this.Next.Process(item);
        }
    }
}
