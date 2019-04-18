using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.BasementDoor", Capability.Contact)]
    public class BasementStairLighting : TriggeredLightingAutomationBase
    {
        public BasementStairLighting(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            SwitchesToActivate =
                new List<SwitchRelay>()
                {
                    _hub.GetDeviceByMappedName<SwitchRelay>("Switch.BasementStairwayLight") as SwitchRelay
                };
        }
    }
}
