using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Models.Automation;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Puppet.Automation
{
    public class NotifyOnDoorUnlock : IAutomation
    {
        HomeAutomationPlatform _hub;

        public NotifyOnDoorUnlock(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }
        
        public void Handle(HubEvent evt, CancellationToken token)
        {
            if(evt.value == "unlocked")
            {
                if(evt.description.Contains("was unlocked by"))
                {
                    var speaker = new Speaker(_hub, DeviceMap.Speaker.WebhookNotifier);
                    speaker.Speak($"{evt.description}.");
                }
            }
        }
    }
}
