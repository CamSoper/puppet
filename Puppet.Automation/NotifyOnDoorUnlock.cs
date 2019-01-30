using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System.Threading;

namespace Puppet.Automation
{
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    public class NotifyOnDoorUnlock : AutomationBase
    {
        public NotifyOnDoorUnlock(HomeAutomationPlatform hub, HubEvent evt) : base (hub,evt)
        {
        }
        
        public override void Handle(CancellationToken token)
        {
            if(_evt.value == "unlocked")
            {
                if(_evt.descriptionText.Contains("was unlocked by"))
                {
                    Speaker speaker = _hub.GetDeviceByName<Speaker>("Speaker.KitchenSpeaker") as Speaker;
                    speaker.Speak($"{_evt.descriptionText}.");
                }
            }
        }
    }
}
