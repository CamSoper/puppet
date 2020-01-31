using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Puppet.Automation.Services;

namespace Puppet.Automation.Integration
{
    [RunPerDevice]
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    [TriggerDevice("Lock.SmartThingsFrontDoorDeadbolt", Capability.Lock)]
    public class FrontDoorLockSmartThingsController : AutomationBase
    {
        LockDevice _frontDoorLock;
        LockDevice _smartthingsLock;
        SmartThingsLockService _service;

        public FrontDoorLockSmartThingsController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _service = new SmartThingsLockService(_hub.Configuration);
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
            if (_evt.DeviceId == _frontDoorLock.Id)
            {
                if (_evt.Value == "unlocking")
                {
                    await _service.Unlock();
                }
                else if (_evt.Value == "locking")
                {
                    await _service.Lock();
                }
                else
                {
                    return;
                }
            }
            else if (_evt.DeviceId == _hub.LookupDeviceId("Lock.SmartThingsFrontDoorDeadbolt"))
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
