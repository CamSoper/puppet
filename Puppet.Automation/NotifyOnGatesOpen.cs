using System;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [RunPerDevice]
    [TriggerDevice("Contact.EastGate", Capability.Contact)]
    [TriggerDevice("Contact.SouthGate", Capability.Contact)]
    [TriggerDevice("Contact.WestGate", Capability.Contact)]
    public class NotifyOnGatesOpen : DoorWatcherBase
    {
        public NotifyOnGatesOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.Zero;
            NotifyOnClose = true;
            NotificationDevices =
                new List<Speaker>() {
                    _hub.GetDeviceByName<Speaker>("Speaker.WebhookNotifier") as Speaker,
                    _hub.GetDeviceByName<Speaker>("Speaker.KitchenSpeaker") as Speaker
                };
            NotificationFormat = @"{0} is open.";
        }
    }
}
