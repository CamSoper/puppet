using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    public abstract class TriggeredLightingAutomationBase : AutomationBase
    {
        public TimeSpan DeactivationWait { get; set; }
        public bool EnableDeactivation { get; set; }
        public List<SwitchRelay> SwitchesToActivate { get; set; }
        public TriggeredLightingAutomationBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        public async override Task Handle()
        {
            if (!(_evt.IsOpenEvent || _evt.IsOnEvent))
                return;

            foreach(var s in SwitchesToActivate)
            {
                s.On();
            }
            if(EnableDeactivation)
            {
                await WaitForCancellation(DeactivationWait);
                foreach(var s in SwitchesToActivate)
                {
                    s.Off();
                }
            }
        }
    }
}
