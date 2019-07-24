using System;
using Puppet.Common.Automation;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    public class PatioLightPowerAllowance : PowerAllowanceBase
    {
        public PatioLightPowerAllowance(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(30);
        }
    }
}