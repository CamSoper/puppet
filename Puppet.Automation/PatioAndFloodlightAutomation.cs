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
        {
            _floodlights = 
                _hub.GetDeviceByMappedName<SwitchRelay>("Switch.Floodlights") as SwitchRelay;
        }

        protected override Task Handle()
        {
            if(_evt.IsOnEvent)
            {
                _floodlights.On();
            }
            else
            {
                _floodlights.Off();
            }
            return Task.CompletedTask;
        }
    }
}