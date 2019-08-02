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
    [TriggerDevice("Contact.GarageEntryDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1Opener", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2Opener", Capability.Contact)]
    public class GarageLighting : TriggeredLightingAutomationBase
    {
        public GarageLighting(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            DeactivationWait = TimeSpan.FromMinutes(30);
            EnableDeactivation = true;
        }

        protected override async Task InitDevices()
        {
            SwitchesToActivate.Add(
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.GarageLights"));
        }
    }
}
