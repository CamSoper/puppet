using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    /// <summary>
    /// IAutomation handler for turning off switch devices
    /// after a certain number of minutes.
    /// </summary>
    public abstract class PowerAllowanceBase : AutomationBase
    {
        public TimeSpan HowLong { get; set; }

        public PowerAllowanceBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {

        }

        public override async Task Handle(CancellationToken token)
        {
            if (_evt.Value == "on")
            {
                await Task.Delay(HowLong, token);
                SwitchRelay relay = _hub.GetDeviceById<SwitchRelay>(_evt.DeviceId) as SwitchRelay;
                relay.Off();
            }
        }
    }
}
