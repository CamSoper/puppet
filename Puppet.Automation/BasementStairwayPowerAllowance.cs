using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Models.Automation;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    public class BasementStairwayPowerAllowance : IAutomation
    {
        HomeAutomationPlatform _hub;

        public BasementStairwayPowerAllowance(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }
        public void Handle(HubEvent evt, CancellationToken token)
        {
            if(evt.value == "on")
            {
                Task.Delay(TimeSpan.FromMinutes(5)).Wait();
                if (token.IsCancellationRequested) return;
                SwitchRelay relay = new SwitchRelay(_hub, DeviceMap.SwitchRelay.BasementStairwayLight);
                relay.Off();
            }

        }
    }
}
