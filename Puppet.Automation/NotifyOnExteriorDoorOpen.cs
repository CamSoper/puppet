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
                    _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier"),
                    _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker")
                };

            HowLong = TimeSpan.FromMinutes(2);
        }
    }
}
