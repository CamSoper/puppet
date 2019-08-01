using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.BasementDoor", Capability.Contact)]
    public class BasementStairLighting : TriggeredLightingAutomationBase
    {
        public BasementStairLighting(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        public override async Task InitDevices()
        {
            SwitchesToActivate =
            new List<SwitchRelay>()
            {
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.BasementStairwayLight"),
            };
        }
    }
}
