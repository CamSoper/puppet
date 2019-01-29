using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    public abstract class AutomationBase : IAutomation
    {
        public AutomationBase(HomeAutomationPlatform hub, HubEvent evt)
        {

        }
        public abstract void Handle(CancellationToken token);
    }
}
