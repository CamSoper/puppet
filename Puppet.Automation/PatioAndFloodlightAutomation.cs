using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Switch.PatioLight", Capability.Switch)]
    public class PatioAndFloodlightAutomation : AutomationBase
    {
        SwitchRelay _floodlights;
        public PatioAndFloodlightAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        private async Task InitDevices()
        {
            _floodlights =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.Floodlights");
        }

        protected override async Task Handle()
        {
            await InitDevices();

            if(_evt.IsOnEvent)
            {
                _floodlights.On();
            }
            else
            {
                _floodlights.Off();
            }
        }
    }
}