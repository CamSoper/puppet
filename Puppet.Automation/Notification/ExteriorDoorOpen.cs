using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Puppet.Automation.Services.Notifiers;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Notification
{
    /// <summary>
    /// Runs when an exterior door is left open for a specific amount of time
    /// </summary>
    [RunPerDevice]
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Contact.SlidingDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageEntry", Capability.Contact)]
    class ExteriorDoorOpen : DoorWatcherBase
    {
        public ExteriorDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(2);
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
