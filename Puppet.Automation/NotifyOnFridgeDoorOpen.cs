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
    [TriggerDevice("Contact.GarageFreezer", Capability.Contact)]
    [TriggerDevice("Contact.GarageFridge", Capability.Contact)]
    [TriggerDevice("Contact.KitchenFridge", Capability.Contact)]
    [TriggerDevice("Contact.BasementFreezer", Capability.Contact)]
    public class NotifyOnFridgeDoorOpen : DoorWatcherBase
    {
        public NotifyOnFridgeDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(1);
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
