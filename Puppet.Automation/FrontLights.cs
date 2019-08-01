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
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1Opener", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2Opener", Capability.Contact)]
    public class FrontLights : AutomationBase
    {
        SwitchRelay _frontLights;
        List<ContactSensor> _doors;
        
        public FrontLights(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        private async Task InitDevices()
        {
            _doors = new List<ContactSensor>();
            _doors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.FrontDoor"));
            _doors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1Opener"));
            _doors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1"));
            _doors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2Opener"));
            _doors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2"));

            _frontLights =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsPower");
        }

        protected override async Task Handle()
        {
            await InitDevices();

            if(_evt.IsOpenEvent && await IsDark(30, -30))
            {
                _frontLights.On();
            }
            else
            {
                if(!_doors.IsAnyOpen() && _frontLights.Status == SwitchStatus.On)
                {
                    await WaitForCancellationAsync(TimeSpan.FromMinutes(10));
                    _frontLights.Off();
                }
            }
        }
    }
}
