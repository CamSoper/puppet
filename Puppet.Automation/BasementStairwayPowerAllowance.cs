using System;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Switch.BasementStairwayLight", Capability.Switch)]
    public class BasementStairwayPowerAllowance : PowerAllowanceBase
    {
        public BasementStairwayPowerAllowance(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(5);
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}
