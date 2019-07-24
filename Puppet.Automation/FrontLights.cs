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
        {
            _doors = new List<ContactSensor>();
            _doors.Add(_hub.GetDeviceByMappedName<ContactSensor>("Contact.FrontDoor") as ContactSensor);
            _doors.Add(_hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1Opener") as ContactSensor);
            _doors.Add(_hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1") as ContactSensor);
            _doors.Add(_hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2Opener") as ContactSensor);
            _doors.Add(_hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2") as ContactSensor);

            _frontLights = 
                _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsPower") as SwitchRelay;
        }

        protected override async Task Handle()
        {
            if(_evt.IsOpenEvent && IsDark(30, -30))
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
