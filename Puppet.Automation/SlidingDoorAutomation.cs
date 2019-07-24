using System;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.SlidingDoor", Capability.Contact)]
    public class SlidingDoorAutomation : AutomationBase
    {
        SwitchRelay _patio;
        public SlidingDoorAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _patio = 
                _hub.GetDeviceByMappedName<SwitchRelay>("Switch.PatioLight") as SwitchRelay;
        }

        protected override async Task Handle()
        {
            if(_evt.IsOpenEvent && IsDark(30, -30))
            {
                _patio.On();
            }
        }
    }
}