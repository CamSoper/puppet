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
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    public class Foyer : TriggeredLightingAutomationBase
    {
        public Foyer(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            DeactivationWait = TimeSpan.FromMinutes(5);
            EnableDeactivation = true;
        }

        protected override async Task InitDevices()
        {
            SwitchesToActivate =
            new List<SwitchRelay>()
            {
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FoyerLight")
            };
        }
    }
}
