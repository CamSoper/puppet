using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.GarageEntryDoor", Capability.Contact)]
    public class GarageEntryLighting : TriggeredLightingAutomationBase
    {
        public GarageEntryLighting(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            if (DateTime.Now.Hour <= 8 || DateTime.Now.Hour >= 17)
            {
                SwitchesToActivate =
                    new List<SwitchRelay>()
                    {
                    _hub.GetDeviceByMappedName<SwitchRelay>("Switch.GarageEntry") as SwitchRelay
                    };
            }
            else
            {
                SwitchesToActivate = new List<SwitchRelay>();
            }
        }
    }
}
