using System;
using System.Collections.Generic;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [RunPerDevice]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    public class NotifyOnGarageDoorOpen : DoorWatcherBase
    {
        public NotifyOnGarageDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            NotificationDevices =
                new List<Speaker>() {
                    _hub.GetDeviceByName<Speaker>("Speaker.WebhookNotifier") as Speaker,
                    _hub.GetDeviceByName<Speaker>("Speaker.KitchenSpeaker") as Speaker
                };

            HowLong = TimeSpan.FromMinutes(5);
            NumberOfNotifications = 2;
        }
    }
}
