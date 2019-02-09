using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    /// <summary>
    /// Runs when an exterior door is left open for a specific amount of time
    /// </summary>
    [RunPerDevice]
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Contact.SlidingDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageEntry", Capability.Contact)]
    class NotifyOnExteriorDoorOpen : DoorWatcherBase
    {
        public NotifyOnExteriorDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            NotificationDevices = 
                new List<Speaker>() {
                    _hub.GetDeviceByName<Speaker>("Speaker.WebhookNotifier") as Speaker,
                    _hub.GetDeviceByName<Speaker>("Speaker.KitchenSpeaker") as Speaker
                };

            HowLong = TimeSpan.FromMinutes(2);
        }
    }
}
