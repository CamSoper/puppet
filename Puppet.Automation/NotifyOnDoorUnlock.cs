using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    public class NotifyOnDoorUnlock : AutomationBase
    {
        public NotifyOnDoorUnlock(HomeAutomationPlatform hub, HubEvent evt) : base (hub,evt)
        {
        }
        
        public override Task Handle(CancellationToken token)
        {
            if(_evt.value == "unlocked")
            {
                if(_evt.descriptionText.Contains("was unlocked by"))
                {
                    Speaker speaker = _hub.GetDeviceByName<Speaker>("Speaker.KitchenSpeaker") as Speaker;
                    speaker.Speak($"{_evt.descriptionText}.");
                }
            }

            return Task.CompletedTask;
        }
    }
}
