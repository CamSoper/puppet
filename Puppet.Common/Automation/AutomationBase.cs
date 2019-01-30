using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    public abstract class AutomationBase : IAutomation
    {
        protected HomeAutomationPlatform _hub;
        protected HubEvent _evt;
        public AutomationBase(HomeAutomationPlatform hub, HubEvent evt)
        {
            _hub = hub;
            _evt = evt;
        }
        public abstract void Handle(CancellationToken token);
    }
}
