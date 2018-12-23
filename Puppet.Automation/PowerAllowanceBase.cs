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
    public abstract class PowerAllowanceBase : IAutomation
    {
        public int Minutes { get; set; }

        HomeAutomationPlatform _hub;
        
        public PowerAllowanceBase(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }

        public void Handle(HubEvent evt, CancellationToken token)
        {
            if (evt.value == "on")
            {
                Task.Delay(TimeSpan.FromMinutes(this.Minutes)).Wait();
                if (token.IsCancellationRequested) return;
                SwitchRelay relay = new SwitchRelay(_hub, evt.deviceId);
                relay.Off();
            }

        }
    }
}
