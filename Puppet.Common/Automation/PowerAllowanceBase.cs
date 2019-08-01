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

        protected override async Task Handle()
        {
            if (_evt.IsOnEvent)
            {
                await WaitForCancellationAsync(HowLong);
                SwitchRelay relay = await _hub.GetDeviceById<SwitchRelay>(_evt.DeviceId);
                await relay.Off();
            }
        }
    }
}
