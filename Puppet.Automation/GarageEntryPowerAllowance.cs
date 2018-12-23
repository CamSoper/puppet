using System;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    public class GarageEntryPowerAllowance : PowerAllowanceBase
    {
        public GarageEntryPowerAllowance(HomeAutomationPlatform hub) : base(hub)
        {
            this.Minutes = 30;
        }
    }
}
