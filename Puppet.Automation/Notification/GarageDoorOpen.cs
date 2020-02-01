using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Notification
{
    [RunPerDevice]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    public class GarageDoorOpen : DoorWatcherBase
    {
        public GarageDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(5);
            NumberOfNotifications = 2;
            MakeAnnouncement = true;
            PushNotification = true;
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}
