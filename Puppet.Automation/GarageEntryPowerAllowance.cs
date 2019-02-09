using System;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Switch.GarageEntry", Capability.Switch)]
    public class GarageEntryPowerAllowance : PowerAllowanceBase
    {
        public GarageEntryPowerAllowance(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(30);
        }
    }
}
