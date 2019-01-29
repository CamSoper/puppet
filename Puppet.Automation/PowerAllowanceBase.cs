using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    public abstract class PowerAllowanceBase : AutomationBase
    {
        public int Minutes { get; set; }

        HomeAutomationPlatform _hub;
        HubEvent _evt;
        
        public PowerAllowanceBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _hub = hub;
            _evt = evt;
        }

        public override void Handle(CancellationToken token)
        {
            if (_evt.value == "on")
            {
                Task.Delay(TimeSpan.FromMinutes(this.Minutes)).Wait();
                if (token.IsCancellationRequested)
                {
                    Console.Write($"{DateTime.Now} Instance of {this.GetType()} resumed after {this.Minutes} minutes, but task was cancelled.");
                    return;
                }
                SwitchRelay relay = new SwitchRelay(_hub, _evt.deviceId);
                relay.Off();
            }
        }
    }
}
