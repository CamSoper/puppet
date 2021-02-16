using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.PowerAllowance
{
    [RunPerDevice]
    [TriggerDevice("Switch.ServerRoomLamp", Capability.Switch)]
    [TriggerDevice("Switch.BasementLamp", Capability.Switch)]
    public class Basement : PowerAllowanceBase
    {
        public Basement(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(60);
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}
