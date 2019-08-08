using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    public class LockFrontDoor : AutomationBase
    {
        ContactSensor _frontDoor;
        LockDevice _frontDoorLock;
        List<Speaker> _notificationDevices;

        public LockFrontDoor(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task InitDevices()
        {
            _frontDoor =
                await _hub.GetDeviceByMappedName<ContactSensor>("Contact.FrontDoor");

            _frontDoorLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.FrontDoorDeadbolt");

            _notificationDevices =
                new List<Speaker>() {
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier"),
                    await _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker")
                };
        }

        protected override async Task Handle()
        {
            if(_evt.Value.Contains("locking") && _evt.DeviceId == _frontDoorLock.Id)
            {
                return;
            }

            await WaitForCancellationAsync(TimeSpan.FromMinutes(5));

            int attempts = 0;
            while (_frontDoor.Status == ContactStatus.Closed && 
                    _frontDoorLock.Status != LockStatus.Locked &&
                    attempts < 3)
            {
                attempts++;
                await _frontDoorLock.Lock();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(30));
            }

            if (_frontDoorLock.Status != LockStatus.Locked)
            {
                await _notificationDevices.Speak("Front door deadbolt failed to lock! Please check it.");
            }
        }
    }
}
