using System;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Switch.PatioLight", Capability.Switch)]
    public class PatioLightPowerAllowance : PowerAllowanceBase
    {
        public PatioLightPowerAllowance(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(30);
        }
        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}