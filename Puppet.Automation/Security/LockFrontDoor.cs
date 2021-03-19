using Puppet.Automation.Services.Notifiers;
using Puppet.Automation.Services.SmartThings;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Notifiers;
using Puppet.Common.Services;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Security
{
    /// <summary>
    /// Sends commands from a virtual Hubitat lock (Lock.FrontDoorDeadbolt) to a SmartThings endpoint.
    /// </summary>
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    public class LockFrontDoor : AutomationBase
    {
        ContactSensor _frontDoor;
        LockDevice _frontDoorLock;
        List<INotifier> _notificationDevices;
        SmartThingsLockService _service;

        public LockFrontDoor(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _service = new SmartThingsLockService(_hub.Configuration);
            _notificationDevices = new List<INotifier> {
                new HassAlexaNotifier(_hub.Configuration, new string[] { "Shared_Spaces" }),
                new HassAppNotifier(_hub.Configuration)
                };
        }

        protected override async Task InitDevices()
        {
            _frontDoor =
                await _hub.GetDeviceByMappedName<ContactSensor>("Contact.FrontDoor");

            _frontDoorLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.FrontDoorDeadbolt");
        }

        protected override async Task Handle()
        {
            if (_evt.Value.Contains("locking") && _evt.DeviceId == _frontDoorLock.Id)
            {
                // Wait for this task to be cancelled when the locking/unlocking completes and a new event is triggered
                await WaitForCancellationAsync(TimeSpan.FromSeconds(15));
                await _notificationDevices.SendMessage("Front door deadbolt is in an unknown state! Please check it.");
                return;
            }

            if (_evt.DeviceId == _frontDoor.Id)
            {
                if (_evt.IsClosedEvent)
                {
                    await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                }
                await _service.Refresh();
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
                await _notificationDevices.SendMessage("Front door deadbolt failed to lock! Please check it.");
            }
        }
    }
}
