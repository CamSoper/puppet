using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Lighting
{
    [TriggerDevice("Switch.PatioLight", Capability.Switch)]
    public class PatioAndFloodlight : AutomationBase
    {
        SwitchRelay _floodlights;
        public PatioAndFloodlight(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        protected override async Task InitDevices()
        {
            _floodlights =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.Floodlights");
        }

        protected override async Task Handle()
        {
            if (_evt.IsOnEvent)
            {
                await _floodlights.On();
            }
            else
            {
                await _floodlights.Off();
            }
        }
    }
}