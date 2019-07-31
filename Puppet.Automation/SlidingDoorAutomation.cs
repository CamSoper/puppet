using System;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.SlidingDoor", Capability.Contact)]
    public class SlidingDoorAutomation : TriggeredLightingAutomationBase
    {
        public SlidingDoorAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            if (IsDark(30, -30))
            {
                SwitchesToActivate.Add(_hub.GetDeviceByMappedName<SwitchRelay>("Switch.PatioLight"));
            }
            EnableDeactivation = true;
            DeactivationWait = TimeSpan.FromMinutes(30);
        }
    }
}