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
    [TriggerDevice("Contact.GarageEntryDoor", Capability.Contact)]
    public class GarageEntry : TriggeredLightingAutomationBase
    {
        public GarageEntry(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task InitDevices()
        {
            if (await IsDark(30, -30))
            {
                SwitchesToActivate =
                    new List<SwitchRelay>()
                    {
                        await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.GarageEntry"),
                    };
            }
            else
            {
                SwitchesToActivate = new List<SwitchRelay>();
            }
        }
    }
}
