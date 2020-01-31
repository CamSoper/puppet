using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Notification
{
    [TriggerDevice("Contact.EastGate", Capability.Contact)]
    [TriggerDevice("Contact.SouthGate", Capability.Contact)]
    [TriggerDevice("Contact.WestGate", Capability.Contact)]
    [TriggerDevice("Contact.SlidingDoor", Capability.Contact)]
    public class SlidingDoorAndGatesOpen : AutomationBase
    {
        List<ContactSensor> _gates;
        ContactSensor _slidingDoor;
        List<Speaker> _notificationDevices;

        public SlidingDoorAndGatesOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected async override Task Handle()
        {
            if (_evt.IsOpenEvent)
            {
                if (_slidingDoor.Status == ContactStatus.Open && _gates.IsAnyOpen())
                {
                    await _notificationDevices.Speak("A gate is open while the sliding door is open. Please account for the pets.");
                }
            }
        }

        protected async override Task InitDevices()
        {
            _gates = new List<ContactSensor>() {
                    await _hub.GetDeviceByMappedName<ContactSensor>("Contact.EastGate"),
                    await _hub.GetDeviceByMappedName<ContactSensor>("Contact.WestGate"),
                    await _hub.GetDeviceByMappedName<ContactSensor>("Contact.SouthGate")
                };

            _slidingDoor = await _hub.GetDeviceByMappedName<ContactSensor>("Contact.SlidingDoor");

            _notificationDevices = new List<Speaker>() {
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier"),
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker")
                };
        }
    }
}
