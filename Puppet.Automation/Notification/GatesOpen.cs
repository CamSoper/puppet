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
    [TriggerDevice("Contact.EastGate", Capability.Contact)]
    [TriggerDevice("Contact.SouthGate", Capability.Contact)]
    [TriggerDevice("Contact.WestGate", Capability.Contact)]
    public class GatesOpen : DoorWatcherBase
    {
        public GatesOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.Zero;
            NotifyOnClose = true;
            NotificationFormat = @"{0} is open.";
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
