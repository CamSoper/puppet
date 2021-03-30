using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puppet.Automation.Services.SmartThings;
using System.Runtime.InteropServices;
using Puppet.Common.Notifiers;
using System.Collections.Generic;
using Puppet.Automation.Services.Notifiers;

namespace Puppet.Automation.Integration
{
    [RunPerDevice]
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    [TriggerDevice("Lock.SmartThingsFrontDoorDeadbolt", Capability.Lock)]
    public class FrontDoorLockSmartThingsController : AutomationBase
    {
        LockDevice _frontDoorLock;
        LockDevice _smartthingsLock;
        readonly SmartThingsLockService _service;
        readonly List<INotifier> _notificationDevices;

        public FrontDoorLockSmartThingsController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _service = new SmartThingsLockService(_hub.Configuration);

            _notificationDevices = new List<INotifier> {
                new HassAlexaNotifier(_hub.Configuration, new string[] { "Shared_Spaces" }),
                new HassAppNotifier(_hub.Configuration)
                };
        }

        protected override async Task InitDevices()
        {

            _frontDoorLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.FrontDoorDeadbolt");

            _smartthingsLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.SmartThingsFrontDoorDeadbolt");

        }

        protected override async Task Handle()
        {
            if ( _frontDoorLock.IsTriggerDevice(_evt))
            {
                switch (_evt.Value)
                {
                    case "unlocking":
                        await _service.Unlock();
                        break;

                    case "locking":
                        await _service.Lock();
                        break;

                    default:
                        return;
                }
 
                await WaitForCancellationAsync(TimeSpan.FromSeconds(10));
                await _notificationDevices.SendMessage("The front door lock is in an unknown state! Please check it.");
            }
            else if (
                    (await 
                        _hub.GetDeviceByMappedName<LockDevice>("Lock.SmartThingsFrontDoorDeadbolt"))
                        .IsTriggerDevice(_evt)
                    )
            {
                if (_evt.Value == "locked")
                {
                    await _frontDoorLock.DoAction("confirmLocked");
                }
                else if (_evt.Value == "unlocked")
                {
                    await _frontDoorLock.DoAction("confirmUnlocked");
                }
                return;
            }
        }
    }
}
