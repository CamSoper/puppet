using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            HowLong = TimeSpan.FromMinutes(5);
            NumberOfNotifications = 2;
        }

        protected override async Task InitDevices()
        {
            NotificationDevices =
                new List<Speaker>() {
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier"),
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker")
                };
        }
    }
}
