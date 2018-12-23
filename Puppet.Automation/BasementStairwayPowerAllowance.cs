using Puppet.Common.Services;

namespace Puppet.Automation
{
    public class BasementStairwayPowerAllowance : PowerAllowanceBase
    {
        public BasementStairwayPowerAllowance(HomeAutomationPlatform hub) : base(hub)
        {
            this.Minutes = 5;
        }
    }
}
