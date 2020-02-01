using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Lighting
{
    [TriggerDevice("Motion.Basement", Capability.Motion)]
    public class Basement : TriggeredLightingAutomationBase
    {

        public Basement(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            DeactivationWait = TimeSpan.FromMinutes(20);
            EnableDeactivation = true;
        }

        protected override async Task InitDevices()
        {
            SwitchesToActivate =
            new List<SwitchRelay>()
            {
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.ServerRoomLamp"),
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.BasementLamp")
            };
        }
    }
}
