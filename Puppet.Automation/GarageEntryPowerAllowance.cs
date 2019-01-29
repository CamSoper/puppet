using System;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice(DeviceMap.SwitchRelay.GarageEntry, Capability.Switch)]
    public class GarageEntryPowerAllowance : PowerAllowanceBase
    {
        public GarageEntryPowerAllowance(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            this.Minutes = 30;
        }
    }
}
