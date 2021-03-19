using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Puppet.Automation.Services.Notifiers;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Notification
{
    [RunPerDevice]
    [TriggerDevice("Garage.Door1Opener", Capability.Contact)]
    [TriggerDevice("Garage.Door2Opener", Capability.Contact)]
    public class GarageDoorOpen : DoorWatcherBase
    {
        public GarageDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(5);
            NumberOfNotifications = 2;
            MakeAnnouncement = true;
            PushNotification = true;

            AnnouncementNotifier = new HassAlexaNotifier(_hub.Configuration, new string[] { "Shared_Spaces" });
            PushNotifier = new HassAppNotifier(_hub.Configuration);
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}
