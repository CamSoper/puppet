using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System.Threading;

namespace Puppet.Automation
{
    [TriggerDevice(DeviceMap.Lock.FrontDoorDeadbolt)]
    public class NotifyOnDoorUnlock : AutomationBase
    {
        HomeAutomationPlatform _hub;
        HubEvent _evt;

        public NotifyOnDoorUnlock(HomeAutomationPlatform hub, HubEvent evt) : base (hub,evt)
        {
            _hub = hub;
            _evt = evt;
        }
        
        public override void Handle(CancellationToken token)
        {
            if(_evt.value == "unlocked")
            {
                if(_evt.descriptionText.Contains("was unlocked by"))
                {
                    var speaker = new Speaker(_hub, DeviceMap.Speaker.WebhookNotifier);
                    speaker.Speak($"{_evt.descriptionText}.");
                }
            }
        }
    }
}
